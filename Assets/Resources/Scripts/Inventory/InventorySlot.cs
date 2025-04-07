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

    public void UpdateCountText(int count)
    {
        m_CountTxt.enabled = true;
        m_CountTxt.text = count.ToString();
    }    


    public void SetItemSprite(Sprite sprite)
    {
        if (sprite == null) return;

        m_IconImage.sprite = sprite;
        SetAlphaColor(1f);
        //m_IsEmpty = false;
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
