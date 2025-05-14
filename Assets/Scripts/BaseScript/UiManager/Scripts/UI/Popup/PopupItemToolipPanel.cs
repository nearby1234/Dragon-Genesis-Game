using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PopupItemToolipPanel : BasePopup
{
    [SerializeField] private Canvas parentCanvas;
    [SerializeField] private RectTransform m_ItemToolipPanel;
    [SerializeField] private TextMeshProUGUI m_ItemName;
    [SerializeField] private TextMeshProUGUI m_ItemDespri;
    [SerializeField] private Image m_ItemImage;
    [SerializeField] private List<TextMeshProUGUI> textList;
    [SerializeField] private VerticalLayoutGroup m_VerticalLayoutGroup;
    [SerializeField] private Config config;
    public static PopupItemToolipPanel Instance { get; private set; }

    private void Awake()
    {
        m_ItemToolipPanel = GetComponent<RectTransform>();
        // Nếu đã có một Instance khác, hủy bản này
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // đảm bảo panel khởi tạo ẩn trước
        Hide();

    }
    private void Start()
    {
        //Hide();
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.UI_SEND_CANVASMAIN, GetCanvas);
        }
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.UI_SEND_CANVASMAIN, GetCanvas);
        }
    }
    private void Update()
    {
        TooltipFollowMouse();
    }
    public void TooltipFollowMouse()
    {
        Vector2 movePos;
        Vector2 mousePosision = Mouse.current.position.ReadValue();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, mousePosision, null, out movePos);

        m_ItemToolipPanel.localPosition = movePos + new Vector2(100, 100);
    }

    public void ShowTooltipItemUSE(QuestItemSO itemSo)
    {
        ClearBonusLines();
        //m_VerticalLayoutGroup.padding.left = config.paddingLeftItem;
        // 2) Cập nhật các thông tin cơ bản
        m_ItemName.text = itemSo.questItemData.itemName;
        m_ItemDespri.text = itemSo.questItemData.ItemDespri;
        m_ItemImage.sprite = itemSo.questItemData.icon;
    }
    public void ShowToolTipItemArmor(QuestItemSO itemSo)
    {
        //ClearBonusLines();

        //m_VerticalLayoutGroup.padding.left = config.paddingLeftArmor;
        m_ItemName.text = itemSo.questItemData.itemName;
        switch(itemSo.questItemData.typeItem)
        {
            case TYPEITEM.ITEM_WEAPON:
                m_ItemDespri.text = itemSo.questItemData.DespristionWeapon;
                break;
            case TYPEITEM.ITEM_ARMOR:
                m_ItemDespri.text = itemSo.questItemData.DespristionArmor;
                break;
                default:
                break;
        }
        
        m_ItemImage.sprite = itemSo.questItemData.icon;
        var pairs = new List<(string name, int value)>
        {
            ("plusStrengthArmor", itemSo.questItemData.plusStrengthArmor),
            ("plusAgilityArmor",  itemSo.questItemData.plusAgilityArmor),
            ("plusHealArmor",   itemSo.questItemData.plusHealArmor),
            ("plusDefendArmor",   itemSo.questItemData.plusDefendArmor),
            ("plusStaminaArmor",  itemSo.questItemData.plusStaminaArmor),
        };
        var positiveBonuses = pairs.Where(p => p.value > 0).ToList().ToDictionary(p => p.name, p => p.value);



        int idx = 0;
        foreach (var kvp in positiveBonuses)
        {
            if (idx >= textList.Count) break;    // tránh vượt
            switch (kvp.Key)
            {
                case "plusStrengthArmor":
                    textList[idx].text = $"Sức Mạnh +{kvp.Value}";
                    break;
                case "plusAgilityArmor":
                    textList[idx].text = $"Trí Tuệ +{kvp.Value}";
                    break;
                case "plusHealArmor":
                    textList[idx].text = $"Máu +{kvp.Value}";
                    break;
                case "plusDefendArmor":
                    textList[idx].text = $"Phòng Ngự +{kvp.Value}";
                    break;
                case "plusStaminaArmor":
                    textList[idx].text = $"Thể Lực +{kvp.Value}";
                    break;
                    //…
            }
            textList[idx].gameObject.SetActive(true);
            idx++;
        }
        // và tắt hết các dòng còn lại:
        for (int j = idx; j < textList.Count; j++)
            textList[j].gameObject.SetActive(false);
    }
    private void ClearBonusLines()
    {
        foreach (var t in textList)
            t.gameObject.SetActive(false);
    }
    private void GetCanvas(object obj)
    {
        if (obj is Canvas canvas)
        {
            parentCanvas = canvas;
        }
    }
}