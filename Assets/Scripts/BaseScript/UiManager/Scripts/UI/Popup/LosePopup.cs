using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LosePopup : BasePopup
{
    [SerializeField] private TextMeshProUGUI m_Title;
    [SerializeField] private TextMeshProUGUI m_NameBoss;
    [SerializeField] private TextMeshProUGUI m_NameDefeat;
    [SerializeField] private TextMeshProUGUI m_TitlePlayAgain;
    [SerializeField] private TextMeshProUGUI m_TitleMainMenu;
    [SerializeField] private Button m_PlayAgainBtn;
    [SerializeField] private Button m_MainMenuBtn;
    [SerializeField] private Button m_ExitBtn;

    public override void Show(object data)
    {
        Debug.Log("LosePopup show");
        base.Show(data);
        if(ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.UI_CLICK_SHOWUI, null);
        }
        if (data != null)
        {
            if (data is PopupMessage msg)
            {
                switch (msg.popupType)
                {
                    case PopupType.LOSE:

                        m_MainMenuBtn.gameObject.SetActive(true);
                        m_PlayAgainBtn.gameObject.SetActive(true);
                        m_Title.gameObject.SetActive(true);
                        m_NameBoss.gameObject.SetActive(false);
                        m_NameDefeat.gameObject.SetActive(false);
                        m_Title.text = msg.titleLose;
                        m_Title.color = msg.TitleLoseColor;
                        m_ExitBtn.image.color = new(1, 1, 1, 0);
                        m_ExitBtn.interactable = false;
                        m_TitlePlayAgain.text = msg.titlePlayAgain;
                        m_PlayAgainBtn.onClick.RemoveAllListeners();
                        m_PlayAgainBtn.onClick.AddListener(() =>
                        {
                            msg.OnPlayAgain?.Invoke();
                            if (GameManager.HasInstance)
                            {
                                GameManager.Instance.HideCursor();
                            }
                            if(ListenerManager.HasInstance)
                            {
                               
                                ListenerManager.Instance.BroadCast(ListenType.UI_DISABLE_SHOWUI, null);
                            }
                            if (AudioManager.HasInstance)
                            {
                                AudioManager.Instance.PlaySE("ExitSound");
                            }
                            if(PlayerManager.HasInstance)
                            {
                                PlayerManager.Instance.m_IsShowingLosePopup = false;
                            }
                            this.Hide();
                        });
                        m_MainMenuBtn.onClick.RemoveAllListeners();
                        m_MainMenuBtn.onClick.AddListener(() =>
                        {
                            msg.OnMainMenu?.Invoke();
                            if (Cheat.HasInstance)
                            {
                                Cheat.Instance.StopParticleOpen();
                            }
                            if (ListenerManager.HasInstance)
                            {
                                ListenerManager.Instance.BroadCast(ListenType.UI_DISABLE_SHOWUI, null);
                            }
                            if (AudioManager.HasInstance)
                            {
                                AudioManager.Instance.PlaySE("ExitSound");
                            }
                            if (PlayerManager.HasInstance)
                            {
                                PlayerManager.Instance.m_IsShowingLosePopup = false;
                            }
                            this.Hide();
                        });

                        break;
                    case PopupType.WORM_DIE:
                        {
                            m_Title.gameObject.SetActive(false);
                            m_NameBoss.gameObject.SetActive(true);
                            m_NameDefeat.gameObject.SetActive(true);
                            m_NameBoss.text = msg.WormDie;
                            m_NameDefeat.text = msg.nameState;
                            m_Title.color = msg.TitleWinColor;
                            m_ExitBtn.image.color = new(1, 1, 1, 0);
                            m_ExitBtn.interactable = false;
                            m_MainMenuBtn.gameObject.SetActive(false);
                            m_PlayAgainBtn.gameObject.SetActive(false);
                            if (ListenerManager.HasInstance)
                            {
                                ListenerManager.Instance.BroadCast(ListenType.UI_DISABLE_SHOWUI, null);
                            }
                            if (Cheat.HasInstance)
                            {
                                Cheat.Instance.StartParticleOpen();
                            }
                            StartCoroutine(HideWinPopup());
                        }
                        break;
                    case PopupType.BULLTANK_DIE:
                        {
                            m_Title.gameObject.SetActive(false);
                            m_NameBoss.gameObject.SetActive(true);
                            m_NameDefeat.gameObject.SetActive(true);
                            m_NameBoss.text = msg.BullTankDie;
                            m_NameDefeat.text = msg.nameState;
                            m_Title.color = msg.TitleWinColor;
                            m_ExitBtn.image.color = new(1, 1, 1, 0);
                            m_ExitBtn.interactable = false;
                            m_MainMenuBtn.gameObject.SetActive(false);
                            m_PlayAgainBtn.gameObject.SetActive(false);
                            if (ListenerManager.HasInstance)
                            {
                                ListenerManager.Instance.BroadCast(ListenType.UI_DISABLE_SHOWUI, null);
                            }
                            StartCoroutine(HideWinPopup());
                        }
                        break;
                    case PopupType.PAUSE:
                        {
                            m_Title.gameObject.SetActive(true);
                            m_NameBoss.gameObject.SetActive(false);
                            m_NameDefeat.gameObject.SetActive(false);
                            m_Title.text = msg.titlePause;
                            m_Title.color = msg.TitlePauseColor;
                            m_ExitBtn.image.color = new(1, 1, 1, 0);
                            m_ExitBtn.interactable = false;
                            m_TitlePlayAgain.text = msg.titleResume;
                            m_MainMenuBtn.gameObject.SetActive(true);
                            m_PlayAgainBtn.gameObject.SetActive(true);
                            m_PlayAgainBtn.onClick.RemoveAllListeners();
                            m_PlayAgainBtn.onClick.AddListener(() =>
                            {
                                msg.OnResume?.Invoke();
                                if (GameManager.HasInstance)
                                {
                                    GameManager.Instance.HideCursor();
                                }
                                if (ListenerManager.HasInstance)
                                {
                                    ListenerManager.Instance.BroadCast(ListenType.UI_DISABLE_SHOWUI, null);
                                }
                                if (AudioManager.HasInstance)
                                {
                                    AudioManager.Instance.PlaySE("ExitSound");
                                }
                                this.Hide();
                            });
                            m_MainMenuBtn.onClick.RemoveAllListeners();
                            m_MainMenuBtn.onClick.AddListener(() =>
                            {
                                msg.OnMainMenu?.Invoke();
                                if(Cheat.HasInstance)
                                {
                                    Cheat.Instance.StopParticleOpen();
                                }
                                if (ListenerManager.HasInstance)
                                {
                                    ListenerManager.Instance.BroadCast(ListenType.UI_DISABLE_SHOWUI, null);
                                }
                                if (AudioManager.HasInstance)
                                {
                                    AudioManager.Instance.PlaySE("ExitSound");
                                }
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
    private IEnumerator HideWinPopup()
    {
        yield return new WaitForSeconds(3f);
        this.Hide();
      
        if (PlayerManager.HasInstance)
        {
            PlayerManager.Instance.m_IsShowingLosePopup = false;
        }

    }
}
