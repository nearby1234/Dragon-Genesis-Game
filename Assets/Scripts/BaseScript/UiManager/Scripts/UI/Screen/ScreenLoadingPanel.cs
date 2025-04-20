using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class ScreenLoadingPanel : BaseScreen
{
    [SerializeField] private TextMeshProUGUI m_TxtProgress;
    [SerializeField] private Slider m_ProgressSlider;
    [SerializeField] private Image m_BG;

    private void Start()
    {
        StartCoroutine(LoadScene());
    }
    public IEnumerator LoadScene()
    {
        // 1. Bắt đầu load async (chưa allow activate)
        AsyncOperation asyncOp = SceneManager.LoadSceneAsync("ProjectRPG");
        asyncOp.allowSceneActivation = false;

        // 2. Khởi tạo fake progress
        //    Tween fakeProgress từ 0 → 0.9 trong 3s
        float fakeProgress = 0f;
        var fakeTween = DOTween.To(
            () => fakeProgress,
            x =>
            {
                fakeProgress = x;
                m_ProgressSlider.value = x;
                m_TxtProgress.SetText($"LOADING : {Mathf.FloorToInt(x * 100f)}%");
            },
            0.9f,         // target
            3f            // duration
        ).SetEase(Ease.Linear);

        // 3. Chờ fake tween xong (3s)
        yield return fakeTween.WaitForCompletion();

        // 4. Sau 3s, đảm bảo slider của bạn là 90%
        m_ProgressSlider.value = 0.9f;
        m_TxtProgress.SetText("LOADING : 90%");

        // 5. Bây giờ đợi đến khi asyncOp.progress >= 0.9 (thực load xong)
        while (asyncOp.progress < 0.9f)
            yield return null;

        // 6. Khi thật sự đã load xong, tween từ 0.9 → 1.0 (100%)
        var finishTween = DOTween.To(
            () => fakeProgress,
            x =>
            {
                fakeProgress = x;
                m_ProgressSlider.value = x;
                m_TxtProgress.SetText($"LOADING : {Mathf.FloorToInt(x * 100f)}%");
            },
            1f,        // 100%
            0.5f       // bạn có thể chỉnh thời gian để đẹp
        ).SetEase(Ease.OutQuad);

        // 7. Khi tween hoàn tất mới activate scene
        yield return finishTween.WaitForCompletion();
        asyncOp.allowSceneActivation = true;
        if (AudioManager.HasInstance)
        {
            AudioManager.Instance.PlayBGM("Age_Of_Heroes_FULL_TRACK",true);
        }
        canvasGroup.DOFade(0, 1f).OnComplete(() =>
        {
            if (GameManager.HasInstance)
            {
                // Gán trực tiếp vào property của GameManager
                GameManager.Instance.GameState = GAMESTATE.START;

                // Bây giờ mới check, và do đúng là START nên sẽ vào block
                if (GameManager.Instance.GameState == GAMESTATE.START)
                {
                    if (UIManager.HasInstance)
                        UIManager.Instance.ShowScreen<ScreenPlayerImformation>();

                    
                }
            }
        });
    }
}
