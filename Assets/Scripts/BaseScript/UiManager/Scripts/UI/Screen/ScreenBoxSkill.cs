using DG.Tweening;
using UnityEngine;

public class ScreenBoxSkill : BaseScreen
{
    [SerializeField] private Vector2 m_Offset;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private CanvasGroup m_canvasGroup;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rectTransform.anchoredPosition = m_Offset;
        ShowFadeBoxSkill();
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

    private void ShowFadeBoxSkill()
    {
        m_canvasGroup.DOFade(1f,1f);
    }
    private void ReceiverEventCLickMainMenu(object value)
    {
        this.Hide();
    }    

}
