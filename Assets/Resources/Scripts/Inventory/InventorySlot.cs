using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(DragDropItem))]
public class InventorySlot : MonoBehaviour
{
    [SerializeField] private bool m_IsEmpty = true;
    public bool IsEmpty => m_IsEmpty;

    [SerializeField] private Image m_IconImage;

    private void Awake()
    {
        if (m_IconImage == null)
        {
            m_IconImage = GetComponent<Image>(); // L?y Image con (item)
        }
    }

    public void SetItemSprite(Sprite sprite)
    {
        if (sprite == null) return;

        m_IconImage.sprite = sprite;
        SetAlphaColor(1f);
        m_IsEmpty = false;
    }

    public void ClearItem()
    {
        m_IconImage.sprite = null;
        SetAlphaColor(0f);
        m_IsEmpty = true;
    }

    private void SetAlphaColor(float alpha)
    {
        if (m_IconImage == null) return;

        Color temp = m_IconImage.color;
        temp.a = alpha;
        m_IconImage.color = temp;
    }
}
