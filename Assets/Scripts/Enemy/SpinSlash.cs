using MaykerStudio.Demo;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class SpinSlash : MonoBehaviour
{
    [SerializeField] private GameObject prefabs;
    [SerializeField] private float PosY;
    [SerializeField] private float RotateZ;
    private readonly int count = 3;
    private readonly float fanAngle = 60f;
    private Queue<GameObject> poolSlash = new();

    public void FireFan()
    {
        // Tính bước góc giữa các phát bắn
        float angleStep = (count > 1) ? fanAngle / (count - 1) : 0f;


        // Bắt đầu từ nửa trái của quạt
        float startAngle = -fanAngle / 2f;

        for (int i = 0; i < count; i++)
        {
            // Offset góc từng viên
            float yaw = startAngle + angleStep * i;

            // Tạo prefab
            GameObject obj = Instantiate(prefabs);
            // Đặt vị trí và xoay quanh trục Y
            obj.transform.SetPositionAndRotation(new Vector3(transform.position.x, PosY, transform.position.z), Quaternion.Euler(0f, transform.eulerAngles.y + yaw, RotateZ));

            //Gọi Fire nếu có component Projectile
            if (obj.TryGetComponent<Projectile>(out var projectile))
            {
                projectile.Fire();
                //if(AudioManager.HasInstance)
                //{
                //    AudioManager.Instance.PlaySE("SFX-Spell-02-Earth_wav");
                //}
                if (ListenerManager.HasInstance)
                {
                    string nameSound = "SFX-Spell-03-Earth_wav";
                    ListenerManager.Instance.BroadCast(ListenType.PLAYSOUNDSE_BOSSBULLTANK, nameSound);
                }

            }
            Destroy(obj,3f);
        }

    }
}
