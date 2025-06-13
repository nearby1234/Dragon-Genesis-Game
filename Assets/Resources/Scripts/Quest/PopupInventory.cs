using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PopupInventory : BasePopup, IStateUi
{
    [SerializeField] private int m_CountBox = 10;
    [SerializeField] private Vector2 m_Offset;
    [SerializeField] private RectTransform m_Rectranform;
    [SerializeField] private Button m_ExitBtn;
    [SerializeField] private TextMeshProUGUI m_MoneyTxt;
    [SerializeField] private Transform m_InventoryBoxPanel;
    [SerializeField] private Transform m_InventoryItemPanel;
    [SerializeField] private Config configSO;
    [SerializeField] private List<QuestItemSO> m_ImageIconList = new();
    [SerializeField] private Vector2 m_PosMove;
    // Danh sách lưu trữ box (là GameObject) và các slot item (là InventorySlot)
    [SerializeField] private List<GameObject> listBoxInventory = new();
    [SerializeField] private List<InventorySlot> listItemInventory = new();

    // Đường dẫn Prefab
    private string m_BoxImgPath;
    private string m_ItemImgPath;

    // Prefab được load từ Resources
    private GameObject m_BoxInventoryPrefab;
    private GameObject m_ItemInventoryPrefab;

    public StateUi StateUi { get; private set; }

    private void Awake()
    {
        if (configSO == null)
            Debug.LogError("PopupInventory: configSO is null!");
        else
            Debug.Log($"PopupInventory: boxPath = '{configSO.m_BoxImgPath}', itemPath = '{configSO.m_ItemImgPath}'");
        m_Rectranform = GetComponent<RectTransform>();
        m_BoxImgPath = configSO.m_BoxImgPath;
        m_ItemImgPath = configSO.m_ItemImgPath;
    }

    private void Start()
    {
        m_Rectranform.anchoredPosition = m_Offset;
        m_BoxInventoryPrefab = Resources.Load<GameObject>(m_BoxImgPath);
        if (m_BoxInventoryPrefab == null) Debug.LogWarning($"không tim thầy đường dẫn : {m_BoxImgPath}");

        m_ItemInventoryPrefab = Resources.Load<GameObject>(m_ItemImgPath);
        if (m_ItemInventoryPrefab == null) Debug.LogWarning($"không tim thầy đường dẫn : {m_ItemImgPath}");

        if (m_BoxInventoryPrefab != null) InitInventoryBoxes();
        else Debug.LogError("Không load được prefab BoxInventory tại: " + m_BoxImgPath);

        if (m_ItemInventoryPrefab != null) InitInventoryItems();
        else Debug.LogError("Không load được prefab ItemInventory tại: " + m_ItemImgPath);

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
            ListenerManager.Instance.Register(ListenType.PU_CHARACTER_IMFORMA, ReceiverEventPUCharacter);
        }
    }

    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.UI_SEND_LIST_ITEM_REWARD, ReceiverListItemReward);
            ListenerManager.Instance.Unregister(ListenType.PU_CHARACTER_IMFORMA, ReceiverEventPUCharacter);
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
            if (box != null)
            {
                box.name = $"{m_BoxInventoryPrefab.name}-{i}";
                listBoxInventory.Add(box);
            }
        }

    }
    private void OnClickExitBtn()
    {
        if (AudioManager.HasInstance)
        {
            AudioManager.Instance.PlaySE("ExitSound");
        }
        if (UIManager.HasInstance)
        {
            UIManager.Instance.RemoverStateInDict<PopupInventory>();
            if (UIManager.Instance.GetObjectInDict<PopupCharacterPanel>())
            {
                if (ListenerManager.HasInstance)
                {
                    ListenerManager.Instance.BroadCast(ListenType.UI_CLICK_SHOWUI, null);
                }
            }
            else
            {
                if (ListenerManager.HasInstance)
                {
                    ListenerManager.Instance.BroadCast(ListenType.UI_DISABLE_SHOWUI, null);
                }
                if (GameManager.HasInstance)
                {
                    GameManager.Instance.HideCursor();
                }
            }
        }
        this.Hide();
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

    private void AddItems(List<QuestItemSO> listItems)
    {
        foreach (var newItem in listItems)
        {
            if (newItem == null) continue;
            var type = newItem.questItemData.typeItem;
            //if (type == TYPEITEM.ITEM_MISSION || type == TYPEITEM.ITEM_EXP)
            //    continue;
            switch (type)
            {
                case TYPEITEM.ITEM_MISSION:
                case TYPEITEM.ITEM_EXP:
                case TYPEITEM.ITEM_SKILL:
                    continue;
            }
            var existing = m_ImageIconList.FirstOrDefault(x => x.questItemData.itemID == newItem.questItemData.itemID);
            if (existing != null)
            {
                existing.questItemData.count += newItem.questItemData.count;
                if (ListenerManager.HasInstance)
                {
                    ListenerManager.Instance.BroadCast(ListenType.UPDATE_COUNT_ITEM, (existing, existing.questItemData.count));
                }
                //var slot = listItemInventory.FirstOrDefault(x => x.m_CurrentItem == existing);
                //if (slot != null)
                //{
                //    slot.UpdateCountText(existing.questItemData.count);
                //}
            }
            else
            {
                m_ImageIconList.Add(newItem);
                var slot = listItemInventory.FirstOrDefault(x => x.IsEmpty);
                if (slot != null)
                {
                    slot.SetItemSprite(newItem);
                    slot.IsEmpty = false;
                }
                else
                {
                    Debug.LogWarning("Không còn slot trống để add item mới!");
                }
            }
        }
    }

    /// <summary>
    /// Hàm nhận danh sách QuestItem (reward) từ hệ thống sự kiện.
    /// </summary>
    /// <param name="value">Đối tượng chứa danh sách QuestItem</param>
    private void ReceiverListItemReward(object value)
    {
        if (value is not List<QuestItemSO> newItems) return;
        AddItems(newItems);
    }
    private void ReceiverEventPUCharacter(object value)
    {
        m_Rectranform.anchoredPosition = m_PosMove;
    }
    public void SetPositionMove()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = m_PosMove;
        }
        else
        {
            Debug.LogWarning($"không tìm thấy {rectTransform}");
        }

    }
    public void SetStateUi(StateUi value)
    {
        StateUi = value;
    }
    StateUi IStateUi.GetStateUi()
    {
        return StateUi;
    }
}
