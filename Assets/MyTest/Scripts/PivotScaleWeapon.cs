using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation.Samples;
using UnityEditor.iOS.Xcode;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
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

    private Vector3 initialScale;

    private Material m_CircleMaterial;

    private readonly float duration = 1f; // 1 giây

    private readonly int[] levelFactors = { 1, 2, 3, 5 };
    private int m_CurrentIndex = 0;

    private Coroutine scalingCoroutine;
    private Coroutine setupScaleDefault;

    private const string AttackAnimationClip = "Great Sword Casting_IK";
    private const string AttackCastingAnimationClip = "Great Sword Casting_attack_IK";

    [SerializeField] private int m_EnergyWeaponDamage;
    private int m_Damage = 10;
    
    public int EnergyWeaponDamage => m_EnergyWeaponDamage;

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
    }

    private void Update()
    {
        HandleInput();
    }
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            // Reset và bắt đầu coroutine
            //ResetScale();
            if (m_AuraPS != null)
            {
                m_AuraPS.gameObject.SetActive(true);
                m_AuraPS.Play();
            }
            animator.Play(AttackAnimationClip);
            animator.SetTrigger("IsPress");

            scalingCoroutine = StartCoroutine(ScaleCoroutine());
        }
        if (Input.GetKeyUp(KeyCode.O))
        {
            animator.SetBool("IsPressN", false);
            setupScaleDefault = StartCoroutine(SetupScaleDefault());
            if (m_AuraPS != null)
            {
                m_AuraPS.gameObject.SetActive(false);
            }
            StartCoroutine(SmoothRotateToCameraDirection(0.3f));
        }
    }

    public void ScaleAndAdjust(Vector3 newScale)
    {
        // Lưu vị trí world hiện tại của điểm cầm (có tính rotation hiện tại)
        Vector3 currentHandleWorldPos = m_TranformEnergyWeapon.transform.TransformPoint(handleLocalOffset); // worldPos = position + rotation × (localOffset × scale)

        // Áp dụng scale mới
        m_TranformEnergyWeapon.transform.localScale = newScale;

        // Sau khi scale, tính vị trí world mới của điểm cầm
        Vector3 newHandleWorldPos = m_TranformEnergyWeapon.transform.TransformPoint(handleLocalOffset);

        // worldPos = position + rotation × (localOffset × scale)
        //Có nghĩa là: chuyển đổi localOffset (đã nhân với scale)
        //từ không gian local sang không gian world bằng cách áp dụng rotation,
        //sau đó cộng với vị trí world của đối tượng để có được vị trí world thực sự của điểm đó.

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

            // Chỉ tăng timer khi nhấn phím N
            if (!Input.GetKey(KeyCode.O))
            {
                yield break;
            }
            timer += Time.deltaTime;
            // Tính level mới dựa trên thời gian đã trôi qua (mỗi 'duration' sẽ tăng level)
            int newLevelIndex = Mathf.Clamp((int)(timer / duration), 0, levelFactors.Length - 1);

            // Nếu có sự thay đổi level (và level tăng)
            if (newLevelIndex > m_CurrentIndex)
            {
                m_CurrentIndex = newLevelIndex;

                m_EnergyWeaponDamage = (int)(m_Damage * (1f + m_CurrentIndex * 0.2f));
                Debug.Log("m_EnergyWeaponDamage : " + m_EnergyWeaponDamage);

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
        //yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
        yield return new WaitUntil(() =>
        {
            var info = animator.GetCurrentAnimatorStateInfo(0);
            return info.IsName(AttackCastingAnimationClip);
        });
        yield return new WaitUntil(() =>
        {
            var info = animator.GetCurrentAnimatorStateInfo(0);
            return (!info.IsName(AttackCastingAnimationClip) || (info.IsName(AttackCastingAnimationClip) && info.normalizedTime >= 1f));
        });
        ResetScale();
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
}
