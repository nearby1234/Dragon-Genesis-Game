using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PopupScrollMagic : BasePopup
{
    [SerializeField] private Button m_ExitBtn;
    [SerializeField] private Button m_RewardBtn;
    [SerializeField] private Transform m_RewardItemParentObject;
    [SerializeField] private Transform m_MisionItemParentObject;
    [SerializeField] private TextMeshProUGUI m_TitleText;
    [SerializeField] private TextMeshProUGUI m_ContentText;
    [SerializeField] private List<QuestItemSO> m_RewardItemObjectList;
    [SerializeField] private List<QuestItemSO> m_MissionItemObjectList;
    [SerializeField] private QuestData m_CurrentQuestData;
    private readonly string QUEST_ITEM_PREFAB_PATH = QuestManager.Instance.m_QuestItemPrefabPath;
    private readonly string QUEST_REWARD_ITEM_PREFAB_PATH = QuestManager.Instance.m_QuestRewardItemPrefabPath;
    private readonly string DO_ITEM_PREFAB_PATH = QuestManager.Instance.m_DOItemPrefabPath;
    private readonly Vector2 TWEEN_TARGET_POS = new(-240f, -300f);
    private const float TWEEN_DURATION = 2f;


    private void Awake()
    {
        m_CurrentQuestData = QuestManager.Instance?.CurrentQuest;
    }

    private void Start()
    {
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
            m_RewardBtn.onClick.AddListener(GetSpriteItemReward);
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
                // Safely get the Image component
                Image image = itemObj.GetComponent<Image>();
                if (image != null)
                {
                    image.sprite = item.questItemData.icon;
                }

                // Safely get the TextMeshProUGUI component from children
                TextMeshProUGUI text = itemObj.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null)
                {
                    text.text = isItemMission
                        ? $"{item.questItemData.itemName} {item.questItemData.completionCount}/{item.questItemData.requestCount}"
                        : $"{item.questItemData.itemName} x {item.questItemData.count}";
                }
            }
        }
    }

    private void GetSpriteItemReward()
    {
        GameObject prefab = Resources.Load<GameObject>(DO_ITEM_PREFAB_PATH);
        if (prefab == null)
        {
            Debug.LogError("Failed to load prefab for reward items.");
            return;
        }

        foreach (var item in m_RewardItemObjectList)
        {
            GameObject itemObj = Instantiate(prefab, m_RewardItemParentObject);
            if (itemObj.TryGetComponent<Image>(out Image image))
            {
                image.sprite = item.questItemData.icon;
            }

            if (itemObj.TryGetComponent<RectTransform>(out RectTransform rectTransform))
            {
                rectTransform.DOAnchorPos(TWEEN_TARGET_POS, TWEEN_DURATION).SetEase(Ease.OutBack).onComplete += () =>
                {
                    if (m_CurrentQuestData.questID == "MainQuest01" && UIManager.HasInstance)
                    {
                        UIManager.Instance.ShowScreen<ScreenIconInventory>();
                        UIManager.Instance.ShowScreen<ScreenBox>();
                    }
                    Destroy(itemObj);
                };
            }
        }

        m_RewardBtn.interactable = false;
        if (m_RewardBtn.TryGetComponent<Image>(out Image buttonImage))
        {
            buttonImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        }

        m_CurrentQuestData.isCompleteMission = true;
        ListenerManager.Instance?.BroadCast(ListenType.UI_SEND_LIST_ITEM_REWARD, m_RewardItemObjectList);
    }
}
