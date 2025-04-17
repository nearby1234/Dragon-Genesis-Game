using DG.Tweening;
using Febucci.UI;
using System.Collections;
using UnityEngine;

public class TextAnimatorFadeIn : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextAnimator_TMP textAnimator;
    [SerializeField] private GameObject m_Sword;
    [SerializeField] private Vector2 m_SwordPos;
    [SerializeField] private GameObject m_ShinyText;
    [SerializeField] private GameObject m_LeftPos;
    [SerializeField] private GameObject m_RightPos;

    // Thời gian chờ trước khi bắt đầu hiệu ứng đánh chữ
    [SerializeField] private float delayBeforeTyping = 1f;
    // Thời gian để tween maxVisibleCharacters từ 0 đến giá trị cuối
    [SerializeField] private float typingDuration = 3f;
    // Tổng số ký tự mà bạn muốn hiện ra, ở đây là 9
    [SerializeField] private int totalCharacters = 9;
    // Thời gian fade in của canvas (tùy chọn, ví dụ 1 giây)
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private CanvasGroup m_buttonGroup;


    private void Start()
    {
        // Đảm bảo canvas ban đầu đang ẩn (alpha = 0)
        canvasGroup.alpha = 0f;
        m_buttonGroup.alpha = 0f;
        // Ở chế độ nền (chưa hiện), thiết lập text và đặt maxVisibleCharacters = 0
        textAnimator.SetText("LONG KHỞI");
        textAnimator.maxVisibleCharacters = 0;
        // Đặt vị trí của m_ShinyText ở bên trái
        m_ShinyText.transform.position = m_LeftPos.transform.position;

        // Bắt đầu chuỗi hiệu ứng
        StartCoroutine(PlaySequence());
    }


    IEnumerator PlaySequence()
    {
        // 1. Chờ 3 giây (delay khi canvas vẫn ẩn)
        yield return new WaitForSeconds(delayBeforeTyping);

        // 2. Fade in canvas group để hiện UI
        canvasGroup.DOFade(1f, fadeDuration);

        // 3. Đồng thời, bắt đầu tween maxVisibleCharacters từ 0 đến totalCharacters trong typingDuration
        DOTween.To(() => textAnimator.maxVisibleCharacters,
                   x => textAnimator.maxVisibleCharacters = x,
                   totalCharacters,
                   typingDuration)
               .SetEase(Ease.Linear).OnComplete(() =>
               {
                   m_Sword.GetComponent<RectTransform>().DOAnchorPos(m_SwordPos, 0.2f).SetEase(Ease.Linear);
                   m_buttonGroup.DOFade(1f, 1f);
                   ShinyText();
               });

    }
    private void ShinyText()
    {
        Sequence shinyLoop = DOTween.Sequence();
        shinyLoop.Append(
            m_ShinyText.transform.DOMove(m_RightPos.transform.position, 1f).SetEase(Ease.Linear)
        );
        shinyLoop.AppendCallback(() =>
        {
            m_ShinyText.transform.DOMove(m_LeftPos.transform.position, 1f).SetEase(Ease.Linear);
        });
        shinyLoop.AppendInterval(2f); // Delay giữa các lần
        shinyLoop.SetLoops(-1, LoopType.Restart);

    }
}
