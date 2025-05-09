using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LosePopup : BasePopup
{
    [SerializeField] private TextMeshProUGUI m_Title;
    [SerializeField] private TextMeshProUGUI m_TitlePlayAgain;
    [SerializeField] private TextMeshProUGUI m_TitleMainMenu;
    [SerializeField] private Button m_PlayAgainBtn;
    [SerializeField] private Button m_MainMenuBtn;
    [SerializeField] private Button m_ExitBtn;

    public override void Show(object data)
    {
        Debug.Log("LosePopup show");
        base.Show(data);
        if (data != null)
        {
            if (data is PopupMessage msg)
            {
                switch (msg.popupType)
                {
                    case PopupType.LOSE:
                        m_Title.text = msg.titleLose;
                        m_Title.color = msg.TitleLoseColor;
                        m_TitlePlayAgain.text = msg.titlePlayAgain;
                        m_PlayAgainBtn.onClick.RemoveAllListeners();
                        m_PlayAgainBtn.onClick.AddListener(() =>
                        {
                            msg.OnPlayAgain?.Invoke();
                            if (GameManager.HasInstance)
                            {
                                GameManager.Instance.HideCursor();
                            }

                            this.Hide();
                        });
                        m_MainMenuBtn.onClick.RemoveAllListeners();
                        m_MainMenuBtn.onClick.AddListener(() =>
                        {
                            msg.OnMainMenu?.Invoke();
                            this.Hide();
                        });

                        break;
                    case PopupType.WIN:
                        {
                            m_Title.text = msg.titleWin;
                            m_Title.color = msg.TitleWinColor;
                            m_MainMenuBtn.gameObject.SetActive(false);
                            m_PlayAgainBtn.gameObject.SetActive(false);
                        }
                        break;
                    case PopupType.PAUSE:
                        {
                            m_Title.text = msg.titlePause;
                            m_ExitBtn.image.color = new(1, 1, 1, 0);
                            m_ExitBtn.interactable = false;
                            m_TitlePlayAgain.text = msg.titleResume;
                            m_PlayAgainBtn.onClick.RemoveAllListeners();
                            m_PlayAgainBtn.onClick.AddListener(() =>
                            {
                                msg.OnResume?.Invoke();
                                if(GameManager.HasInstance)
                                {
                                    GameManager.Instance.HideCursor();
                                }
                                this.Hide();
                            });
                            m_MainMenuBtn.onClick.RemoveAllListeners();
                            m_MainMenuBtn.onClick.AddListener(() =>
                            {
                                msg.OnMainMenu?.Invoke();
                                this.Hide();
                            });
                        }
                        break;
                    default:
                        Debug.Log($"không có {msg.popupType} này");
                        break;
                }
            }
        }
    }
}
