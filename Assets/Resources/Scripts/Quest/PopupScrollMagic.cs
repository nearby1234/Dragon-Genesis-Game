using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupScrollMagic : BasePopup
{
    [SerializeField] private Animator m_Animator;
    [SerializeField] private Button m_ExitBtn;
    [SerializeField] private Button m_RewardBtn;
    [SerializeField] private Button m_NextMisstionBtn;
    [SerializeField] private Transform m_RewardItemParentObject;
    [SerializeField] private Transform m_MisionItemParentObject;
    [SerializeField] private TextMeshProUGUI m_TitleText;
    [SerializeField] private TextMeshProUGUI m_ContentText;
    [SerializeField] private List<QuestItemSO> m_RewardItemObjectList;
    [SerializeField] private List<QuestItemSO> m_MissionItemObjectList;
    [SerializeField] private QuestData m_CurrentQuestData;
    private readonly string QUEST_ITEM_PREFAB_PATH = QuestManager.Instance.m_QuestItemPrefabPath;
    private readonly string QUEST_REWARD_ITEM_PREFAB_PATH = QuestManager.Instance.m_QuestRewardItemPrefabPath;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_CurrentQuestData = QuestManager.Instance?.CurrentQuest;
    }

    private void Start()
    {
        m_Animator.Play("MoveOutSide");

        if (m_ExitBtn != null)
        {
            m_ExitBtn.onClick.AddListener(OnClickBtnExitScrollView);
        }
        else
        {
            Debug.LogWarning("Exit button is not assigned.");
        }

        if (m_RewardBtn != null)
        {
            // Khi nhận click, chỉ gửi sự kiện thông báo nhận phần thưởng qua QuestManager
            m_RewardBtn.onClick.AddListener(OnRewardBtnClick);
        }
        else
        {
            Debug.LogWarning("Reward button is not assigned.");
        }

        if (m_CurrentQuestData == null)
        {
            Debug.LogError("Current quest data is null. Cannot initialize PopupScrollMagic.");
            return;
        }

        m_TitleText.text = m_CurrentQuestData.questName;
        m_ContentText.text = m_CurrentQuestData.description;

        GetListItem(m_RewardItemObjectList, m_CurrentQuestData.bonus.itemsReward);
        InitItemObject(m_RewardItemObjectList, QUEST_REWARD_ITEM_PREFAB_PATH, m_RewardItemParentObject, false);

        GetListItem(m_MissionItemObjectList, m_CurrentQuestData.ItemMission);
        InitItemObject(m_MissionItemObjectList, QUEST_ITEM_PREFAB_PATH, m_MisionItemParentObject, true);
    }

    public void OnClickBtnExitScrollView()
    {
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
        // Gọi xử lý nhận phần thưởng từ QuestManager (Controller)
        QuestManager.Instance.GrantReward(m_CurrentQuestData.bonus);

        // Update UI của Popup: disable button và thay đổi màu như logic ban đầu
        m_RewardBtn.interactable = false;
        if (m_RewardBtn.TryGetComponent<Image>(out Image buttonImage))
        {
            buttonImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        }
    }
}
