using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;

public class ScreenOriginalScrollBtn : BaseScreen
{
    private RectTransform m_RectTransform;
    private Button button;
    private Image image;
    private TextMeshProUGUI textMeshProUGUI;
    private readonly string m_DOItemPrefabPath = QuestManager.Instance.m_DOItemPrefabPath;
    [SerializeField] private Vector3 m_Offset;
    [SerializeField] private Vector2 m_targetPos;
    [SerializeField] private InputAction m_ButtonPress;

    private void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
        button = GetComponent<Button>();
        image = GetComponent<Image>();
        textMeshProUGUI = GetComponentInChildren<TextMeshProUGUI>();
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
    }
    private void OnDestroy()
    {
        m_ButtonPress.Disable();
        m_ButtonPress.performed -= OnClickButtonShowPopupScrollView;
    }
    private void OnClickButtonShowPopupScrollView(InputAction.CallbackContext callback)
    {
        if(ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.SE_ICONSCROLLMAGIC_ONCLICK, true);
        }
        if (AudioManager.HasInstance)
        {
            AudioManager.Instance.PlaySE("ClickSound");
        }

        if (UIManager.HasInstance)
        {
            UIManager.Instance.ShowPopup<PopupScrollMagic>();
        }
        if (PlayerManager.HasInstance)
        {
            PlayerManager.instance.isInteractingWithUI = true;
        }
        if(GameManager.HasInstance)
        {
            GameManager.Instance.ShowCursor();
        }


        this.Hide();
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
    }
}
