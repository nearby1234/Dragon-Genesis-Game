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
    [SerializeField] private Button m_SettingBtn;
    [SerializeField] private Button m_IntructionBtn;
    [SerializeField] private Button m_ExitBtn;
    [SerializeField] private GridLayoutGroup parentButton;

    public override void Show(object data)
    {
        Debug.Log("LosePopup show");
        base.Show(data);
        if (ListenerManager.HasInstance)
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

                        if (PlayerManager.HasInstance) PlayerManager.instance.m_IsShowingLosePopup = true;

                        AddlistenerPlayAgainButton(true, msg,msg.popupType);
                        AddlistenerMainMenuButton(true, msg);
                        SetTilte(true, msg.TitleLoseColor, msg.titleLose);
                        ShowSetting(false);
                        SetContentBossShow(false);
                        SetButtonExit(false);
                        m_TitlePlayAgain.text = msg.titlePlayAgain;
                        parentButton.padding.left = -310;

                        break;
                    case PopupType.WORM_DIE:
                        {
                            SetTilte(false);
                            SetContentBossShow(true, msg.WormDie, msg.nameState);
                            SetButtonExit(true);
                            ShowSetting(false);
                            AddlistenerPlayAgainButton(false);
                            SetGrizLayout(true);
                            AddlistenerMainMenuButton(true, msg);
                            if (Cheat.HasInstance)
                            {
                                Cheat.Instance.StartParticleOpen();
                            }
                            if (GameManager.HasInstance)
                            {
                                GameManager.Instance.ShowCursor();
                            }
                            parentButton.padding.left = -130;
                        }
                        break;
                    case PopupType.BULLTANK_DIE:
                        {
                            SetTilte(false);
                            SetContentBossShow(true, msg.BullTankDie, msg.nameState);
                            SetButtonExit(true);
                            ShowSetting(false);
                            AddlistenerMainMenuButton(true, msg);
                            AddlistenerPlayAgainButton(false);
                            SetGrizLayout(true);
                            if (ListenerManager.HasInstance)
                            {
                                ListenerManager.Instance.BroadCast(ListenType.UI_CLICK_SHOWUI, null);
                            }
                            if (GameManager.HasInstance)
                            {
                                GameManager.Instance.ShowCursor();
                            }
                            parentButton.padding.left = -130;
                        }
                        break;
                    case PopupType.PAUSE:
                        {
                            SetTilte(true, msg.TitlePauseColor, msg.titlePause);
                            SetContentBossShow(false);
                            SetButtonExit(false);
                            ShowSetting(true);
                            m_TitlePlayAgain.text = msg.titleResume;
                            AddlistenerPlayAgainButton(true, msg, msg.popupType);
                            AddlistenerMainMenuButton(true, msg);
                            parentButton.padding.left = -310;

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

    private void SetGrizLayout(bool isSet)
    {
        if (isSet)
        {
            parentButton.padding.left = -120;
        }
        else
        {
            parentButton.padding.left = -324;
        }

    }
    private void AddlistenerMainMenuButton(bool isEnable = false, PopupMessage msg = null)
    {
        if (!isEnable)
        {
            m_MainMenuBtn.gameObject.SetActive(false);
            return;

        }
        else
        {
            m_MainMenuBtn.gameObject.SetActive(true);
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
                    AudioManager.Instance.PlaySE("ClickSound");
                }
                this.Hide();
            });
        }

    }
    private void AddlistenerPlayAgainButton(bool isEnable = false, PopupMessage msg = null, PopupType popupType = PopupType.Default)
    {
        if (!isEnable)
        {
            m_PlayAgainBtn.gameObject.SetActive(false);
            return;
        }
        else
        {
            m_PlayAgainBtn.gameObject.SetActive(true);
            m_PlayAgainBtn.onClick.RemoveAllListeners();
            m_PlayAgainBtn.onClick.AddListener(() =>
            {
                switch (popupType)
                {
                    case PopupType.LOSE:
                        msg.OnPlayAgain?.Invoke();
                        break;
                    case PopupType.PAUSE:
                        msg.OnResume?.Invoke();
                        break;
                }
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
                    AudioManager.Instance.PlaySE("ClickSound");
                }
                this.Hide();
            });
        }

    }
    private void SetTilte(bool isShow = false, Color color = default, string content = null)
    {
        if (!isShow)
        {
            m_Title.gameObject.SetActive(false);
            return;
        }
        else
        {
            m_Title.gameObject.SetActive(true);
            m_Title.color = color;
            m_Title.text = content;
        }

    }
    private void SetContentBossShow(bool isShow = false, string nameBoss = null, string nameDefeat = null)
    {
        if (!isShow)
        {
            m_NameBoss.gameObject.SetActive(false);
            m_NameDefeat.gameObject.SetActive(false);
            return;
        }
        else
        {
            m_NameBoss.gameObject.SetActive(true);
            m_NameDefeat.gameObject.SetActive(true);
            m_NameBoss.text = nameBoss;
            m_NameDefeat.text = nameDefeat;
        }
    }
    private void SetButtonExit(bool isShow)
    {
        if (!isShow)
        {
            m_ExitBtn.image.color = new(1, 1, 1, 0);
            m_ExitBtn.interactable = false;
            return;
        }
        else
        {
            m_ExitBtn.image.color = new(1, 1, 1, 1);
            m_ExitBtn.interactable = true;
            m_ExitBtn.onClick.RemoveAllListeners();
            m_ExitBtn.onClick.AddListener(() =>
            {
                if (ListenerManager.HasInstance)
                {
                    ListenerManager.Instance.BroadCast(ListenType.UI_DISABLE_SHOWUI, null);
                }
                if(PlayerManager.HasInstance)
                {
                    PlayerManager.Instance.m_IsShowingLosePopup = false;
                }    
                if(GameManager.HasInstance)
                {
                    GameManager.Instance.HideCursor();
                }
                if (AudioManager.HasInstance)
                {
                    AudioManager.Instance.PlaySE("ExitSound");
                }
                this.Hide();
            });
        }
    }
    private void ShowSetting(bool isShow)
    {
        if(!isShow)
        {
            m_SettingBtn.gameObject.SetActive(false);
            m_IntructionBtn.gameObject.SetActive(false);
            return;
        }else
        {
            m_SettingBtn.gameObject.SetActive(true);
            m_SettingBtn.onClick.RemoveAllListeners();
            m_SettingBtn.onClick.AddListener(() =>
            {
               if(UIManager.HasInstance)
                {
                    UIManager.Instance.ShowPopup<PopupSettingBoxImg>();
                }
                if (AudioManager.HasInstance)
                {
                    AudioManager.Instance.PlaySE("ClickSound");
                }
            });
            m_IntructionBtn.gameObject.SetActive(true);
            m_IntructionBtn.onClick.RemoveAllListeners();
            m_IntructionBtn.onClick.AddListener(() =>
            {
                if (UIManager.HasInstance)
                {
                    UIManager.Instance.ShowPopup<PopupInstructions>();
                }
                if (AudioManager.HasInstance)
                {
                    AudioManager.Instance.PlaySE("ClickSound");
                }
            });
        }
    }
}
