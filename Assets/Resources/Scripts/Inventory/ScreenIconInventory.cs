using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class ScreenIconInventory : BaseScreen
{
    //[SerializeField] private Button m_IconButton;
    [SerializeField] private InputAction m_ButtonPress;
    [SerializeField] private Vector2 m_IconPosition;
    [SerializeField] private Vector2 m_PosMove;
    private RectTransform m_IconTransform;
    private bool isClick;

    private void Awake()
    {
        m_IconTransform = GetComponent<RectTransform>();
    }
    private void Start()
    {
        m_ButtonPress.Enable();
        m_IconTransform.localScale = new Vector3(0f, 0f, 0f);
        m_IconTransform.anchoredPosition = m_IconPosition;
        DoScaleIconInventory();
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.CLICK_BUTTON_MAINMENU, ReceiverEventCLickMainMenu);
        }
    }
    private void OnDestroy()
    {
        m_ButtonPress.Disable();
        m_ButtonPress.performed -= OnClickIconInventory;
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.CLICK_BUTTON_MAINMENU, ReceiverEventCLickMainMenu);
        }
    }
    private void DoScaleIconInventory()
    {
        m_IconTransform.DOScale(new Vector3(1f, 1f, 1f), 1f).OnComplete(() =>
        {
            m_ButtonPress.performed += OnClickIconInventory;
        });
    }
    private void OnClickIconInventory(InputAction.CallbackContext callback)
    {
        isClick=!isClick;
        if(isClick)
        {
            if (UIManager.HasInstance)
            {
                if (UIManager.Instance.GetObjectInDict<PopupCharacterPanel>())
                {
                    ListenerManager.Instance.BroadCast(ListenType.PU_CHARACTER_IMFORMA, null);
                }
                UIManager.Instance.ShowPopup<PopupInventory>();
                UIManager.Instance.SetStatePopup<PopupInventory>(StateUi.Opening);
                StateUi popupCharacter = UIManager.Instance.GetStatePopup<PopupCharacterPanel>();
                if (popupCharacter.Equals(StateUi.Opening))
                {
                    PopupCharacterPanel popupCharacterPanel = UIManager.Instance.GetComponentbase<PopupCharacterPanel>();
                    popupCharacterPanel.SetPositionMove();
                    PopupInventory popupInventory = UIManager.Instance.GetComponentbase<PopupInventory>();
                    popupInventory.SetPositionMove();

                }
                var popup = UIManager.Instance.GetComponentbase<PopupInventory>();
                if (popup != null)
                    UIManager.Instance.AddStateInDict(popup);
            }

            if (ListenerManager.HasInstance)
            {
                ListenerManager.Instance.BroadCast(ListenType.UI_CLICK_SHOWUI, null);
            }
            if (GameManager.HasInstance)
            {
                GameManager.Instance.ShowCursor();
            }
            if (AudioManager.HasInstance)
            {
                AudioManager.Instance.PlaySE("ClickSound");
            }
        }
        else
        {
            if (GameManager.HasInstance)
            {
                if (UIManager.Instance.GetObjectInDict<PopupCharacterPanel>())
                {

                    GameManager.Instance.ShowCursor();
                    ListenerManager.Instance.BroadCast(ListenType.UI_CLICK_SHOWUI, null);
                }
                else
                {
                    if (ListenerManager.HasInstance)
                    {
                        ListenerManager.Instance.BroadCast(ListenType.UI_DISABLE_SHOWUI, null);
                    }
                    GameManager.Instance.HideCursor();
                }
            }


            if (UIManager.HasInstance)
            {
                UIManager.Instance.SetStatePopup<PopupInventory>(StateUi.closing);
                UIManager.Instance.RemoverStateInDict<PopupInventory>();
            }
            if (AudioManager.HasInstance)
            {
                AudioManager.Instance.PlaySE("ExitSound");
            }
            if(UIManager.HasInstance)
            {
                UIManager.Instance.HidePopup<PopupInventory>();
            }
        }
    }
    private void ReceiverEventCLickMainMenu(object value)
    {
        this.Hide();
    } 
        
}
