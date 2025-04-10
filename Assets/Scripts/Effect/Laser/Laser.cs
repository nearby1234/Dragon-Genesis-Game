using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private WormBoss m_WormBoss;
    private const string m_AnimationNamePhase1 = "Attack05ST"; 
    private const string m_AnimationNamePhase2 = "Attack06ST";
    [SerializeField] private bool m_IsReceiverDamageLaser;
    public float damageOverTime;
    public GameObject HitEffect;
    public float HitOffset = 0;
    public bool useLaserRotation = false;
    public float MaxLength;

    // Lấy tất cả các LineRenderer trong object
    private LineRenderer[] lasers;

    public float MainTextureLength = 1f;
    public float NoiseTextureLength = 1f;
    private Vector4 Length = new Vector4(1, 1, 1, 1);

    private bool LaserSaver = false;
    //private bool UpdateSaver = false;

    private ParticleSystem[] Effects;
    private ParticleSystem[] Hit;

    private void Awake()
    {
        m_WormBoss = GetComponentInParent<WormBoss>();
    }
    void Start()
    {
        // Lấy tất cả các LineRenderer được gắn trên GameObject
        lasers = GetComponentsInChildren<LineRenderer>();
        Effects = GetComponentsInChildren<ParticleSystem>();
        Hit = HitEffect.GetComponentsInChildren<ParticleSystem>();
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.BOSS_SENDER_DAMAGED, ReceiverBossDamageLaser);
        }
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.BOSS_SENDER_DAMAGED, ReceiverBossDamageLaser);
        }
    }

    void Update()
    {
        SetLaser();
    }
    public void SetLaser()
    {
        // Lặp qua từng LineRenderer để cập nhật trạng thái
        foreach (LineRenderer Laser in lasers)
        {
            Laser.material.SetTextureScale("_MainTex", new Vector2(Length[0], Length[1]));
            //Laser.material.SetTextureScale("_Noise", new Vector2(Length[2], Length[3]));
            Laser.SetPosition(0, transform.position);

            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, MaxLength))
            {
                // Nếu có va chạm, đặt điểm cuối của laser tại vị trí hit
                Laser.SetPosition(1, hit.point);
                HitEffect.transform.position = hit.point + hit.normal * HitOffset;
                if (useLaserRotation)
                    HitEffect.transform.rotation = transform.rotation;
                else
                    HitEffect.transform.LookAt(hit.point + hit.normal);

                foreach (var AllPs in Effects)
                {
                    if (!AllPs.isPlaying)
                        AllPs.Play();
                }
                Length[0] = MainTextureLength * Vector3.Distance(transform.position, hit.point);
                Length[2] = NoiseTextureLength * Vector3.Distance(transform.position, hit.point);

                // Gây sát thương nếu tag của collider là "Target"
                if (hit.collider.CompareTag("Player"))
                {
                    //hit.collider.GetComponent<HS_HittedObject>().TakeDamage(damageOverTime * Time.deltaTime);
                    if (!m_IsReceiverDamageLaser)
                    {
                        hit.collider.GetComponent<PlayerHeal>().ReducePlayerHeal((int)damageOverTime);
                        m_IsReceiverDamageLaser = true;
                    }
                }
            }
            else
            {
                // Nếu không có va chạm, laser hiển thị theo hướng ban đầu với độ dài tối đa
                Vector3 EndPos = transform.position + transform.forward * MaxLength;
                Laser.SetPosition(1, EndPos);
                HitEffect.transform.position = EndPos;
                foreach (var AllPs in Hit)
                {
                    if (AllPs.isPlaying)
                    {
                        AllPs.Stop();
                        AllPs.Play();
                    }
                }
                Length[0] = MainTextureLength * Vector3.Distance(transform.position, EndPos);
                Length[2] = NoiseTextureLength * Vector3.Distance(transform.position, EndPos);
            }
            // Bảo đảm Laser hiển thị nếu chưa được bật
            if (!Laser.enabled && !LaserSaver)
            {
                LaserSaver = true;
                Laser.enabled = true;
            }
        }
    }
    public void DisablePrepare()
    {
        // Vô hiệu hóa tất cả các LineRenderer
        foreach (LineRenderer Laser in lasers)
        {
            if (Laser != null)
            {
                Laser.enabled = false;
            }
        }
        //UpdateSaver = true;
        if (Effects != null)
        {
            foreach (var AllPs in Effects)
            {
                if (AllPs.isPlaying)
                    AllPs.Stop();
            }
        }
    }

    private void ReceiverBossDamageLaser(object value)
    {
        if (value != null)
        {
            if (m_WormBoss != null)

            {
                if (value is (int currentAttackIndex, List<WormAttackData> attackDataList))
                {
                    if (attackDataList[currentAttackIndex].animationName.Equals(m_AnimationNamePhase1) 
                        || attackDataList[currentAttackIndex].animationName.Equals(m_AnimationNamePhase2))
                    {
                        damageOverTime = attackDataList[currentAttackIndex].CalculateDamage(m_WormBoss);
                    }
                    
                }
            }

        }
    }
}
