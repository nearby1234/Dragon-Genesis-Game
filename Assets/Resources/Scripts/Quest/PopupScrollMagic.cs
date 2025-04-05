using System.Collections.Generic;
using TMPro;
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
    [SerializeField] private List<QuestItem> m_RewardItemObject;
    [SerializeField] private List<QuestItem> m_MissionItemObject;
    [SerializeField] private QuestData m_CurrentQuestData;
    private const string m_QuestItemPrefabPath = "Prefabs/UI/Quest/ItemMission/ItemMissionImg";
    private const string m_QuestRewardItemPrefabPath = "Prefabs/UI/Quest/ItemReward/ItemRewardImg";

    private void Awake()
    {
        if(QuestManager.HasInstance)
        {
            m_CurrentQuestData = QuestManager.Instance.CurrentQuest;
        }
        
    }
    private void Start()
    {
        if(m_ExitBtn != null)
        {
            m_ExitBtn.onClick.AddListener(OnClickBtnExitScrollView);
        }else
        {
            Debug.Log("khong getcomponent Button duoc");
        }
        m_TitleText.text = m_CurrentQuestData.questName;
        m_ContentText.text = m_CurrentQuestData.description;
        GetListRewardItem();


    }
    private void Update()
    {
        IsLastSibling(this.transform);
    }
    public void OnClickBtnExitScrollView()
    {
        this.Hide();
        if (UIManager.HasInstance)
        {
            UIManager.Instance.ShowScreen<ScreenOriginalScrollBtn>();
        }
        if(PlayerManager.HasInstance)
        {
            PlayerManager.instance.isInteractingWithUI = false;
        }
    }
    private void GetListRewardItem()
    {
        foreach (var item in m_CurrentQuestData.bonus.itemsReward)
        {
            m_RewardItemObject.Add(item);
        }
        InitItemRewardObject();
    }
    private void InitItemRewardObject()
    {
        foreach (var item in m_RewardItemObject)
        {
            GameObject obj = Resources.Load<GameObject>(m_QuestRewardItemPrefabPath);
            if (obj != null)
            {
                GameObject itemObj = Instantiate(obj, m_RewardItemParentObject);
                if(itemObj != null)
                {
                    Image image = itemObj.GetComponent<Image>();
                    TextMeshProUGUI text = itemObj.GetComponentInChildren<TextMeshProUGUI>();
                    if (image != null && text != null)
                    {
                        image.sprite = item.icon;
                        text.text = item.itemName;
                    }
                }
            }
            else
            {
                Debug.Log("khong load duoc prefab");
            }
        }
    }

    private void IsLastSibling(Transform child)
    {
        Transform uiElement = child.transform;
        Transform parent = uiElement.parent;

        if (parent != null && parent.GetChild(parent.childCount - 1) != uiElement)
        {
            uiElement.SetAsLastSibling(); // Đưa nó xuống cuối cùng
        }

    }
}
