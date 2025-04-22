using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupInventory : BasePopup
{
    [SerializeField] private int m_CountBox = 10;
    [SerializeField] private Vector2 m_Offset;
    [SerializeField] private RectTransform m_Rectranform;
    [SerializeField] private Button m_ExitBtn;
    [SerializeField] private TextMeshProUGUI m_MoneyTxt;
    [SerializeField] private Transform m_InventoryBoxPanel;
    [SerializeField] private Transform m_InventoryItemPanel;
    [SerializeField] private List<QuestItemSO> m_ImageIconList = new();

    // Danh sách lưu trữ box (là GameObject) và các slot item (là InventorySlot)
    [SerializeField] private List<GameObject> listBoxInventory = new();
    [SerializeField] private List<InventorySlot> listItemInventory = new();

    // Đường dẫn Prefab
    private const string m_BoxImgPath = "Prefabs/Inventory/BoxItem/Box";
    private const string m_ItemImgPath = "Prefabs/Inventory/Item/ItemImg";

    // Prefab được load từ Resources
    private GameObject m_BoxInventoryPrefab;
    private GameObject m_ItemInventoryPrefab;

    private void Awake()
    {
        // Load các prefab từ Resources
        m_BoxInventoryPrefab = Resources.Load<GameObject>(m_BoxImgPath);
        m_ItemInventoryPrefab = Resources.Load<GameObject>(m_ItemImgPath);
        m_Rectranform = GetComponent<RectTransform>();
        if (m_BoxInventoryPrefab != null)
        {
            InitInventoryBoxes();
        }
        else
        {
            Debug.LogError("Không load được prefab BoxInventory tại: " + m_BoxImgPath);
        }

        if (m_ItemInventoryPrefab != null)
        {
            InitInventoryItems();
        }
        else
        {
            Debug.LogError("Không load được prefab ItemInventory tại: " + m_ItemImgPath);
        }
    }

    private void Start()
    {
        // Đánh dấu player đang tương tác với UI
        if (PlayerManager.HasInstance)
        {
            PlayerManager.instance.isInteractingWithUI = true;
        }

        if (m_ExitBtn != null)
        {
            m_ExitBtn.onClick.AddListener(OnClickExitBtn);
        }
        else
        {
            Debug.LogWarning("Không gán được Button Exit!");
        }

        // Đăng ký sự kiện nhận danh sách item reward
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.UI_SEND_LIST_ITEM_REWARD, ReceiverListItemReward);
        }
        m_Rectranform.anchoredPosition = m_Offset;

        // Thêm các item reward vào các slot trống của inventory
        AddItems(m_ImageIconList, listItemInventory);
    }

    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.UI_SEND_LIST_ITEM_REWARD, ReceiverListItemReward);
        }
    }

    /// <summary>
    /// Khởi tạo Box Inventory – danh sách chứa các box (GameObject) trên panel box.
    /// </summary>
    private void InitInventoryBoxes()
    {
        for (int i = 0; i < m_CountBox; i++)
        {
            GameObject box = Instantiate(m_BoxInventoryPrefab, m_InventoryBoxPanel);
            box.name = $"{m_BoxInventoryPrefab.name}-{i}";
            listBoxInventory.Add(box);
        }
    }

    /// <summary>
    /// Khởi tạo Item Inventory – danh sách chứa các slot (InventorySlot) trên panel item.
    /// </summary>
    private void InitInventoryItems()
    {
        for (int i = 0; i < m_CountBox; i++)
        {
            GameObject itemGO = Instantiate(m_ItemInventoryPrefab, m_InventoryItemPanel);
            itemGO.name = $"{m_ItemInventoryPrefab.name}-{i}";
            InventorySlot slot = itemGO.GetComponent<InventorySlot>();
            if (slot != null)
            {
                listItemInventory.Add(slot);
            }
            else
            {
                Debug.LogWarning("Prefab ItemInventory thiếu thành phần InventorySlot: " + itemGO.name);
            }
        }
    }

    private void AddItems(List<QuestItemSO> listIcon, List<InventorySlot> itemInventory)
    {
        foreach (var questItem in listIcon)
        {
            var type = questItem.questItemData.typeItem;
            if (type == TYPEITEM.ITEM_MISSION || type == TYPEITEM.ITEM_EXP)
                continue;
            foreach (InventorySlot slot in itemInventory)
            {
                if (slot.IsEmpty)
                {
                    slot.SetItemSprite(questItem);
                    slot.IsEmpty = false;
                    break; // Khi đã tìm được slot trống, chuyển sang QuestItem tiếp theo
                }
            }
        }
    }
    private void OnClickExitBtn()
    {
        this.Hide();
        if (PlayerManager.HasInstance)
        {
            PlayerManager.instance.isInteractingWithUI = false;
        }
    }

    /// <summary>
    /// Hàm nhận danh sách QuestItem (reward) từ hệ thống sự kiện.
    /// </summary>
    /// <param name="value">Đối tượng chứa danh sách QuestItem</param>
    private void ReceiverListItemReward(object value)
    {
        if (value is List<QuestItemSO> listItem)
        {
            foreach (var item in listItem)
            {
                if (item != null)
                {
                    m_ImageIconList.Add(item);
                }
            }
        }
    }
}
