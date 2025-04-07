using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ScreenIconInventory : BaseScreen
{
    [SerializeField] private Button m_IconButton;
    [SerializeField] private Vector2 m_IconPosition;
    private RectTransform m_IconTransform;
    private Image m_Image;
    
    private void Awake()
    {
        m_IconTransform = GetComponent<RectTransform>();
        m_Image = GetComponentInChildren<Image>();
    }
    private void Start()
    {
        m_IconTransform.localScale = new Vector3(0f, 0f, 0f);
        m_IconTransform.anchoredPosition = m_IconPosition;
        DoScaleIconInventory();
    }
    private void DoScaleIconInventory()
    {
        m_IconTransform.DOScale(new Vector3(1f, 1f, 1f), 1f).OnComplete(() =>
        {
            m_IconButton.onClick.AddListener(OnClickIconInventory);
        });
    }
    private void OnClickIconInventory()
    {
        if (UIManager.HasInstance)
        {
            UIManager.Instance.ShowPopup<PopupInventory>();
        }
        if (PlayerManager.HasInstance)
        {
            PlayerManager.instance.isInteractingWithUI = true;
        }

    }
}
