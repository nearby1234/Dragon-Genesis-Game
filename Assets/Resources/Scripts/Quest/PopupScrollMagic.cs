using UnityEngine;
using UnityEngine.UI;

public class PopupScrollMagic : BasePopup
{
    [SerializeField] private Button m_ExitBtn;
    [SerializeField] private Button m_RewardBtn;
    private bool m_IsClickExitBtn = false;

    private void Start()
    {
        if(m_ExitBtn != null)
        {
            m_ExitBtn.onClick.AddListener(OnClickBtnExitScrollView);
        }else
        {
            Debug.Log("khong getcomponent Button duoc");
        }
        
        
    }
    private void Update()
    {
        IsLastSibling(this.transform);
    }
    public void OnClickBtnExitScrollView()
    {
        this.Hide();
        if (UIManager.HasInstance)
        {
            UIManager.Instance.ShowScreen<ScreenOriginalScrollBtn>();
        }
    }

    private void IsLastSibling(Transform child)
    {
        Transform uiElement = child.transform;
        Transform parent = uiElement.parent;

        if (parent != null && parent.GetChild(parent.childCount - 1) != uiElement)
        {
            uiElement.SetAsLastSibling(); // Đưa nó xuống cuối cùng
        }

    }
}
