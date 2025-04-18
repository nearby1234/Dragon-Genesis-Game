using DG.Tweening;
using Febucci.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class ScreenMenuPanel : BaseScreen
{
    [Header("Menu UI")]
    //[SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextAnimator_TMP textAnimator;
    [SerializeField] private GameObject m_Sword;
    [SerializeField] private Vector2 m_SwordPos;
    [SerializeField] private GameObject m_ShinyText;
    [SerializeField] private GameObject m_LeftPos;
    [SerializeField] private GameObject m_RightPos;
    [SerializeField] private CanvasGroup m_TextCanvasGroup; // Đảm bảo rằng bạn đã gán CanvasGroup trong Inspector
    [SerializeField] private CanvasGroup m_ListButtonCanvasGroup; // Đảm bảo rằng bạn đã gán CanvasGroup trong Inspector

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
    [SerializeField] private Button m_ExitBtn;
    private Sequence _shinyLoop;

    [Header("Video UI")]
    public VideoPlayer videoPlayer;
    public string videoFileName = "Video/DragonGenesisBG.mp4";

    private void Awake()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }
    }

    private void Start()
    {
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
        videoPlayer.url = path;
        videoPlayer.Play();

        // Đảm bảo canvas ban đầu đang ẩn (alpha = 0)
        m_TextCanvasGroup.alpha = 0f;
        // Ở chế độ nền (chưa hiện), thiết lập text và đặt maxVisibleCharacters = 0
        textAnimator.SetText("LONG KHỞI");
        textAnimator.maxVisibleCharacters = 0;
        // Đặt vị trí của m_ShinyText ở bên trái
        m_ShinyText.transform.position = m_LeftPos.transform.position;

        // Bắt đầu chuỗi hiệu ứng
        StartCoroutine(PlaySequence());
        m_StartBtn.onClick.AddListener(OnClickStartButton);
    }
    private void OnDestroy()
    {
        DOTween.KillAll(); // hoặc DOTween.Kill(m_ShinyText.transform);
    }


    IEnumerator PlaySequence()
    {
        // 1. Chờ 3 giây (delay khi canvas vẫn ẩn)
        yield return new WaitForSeconds(delayBeforeTyping);


        m_TextCanvasGroup.DOFade(1f, fadeDuration);

        int lastCharIndex = -1;
        // 3. Đồng thời, bắt đầu tween maxVisibleCharacters từ 0 đến totalCharacters trong typingDuration
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
                if (AudioManager.HasInstance)
                {
                    AudioManager.Instance.PlaySE("TextSound");
                }
            }
        })
        .OnComplete(() =>
        {
            m_Sword.GetComponent<RectTransform>().DOAnchorPos(m_SwordPos, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
            {
                if (AudioManager.HasInstance)
                {
                    m_ListButtonCanvasGroup.DOFade(1f, 0.5f);
                    m_ListButtonCanvasGroup.interactable = true;
                    m_ListButtonCanvasGroup.blocksRaycasts = true;
                    AudioManager.Instance.PlaySE("SwordSound");
                    AudioManager.Instance.PlayBGM("Forest_FULL_TRACK");
                }
            });
            ShinyText();
        });
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
    private void OnClickStartButton()
    {
        if (AudioManager.HasInstance)
        {
            AudioManager.Instance.PlaySE("ClickSound");
        }
        m_ListButtonCanvasGroup.DOFade(0f, 1f)
          .OnComplete(() =>
          {
              m_ListButtonCanvasGroup.interactable = false;
              m_ListButtonCanvasGroup.blocksRaycasts = false;
          });

        canvasGroup.DOFade(0f, 1f)
            .OnComplete(() =>
            {
                // 3) Dừng chính xác sequence ShinyText
                if (_shinyLoop != null && _shinyLoop.IsActive())
                    _shinyLoop.Kill();

                // Hoặc: DOTween.Kill("ShinyTextLoop");
            });

        // Hiện loading ngay
        if (UIManager.HasInstance)
            UIManager.Instance.ShowScreen<ScreenLoadingPanel>();

        // Fade menu ra


    }
}
