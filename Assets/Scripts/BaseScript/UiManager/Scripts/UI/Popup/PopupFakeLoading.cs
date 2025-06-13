using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class PopupFakeLoading : BasePopup
{
    [SerializeField] private TextMeshProUGUI m_TxtProgress;
    [SerializeField] private Slider m_ProgressSlider;
    [SerializeField] private Image m_BG;
    [SerializeField] private float m_Timer;
    [Header("Video UI")]
    public VideoPlayer videoPlayer;
    public string videoFileName = "Video/LoadingVideo.mp4";

    private void Awake()
    {
        videoPlayer = videoPlayer != null ? videoPlayer : GetComponent<VideoPlayer>();
    }
    private void Start()
    {
        InitializeVideoPlayer();
        StartCoroutine(LoadScene());
    }
    private void InitializeVideoPlayer()
    {
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
        videoPlayer.url = path;
        videoPlayer.Play();
    }
    public override void Show(object data)
    {
        base.Show(data);
      if(data != null )
        {
            if(data is FakeLoadingSetting fake)
            {
                m_ProgressSlider.value = fake.m_ProgressValue;
                m_TxtProgress.SetText($"LOADING : {Mathf.FloorToInt(fake.m_ProgressValue * 100)}%");
                canvasGroup.alpha = 1f;
                StartCoroutine(LoadScene());
            }
        }
    }

    public IEnumerator LoadScene()
    {
        Tween sliderTween = m_ProgressSlider.DOValue(m_ProgressSlider.maxValue, m_Timer).OnUpdate(() =>
        {
            m_TxtProgress.SetText($"LOADING : {Mathf.FloorToInt(m_ProgressSlider.value * 100f)}%");
        })
            .OnComplete(() =>
            {
                if (AudioManager.HasInstance)
                {
                    AudioManager.Instance.PlayBGM("Age_Of_Heroes_FULL_TRACK", true);
                }
                canvasGroup.DOFade(0, 1f).OnComplete(() =>
                {
                    this.Hide();
                    if(UIManager.HasInstance)
                    {
                        UIManager.Instance.ShowNotify<NotifyMission>();
                    }
                });
               
            });
        yield return sliderTween.WaitForCompletion();
      
      
    }
}
