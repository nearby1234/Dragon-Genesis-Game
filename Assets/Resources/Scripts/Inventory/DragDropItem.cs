using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDropItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // Các thành phần cần thiết
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform rectTransform;
    private Image image;  // Image component chứa sprite của item
    private Vector2 originalPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        image = GetComponent<Image>(); // Giả sử Image nằm trên cùng GameObject này
        originalPosition = rectTransform.anchoredPosition;
    }

    // Khi bắt đầu kéo, cho phép raycast xuyên qua đối tượng
    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
    }

    // Khi kéo, cập nhật vị trí (visual feedback) của đối tượng đang kéo
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta;
    }

    // Khi kết thúc kéo, thực hiện việc đổi sprite theo yêu cầu
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        // Xác định Box (InventorySlot) mà con trỏ đang hướng tới.
        // Lưu ý: Đây là GameObject có chứa component InventorySlot.
        InventorySlot targetSlot = null;
        if (eventData.pointerEnter != null)
        {
            // Cố gắng tìm InventorySlot ở con hoặc parent của đối tượng mà pointer trỏ vào.
            targetSlot = eventData.pointerEnter.GetComponentInChildren<InventorySlot>() ??
                  eventData.pointerEnter.GetComponentInParent<InventorySlot>();
        }
        else
        {
            Debug.Log("PointerEnter is null.");
        }

        if (targetSlot != null)
        {
            // Lấy Image component của item trong Box đích.
            // Giả sử mỗi Box luôn có sẵn một GameObject item (với Image) làm con của Box.
            Image targetImage = targetSlot.GetComponentInChildren<Image>();
            if (targetImage != null)
            {
                Sprite draggedSprite = image.sprite;
                Sprite targetSprite = targetImage.sprite;

                // Nếu Box đích đã có sprite (tức là Item B đã có sprite) → hoán đổi sprite
                if (targetSprite != null)
                {
                    image.sprite = targetSprite;
                    SetAlphaColor(0f, image);
                    targetImage.sprite = draggedSprite;
                    SetAlphaColor(1f, targetImage);
                }
                else
                {
                    // Nếu Box đích trống → chuyển sprite từ item đang kéo sang Box đích
                    targetImage.sprite = draggedSprite;
                    SetAlphaColor(1f, targetImage);
                    image.sprite = null;
                    SetAlphaColor(0f, image);
                }
            }
            // Sau khi đổi sprite, reset vị trí của đối tượng kéo về vị trí ban đầu (vì chúng ta không di chuyển GameObject)
            rectTransform.anchoredPosition = Vector2.zero;
        }
        else
        {
            // Nếu không thả vào Box hợp lệ, đặt lại vị trí ban đầu của item đang kéo.
            rectTransform.anchoredPosition = originalPosition;
        }
    }
    private void SetAlphaColor(float alpha,Image image)
    {
        if (image == null) return;

        Color temp = image.color;
        temp.a = alpha;
        image.color = temp;
    }
}
