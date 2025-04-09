using Febucci.UI;
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
    private QuestData m_QuestDataMissionOne;
    private PlayerDialog m_PlayerDialog;
    private TypewriterByCharacter TypewriterByCharacter;
    private bool m_HasShownContent;
    private bool m_HasShownAlternative;
    private bool m_HasAcceptMission;
    private const string m_QuestID = "MainQuest01";

    private void Awake()
    {
        m_PlayerDialog = GameObject.Find("Player").GetComponent<PlayerDialog>();
        TypewriterByCharacter = m_ContentMission.GetComponent<TypewriterByCharacter>();
    }

    private void Start()
    {
        m_ButtonAccept.gameObject.SetActive(false);
        m_QuestDataMissionOne = DataManager.Instance.GetData<QuestData, QuestType>(QuestType.MainQuest);
        m_QuestDataMissionOne.isAcceptMission = false;
       
        m_ButtonAccept.onClick.AddListener(OnClickAcceptButton);
        m_ButtonExit.onClick.AddListener(OnButtonExit);
        TypewriterByCharacter.onTextShowed.AddListener(OnFinishedText);
        ShowContentMission();
    }
    //Sau khi text chạy hết
    private void OnFinishedText() // API của package Text Animator
    {
        m_ButtonAccept.gameObject.SetActive(true);
    }
    private void OnClickAcceptButton()
    {
        m_HasAcceptMission = true;
        if(ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.PLAYER_HAS_ACCEPT_QUEST, m_HasAcceptMission);
        }    
        if (QuestManager.HasInstance)
        {
            QuestManager.Instance.AcceptQuest(m_QuestDataMissionOne);
            if(GameManager.HasInstance)
            {
                GameObject npc = GameManager.Instance.GetNPC("Abe");
                m_QuestDataMissionOne.QuestGiver = npc;
            }
            m_PlayerDialog.SetIsTalkingNPC(false);
        }
        if (CameraManager.HasInstance)
        {
            CinemachineVirtualCameraBase cameraBase = CameraManager.Instance.GetCameraCinemachine("NPCCamera");
            if (cameraBase != null)
            {
                cameraBase.Priority = 8;
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
}
