using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PopupItemToolipPanel : BasePopup
{
    public static PopupItemToolipPanel Instance;
    [SerializeField] private Canvas parentCanvas;
    [SerializeField] private RectTransform m_ItemToolipPanel;
    [SerializeField] private TextMeshProUGUI m_ItemName;
    [SerializeField] private TextMeshProUGUI m_ItemDespri;
    [SerializeField] private Image m_ItemImage;
    private void Awake()
    {
        m_ItemToolipPanel = GetComponent<RectTransform>();
        Instance = this;

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

        m_ItemToolipPanel.localPosition = movePos + new Vector2(100,100);
    }

    public void ShowTooltip(string itemName, string itemDespri, Sprite itemImage)
    {
        this.Show(null);
        m_ItemName.text = itemName;
        m_ItemDespri.text = itemDespri;
        m_ItemImage.sprite = itemImage;
    }
    public override void Hide()
    {
        base.Hide();
    }
    private void GetCanvas(object obj)
    {
        if (obj is Canvas canvas)
        {
            parentCanvas = canvas;
        }
    }
}