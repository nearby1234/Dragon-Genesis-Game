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
       ShowCusor(true);
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
        ShowCusor(false);
    }

    private void ShowCusor(bool isShow)
    {
        if(GameManager.HasInstance)
        {
            if(isShow)
            {
                GameManager.Instance.ShowCursor();
            }else
            {
                GameManager.Instance.HideCursor();
            }
        }
    }
}
