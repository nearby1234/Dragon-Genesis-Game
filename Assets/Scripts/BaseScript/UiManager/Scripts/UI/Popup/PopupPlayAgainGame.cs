using UnityEngine;
using UnityEngine.UI;

public class PopupPlayAgainGame : BasePopup
{
    [SerializeField] private Button m_PlayAgainBtn;
    [SerializeField] private Button m_MainMenuBtn;

    private void Start()
    {
        m_PlayAgainBtn.onClick.AddListener(OnClickPlayAgainButton);
        if(GameManager.HasInstance)
        {
            GameManager.Instance.ShowCursor();
        }
    }

    private void OnClickPlayAgainButton()
    {
       if(UIManager.HasInstance)
        {
            UIManager.Instance.ShowPopup<PopupFakeLoading>();
            this.Hide();
        }
    }
}
