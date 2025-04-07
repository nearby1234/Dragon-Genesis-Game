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
    [SerializeField] private List<QuestItem> m_RewardItemObjectList;
    [SerializeField] private List<QuestItem> m_MissionItemObjectList;
    [SerializeField] private QuestData m_CurrentQuestData;
    private readonly string m_QuestItemPrefabPath = QuestManager.Instance.m_QuestItemPrefabPath;
    private readonly string m_QuestRewardItemPrefabPath = QuestManager.Instance.m_QuestRewardItemPrefabPath;
    private readonly string m_DOItemPrefabPath = QuestManager.Instance.m_DOItemPrefabPath;


    private void Awake()
    {
        if (QuestManager.HasInstance)
        {
            m_CurrentQuestData = QuestManager.Instance.CurrentQuest;
        }
    }
    private void Start()
    {
       
        if (m_ExitBtn != null)
        {
            m_ExitBtn.onClick.AddListener(OnClickBtnExitScrollView);
        }
        else Debug.Log("khong getcomponent Button duoc");
        if (m_RewardBtn != null)
        {
            m_RewardBtn.onClick.AddListener(GetSpriteItemReward);
        }
        else Debug.Log("khong getcomponent Button duoc");

        m_TitleText.text = m_CurrentQuestData.questName;
        m_ContentText.text = m_CurrentQuestData.description;
        GetListItem(m_RewardItemObjectList, m_CurrentQuestData.bonus.itemsReward);
        InitItemObject(m_RewardItemObjectList,  m_QuestRewardItemPrefabPath, m_RewardItemParentObject, false);
        GetListItem(m_MissionItemObjectList, m_CurrentQuestData.ItemMission);
        InitItemObject(m_MissionItemObjectList, m_QuestItemPrefabPath, m_MisionItemParentObject, true);
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
    private void GetListItem(List<QuestItem> questItems, List<QuestItem> questDatas)
    {
        foreach (var item in questDatas)
        {
            questItems.Add(item);
        }
    }
    private void InitItemObject(List<QuestItem> questItems, string path, Transform transform, bool IsItemMission)
    {
        foreach (var item in questItems)
        {
            GameObject obj = Resources.Load<GameObject>(path);
            if (obj != null)
            {
                GameObject itemObj = Instantiate(obj, transform);
                if (itemObj != null)
                {
                    Image image = itemObj.GetComponent<Image>();
                    TextMeshProUGUI text = itemObj.GetComponentInChildren<TextMeshProUGUI>();
                    if (image != null && text != null)
                    {
                        image.sprite = item.icon;
                        if (IsItemMission)
                        {
                            text.text = $"{item.itemName} {item.completionCount}/{item.requestCount}";
                        }
                        else
                        {
                            text.text = $"{item.itemName} x {item.count}";
                        }
                    }
                }
            }
            else
            {
                Debug.Log("khong load duoc prefab");
            }
        }
    }
    private void GetSpriteItemReward()
    {
        // Nếu không load được prefab thì log và thoát luôn
        GameObject obj = Resources.Load<GameObject>(m_DOItemPrefabPath);
        if (obj == null)
        {
            Debug.Log("khong load duoc prefab");
            return;
        }
        // Cache target position và tween duration nếu giá trị không đổi
        Vector2 targetPos = new(-240f, -300f);
        float tweenDuration = 2f;

        foreach (var item in m_RewardItemObjectList)
        {
            GameObject itemObj = Instantiate(obj, m_RewardItemParentObject);
            if (itemObj != null)
            {
                // Sử dụng TryGetComponent cho gọn
                if (itemObj.TryGetComponent<Image>(out Image image))
                {
                    image.sprite = item.icon;
                }

                if (itemObj.TryGetComponent<RectTransform>(out RectTransform rectTransform))
                {
                    rectTransform.DOAnchorPos(targetPos, tweenDuration).SetEase(Ease.OutBack).onComplete += () =>
                    {
                        // Nếu questID bằng MainQuest01 thì hiển thị ScreenIconInventory
                        if (m_CurrentQuestData.questID.Equals("MainQuest01"))
                        {
                            if (UIManager.HasInstance)
                            {
                                UIManager.Instance.ShowScreen<ScreenIconInventory>();
                                UIManager.Instance.ShowScreen<ScreenBox>();
                            }
                        }
                        // Hủy đối tượng sau khi tween hoàn thành
                        Destroy(itemObj);
                    };
                }
            }
        }

        m_RewardBtn.interactable = false;
        // Thay vì TryGetComponent theo kiểu cũ, có thể dùng luôn
        if (m_RewardBtn.TryGetComponent<Image>(out Image imageBtn))
        {
            imageBtn.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        }
        if (m_CurrentQuestData != null)
        {
            m_CurrentQuestData.isCompleteMission = true;
        }
        if(ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.UI_SEND_LIST_ITEM_REWARD, m_RewardItemObjectList);
        }
    }
}
