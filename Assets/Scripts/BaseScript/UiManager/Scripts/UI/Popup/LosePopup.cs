using UnityEngine;
using UnityEngine.UI;

public class LosePopup : BasePopup
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
            m_Button.onClick.AddListener(HideLosePopup);
        }
    }
    public override void Hide()
    {
        base.Hide();
    }
    public override void Init()
    {
        base.Init();
    }
    public override void Show(object data)
    {
        base.Show(data);
    }

    public void HideLosePopup()
    {
        this.Hide();
    }
}
