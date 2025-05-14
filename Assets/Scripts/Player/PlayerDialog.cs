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
    [SerializeField] private float m_Distance;
    public bool HasAcceptQuest
    {
        get => m_HasAcceptQuest;
        set => m_HasAcceptQuest = value;
    }
    //Property để quản lý trạng thái và broadcast sự thay đổi
    public bool IsTalkingNPC
    {
        get => m_IsTalkingNPC;
        set
        {
            if (m_IsTalkingNPC != value)
            {
                m_IsTalkingNPC = value;
                OnIsTalkingNPCChanged();
            }
        }
    }

    private void Update()
    {
        CheckNPCDistance();
        HandleInput();
       
    }
    private void HandleInput()
    {
        if (!m_IsCollisionNpc) return;
            
        if (Input.GetKeyDown(KeyCode.J))
        {
            if(AudioManager.HasInstance)
            {
                AudioManager.Instance.PlaySE("HoverSound");
            }
            if(GameManager.HasInstance) GameManager.Instance.ShowCursor();
            m_IsPressButtonJ = true;
            SetIsTalkingNPC(true);
            if (ListenerManager.HasInstance)
            {
                ListenerManager.Instance.BroadCast(ListenType.UI_CLICK_SHOWUI, null);
                ListenerManager.Instance.BroadCast(ListenType.CLICK_TALK_NPC, null);
            }
        }
    }
    private void CheckNPCDistance()
    {
        if (DistanceWithNPC() <= m_Distance)
        {
            m_IsCollisionNpc = true;

            if (m_IsPressButtonJ && m_IsCollisionNpc)
            {
                HandleNPCInteraction();
                m_IsPressButtonJ = false;
            }
        }
        else
        {
            m_IsCollisionNpc = false;
        }
    }

    private void HandleNPCInteraction()
    {
        if (CameraManager.HasInstance)
        {
            var camera = CameraManager.Instance.GetCameraCinemachine("NPCCamera");
            int playerLayer = LayerMask.NameToLayer("Player");
            int weaponLayer = LayerMask.NameToLayer("Weapon");
            int fxLayer = LayerMask.NameToLayer("FX");

            int combinedMask = (1 << playerLayer) | (1 << weaponLayer)| (1 << fxLayer);
            if (camera != null )
            {
                camera.Priority = 15;
                Camera.main.cullingMask &= ~combinedMask;
            }
        }

        if (UIManager.HasInstance)
        {
            UIManager.Instance.HidePopup<DialobGuidePopup>();
            UIManager.Instance.ShowPopup<QuestMissionOnePanel>();
        }
    }
    private void OnIsTalkingNPCChanged()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.PLAYER_IS_TALKING_NPC, m_IsTalkingNPC);
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
            var data = QuestManager.Instance?.CurrentQuest;
            if (data != null && data.name == other.name && data.isAcceptMission)
            {
                HasAcceptQuest = true;
            }
        }
    }

}
