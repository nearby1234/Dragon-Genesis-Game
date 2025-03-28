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
    public Vector3 handleLocalOffset = new(0f, -0.5f, 0f);
    public Transform m_TranformEnergyWeapon;
    public Transform m_Sword;
    public MeshFilter m_CurrentMeshFilter;
    public MeshFilter m_BeforMeshFilter;
    public MeshRenderer m_EnergyWeaponMesh;

    public ParticleSystem m_Cirlcle;
    public ParticleSystem m_ShockWave;
    public ParticleSystem m_Trial;
    public ParticleSystem m_aura;
    public ParticleSystem m_Explosion;
    public VisualEffect m_Energy;
    public Animator animator;

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

    private void Awake()
    {
        animator = GetComponent<Animator>();
        m_BeforMeshFilter = m_Sword.GetComponent<MeshFilter>();
        m_CurrentMeshFilter = m_TranformEnergyWeapon.GetComponent<MeshFilter>();
        m_EnergyWeaponMesh = m_TranformEnergyWeapon.GetComponent<MeshRenderer>();
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
        //// Ví dụ: tăng giảm scale theo thời gian sử dụng PingPong
        //float scaleFactor = 1f + Mathf.PingPong(Time.time * 0.5f, 1f);
        //ScaleAndAdjust(initialScale * scaleFactor);

        //ScaleWhenPressButton();
        //ScaleWithTimer();

        if (Input.GetKeyDown(KeyCode.O))
        {
            // Reset và bắt đầu coroutine
            //ResetScale();
            if (m_aura != null)
            {
                m_aura.gameObject.SetActive(true);
                m_aura.Play();
                animator.Play(AttackAnimationClip);
                animator.SetTrigger("IsPress");


            }
            scalingCoroutine = StartCoroutine(ScaleCoroutine());
        }
        if (Input.GetKeyUp(KeyCode.O))
        {
            animator.SetBool("IsPressN", false);
            setupScaleDefault = StartCoroutine(SetupScaleDefault());
            m_aura.gameObject.SetActive(false);
        }
    }
    private void OnChildTriggerEnter(TriggerData data)
    {
        if (data != null)
        {
            data.Collider.TryGetComponent<WormBoss>(out var wormBoss);
            wormBoss.GetDamage(m_EnergyWeaponDamage);
            Vector3 pointCol = data.Collider.ClosestPoint(data.ChildPos);
            if (m_Explosion != null)
            {
                m_Explosion.gameObject.transform.position = pointCol;
                m_Explosion.gameObject.SetActive(true);
                if (!m_Explosion.isPlaying)
                {
                    m_Explosion.Play();
                }
                StartCoroutine(TurnOffExplosion());
            }
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
    public void AeSetupEffect()
    {
        if (m_Explosion != null)
        {
            m_Explosion.gameObject.SetActive(true);
            if (!m_Explosion.isPlaying)
            {
                m_Explosion.Play();
            }
        }
    }
    public void AeStopEffect()
    {
        if (m_Explosion != null)
        {
            m_Explosion.gameObject.SetActive(false);
        }
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
        
        float timer = 0f;
        while (true)
        {
           
            // Chỉ tăng timer khi nhấn phím N
            if (Input.GetKey(KeyCode.O))
            {
                timer += Time.deltaTime;
            }
            
            // Tính level mới dựa trên thời gian đã trôi qua (mỗi 'duration' sẽ tăng level)
            int newLevelIndex = Mathf.Clamp((int)(timer / duration), 0, levelFactors.Length - 1);

            // Nếu có sự thay đổi level (và level tăng)
            if (newLevelIndex > m_CurrentIndex)
            {
                m_CurrentIndex = newLevelIndex;
                // Setup hiệu ứng (shockwave, energy) mỗi khi nâng cấp level

                m_EnergyWeaponMesh.enabled = true;
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

    private IEnumerator TurnOffExplosion()
    {
        yield return new WaitForSeconds(1f);
        m_Explosion.gameObject.SetActive(false);
    }
    private IEnumerator SetupScaleDefault()
    {
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
    private void ResetScale()
    {
        m_CurrentIndex = 0;
        m_TranformEnergyWeapon.transform.localScale = Vector3.one;
        m_TranformEnergyWeapon.transform.localPosition = Vector3.zero;
        m_EnergyWeaponMesh.enabled = false;
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
