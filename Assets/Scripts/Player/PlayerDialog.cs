using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerDialog : MonoBehaviour
{
    [SerializeField] private bool m_IsPressButtonJ;
    private GameObject npcObject;
    [SerializeField] private bool m_IsCollisionNpc;
    [SerializeField] private bool m_IsTalkingNPC;
    [SerializeField] private bool m_HasAcceptQuest;
    public bool HasAcceptQuest
    {
        get => m_HasAcceptQuest;
        set
        {
            m_HasAcceptQuest = value;
            //if (ListenerManager.HasInstance)
            //{
            //    ListenerManager.Instance.BroadCast(ListenType.PLAYER_HAS_ACCEPT_QUEST, m_HasAcceptQuest);
            //}
        }
    }


    [SerializeField] private float m_Distance;

    //Property để quản lý trạng thái và broadcast sự thay đổi
    public bool IsTalkingNPC
    {
        get => m_IsTalkingNPC;
        set
        {
            // Chỉ broadcast khi giá trị thực sự thay đổi
            m_IsTalkingNPC = value;
            // Gửi sự kiện luôn khi trạng thái thay đổi
            if (ListenerManager.HasInstance)
            {
                ListenerManager.Instance.BroadCast(ListenType.PLAYER_IS_TALKING_NPC, m_IsTalkingNPC);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            m_IsPressButtonJ = true;
            SetIsTalkingNPC(true);

        }
        if (DistanceWithNPC() <= m_Distance)
        {
            m_IsCollisionNpc = true;

            if (m_IsPressButtonJ && m_IsCollisionNpc)
            {
                if (CameraManager.HasInstance)
                {
                    CinemachineVirtualCameraBase camera = CameraManager.Instance.GetCameraCinemachine("NPCCamera");
                    if (camera != null)
                    {
                        camera.Priority = 15;
                    }
                }
                if (UIManager.HasInstance)
                {
                    UIManager.Instance.HidePopup<DialobGuidePopup>();
                    UIManager.Instance.ShowPopup<QuestMissionOnePanel>();


                }
                // Reset biến sau khi đã xử lý
                m_IsPressButtonJ = false;
            }
        }
        else
        {
            m_IsCollisionNpc = false;
        }
    }
    public void SetIsTalkingNPC(bool result)
    {
        IsTalkingNPC = result;
    }
    private float DistanceWithNPC()
    {
        if (npcObject != null)
        {
            return Vector3.Distance(transform.position, npcObject.transform.position);
        }
        return Mathf.Infinity;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            npcObject = other.gameObject;
            QuestData data = QuestManager.Instance.CurrentQuest;
            if(data.name == other.name)
            {
                if(data.isAcceptMission)
                {
                    //HasAcceptQuest = true;
                }
            }

        }
    }

}
