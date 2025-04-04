using UnityEngine;
using UnityEngine.UI;

public class PopupScrollMagic : BasePopup
{
    [SerializeField] private Button m_ExitBtn;
    private bool m_IsClickExitBtn = false;

    private void Start()
    {
        m_ExitBtn.onClick.AddListener(OnClickBtnHideScrollView);
    }
    private void OnClickBtnHideScrollView()
    {
        m_IsClickExitBtn = true;
        if (ListenerManager.HasInstance)
        {

        }
        this.Hide();

    }
}
