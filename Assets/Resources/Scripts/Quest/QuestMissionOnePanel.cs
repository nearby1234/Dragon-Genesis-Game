using Febucci.UI;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class QuestMissionOnePanel : BasePopup
{
    [SerializeField] private GameObject m_QuestMissionBar;
    [SerializeField] private GameObject m_QuestMissionBarElse;
    [SerializeField] private Button m_ButtonAccept;
    [SerializeField] private Button m_ButtonExit;
    [SerializeField] private TextMeshProUGUI m_ContentMission;
    [SerializeField] private TextMeshProUGUI m_ContentMissionElse;
    [SerializeField] private TextMeshProUGUI m_TitleMission;
    [InlineEditor]
    [SerializeField] private QuestData m_QuestDataMissionOne;
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
        m_ButtonAccept.gameObject.SetActive(false);
        //m_QuestDataMissionOne = DataManager.Instance.GetDataByID<QuestData,QuestType>("-QuestMissionOne");
        m_QuestDataMissionOne = QuestManager.Instance.questList[0];
        m_QuestDataMissionOne.isAcceptMission = false;
       
        m_ButtonAccept.onClick.AddListener(OnClickAcceptButton);
        m_ButtonExit.onClick.AddListener(OnButtonExit);
        TypewriterByCharacter.onTextShowed.AddListener(OnFinishedText);
        ShowContentMission();
        if(ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.CLICK_BUTTON_MAINMENU, ReceiverEventClickMainMenu);
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
        m_ButtonAccept.gameObject.SetActive(true);
    }
    private void OnClickAcceptButton()
    {
        if(AudioManager.HasInstance)
        {
            AudioManager.Instance.PlaySE("ClickSound");
        }
        if(GameManager.HasInstance) GameManager.Instance.HideCursor();
        m_HasAcceptMission = true;
        if(ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.PLAYER_HAS_ACCEPT_QUEST, m_HasAcceptMission);
            ListenerManager.Instance.BroadCast(ListenType.UI_DISABLE_SHOWUI, null);
        }    
        if (QuestManager.HasInstance)
        {
            QuestManager.Instance.AcceptQuest(m_QuestDataMissionOne);
            foreach(var item in m_QuestDataMissionOne.ItemMission)
            {
                if (item.questItemData.itemID.Equals("-ScrollGenesis"))
                {
                    item.questItemData.completionCount = 1;
                }
            }
            m_PlayerDialog.SetIsTalkingNPC(false);
        }
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
            this.Hide();
        }
        if(UIManager.HasInstance)
        {
            UIManager.Instance.ShowScreen<ScreenOriginalScrollBtn>();
        }
        if (PlayerManager.HasInstance)
        {
            PlayerManager.instance.isInteractingWithUI = false;
        }
        if(m_QuestDataMissionOne != null && m_QuestDataMissionOne.questID.Equals(m_QuestID))
        {
            m_QuestDataMissionOne.ItemMission[0].questItemData.completionCount = 1;
        }
    }
    private void OnButtonExit()
    {
        if (AudioManager.HasInstance)
        {
            AudioManager.Instance.PlaySE("ExitSound");
        }
        m_PlayerDialog.SetIsTalkingNPC(false);
        if (CameraManager.HasInstance)
        {
            CinemachineVirtualCameraBase cameraBase = CameraManager.Instance.GetCameraCinemachine("NPCCamera");
            if (cameraBase != null)
            {
                cameraBase.Priority = 8;
            }
            this.Hide();
        }
    }   
    private void ShowContentMission()
    {
        if (m_QuestDataMissionOne != null)
        {
            if (!m_QuestDataMissionOne.isAcceptMission && !m_PlayerDialog.HasAcceptQuest )
            {
                if(!m_HasShownContent)
                {
                    ShowInitialMissionContent();
                }
            }
            else
            {
                if(!m_HasShownAlternative)
                {
                    ShowAlternativeContent();
                }
            }
        }
    }
    private void ShowInitialMissionContent()
    {
        m_TitleMission.text = m_QuestDataMissionOne.questName;
        m_ContentMission.text = m_QuestDataMissionOne.description;
        m_QuestMissionBar.SetActive(true);
        m_HasShownContent = true;
    }
    private void ShowAlternativeContent()
    {
        m_QuestMissionBar.SetActive(false);
        m_ContentMissionElse.text = m_QuestDataMissionOne.descriptionElse;
        m_QuestMissionBarElse.SetActive(true);
        m_HasShownAlternative = true;
    }
    private void ReceiverEventClickMainMenu(object value)
    {
        m_QuestDataMissionOne.isAcceptMission = false;
        m_PlayerDialog.HasAcceptQuest = false;
        m_HasShownContent = false;
        m_HasShownAlternative = false;
    }
}
