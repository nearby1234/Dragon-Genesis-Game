using DG.Tweening;
using Febucci.UI;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class ScreenMenuPanel : BaseScreen
{
    [Header("Menu UI")]
    //[SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextAnimator_TMP textAnimator;
    [SerializeField] private GameObject m_Sword;
    [SerializeField] private Vector2 m_SwordBackup;
    [SerializeField] private Vector2 m_SwordPos;
    [SerializeField] private GameObject m_ShinyText;
    [SerializeField] private GameObject m_LeftPos;
    [SerializeField] private GameObject m_RightPos;
    [SerializeField] private CanvasGroup m_TextCanvasGroup; // Đảm bảo rằng bạn đã gán CanvasGroup trong Inspector
    [SerializeField] private CanvasGroup m_ListButtonCanvasGroup; // Đảm bảo rằng bạn đã gán CanvasGroup trong Inspector

    [Header("Timings")]
    // Thời gian chờ trước khi bắt đầu hiệu ứng đánh chữ
    [SerializeField] private float delayBeforeTyping = 1f;
    // Thời gian để tween maxVisibleCharacters từ 0 đến giá trị cuối
    [SerializeField] private float typingDuration = 5f;
    // Tổng số ký tự mà bạn muốn hiện ra, ở đây là 9
    [SerializeField] private int totalCharacters = 9;
    // Thời gian fade in của canvas (tùy chọn, ví dụ 1 giây)
    [SerializeField] private float fadeDuration = 1f;

    [Header("Button UI")]
    [SerializeField] private Button m_StartBtn;
    [SerializeField] private Button m_SettingBtn;
    [SerializeField] private Button m_IntructionBtn;
    [SerializeField] private Button m_ExitGameBtn;
    private Sequence _shinyLoop;

    [Header("Video UI")]
    public VideoPlayer videoPlayer;
    public string videoFileName = "Video/MainVideo.mp4";

    private const float BUTTON_FADE_DURATION = 0.5f;
    private const float SWORD_MOVE_DURATION = 0.2f;

    private void Awake()
    {
        videoPlayer = videoPlayer != null ? videoPlayer : GetComponent<VideoPlayer>();
        RectTransform rectTransform = m_Sword.GetComponent<RectTransform>();
        m_SwordBackup = rectTransform.anchoredPosition;
    }

    private void Start()
    {
        Time.timeScale = 1f;
        InitializeVideoPlayer();
        InitializeUI();
        
        // Bắt đầu chuỗi hiệu ứng
        StartCoroutine(PlaySequence());
        m_StartBtn.onClick.AddListener(() => HandleButtonClick(OnClickStartButton));
        m_SettingBtn.onClick.AddListener(() => HandleButtonClick(OnClickSettingButton));
        m_IntructionBtn.onClick.AddListener(() => HandleButtonClick(OnClickIntructionButton));
        m_ExitGameBtn.onClick.AddListener(() => HandleButtonClick(OnClickExitGameButton));
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.CLICK_BUTTON_MAINMENU, ReceiverEventClickMainMenu);
        }
    }
    private void OnDestroy()
    {
        DOTween.KillAll(); // hoặc DOTween.Kill(m_ShinyText.transform);
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.CLICK_BUTTON_MAINMENU, ReceiverEventClickMainMenu);
        }
    }
    private void InitializeVideoPlayer()
    {
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
        videoPlayer.url = path;
        videoPlayer.Play();
    }
    private void InitializeUI()
    {

        m_TextCanvasGroup.alpha = 0f;
        textAnimator.SetText("LONG KHỞI");
        textAnimator.maxVisibleCharacters = 0;
        m_ShinyText.transform.position = m_LeftPos.transform.position;
    }


    IEnumerator PlaySequence()
    {
        yield return new WaitForSeconds(delayBeforeTyping);
       
        m_TextCanvasGroup.DOFade(1f, fadeDuration);

        AnimateTextTyping(() =>
        {
            AnimateSword(() =>
            {
                ShowButtons();
                ShinyText();
            });
        });
    }
    private void AnimateTextTyping(TweenCallback onComplete)
    {
        int lastCharIndex = -1;

        DOTween.To(() => textAnimator.maxVisibleCharacters,
            x => textAnimator.maxVisibleCharacters = x,
            totalCharacters,
            typingDuration)
        .SetEase(Ease.Linear)
        .OnUpdate(() =>
        {
            int currentChar = textAnimator.maxVisibleCharacters;
            if (currentChar != lastCharIndex)
            {
                lastCharIndex = currentChar;
                AudioManager.Instance?.PlaySE("TextSound");
            }
        })
        .OnComplete(onComplete);
    }
    private void AnimateSword(TweenCallback onComplete)
    {
        m_Sword.GetComponent<RectTransform>()
            .DOAnchorPos(m_SwordPos, SWORD_MOVE_DURATION)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                if (AudioManager.HasInstance)
                {
                    AudioManager.Instance.PlaySE("SwordSound");
                    AudioManager.Instance.PlayBGM("Forest_FULL_TRACK");
                }

                onComplete?.Invoke();
            });
    }
    private void ShowButtons()
    {
        m_ListButtonCanvasGroup.DOFade(1f, BUTTON_FADE_DURATION);
        m_ListButtonCanvasGroup.interactable = true;
        m_ListButtonCanvasGroup.blocksRaycasts = true;
    }
    private void ShinyText()
    {
        if (_shinyLoop != null && _shinyLoop.IsActive())
            return; // đã chạy thì thôi

        _shinyLoop = DOTween.Sequence()
            .Append(m_ShinyText.transform.DOMove(m_RightPos.transform.position, 1f).SetEase(Ease.Linear))
            .AppendInterval(1f)
            .Append(m_ShinyText.transform.DOMove(m_LeftPos.transform.position, 1f).SetEase(Ease.Linear))
            .AppendInterval(2f)
            .SetLoops(-1, LoopType.Restart)
            // 2) Gán ID cho dễ kill
            .SetId("ShinyTextLoop");
    }
    private void HandleButtonClick(System.Action action)
    {
        if (AudioManager.HasInstance)
        {
            AudioManager.Instance.PlaySE("ClickSound");
        }

        action?.Invoke();
    }
    private void OnClickStartButton()
    {
        m_ListButtonCanvasGroup.DOFade(0f, 1f)
             .OnComplete(() =>
             {
                 m_ListButtonCanvasGroup.interactable = false;
                 m_ListButtonCanvasGroup.blocksRaycasts = false;
             });

        canvasGroup.DOFade(0f, 1f)
            .OnComplete(() =>
            {
                _shinyLoop?.Kill();
            });

        if (UIManager.HasInstance)
        {
            UIManager.Instance.ShowScreen<ScreenLoadingPanel>();
        }

    }
    private void OnClickIntructionButton()
    {
        if (UIManager.HasInstance)
        {
            UIManager.Instance.ShowPopup<PopupInstructions>();
        }
    }
    private void OnClickSettingButton()
    {

        if (UIManager.HasInstance)
        {
            UIManager.Instance.ShowPopup<PopupSettingBoxImg>();
        }
    }
    private void ReceiverEventClickMainMenu(object value)
    {
        this.Show(null);

        // reset vị trí gươm
        var rt = m_Sword.GetComponent<RectTransform>();
        rt.anchoredPosition = m_SwordBackup;

        InitializeUI();
        StartCoroutine(PlaySequence());
    }
    private void OnClickExitGameButton()
    {

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
