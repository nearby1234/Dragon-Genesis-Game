using Febucci.UI;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;
public enum DialogState
{
    Default = 0,
    Accept,
    Deny,
    ChooseReward,
    Reward,
    Wait,
}
public class PopupDialogMission : BasePopup
{
    [SerializeField] private GameObject m_QuestMissionBar;
    [SerializeField] private Button m_ButtonAccept;
    [SerializeField] private TextMeshProUGUI m_AcceptTxt;
    [SerializeField] private Button m_ButtonDeny;
    [SerializeField] private TextMeshProUGUI m_DenyTxt;
    [SerializeField] private Button m_ButtonExit;
    [SerializeField] private TextMeshProUGUI m_ContentMission;
    [SerializeField] private TextMeshProUGUI m_ContentMissionElse;
    [SerializeField] private TextMeshProUGUI m_TitleMission;
    [SerializeField] private GridLayoutGroup m_GridLayoutButton;
    [SerializeField] private int basePaddingLeft;
    [SerializeField] private Transform rewardPartent;
    [SerializeField] private GameObject rewardPrefabs;
    [SerializeField] private TextMeshProUGUI titledRewardTxt;

    [InlineEditor]
    [SerializeField] private QuestData currentMission;
    [SerializeField] private DialogSystemSO systemSO;

    private PlayerDialog m_PlayerDialog;
    private TypewriterByCharacter TypewriterByCharacter;

    [ShowInInspector]
    private readonly List<RewardItem> itemRewardList = new();

    private void Awake()
    {
        m_PlayerDialog = GameObject.Find("Player").GetComponent<PlayerDialog>();
        TypewriterByCharacter = m_ContentMission.GetComponent<TypewriterByCharacter>();
    }

    private void Start()
    {
        basePaddingLeft = m_GridLayoutButton.padding.left;
        SetShowButton(false, false, false);
        ShowCanvasGroup(rewardPartent, false);
        ShowCanvasGroup(titledRewardTxt.transform, false);
        m_ButtonExit.onClick.AddListener(OnButtonExit);
        TypewriterByCharacter.onTextShowed.AddListener(OnFinishedText);
        UpdateTitleReward();
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.CLICK_BUTTON_MAINMENU, ReceiverEventClickMainMenu);
            ListenerManager.Instance.Register(ListenType.QUEST_COMPLETE, OnEventQuestComplete);
            ListenerManager.Instance.Register(ListenType.ITEM_CHOSED, OnEventItemChosed);
            ListenerManager.Instance.Register(ListenType.ITEM_DISABLE_CHOSED, OnEventDisableItemChosed);
        }
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.CLICK_BUTTON_MAINMENU, ReceiverEventClickMainMenu);
            ListenerManager.Instance.Unregister(ListenType.QUEST_COMPLETE, OnEventQuestComplete);
            ListenerManager.Instance.Unregister(ListenType.ITEM_CHOSED, OnEventItemChosed);
            ListenerManager.Instance.Unregister(ListenType.ITEM_DISABLE_CHOSED, OnEventDisableItemChosed);
        }
    }

    public override void Show(object data)
    {
        base.Show(data);
        if (data == null) return;
        if (data is DataStateMission dataStateMission)
        {
            currentMission = dataStateMission.questData;
            systemSO = dataStateMission.dialogSystemSO;
            //ClearAllButtonListeners();
            switch (dataStateMission.dialogSystemSO.dialogMission)
            {
                case DialogMission.DialogMissionFirst:
                    {
                        if (!dataStateMission.isCompleteMission)
                        {
                            m_TitleMission.text = systemSO.DialogTitle;
                            SetupInitialDialog();
                        }
                        else
                        {
                            m_TitleMission.text = systemSO.DialogTitle;
                            SetupRewardDialog();
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }


    //Sau khi text chạy hết
    private void OnFinishedText() // API của package Text Animator
    {
        switch (systemSO.currentDialogState)
        {
            case DialogState.Accept:
                SetShowButton(false, false, true);
                break;
            case DialogState.Deny:
                SetShowButton(false, false, true);
                break;

            case DialogState.ChooseReward:
                SetShowButton(false, true, false);
                SetAlightmentButton(true);
                break;
            case DialogState.Reward:
                SetShowButton(false, true, false);
                SetAlightmentButton(true);
                break;

            case DialogState.Wait:
                SetShowButton(true, true, false);
                SetAlightmentButton(false);
                break;
            default:
                SetShowButton(true, true, false);
                SetAlightmentButton(false);
                break;
        }
    }
    private void SetAlightmentButton(bool isPadding)
    {
        if (isPadding) m_GridLayoutButton.padding.left = (int)systemSO.alighnmentLeftChoseRewardButton;
        else m_GridLayoutButton.padding.left = basePaddingLeft;
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_GridLayoutButton.GetComponent<RectTransform>());
    }
    private void SetState(DialogState dialogState)
    {
        systemSO.currentDialogState = dialogState;
    }

    private void OnButtonExit()
    {
        if (AudioManager.HasInstance)
        {
            AudioManager.Instance.PlaySE("ExitSound");
        }
        m_PlayerDialog.SetIsTalkingNPC(false);
        SettingCamera();
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.UI_DISABLE_SHOWUI, null);
            ListenerManager.Instance.BroadCast(ListenType.CLICK_TALK_NPC, false);
        }
        if (GameManager.HasInstance)
        {
            GameManager.Instance.HideCursor();
        }
        this.Hide();
    }
    private void ReceiverEventClickMainMenu(object value)
    {
        currentMission.isAcceptMission = false;
        m_PlayerDialog.HasAcceptQuest = false;
        //m_HasShownContent = false;
        //m_HasShownAlternative = false;
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
    private void SetShowButton(bool isShowAcceptBtn, bool isShowDenyBtn, bool isShowExitBtn)
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
        if (m_ButtonExit.TryGetComponent(out CanvasGroup canvasGroupExit))
        {
            canvasGroupExit.alpha = isShowExitBtn ? 1f : 0f;
            canvasGroupExit.interactable = isShowExitBtn;
            canvasGroupExit.blocksRaycasts = isShowExitBtn;
        }
    }
    void ClearAllButtonListeners()
    {
        m_ButtonAccept.onClick.RemoveAllListeners();
        m_ButtonDeny.onClick.RemoveAllListeners();
        m_ButtonExit.onClick.RemoveAllListeners();
    }

    void PlayVoiceIntro(string nameVoice)
    {
        if (AudioManager.HasInstance)
        {
            AudioManager.Instance.PlayVoiceSe(nameVoice);
        }
    }
    void PlaySound(string nameSound)
    {
        if (AudioManager.HasInstance)
        {
            AudioManager.Instance.PlaySE(nameSound);
        }
    }
    void SetVoiceIntro()
    {
        if (systemSO.currentDialogState == DialogState.Deny) PlayVoiceIntro("AbeVoice2");
        else if (systemSO.currentDialogState == DialogState.Accept) PlayVoiceIntro("AbeVoice3");
    }
    void SetupInitialDialog()
    {
        // 1) gán text cho 3 label
        m_AcceptTxt.text = systemSO.contentAcceptButton;
        m_DenyTxt.text = systemSO.contentDenyButton;
        m_ContentMission.text = systemSO.DialogContent;
        PlayVoiceIntro("AbeVoice1");

        // 2) gán listener
        m_ButtonDeny.onClick.AddListener(OnDenyInitial);
        m_ButtonAccept.onClick.AddListener(OnAcceptInitial);

    }
    private void SetContent(string content)
    {
        m_ContentMission.text = content;
    }
    void SetupRewardDialog()
    {
        PlayVoiceIntro("AbeVoice4");
        // đổi nội dung và button
        m_ContentMission.text = systemSO.DialogReward;
        m_DenyTxt.text = systemSO.contentChoseRewardButton;
        SetState(DialogState.ChooseReward);
        OnFinishedText();
        m_ButtonDeny.onClick.RemoveAllListeners();
        m_ButtonDeny.onClick.AddListener(OnChooseReward);

    }
    void OnDenyInitial()
    {
        PlaySound("ClickSound");
        SetState(DialogState.Deny);
        SetContent(systemSO.dialogClickDenyButton);
        SetVoiceIntro();
        BroadcastQuestAccepted();
        ShowUI();
        OnFinishedText();
    }
    void OnAcceptInitial()
    {
        PlaySound("ClickSound");
        SetState(DialogState.Accept);
        SetContent(systemSO.dialogClickAcceptButton);
        SetVoiceIntro();
        BroadcastQuestAccepted();
        ShowUI();
        OnFinishedText();
    }
    void OnChooseReward()
    {
        PlaySound("ClickSound");
        ShowCanvasGroup(rewardPartent, true);
        ShowCanvasGroup(titledRewardTxt.transform, true);
        InitItemPrefabs();
        SetState(DialogState.Reward);
        m_DenyTxt.text = systemSO.contentRewardButton;
        m_ButtonDeny.onClick.RemoveAllListeners();
        m_ButtonDeny.onClick.AddListener(OnReward);
    }

    void OnReward()
    {
        PlaySound("ClickSound");
        SetState(DialogState.Default);
        if (QuestManager.HasInstance)
        {
            QuestManager.Instance.GrantReward(itemRewardList);
        }
    }
    void BroadcastQuestAccepted()
    {
        if (!ListenerManager.HasInstance) return;
        ListenerManager.Instance.BroadCast(ListenType.SEND_QUESTMISSION_CURRENT, currentMission);
        ListenerManager.Instance.BroadCast(ListenType.PLAYER_HAS_ACCEPT_QUEST, true);
    }
    void ShowUI()
    {
        if (UIManager.HasInstance)
        {
            UIManager.Instance.ShowNotify<NotifyMission>(currentMission, true);
            NotifyMessageMission<PopupDialogMission> notifyMessageMission = new()
            {
                uiElement = this,
                questData = currentMission,
            };
            UIManager.Instance.ShowNotify<NotifySystem>(notifyMessageMission, true);
        }
    }
    void ShowCanvasGroup(Transform transform, bool isShow)
    {
        if (transform.TryGetComponent(out CanvasGroup canvasGroup))
        {
            canvasGroup.alpha = isShow ? 1f : 0f;
            canvasGroup.interactable = isShow;
            canvasGroup.blocksRaycasts = isShow;
        }
    }
    private void OnEventQuestComplete(object value)
    {
        if (value is bool isQuestComplete)
        {
            currentMission.isCompleteMission = isQuestComplete;
        }
    }

    void InitItemPrefabs()
    {
        for (int i = 0; i < currentMission.bonus.itemsReward.Count; i++)
        {
            GameObject gameObject = Instantiate(rewardPrefabs, rewardPartent.transform);
            gameObject.name = $"{currentMission.bonus.itemsReward[i].questItemData.itemName}";
            if (gameObject.TryGetComponent(out RewardItem item))
            {
                item.SetItem(currentMission.bonus.itemsReward[i]);
                item.InitItem();
            }
        }
    }
    private void UpdateTitleReward()
    {
        titledRewardTxt.text = $"Hãy chọn 4 món trong các phần thường dưới đây : {itemRewardList.Count} / 4";
    }
    private void OnEventItemChosed(object value)
    {

        if (value is RewardItem item)
        {
            if (!itemRewardList.Find(x => x.CurrentItem.questItemData.itemID == item.CurrentItem.questItemData.itemID))
            {
                itemRewardList.Add(item);
                UpdateTitleReward();
                if (itemRewardList.Count >= 4)
                {
                    if (ListenerManager.HasInstance)
                    {
                        ListenerManager.Instance.BroadCast(ListenType.FULL_LIST_ITEM_REWARD, true);
                    }
                    return;
                }
            }
        }
    }
    private void OnEventDisableItemChosed(object value)
    {
        if (value is RewardItem item)
        {
            if (itemRewardList.Find(x => x.CurrentItem.questItemData.itemID == item.CurrentItem.questItemData.itemID))
            {
                itemRewardList.Remove(item);
                UpdateTitleReward();
                if (itemRewardList.Count < 4)
                    if (ListenerManager.HasInstance)
                    {
                        ListenerManager.Instance.BroadCast(ListenType.FULL_LIST_ITEM_REWARD, false);
                    }
            }
        }
    }
}
