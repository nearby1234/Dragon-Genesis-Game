using UnityEngine;
using UnityEngine.UI;

public class ScreenOriginalScrollBtn : BaseScreen
{
    private RectTransform m_RectTransform;
    private Button button;
    [SerializeField] private Vector3 m_Offset;

    private void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
        button = GetComponent<Button>();
    }
    private void Start()
    {
        m_RectTransform.anchoredPosition3D = m_Offset;
        button.onClick.AddListener(OnClickButtonShowPopupScrollView);
        
    }
   
    private void OnClickButtonShowPopupScrollView()
    {
        if (UIManager.HasInstance)
        {
            UIManager.Instance.ShowPopup<PopupScrollMagic>();
        }
        if(PlayerManager.HasInstance)
        {
            PlayerManager.instance.isInteractingWithUI = true;
        }
        this.Hide();
    }
  
}
