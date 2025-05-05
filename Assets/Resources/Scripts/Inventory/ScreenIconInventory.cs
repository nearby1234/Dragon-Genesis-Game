using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ScreenIconInventory : BaseScreen
{
    //[SerializeField] private Button m_IconButton;
    [SerializeField] private InputAction m_ButtonPress;
    [SerializeField] private Vector2 m_IconPosition;
    [SerializeField] private Vector2 m_PosMove;
    private RectTransform m_IconTransform;
    private bool m_IsOpen;

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
    }
    private void OnDestroy()
    {
        m_ButtonPress.Disable();
        m_ButtonPress.performed -= OnClickIconInventory;
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
        if (UIManager.HasInstance)
        {
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
            {
                UIManager.Instance.AddStateInDict(popup);
            }
            else
            {
                Debug.LogWarning("PopupInventory not found from UIManager");
            }
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
}
