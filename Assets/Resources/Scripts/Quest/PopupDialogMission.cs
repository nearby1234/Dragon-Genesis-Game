using Febucci.UI;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class PopupDialogMission : BasePopup
{
    [SerializeField] private GameObject m_QuestMissionBar;
    [SerializeField] private GameObject m_QuestMissionBarElse;
    [SerializeField] private Button m_ButtonAccept;
    [SerializeField] private TextMeshProUGUI m_AcceptTxt;
    [SerializeField] private Button m_ButtonDeny;
    [SerializeField] private TextMeshProUGUI m_DenyTxt;
    [SerializeField] private Button m_ButtonExit;
    [SerializeField] private TextMeshProUGUI m_ContentMission;
    [SerializeField] private TextMeshProUGUI m_ContentMissionElse;
    [SerializeField] private TextMeshProUGUI m_TitleMission;
    [InlineEditor]
    [SerializeField] private QuestData currentMission;
    [SerializeField] private DialogSystemSO systemSO;
    private PlayerDialog m_PlayerDialog;
    private TypewriterByCharacter TypewriterByCharacter;
    private bool m_HasShownContent;
    [SerializeField] private bool m_HasShownAlternative;
    private bool m_HasAcceptMission;
    private const string m_QuestID = "-MainQuest01";

    private void Awake()
    {
        m_PlayerDialog = GameObject.Find("Player").GetComponent<PlayerDialog>();
        TypewriterByCharacter = m_ContentMission.GetComponent<TypewriterByCharacter>();
    }

    private void Start()
    {
        SetShowButton(false, false);
        //m_ButtonAccept.gameObject.SetActive(false);
        //m_QuestDataMissionOne = DataManager.Instance.GetDataByID<QuestData,QuestType>("-QuestMissionOne");
        currentMission = QuestManager.Instance.questList[0];
        currentMission.isAcceptMission = false;

        m_ButtonAccept.onClick.AddListener(OnClickAcceptButton);
        m_ButtonExit.onClick.AddListener(OnButtonExit);
        TypewriterByCharacter.onTextShowed.AddListener(OnFinishedText);
        ShowContentMission();
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.CLICK_BUTTON_MAINMENU, ReceiverEventClickMainMenu);
        }
    }
    public override void Show(object data)
    {
        base.Show(data);
        if (data is DialogSystemSO dialog)
        {
            systemSO = dialog;
            switch (dialog.dialogMission)
            {
                case DialogMission.DialogMissionFirst:
                    {
                        m_ButtonDeny.onClick.AddListener(() =>
                        {
                            if (AudioManager.HasInstance)
                            {
                                AudioManager.Instance.PlaySE("ClickSound");
                            }
                            systemSO.isClickDenyButton = true;
                           
                            systemSO.OnClickDenyButton?.Invoke();
                        });

                        if (systemSO.isClickDenyButton)
                        {
                            SetShowButton(false, false);
                            Debug.Log("isClickDenyButton: " + systemSO.isClickDenyButton);
                            m_ContentMission.text = systemSO.dialogClickDenyButton;
                            return;
                        }

                        m_TitleMission.text = systemSO.DialogTitle;
                        m_AcceptTxt.text = systemSO.contentAcceptButton;
                        m_DenyTxt.text = systemSO.contentDenyButton;
                        m_ContentMission.text = systemSO.DialogContent;

                    }
                    break;
                default:
                    break;
            }
        }
    }
    private void Update()
    {
        ShowContentMission();
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.CLICK_BUTTON_MAINMENU, ReceiverEventClickMainMenu);
        }
    }
    //Sau khi text chạy hết
    private void OnFinishedText() // API của package Text Animator
    {
        if(systemSO.isClickDenyButton)
        {
            SetShowButton(false, false);
            return;
        }
        SetShowButton(true, true);
    }
    private void OnClickAcceptButton()
    {
        this.Hide();
        if (AudioManager.HasInstance)
        {
            AudioManager.Instance.PlaySE("ClickSound");
        }
        if (GameManager.HasInstance) GameManager.Instance.HideCursor();
        m_HasAcceptMission = true;
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.PLAYER_HAS_ACCEPT_QUEST, m_HasAcceptMission);
            ListenerManager.Instance.BroadCast(ListenType.UI_DISABLE_SHOWUI, null);
        }
        if (QuestManager.HasInstance)
        {
            QuestManager.Instance.AcceptQuest(currentMission);
            foreach (var item in currentMission.ItemMission)
            {
                if (item.questItemData.itemID.Equals("-ScrollGenesis"))
                {
                    item.questItemData.completionCount = 1;
                }
            }
            m_PlayerDialog.SetIsTalkingNPC(false);
        }
        SettingCamera();
        if (UIManager.HasInstance)
        {
            NotifyMessageMission<PopupDialogMission> notifyMessageMission = new()
            {
                uiElement = this,
                questData = currentMission,
            };
            UIManager.Instance.ShowScreen<ScreenOriginalScrollBtn>();
            UIManager.Instance.ShowNotify<NotifySystem>(notifyMessageMission, true);
            UIManager.Instance.ShowNotify<NotifyMission>();
        }
        if (PlayerManager.HasInstance)
        {
            PlayerManager.instance.isInteractingWithUI = false;
        }
        if (currentMission != null && currentMission.questID.Equals(m_QuestID))
        {
            currentMission.ItemMission[0].questItemData.completionCount = 1;
        }
    }
    private void OnButtonExit()
    {
        if (AudioManager.HasInstance)
        {
            AudioManager.Instance.PlaySE("ExitSound");
        }
        m_PlayerDialog.SetIsTalkingNPC(false);
        SettingCamera();
        this.Hide();
    }
    private void ShowContentMission()
    {
        if (currentMission != null)
        {
            if (!currentMission.isAcceptMission && !m_PlayerDialog.HasAcceptQuest)
            {
                if (!m_HasShownContent)
                {
                    ShowInitialMissionContent();
                }
            }
            else
            {
                if (!m_HasShownAlternative)
                {
                    ShowAlternativeContent();
                }
            }
        }
    }
    private void ShowInitialMissionContent()
    {
        //m_TitleMission.text = currentMission.questName;
        //m_ContentMission.text = currentMission.description;
        m_QuestMissionBar.SetActive(true);
        m_HasShownContent = true;
    }
    private void ShowAlternativeContent()
    {
        m_QuestMissionBar.SetActive(false);
        //m_ContentMissionElse.text = currentMission.descriptionElse;
        m_QuestMissionBarElse.SetActive(true);
        m_HasShownAlternative = true;
    }
    private void ReceiverEventClickMainMenu(object value)
    {
        currentMission.isAcceptMission = false;
        m_PlayerDialog.HasAcceptQuest = false;
        m_HasShownContent = false;
        m_HasShownAlternative = false;
    }
    private void SettingCamera()
    {
        if (CameraManager.HasInstance)
        {
            CinemachineVirtualCameraBase cameraBase = CameraManager.Instance.GetCameraCinemachine("NPCCamera");
            if (cameraBase != null)
            {
                cameraBase.Priority = 8;
                int playerLayer = LayerMask.NameToLayer("Player");
                int weaponLayer = LayerMask.NameToLayer("Weapon");
                int fxLayer = LayerMask.NameToLayer("FX");

                int combinedMask = (1 << playerLayer) | (1 << weaponLayer) | (1 << fxLayer);
                Camera.main.cullingMask |= combinedMask;
            }
        }
    }
    private void SetShowButton(bool isShowAcceptBtn, bool isShowDenyBtn)
    {
        if (m_ButtonAccept.TryGetComponent(out CanvasGroup canvasGroupAccept))
        {
            canvasGroupAccept.alpha = isShowAcceptBtn ? 1f : 0f;
            canvasGroupAccept.interactable = isShowAcceptBtn;
            canvasGroupAccept.blocksRaycasts = isShowAcceptBtn;
        }
        if (m_ButtonDeny.TryGetComponent(out CanvasGroup canvasGroupDeny))
        {
            canvasGroupDeny.alpha = isShowDenyBtn ? 1f : 0f;
            canvasGroupDeny.interactable = isShowDenyBtn;
            canvasGroupDeny.blocksRaycasts = isShowDenyBtn;
        }
    }

}
