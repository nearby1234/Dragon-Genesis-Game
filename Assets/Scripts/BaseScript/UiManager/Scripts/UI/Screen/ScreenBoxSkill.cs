using DG.Tweening;
using UnityEngine;

public class ScreenBoxSkill : BaseScreen
{
    [SerializeField] private Vector2 m_Offset;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private CanvasGroup m_canvasGroup;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rectTransform.anchoredPosition = m_Offset;
        ShowFadeBoxSkill();
    }

    private void ShowFadeBoxSkill()
    {
        m_canvasGroup.DOFade(1f,1f);
    }

}
