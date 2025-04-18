using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image m_MaterialBtn;

    private void Awake()
    {
        if (m_MaterialBtn == null)
        {
            m_MaterialBtn = GetComponent<Image>();
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        Color color = m_MaterialBtn.color;
        color.a = 1f;
        m_MaterialBtn.color = color;
        if(AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySE("HoverSound");
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Color color = m_MaterialBtn.color;
        color.a = 0f;
        m_MaterialBtn.color = color;
    }


}
