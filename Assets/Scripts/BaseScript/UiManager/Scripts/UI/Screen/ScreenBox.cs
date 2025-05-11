using UnityEngine;

public class ScreenBox : BaseScreen
{
    [SerializeField] private Vector2 m_Offset;
    [SerializeField] private RectTransform rectTransform;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rectTransform.anchoredPosition = m_Offset;
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.CLICK_BUTTON_MAINMENU, ReceiverEventCLickMainMenu);
        }
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.CLICK_BUTTON_MAINMENU, ReceiverEventCLickMainMenu);
        }
    }

    private void ReceiverEventCLickMainMenu(object value)
    {
        this.Hide();
    }


}
