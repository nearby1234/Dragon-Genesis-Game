using DG.Tweening;
using Febucci.UI;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupScrollMagic : BasePopup
{
    [SerializeField] private Animator m_Animator;
    //[SerializeField] private Button m_ExitBtn;
    [SerializeField] private HandleCanvasGroup m_RewardBtn;
    [SerializeField] private HandleCanvasGroup m_NextMisstionBtn;
    [SerializeField] private TypewriterByCharacter typewriterByCharacter;
    [SerializeField] private HandleCanvasGroup m_RewardItemParentObject;
    [SerializeField] private HandleCanvasGroup m_MisionItemParentObject;
    [SerializeField] private TextMeshProUGUI m_TitleText;
    [SerializeField] private TextMeshProUGUI m_ContentText;
    [InlineEditor]
    [SerializeField] private List<QuestItemSO> m_RewardItemObjectList;
    [InlineEditor]
    [SerializeField] private List<QuestItemSO> m_MissionItemObjectList;
    [InlineEditor]
    [SerializeField] private QuestData m_CurrentQuestData;
    private readonly string QUEST_ITEM_PREFAB_PATH = QuestManager.Instance.m_QuestItemPrefabPath;
    private readonly string QUEST_REWARD_ITEM_PREFAB_PATH = QuestManager.Instance.m_QuestRewardItemPrefabPath;
    private bool m_IsShowRewardBtn;

    private void Awake()
    {
        if (typewriterByCharacter != null) typewriterByCharacter.onTextShowed.AddListener(OnFinishWrittingText);
        m_Animator = GetComponent<Animator>();
        m_CurrentQuestData = QuestManager.Instance != null ? QuestManager.Instance.CurrentQuest : null;
    }

    private void Start()
    {
        m_Animator.Play("MoveOutSide");

        if (AudioManager.HasInstance)
        {
            AudioManager.Instance.PlaySE("ScrollSound");
        }
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.SE_ICONSCROLLMAGIC_ONCLICK, ReceiverPlayAnimMoveScroll);
            ListenerManager.Instance.Register(ListenType.UI_UPDATE_ITEM_MISSION, ReceiUpdateTextMission);
            ListenerManager.Instance.Register(ListenType.QUEST_COMPLETE, ReceiverEventIsCompleteQuest);
            ListenerManager.Instance.Register(ListenType.HIDE_SCOLLVIEW, ReceiverEventDisableUi);


        }

        //if (m_ExitBtn != null)
        //{
        //    m_ExitBtn.onClick.AddListener(OnClickBtnExitScrollView);
        //}
        else
        {
            Debug.LogWarning("Exit button is not assigned.");
        }

        if (m_RewardBtn != null) m_RewardBtn.GetComponent<Button>().onClick.AddListener(OnRewardBtnClick);
        else Debug.LogWarning("Reward button is not assigned.");

        if (m_NextMisstionBtn != null) m_NextMisstionBtn.GetComponent<Button>().onClick.AddListener(OnClickNextMission);
        else Debug.LogWarning("Next Mission button is not assigned.");
        if (m_CurrentQuestData == null)
        {
            Debug.LogError("Current quest data is null. Cannot initialize PopupScrollMagic.");
            return;
        }


        m_TitleText.text = m_CurrentQuestData.questName;
        m_ContentText.text = m_CurrentQuestData.description;

        m_RewardItemParentObject.HideCanvasGroup();
        m_MisionItemParentObject.HideCanvasGroup();
        m_RewardBtn.HideCanvasGroup();
        m_NextMisstionBtn.HideCanvasGroup();

        GetListItem(m_RewardItemObjectList, m_CurrentQuestData.bonus.itemsReward);
        InitItemObject(m_RewardItemObjectList, QUEST_REWARD_ITEM_PREFAB_PATH, m_RewardItemParentObject.transform, false);

        GetListItem(m_MissionItemObjectList, m_CurrentQuestData.ItemMission);
        InitItemObject(m_MissionItemObjectList, QUEST_ITEM_PREFAB_PATH, m_MisionItemParentObject.transform, true);
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.SE_ICONSCROLLMAGIC_ONCLICK, ReceiverPlayAnimMoveScroll);
            ListenerManager.Instance.Unregister(ListenType.QUEST_COMPLETE, ReceiverEventIsCompleteQuest);
            ListenerManager.Instance.Unregister(ListenType.HIDE_SCOLLVIEW, ReceiverEventDisableUi);
        }
    }

    private void OnFinishWrittingText()
    {
        m_RewardItemParentObject.ShowCanvasGroup();
        m_MisionItemParentObject.ShowCanvasGroup();
        if (m_CurrentQuestData.isCompleteMission)
        {
            m_RewardBtn.ShowCanvasGroup();
        }
        else
        {
            m_RewardBtn.HideCanvasGroup();
        }
        if(ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.SEND_QUESTMISSION_CURRENT, m_CurrentQuestData);
        }

    }
    //private void OnClickBtnExitScrollView()
    //{
    //    if (AudioManager.HasInstance)
    //    {
    //        AudioManager.Instance.PlaySE("ScrollSound");
    //    }
    //    m_Animator.Play("MoveCenter");
    //    if (GameManager.HasInstance)
    //    {
    //        GameManager.Instance.HideCursor();
    //    }
    //    if(ListenerManager.HasInstance)
    //    {
    //        ListenerManager.Instance.BroadCast(ListenType.UI_DISABLE_SHOWUI, null);
    //    }
    //    StartCoroutine(DelayHide());

    //}
    private void OnClickNextMission()
    {
        if (QuestManager.HasInstance)
        {
            QuestManager.Instance.AcceptQuest();
            m_CurrentQuestData = QuestManager.Instance.CurrentQuest;
            m_CurrentQuestData.isAcceptMission = true;
            UpdateUI();
        }
        if (AudioManager.HasInstance)
        {
            AudioManager.Instance.PlaySE("ScrollSound");
        }
        m_IsShowRewardBtn = false;
    }
    private void UpdateUI()
    {
        StartCoroutine(DelayAnimation());
    }

    private void GetListItem(List<QuestItemSO> questItems, List<QuestItemSO> questDatas)
    {
        questItems.AddRange(questDatas);
    }

    private void InitItemObject(List<QuestItemSO> questItems, string path, Transform parentTransform, bool isItemMission)
    {
        GameObject prefab = Resources.Load<GameObject>(path);
        if (prefab == null)
        {
            Debug.LogError($"Failed to load prefab at path: {path}");
            return;
        }

        foreach (var item in questItems)
        {
            GameObject itemObj = Instantiate(prefab, parentTransform);
            if (itemObj != null)
            {
                Image image = itemObj.GetComponent<Image>();
                if (image != null)
                {
                    image.sprite = item.questItemData.icon;
                }

                TextMeshProUGUI text = itemObj.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null)
                {
                    string displayText = item.questItemData.typeItem == TYPEITEM.ITEM_EXP
                         ? $"{item.questItemData.CountExp}"
                         : $"{item.questItemData.count}";

                    text.text = isItemMission
                        ? $"{item.questItemData.itemName} {item.questItemData.completionCount}/{item.questItemData.requestCount}"
                        : $"{item.questItemData.itemName} x {displayText}";
                }
            }
        }
    }

    // Phương thức chỉ gửi sự kiện thông báo đã bấm nút nhận phần thưởng
    private void OnRewardBtnClick()
    {
        if (AudioManager.HasInstance)
        {
            AudioManager.Instance.PlaySE("ClickSound");
        }
        // Gọi xử lý nhận phần thưởng từ QuestManager (Controller)
        QuestManager.Instance.GrantReward(m_CurrentQuestData.bonus);
        m_NextMisstionBtn.ShowCanvasGroup();
        // Update UI của Popup: disable button và thay đổi màu như logic ban đầu
        m_RewardBtn.GetComponent<Button>().interactable = false;
        if (m_RewardBtn.TryGetComponent<Image>(out Image buttonImage))
        {
            buttonImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        }

    }
    private void ShowRewardButton()
    {
        m_IsShowRewardBtn = true;
        m_RewardBtn.ShowCanvasGroup();
        m_RewardBtn.GetComponent<Button>().interactable = true;
        if (m_RewardBtn.TryGetComponent<Image>(out Image buttonImage))
        {
            buttonImage.color = new Color(1f, 1f, 1f, 1f);
        }
    }
    IEnumerator DelayHide()
    {
        yield return new WaitForSeconds(1f);
        this.Hide();
        if (UIManager.HasInstance)
        {
            UIManager.Instance.ShowScreen<ScreenOriginalScrollBtn>();
        }
        if (PlayerManager.HasInstance)
        {
            PlayerManager.instance.isInteractingWithUI = false;
        }
    }
    private void ReceiverPlayAnimMoveScroll(object value)
    {
        if (value is bool IsClick)
        {
            if (IsClick)
            {
                m_CurrentQuestData.isCompleteMission = true;
                m_Animator.Play("MoveOutSide");
                if (AudioManager.HasInstance)
                {
                    AudioManager.Instance.PlaySE("ScrollSound");
                }
            }

        }
    }
    private IEnumerator DelayAnimation()
    {
        // 1. Chờ animation MoveCenter
        yield return PlayAndWait("MoveCenter");

        // 2. Bật Shake, reset UI
        m_Animator.Play("Shake");
        ResetUI();

        // 3. Xóa và dựng lại items
        yield return ClearAndRebuildItems();

        // 4. Chờ animation Shake xong
        yield return PlayAndWait("Shake");

        if (AudioManager.HasInstance)
        {
            AudioManager.Instance.PlaySE("ScrollSound");
        }
        // 5. Quay về MoveOutSide và chờ xong
        yield return PlayAndWait("MoveOutSide");


        // 6. Cập nhật lại text và hiện UI
        RefreshUI();
    }

    // ———————— Helpers ————————

    private IEnumerator PlayAndWait(string stateName)
    {
        m_Animator.Play(stateName);
        yield return new WaitUntil(() =>
        {
            var st = m_Animator.GetCurrentAnimatorStateInfo(0);
            return st.IsName(stateName) && st.normalizedTime >= 1f;
        });
    }

    private void ResetUI()
    {
        m_TitleText.text = "";
        m_ContentText.text = "";
        m_RewardItemParentObject.HideCanvasGroup();
        m_MisionItemParentObject.HideCanvasGroup();
        m_RewardBtn.HideCanvasGroup();
        m_NextMisstionBtn.HideCanvasGroup();
    }

    private IEnumerator ClearAndRebuildItems()
    {
        // A. Destroy tất cả child
        for (int i = m_RewardItemParentObject.transform.childCount - 1; i >= 0; i--)
            Destroy(m_RewardItemParentObject.transform.GetChild(i).gameObject);
        for (int i = m_MisionItemParentObject.transform.childCount - 1; i >= 0; i--)
            Destroy(m_MisionItemParentObject.transform.GetChild(i).gameObject);

        // B. Clear cache và data
        m_RewardItemParentObject.ClearItemsList();
        m_MisionItemParentObject.ClearItemsList();
        m_RewardItemObjectList.Clear();
        m_MissionItemObjectList.Clear();

        // C. Đợi đến cuối frame để Unity hoàn tất Destroy()
        yield return new WaitForEndOfFrame();

        // D. Khởi tạo lại items mới
        GetListItem(m_RewardItemObjectList, m_CurrentQuestData.bonus.itemsReward);
        InitItemObject(m_RewardItemObjectList, QUEST_REWARD_ITEM_PREFAB_PATH, m_RewardItemParentObject.transform, false);

        GetListItem(m_MissionItemObjectList, m_CurrentQuestData.ItemMission);
        InitItemObject(m_MissionItemObjectList, QUEST_ITEM_PREFAB_PATH, m_MisionItemParentObject.transform, true);

        // E. Cập nhật cache RectTransform
        m_RewardItemParentObject.AddItemMission();
        m_MisionItemParentObject.AddItemMission();
    }

    private void RefreshUI()
    {
        m_TitleText.text = m_CurrentQuestData.questName;
        //m_ContentText.text = m_CurrentQuestData.description;
        if (typewriterByCharacter != null)
        {
            // Chạy lại typewriter cho nội dung mới
            typewriterByCharacter.ShowText(m_CurrentQuestData.description);
        }
        //OnFinishWrittingText();
    }

    private void ReceiUpdateTextMission(object value)
    {
        var first = m_CurrentQuestData.ItemMission.FirstOrDefault(x => x.questItemData.typeItem.Equals(TYPEITEM.ITEM_COLLECT));
        if (first == null) return;
        m_MisionItemParentObject.UpdateTextItemMission(first);
        //if (m_CurrentQuestData.isCompleteMission)
        //{
        //    ShowRewardButton();
        //}
    }

    private void ReceiverEventIsCompleteQuest(object value)
    {
        if (value is bool isComplete)
        {
            if (isComplete && !m_IsShowRewardBtn)
            {
                ShowRewardButton();
                m_IsShowRewardBtn = true;
            }
        }
    }
    private void ReceiverEventDisableUi(object value)
    {
        Debug.Log("ReceiverEventDisableUi");
        if (AudioManager.HasInstance)
        {
            AudioManager.Instance.PlaySE("ScrollSound");
        }
        m_Animator.Play("MoveCenter");
        if (GameManager.HasInstance)
        {
            GameManager.Instance.HideCursor();
        }
        //if (ListenerManager.HasInstance)
        //{
        //    ListenerManager.Instance.BroadCast(ListenType.UI_DISABLE_SHOWUI, null);
        //}
        StartCoroutine(DelayHide());
    }
    public void PlaySE()
    {
        if (AudioManager.HasInstance)
        {
            AudioManager.Instance.PlaySE("AcceptMision");
        }
    }
}
