using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;

public class ScreenOriginalScrollBtn : BaseScreen
{
    private RectTransform m_RectTransform;
    private Image image;
    private TextMeshProUGUI textMeshProUGUI;
    private string m_DOItemPrefabPath;
    [SerializeField] private Vector3 m_Offset;
    [SerializeField] private Vector2 m_targetPos;
    [SerializeField] private InputAction m_ButtonPress;
    [SerializeField] private bool isClick;

    private void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        textMeshProUGUI = GetComponentInChildren<TextMeshProUGUI>();
        m_DOItemPrefabPath = QuestManager.Instance.m_DOItemPrefabPath;
    }
    private void Start()
    {
        m_ButtonPress.Enable();
        m_ButtonPress.performed += OnClickButtonShowPopupScrollView;
        if (image != null)
        {
            image.color = new Color(1f, 1f, 1f, 0f);
        }
        textMeshProUGUI.enabled = false;
        m_RectTransform.anchoredPosition3D = m_Offset;
        //button.onClick.AddListener(OnClickButtonShowPopupScrollView);
        InitObjDoMove();
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.CLICK_BUTTON_MAINMENU, ReceiverEventCLickMainMenu);
            ListenerManager.Instance.Register(ListenType.UI_DISABLE_SHOWUI, OnEventClickDisableUi);

        }
    }
    private void OnDestroy()
    {
        m_ButtonPress.Disable();
        m_ButtonPress.performed -= OnClickButtonShowPopupScrollView;
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.CLICK_BUTTON_MAINMENU, ReceiverEventCLickMainMenu);
            ListenerManager.Instance.Unregister(ListenType.UI_DISABLE_SHOWUI, OnEventClickDisableUi);
        }
    }
    private void OnClickButtonShowPopupScrollView(InputAction.CallbackContext callback)
    {
        isClick = !isClick;
        if (isClick)
        {
            if (ListenerManager.HasInstance)
            {
                ListenerManager.Instance.BroadCast(ListenType.SE_ICONSCROLLMAGIC_ONCLICK, true);
                ListenerManager.Instance.BroadCast(ListenType.UI_CLICK_SHOWUI, null);
            }
            if (AudioManager.HasInstance)
            {
                AudioManager.Instance.PlaySE("ClickSound");
                AudioManager.Instance.PlaySE("ScrollSound");
            }

            if (UIManager.HasInstance)
            {
                UIManager.Instance.ShowPopup<PopupScrollMagic>();
                UIManager.Instance.HideNotify<NotifyMission>();
            }

            if (GameManager.HasInstance)
            {
                GameManager.Instance.ShowCursor();
            }
            this.Hide();
        }
        else
        {
            if (ListenerManager.HasInstance)
            {
                ListenerManager.Instance.BroadCast(ListenType.HIDE_SCOLLVIEW, null);
                ListenerManager.Instance.BroadCast(ListenType.UI_DISABLE_SHOWUI, null);
            }
            if (UIManager.HasInstance)
            {
                UIManager.Instance.ShowNotify<NotifyMission>();
            }
        }

    }
    private void InitObjDoMove()
    {
        GameObject obj = Resources.Load<GameObject>(m_DOItemPrefabPath);
        if (obj != null)
        {
            GameObject prefabs = Instantiate(obj, this.transform);
            if (prefabs.TryGetComponent<Image>(out var image))
            {
                image.sprite = this.image.sprite;
            }
            if (prefabs.TryGetComponent<RectTransform>(out var rectTransform))
            {
                rectTransform.anchoredPosition = m_targetPos;
                Sequence sequence = DOTween.Sequence();
                sequence.AppendCallback(() =>
                {
                    if (AudioManager.HasInstance)
                    {
                        AudioManager.Instance.PlaySE("WhooshMoveSound");
                    }
                });
                sequence.Append(rectTransform.DOAnchorPos(new Vector2(0f, 0f), 2f).SetEase(Ease.InBack));

                sequence.AppendCallback(() =>
                {
                    if (AudioManager.HasInstance)
                    {
                        AudioManager.Instance.PlaySE("WhooshScaleSound");
                    }

                });

                sequence.Append(rectTransform.DOScale(new Vector3(2f, 2f, 2f), 1f).SetEase(Ease.OutBack));
                sequence.AppendCallback(() =>
                {
                    Destroy(prefabs);
                    this.image.color = new Color(1f, 1f, 1f, 1f);
                    textMeshProUGUI.enabled = true;
                });
            }
        }
        else
        {
            Debug.LogWarning($"kh�ng t�m th?y ???ng d?n : {m_DOItemPrefabPath}");
        }
    }

    private void ReceiverEventCLickMainMenu(object value)
    {
        this.Hide();
    }
    private void OnEventClickDisableUi(object value)
    {
        if (isClick)
        {
            isClick = false;
        }
    }
}
