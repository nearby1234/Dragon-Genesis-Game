using UnityEngine;
using UnityEngine.InputSystem;



public class PlayerDialog : MonoBehaviour
{
    [SerializeField] private bool m_IsPressButtonJ;
    private GameObject npcObject;
    [SerializeField] private bool m_IsCollisionNpc;
    [SerializeField] private bool m_IsTalkingNPC;
    [SerializeField] private bool m_HasAcceptQuest;
    [SerializeField] private float m_Distance;
    [SerializeField] private InputAction pressedButton;
    private bool isShowPopupGuide;
    private bool isHidePopupGuide;
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
    private void Start()
    {
        pressedButton.Enable();
        pressedButton.performed += (Context) => OnClickButton();

    }
    private void OnDestroy()
    {
        pressedButton.Disable();
        pressedButton.performed -= (Context) => OnClickButton();

    }
    private void Update()
    {
        CheckNPCDistance();
        //HandleInput();

    }
    private void OnClickButton()
    {
        if (AudioManager.HasInstance)
        {
            AudioManager.Instance.PlaySE("HoverSound");
        }
        HandleNPCInteraction();
        if (GameManager.HasInstance) GameManager.Instance.ShowCursor();
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.UI_CLICK_SHOWUI, null);
            ListenerManager.Instance.BroadCast(ListenType.CLICK_TALK_NPC, false);
        }


    }
    private void CheckNPCDistance()
    {
        if (DistanceWithNPC() <= m_Distance)
        {
            if (isShowPopupGuide) return;
            isHidePopupGuide = false;
            if (UIManager.HasInstance)
            {
                UIManager.Instance.ShowPopup<DialobGuidePopup>();
            }
            isShowPopupGuide = true;
            //m_IsCollisionNpc = true;

            //if (m_IsPressButtonJ && m_IsCollisionNpc)
            //{
            //    HandleNPCInteraction();
            //    m_IsPressButtonJ = false;
            //}
        }
        else
        {
            if (isHidePopupGuide) return;
            isShowPopupGuide = false;
            if (UIManager.HasInstance)
            {
                UIManager.Instance.HidePopup<DialobGuidePopup>();
            }

            isHidePopupGuide = true;
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

            int combinedMask = (1 << playerLayer) | (1 << weaponLayer) | (1 << fxLayer);
            if (camera != null)
            {
                camera.Priority = 15;
                //Camera.main.cullingMask &= ~combinedMask;
            }
        }
        CheckMission();
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
    private void ShowDialogPopupMission(DialogSystemSO dialogMission, QuestData questData = null)
    {
        if (UIManager.HasInstance)
        {
            UIManager.Instance.HidePopup<DialobGuidePopup>();
            isHidePopupGuide = true;
            if (DataManager.HasInstance)
            {
                if (SaveManager.HasInstance)
                {
                    var saveManager = SaveManager.Instance;
                    saveManager.SaveOrUpdateDialog(dialogMission);
                }

                //UIManager.Instance.ShowPopup<PopupDialogMission>(dataStateMission, true);
            }

        }
    }


    private void CheckMission()
    {
        if (npcObject.TryGetComponent(out QuestGiver questGiver))
        {
            if (questGiver.QuestData.QuestGiver == questGiver.NPCName)
            {
                if (UIManager.HasInstance)
                {
                    UIManager.Instance.HidePopup<DialobGuidePopup>();
                    isHidePopupGuide = true;
                }
                if (DialogManager.HasInstance)
                {
                    if (questGiver.QuestData.playerChoice == PlayerChoice.Reward)
                    {
                        if (UIManager.HasInstance)
                        {
                            for (int i = 0; i < questGiver.DialogSystemSO.dialogEntries.Count; i++)
                            {
                                if (questGiver.DialogSystemSO.dialogEntries[i].state == DialogState.Exit)
                                {
                                    var entry = questGiver.DialogSystemSO.dialogEntries[i];
                                    UIManager.Instance.ShowPopup<PopupDialogMission>(entry);
                                    if (ListenerManager.HasInstance)
                                    {
                                        ListenerManager.Instance.BroadCast(ListenType.SHOW_DIALOG_LINE, entry);
                                    }
                                    return;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!questGiver.IsSendQuestMission)
                        {
                            if (QuestManager.HasInstance)
                            {
                                QuestManager.Instance.CurrentQuest = questGiver.QuestData;
                                questGiver.IsSendQuestMission = true;
                                if (!DialogManager.Instance.IsPlaying)
                                {
                                    DialogManager.Instance.StartDialog(questGiver.DialogSystemSO);
                                }
                            }
                        }
                        if (QuestManager.Instance.CurrentQuest.isCompleteMission)
                        {
                            DialogManager.Instance.ShowNextDialog(DialogState.ChooseReward);
                        }
                        else
                        {
                            switch (QuestManager.Instance.CurrentQuest.playerChoice)
                            {
                                case PlayerChoice.Accept:
                                    DialogManager.Instance.ShowNextDialog(DialogState.Accept);
                                    break;
                                case PlayerChoice.Deny:
                                    DialogManager.Instance.ShowNextDialog(DialogState.Deny);
                                    break;
                                default:
                                    break;

                            }
                        }
                    }



                }

            }
        }
    }

}
