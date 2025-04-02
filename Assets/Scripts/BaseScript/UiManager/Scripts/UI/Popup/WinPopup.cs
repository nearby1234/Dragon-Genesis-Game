using UnityEngine;
using UnityEngine.UI;

public class WinPopup : BasePopup
{
    private Button m_Button;
    private void Awake()
    {
        m_Button = GetComponentInChildren<Button>();
    }
    private void Start()
    {
        if(m_Button != null)
        {
            m_Button.onClick.AddListener(HideWinPopup);
        }
    }
    public override void Init()
    {
        base.Init();
    }
    public override void Hide()
    {
        base.Hide();
    }

    public override void Show(object data)
    {
        base.Show(data);
    }

    public void HideWinPopup()
    {
        this.Hide();
    }
}
