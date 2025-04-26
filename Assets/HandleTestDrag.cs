using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandleTestDrag : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Image image;
    private TextMeshProUGUI textMeshPro;
    private InventorySlot inventorySlot;

    // Layer in scene to host dragged items; auto-find in Awake
    private Transform dragLayer;

    // State
    private Transform originalParent;
    private Vector2 originalAnchoredPos;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
        image = GetComponent<Image>();
        textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
        inventorySlot = GetComponent<InventorySlot>();

        // T? ??ng tìm DragOverlay trong scene
        var overlayGO = GameObject.Find("DragOverlay");
        if (overlayGO != null)
        {
            dragLayer = overlayGO.transform;
        }
        else
        {
            Debug.LogError("[DragDropItem] Không tìm th?y GameObject 'DragOverlay' trong scene. Vui lòng thêm m?t Transform con c?a Canvas v?i tên này.");
        }

        // Validate các component
        if (rectTransform == null || canvasGroup == null || image == null || textMeshPro == null || inventorySlot == null)
            Debug.LogError("[DragDropItem] Thi?u component c?n thi?t trên " + gameObject.name);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = rectTransform.parent;
        originalAnchoredPos = rectTransform.anchoredPosition;

        // Reparent vào t?ng DragOverlay
        if (dragLayer != null)
        {
            rectTransform.SetParent(dragLayer, worldPositionStays: false);
            rectTransform.SetAsLastSibling();
        }

        canvasGroup.blocksRaycasts = false;
        if (PlayerManager.HasInstance)
            PlayerManager.instance.isInteractingWithUI = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta;
        if (PlayerManager.HasInstance)
            PlayerManager.instance.isInteractingWithUI = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        InventorySlot targetSlot = null;
        if (eventData.pointerEnter != null)
            targetSlot = eventData.pointerEnter.GetComponent<InventorySlot>();

        if (targetSlot != null)
        {
            var targetImage = targetSlot.GetComponent<Image>();
            var targetText = targetSlot.GetComponentInChildren<TextMeshProUGUI>();
            if (targetImage != null && targetText != null)
            {
                var draggedSprite = image.sprite;
                var existingSprite = targetImage.sprite;
                var draggedString = textMeshPro.text;
                var existingString = targetText.text;

                if (existingSprite != null)
                {
                    SwapSprites(targetImage, draggedSprite, existingSprite);
                    SwapText(existingString, draggedString, targetText);
                    SwapCurrentItem(targetSlot);
                }
                else
                {
                    MoveSpriteToTarget(targetImage, draggedSprite);
                    MoveTextToTarget(targetText, draggedString);
                    MoveCurrentItemToTarget(targetSlot);
                }
            }
        }

        // Tr? v? container g?c và reset v? trí
        rectTransform.SetParent(originalParent, worldPositionStays: false);
        rectTransform.anchoredPosition = originalAnchoredPos;

        if (PlayerManager.HasInstance)
            PlayerManager.instance.isInteractingWithUI = false;
    }

    // Helper methods (gi? nguyên logic)
    private void SwapSprites(Image targetImage, Sprite draggedSprite, Sprite targetSprite)
    {
        image.sprite = targetSprite;
        SetAlpha(1f, image);
        targetImage.sprite = draggedSprite;
        SetAlpha(1f, targetImage);
    }

    private void SwapText(string existingText, string draggedText, TextMeshProUGUI targetText)
    {
        textMeshPro.text = existingText;
        targetText.text = draggedText;
    }

    private void SwapCurrentItem(InventorySlot targetSlot)
    {
        (targetSlot.m_CurrentItem, inventorySlot.m_CurrentItem) = (inventorySlot.m_CurrentItem, targetSlot.m_CurrentItem);
    }

    private void MoveSpriteToTarget(Image targetImage, Sprite draggedSprite)
    {
        targetImage.sprite = draggedSprite;
        SetAlpha(1f, targetImage);
        image.sprite = null;
        SetAlpha(0f, image);
    }

    private void MoveTextToTarget(TextMeshProUGUI targetText, string draggedText)
    {
        textMeshPro.enabled = false;
        targetText.text = draggedText;
        targetText.enabled = true;
        targetText.color = new Color(1, 1, 1, 1);
    }

    private void MoveCurrentItemToTarget(InventorySlot targetSlot)
    {
        targetSlot.m_CurrentItem = inventorySlot.m_CurrentItem;
        inventorySlot.m_CurrentItem = null;
        inventorySlot.ClearItem();
    }

    private void SetAlpha(float alpha, Image img)
    {
        if (img == null) return;
        var c = img.color;
        c.a = alpha;
        img.color = c;
    }
}
