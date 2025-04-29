using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class PivotScaleWeapon : MonoBehaviour
{
    // Định nghĩa offset ban đầu từ pivot (điểm cầm) theo không gian local của đối tượng con
    [SerializeField] private Vector3 handleLocalOffset = new(0f, -0.5f, 0f);
    [SerializeField] private Transform m_TranformEnergyWeapon;
    [SerializeField] private Transform m_Sword;
    [SerializeField] private MeshFilter m_CurrentMeshFilter;
    [SerializeField] private MeshFilter m_BeforMeshFilter;
    //public MeshRenderer m_EnergyWeaponMesh;

    [SerializeField] private ParticleSystem m_Cirlcle;
    [SerializeField] private ParticleSystem m_ShockWave;
    [SerializeField] private ParticleSystem m_Trial;
    [SerializeField] private ParticleSystem m_AuraPS;
    [SerializeField] private VisualEffect m_Energy;
    [SerializeField] private Animator animator;
    [SerializeField] private ChildTriggerForwarder m_ChildTriggerForwarder;

    private Vector3 initialScale;
    private InputAction m_ButtonPress;

    private Material m_CircleMaterial;

    private readonly float duration = 1f; // 1 giây

    private readonly int[] levelFactors = { 1, 2, 3, 5 };
    private int m_CurrentIndex = 0;

    private Coroutine scalingCoroutine;
    private Coroutine setupScaleDefault;

    private const string attackAnimationClip = "Great Sword Casting_IK";
    private const string attackCastingAnimationClip = "Great Sword Casting_attack_IK";
    private const string itemSkillID = "-CarriarGreatSwordSkill";

    [SerializeField] private int m_EnergyWeaponDamage;
    public int EnergyWeaponDamage => m_EnergyWeaponDamage;
    [SerializeField] private float m_EnergyWeaponConsumption; // Tỉ lệ tiêu hao năng lượng
    [SerializeField] private float m_IncreasePercentagePerIndex;
    [SerializeField] private bool m_IsManaEmpty;

    private int m_Damage;
    private float m_ManaMax;



    private void Awake()
    {
        animator = GetComponent<Animator>();
        m_BeforMeshFilter = m_Sword.GetComponent<MeshFilter>();
        m_CurrentMeshFilter = m_TranformEnergyWeapon.GetComponent<MeshFilter>();
        //m_EnergyWeaponMesh = m_TranformEnergyWeapon.GetComponent<MeshRenderer>();
        ParticleSystemRenderer psRenderer = m_Cirlcle.GetComponent<ParticleSystemRenderer>();
        if (psRenderer != null)
        {
            m_CircleMaterial = psRenderer.material;
        }
    }

    private void Start()
    {
        // Lưu lại scale ban đầu
        initialScale = m_TranformEnergyWeapon.transform.localScale;
        m_CurrentMeshFilter.mesh = m_BeforMeshFilter.mesh;
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.PLAYER_SEND_DAMAGE_VALUE, ReceiverPlayerDamage);
            ListenerManager.Instance.Register(ListenType.PLAYER_SEND_MANA_VALUE, ReceiverPlayerMana);
            ListenerManager.Instance.Register(ListenType.PLAYER_MANA_EMPTY, ReceiverStateMove);
            ListenerManager.Instance.Register(ListenType.UI_SEND_BUTTON_PRESS_AND_TYPESKILL, ReceiverEventButtonAndTypeSKill);
        }
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.PLAYER_SEND_DAMAGE_VALUE, ReceiverPlayerDamage);
            ListenerManager.Instance.Unregister(ListenType.PLAYER_SEND_MANA_VALUE, ReceiverPlayerMana);
            ListenerManager.Instance.Unregister(ListenType.PLAYER_MANA_EMPTY, ReceiverStateMove);
            ListenerManager.Instance.Unregister(ListenType.UI_SEND_BUTTON_PRESS_AND_TYPESKILL, ReceiverEventButtonAndTypeSKill);
        }
        if (m_ButtonPress != null)
        {
            m_ButtonPress.started -= OnButtonStarted;
            m_ButtonPress.canceled -= OnButtonCanceled;
            m_ButtonPress.Disable();
            m_ButtonPress = null;
        }

    }
    public void GetChildTriggerForwarder(ChildTriggerForwarder childTriggerForwarder)
    {
        m_ChildTriggerForwarder = childTriggerForwarder;
    }

    public void ScaleAndAdjust(Vector3 newScale)
    {
        // Lưu vị trí world hiện tại của điểm cầm (có tính rotation hiện tại)
        Vector3 currentHandleWorldPos = m_TranformEnergyWeapon.transform.TransformPoint(handleLocalOffset); // worldPos = position + rotation × (localOffset × scale)

        // Áp dụng scale mới
        m_TranformEnergyWeapon.transform.localScale = newScale;

        // Sau khi scale, tính vị trí world mới của điểm cầm
        Vector3 newHandleWorldPos = m_TranformEnergyWeapon.transform.TransformPoint(handleLocalOffset);

        // Tính delta hiệu chỉnh
        Vector3 delta = currentHandleWorldPos - newHandleWorldPos;

        // Điều chỉnh lại vị trí của đối tượng để điểm cầm không dịch chuyển
        m_TranformEnergyWeapon.transform.position += delta;
    }
    public void AeStartFade()
    {
        if (m_Cirlcle != null)
        {

            m_Cirlcle.gameObject.SetActive(true);
            StartCoroutine(FadeAlphaCoroutine(0.5f));
        }
    }
    public void StopFade()
    {
        if (m_Cirlcle != null)
        {
            m_Cirlcle.gameObject.SetActive(false);
        }
    }
    private IEnumerator ScaleCoroutine()
    {
        m_TranformEnergyWeapon.gameObject.SetActive(true);
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;
            // Tính level mới dựa trên thời gian đã trôi qua (mỗi 'duration' sẽ tăng level)
            int newLevelIndex = Mathf.Clamp((int)(timer / duration), 0, levelFactors.Length - 1);

            // Nếu có sự thay đổi level (và level tăng)
            if (newLevelIndex > m_CurrentIndex)
            {
                m_CurrentIndex = newLevelIndex;

                m_EnergyWeaponDamage = (int)(m_Damage * (1f + m_CurrentIndex * m_IncreasePercentagePerIndex));
                // Tính toán năng lượng tiêu hao và gửi sự kiện


                // Setup hiệu ứng (shockwave, energy) mỗi khi nâng cấp level

                //m_EnergyWeaponMesh.enabled = true;
                SetupShockWave(m_ShockWave, true);
                SetupShockWave(m_Trial, true);
                SetupEnergy(m_Energy, true);
                ScaleAndAdjust(initialScale * levelFactors[m_CurrentIndex]);
                StopFade();
                animator.SetBool("IsPressN", true);

                // Nếu đạt level cao nhất (LEVEL3) thì dừng coroutine
                if (m_CurrentIndex == levelFactors.Length - 1)
                {
                    yield break;
                }
            }
            yield return null;
        }
    }

    private IEnumerator SetupScaleDefault()
    {
        // chờ qua state casting
        yield return new WaitUntil(() =>
            animator.GetCurrentAnimatorStateInfo(0).IsName(attackCastingAnimationClip));
        yield return new WaitUntil(() =>
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

        // Lúc này mới ResetScale
        ResetScale();
        StartCoroutine(m_ChildTriggerForwarder.ResetSwing());
    }
    private IEnumerator SmoothRotateToCameraDirection(float rotateDuration)
    {
        // Lưu lại góc ban đầu của player
        Quaternion initialRotation = transform.rotation;

        // Lấy hướng forward của camera (chỉ tính trên mặt phẳng ngang)
        Vector3 camForward = Camera.main.transform.forward;
        camForward.y = 0f; // Bỏ phần y để xoay theo mặt đất
        Quaternion targetRotation = Quaternion.LookRotation(camForward);

        float elapsed = 0f;
        while (elapsed < rotateDuration)
        {
            elapsed += Time.deltaTime;
            // Nội suy xoay mượt theo thời gian
            transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, elapsed / rotateDuration);
            yield return null;
        }
        transform.rotation = targetRotation; // Đảm bảo xoay đúng cuối
    }
    private void ResetScale()
    {
        m_TranformEnergyWeapon.gameObject.SetActive(false);
        m_CurrentIndex = 0;
        m_TranformEnergyWeapon.transform.localScale = Vector3.one;
        m_TranformEnergyWeapon.transform.localPosition = Vector3.zero;
        //m_EnergyWeaponMesh.enabled = false;
        SetupShockWave(m_ShockWave, false);
        SetupShockWave(m_Trial, false);
        SetupEnergy(m_Energy, false);
    }

    #region SETUP EFFECT 
    private void SetupShockWave(ParticleSystem particleSystem, bool turn)
    {
        if (particleSystem != null)
        {
            if (turn)
            {
                particleSystem.gameObject.SetActive(true);
                if (!particleSystem.isPlaying)
                {
                    particleSystem.Play();
                }
                else
                {
                    particleSystem.Stop();
                    particleSystem.Play();
                }
            }
            else
            {
                particleSystem.gameObject.SetActive(false);
            }

        }
    }
    private void SetupEnergy(VisualEffect energy, bool turn)
    {
        if (energy != null)
        {
            if (turn)
            {
                energy.gameObject.SetActive(true);
                if (energy.aliveParticleCount <= 0)
                {
                    energy.Play();
                }
                else
                {
                    energy.Stop();
                    energy.Play();
                }
            }
            else
            {
                energy.gameObject.SetActive(false);
            }

        }
    }

    private IEnumerator FadeAlphaCoroutine(float duration)
    {
        float elapsed = 0f;
        Color initialColor = m_CircleMaterial.GetColor("_BaseColor");
        // Giả sử alpha ban đầu là 0, và muốn đạt 1 sau duration
        initialColor.a = 1f;
        m_CircleMaterial.SetColor("_BaseColor", initialColor);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(1f, 0f, Mathf.Clamp01(elapsed / duration));
            Color currentColor = m_CircleMaterial.GetColor("_BaseColor");
            currentColor.a = newAlpha;
            m_CircleMaterial.SetColor("_BaseColor", currentColor);
            yield return null;
        }

    }
    #endregion
    private void ReceiverPlayerDamage(object value)
    {
        if (value is int damage)
        {
            m_Damage = damage;
        }
    }
    private void ReceiverPlayerMana(object value)
    {
        if (value is float manaMax)
        {
            m_ManaMax = manaMax;
        }
    }

    private void ManaConsumption(float manaMax, int index)
    {
        m_EnergyWeaponConsumption = manaMax * (index * m_IncreasePercentagePerIndex);
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.PLAYER_SKILL_CONSUMPTION_MANA, m_EnergyWeaponConsumption);
        }
    }
    private void ReceiverStateMove(object value)
    {
        if (value is bool isStaminaEmpty)
        {
            // Nếu empty == true → stamina cạn
            m_IsManaEmpty = isStaminaEmpty;
        }
    }
    private void ResetPivotScaleWeapon()
    {
        if (scalingCoroutine != null)
            StopCoroutine(scalingCoroutine);
        animator.SetBool("IsPressN", false);

        if (m_AuraPS != null)
        {
            m_AuraPS.Stop();
            m_AuraPS.gameObject.SetActive(false);
        }
        // Khi thả phím thì tính toán tiêu hao mana và gửi dữ liệu cho PlayerMana
        ManaConsumption(m_ManaMax, m_CurrentIndex);
        if (m_CurrentIndex > 0)
        {
            Debug.Log($"m_CurrentIndex : {m_CurrentIndex}");
            setupScaleDefault = StartCoroutine(SetupScaleDefault());
        }
        else
        {
            ResetScale();
            animator.Play("Move"); // hoặc tên state Idle của bạn
        }

        StartCoroutine(SmoothRotateToCameraDirection(0.3f));
    }
    private void OnButtonStarted(InputAction.CallbackContext callback)
    {
        // Gửi sự kiện "PLAYER_SKILL_KEYDOWN" để kiểm tra mana cần thiết.
        if (ListenerManager.HasInstance)
        {
            // Bạn có thể gửi dữ liệu bổ sung nếu cần, ví dụ, yêu cầu mức tiêu hao mana cho skill.
            ListenerManager.Instance.BroadCast(ListenType.PLAYER_SKILL_KEYDOWN, null);
        }
        // Sau khi broadcast, flag m_IsManaEmpty sẽ được cập nhật từ PlayerMana (thông qua ReceiverStateMove).
        if (m_IsManaEmpty)
        {
            Debug.Log("Không đủ mana để thi triển skill");
            return; // Không thi triển nếu mana không đủ.
        }

        // Nếu đủ mana, thực hiện các hiệu ứng và thi triển skill.
        if (m_AuraPS != null)
        {
            m_AuraPS.gameObject.SetActive(true);
            m_AuraPS.Play();
        }
        animator.Play(attackAnimationClip);
        animator.SetTrigger("IsPress");
        scalingCoroutine = StartCoroutine(ScaleCoroutine());
    }
    private void OnButtonCanceled(InputAction.CallbackContext callback)
    {
        ResetPivotScaleWeapon();
    }
    private void ReceiverEventButtonAndTypeSKill(object value)
    {
        if (value is (InputAction button, QuestItemSO itemskill))
        {
            if (itemskill.questItemData.itemID.Equals(itemSkillID))
            {
                m_ButtonPress = button;
                m_ButtonPress.Enable();
                m_ButtonPress.started += OnButtonStarted;
                m_ButtonPress.canceled += OnButtonCanceled;
            }

        }
    }
}
