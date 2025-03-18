using UnityEngine;

public class HouvlLaserTest : MonoBehaviour
{
    public int damageOverTime = 30;
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
    private bool UpdateSaver = false;

    private ParticleSystem[] Effects;
    private ParticleSystem[] Hit;

    void Start()
    {
        // Lấy tất cả các LineRenderer được gắn trên GameObject
        lasers = GetComponentsInChildren<LineRenderer>();
        Effects = GetComponentsInChildren<ParticleSystem>();
        Hit = HitEffect.GetComponentsInChildren<ParticleSystem>();
    }

    void Update()
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
                if (hit.collider.tag == "Target")
                {
                    //hit.collider.GetComponent<HS_HittedObject>().TakeDamage(damageOverTime * Time.deltaTime);
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
        UpdateSaver = true;
        if (Effects != null)
        {
            foreach (var AllPs in Effects)
            {
                if (AllPs.isPlaying)
                    AllPs.Stop();
            }
        }
    }
}
