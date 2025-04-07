using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(DragDropItem))]
public class InventorySlot : MonoBehaviour
{
    [SerializeField] private bool m_IsEmpty = true;
    [SerializeField] private Image m_IconImage;
    [SerializeField] private TextMeshProUGUI m_CountTxt;
    public QuestItem m_CurrentItem;
    public bool IsEmpty
    {
        get => m_IsEmpty;
        set => m_IsEmpty = value;
    }
    private void Awake()
    {
        if (m_IconImage == null)
        {
            m_IconImage = GetComponent<Image>();
        }
        if (m_CountTxt == null)
        {
            m_CountTxt = GetComponentInChildren<TextMeshProUGUI>();
        }
    }
    public void SetItemSprite(QuestItem sprite)
    {
        if (sprite == null) return;
        m_CurrentItem = sprite; // Cập nhật biến lưu trữ item hiện tại
        m_IconImage.sprite = sprite.icon;
        SetAlphaColor(1f);
        UpdateCountText(sprite.count);
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
    private void UpdateCountText(int count)
    {
        m_CountTxt.enabled = true;
        m_CountTxt.text = count.ToString();
    }
}
