using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ScreenBookSkill : BaseScreen
{
    [SerializeField] private Vector2 offSet;
    [SerializeField] private Button button;
    [SerializeField] private InputAction m_ButtonPress;
    private RectTransform m_IconTransform;
    private bool m_IsPress;


    private void Awake()
    {
        m_IconTransform = button.GetComponent<RectTransform>();
        m_IconTransform.anchoredPosition = offSet;
    }

    private void Start()
    {
        m_IconTransform.localScale = new Vector3(0f, 0f, 0f);
        DoScaleIconBookSkill();
        m_ButtonPress.Enable();
        m_ButtonPress.performed += (calback) => OnClickButtonPress();
        button.onClick.AddListener(OnClickButton);
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.CLICK_BUTTON_MAINMENU, ReceiverEventCLickMainMenu);
            ListenerManager.Instance.Register(ListenType.UI_DISABLE_SHOWUI, OnEventClickDisableUi);
        }
    }
    private void OnDestroy()
    {
        m_ButtonPress.Disable();
        m_ButtonPress.performed -= (calback) => OnClickButtonPress();
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.CLICK_BUTTON_MAINMENU, ReceiverEventCLickMainMenu);
            ListenerManager.Instance.Unregister(ListenType.UI_DISABLE_SHOWUI, OnEventClickDisableUi);
        }
    }

    private void OnClickButton()
    {
        ShowPopupSkillBox();
    }
    private void DoScaleIconBookSkill()
    {
        m_IconTransform.DOScale(new Vector3(1f, 1f, 1f), 1f).OnComplete(() =>
        {
            if (UIManager.HasInstance)
            {
                UIManager.Instance.ShowScreen<ScreenBoxSkill>();
            }
        });
    }
    private void ShowPopupSkillBox()
    {
        if (UIManager.HasInstance)
        {
            UIManager.Instance.ShowPopup<PopupBoxSkill>();
        }
        if (PlayerManager.HasInstance)
        {
            PlayerManager.instance.isInteractingWithUI = true;
        }
        if (GameManager.HasInstance)
        {
            GameManager.Instance.ShowCursor();
        }
    }
    private void HidePopupSkillBox()
    {
        if (UIManager.HasInstance)
        {
            UIManager.Instance.HidePopup<PopupBoxSkill>();
        }
        if (PlayerManager.HasInstance)
        {
            PlayerManager.instance.isInteractingWithUI = false;
        }
        if (GameManager.HasInstance)
        {
            GameManager.Instance.HideCursor();
        }
    }
    private void OnClickButtonPress()
    {
        m_IsPress = !m_IsPress;
       
        if (!ListenerManager.HasInstance) return;

        if (m_IsPress)
        {
            if (AudioManager.HasInstance)
            {
                AudioManager.Instance.PlaySE("ClickSound");
            }
            ShowPopupSkillBox();
            ListenerManager.Instance.BroadCast(ListenType.UI_CLICK_SHOWUI, null);
        }
        else
        {
            if (AudioManager.HasInstance)
            {
                AudioManager.Instance.PlaySE("ExitSound");
            }
            HidePopupSkillBox();
            ListenerManager.Instance.BroadCast(ListenType.UI_DISABLE_SHOWUI, null);
        }


    }
    private void ReceiverEventCLickMainMenu(object value)
    {
        this.Hide();
    }
    private void OnEventClickDisableUi(object value)
    {
        if (m_IsPress)
        {
            m_IsPress = false;
        }
    }
}
