using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDropItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // Các thành phần cần thiết
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private GameObject DragContainer;
    private Image image;  
    private TextMeshProUGUI textMeshPro;  
    private InventorySlot inventorySlot;  
    private Vector2 originalAnchoredPos;
    private GameObject OriginalParent;
    private GameObject placeholder;
    private int originalSiblingIndex;

    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        image = GetComponent<Image>(); // Giả sử Image nằm trên cùng GameObject này
        textMeshPro = GetComponentInChildren<TextMeshProUGUI>(); // Giả sử TextMeshProUGUI nằm trong con của GameObject này
        inventorySlot = GetComponent<InventorySlot>(); // Giả sử InventorySlot nằm trên cùng GameObject này
       
        //originalPosition = rectTransform.anchoredPosition;


        // Kiểm tra xem các thành phần cần thiết có tồn tại không
        if (rectTransform == null || canvasGroup == null || image == null)
        {
            Debug.LogError("Missing required components on DragDropItem.");
        }
    }
    private void Start()
    {
        DragContainer = GameObject.Find("DragOverlay");
    }
    // Khi bắt đầu kéo, cho phép raycast xuyên qua đối tượng
    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        originalAnchoredPos = rectTransform.anchoredPosition; // Lưu vị trí ban đầu

       // ===========================
        OriginalParent = rectTransform.parent.gameObject;
        originalSiblingIndex = rectTransform.GetSiblingIndex();
        if (OriginalParent.name != "InventoryItemPanel")
        {
            CreatePlaceholder();
            if(DragContainer != null)
            rectTransform.SetParent(DragContainer.transform);
        }
       
        if(eventData != null) 
        if (PlayerManager.HasInstance)
        {
            PlayerManager.instance.isInteractingWithUI = true; // Đặt trạng thái tương tác với UI
        }
    }

    // Khi kéo, cập nhật vị trí (visual feedback) của đối tượng đang kéo
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta;

        if (PlayerManager.HasInstance)
        {
            PlayerManager.instance.isInteractingWithUI = true; // Đặt trạng thái tương tác với UI
        }
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
            Debug.Log("Pointer Enter: " + eventData.pointerEnter.name);
            targetSlot = eventData.pointerEnter.GetComponent<InventorySlot>();
        }
        else
        {
            rectTransform.anchoredPosition = originalAnchoredPos;
            rectTransform.SetParent(OriginalParent.transform);
        }

        if (targetSlot != null)
        {
            // Lấy Image component của item trong Box đích.
            // Giả sử mỗi Box luôn có sẵn một GameObject item (với Image) làm con của Box.
            Image targetImage = targetSlot.GetComponent<Image>();
            TextMeshProUGUI text = targetSlot.GetComponentInChildren<TextMeshProUGUI>();
            InventorySlot slot = targetSlot.GetComponent<InventorySlot>();
            if (targetImage != null && text != null)
            {
                Sprite draggedSprite = image.sprite;
                Sprite targetSprite = targetImage.sprite;
                string targetText = text.text; // Lấy text của item trong Box đích
                string draggedText = textMeshPro.text; // Lấy text của item đang kéo

                // Nếu Box đích đã có sprite (tức là Item B đã có sprite) → hoán đổi sprite
                if (targetSprite != null)
                {
                    SwapSprites(targetImage, draggedSprite, targetSprite);
                    SwapText(targetText, draggedText, text);
                    SwapCurrentItem(slot);
                }
                else
                {
                    // Nếu Box đích trống → chuyển sprite từ item đang kéo sang Box đích
                    MoveSpriteToTarget(targetImage, draggedSprite);
                    MoveTextToTarget(text, draggedText);
                    MoveCurrentItemToTarget(slot);

                }
            }
            // Sau khi đổi sprite, reset vị trí của đối tượng kéo về vị trí ban đầu (vì chúng ta không di chuyển GameObject)
            rectTransform.anchoredPosition = originalAnchoredPos;
            rectTransform.SetParent(OriginalParent.transform);
        }
        else
        {
            // Nếu không thả vào Box hợp lệ, đặt lại vị trí ban đầu của item đang kéo.
            rectTransform.anchoredPosition = originalAnchoredPos;
            rectTransform.SetParent(OriginalParent.transform);
        }
        if (placeholder != null)
        {
            rectTransform.SetParent(OriginalParent.transform);
            rectTransform.SetSiblingIndex(originalSiblingIndex);
            Destroy(placeholder);
            placeholder = null;
        }
        else
        {
            // Với item của PopupInventory: chỉ reset vị trí
            rectTransform.anchoredPosition = originalAnchoredPos;
        }
        if (PlayerManager.HasInstance)
        {
            PlayerManager.instance.isInteractingWithUI = false; // Đặt trạng thái không tương tác với UI
        }
        //Restore raycast block
    }
    //Hàm tạo ô clone để giữ vị trí khi drag
    private void CreatePlaceholder()
    {
        // Tạo placeholder ngay trước khi reparent
        placeholder = new GameObject("Placeholder");
        placeholder.transform.SetParent(OriginalParent.transform);

        // Copy layout size để giữ chỗ
        var le = placeholder.AddComponent<LayoutElement>();
        var rLE = rectTransform.GetComponent<LayoutElement>();
        le.minHeight = rLE.minHeight;
        le.minWidth = rLE.minWidth;

        // Đặt vị trí placeholder đúng chỗ item cũ
        placeholder.transform.SetSiblingIndex(originalSiblingIndex);
    }
    private void SwapSprites(Image targetImage, Sprite draggedSprite, Sprite targetSprite)
    {
        image.sprite = targetSprite;
        SetAlphaColor(1f, image);
        targetImage.sprite = draggedSprite;
        SetAlphaColor(1f, targetImage);
    }
    private void SwapText(string TargetText, string draggedText, TextMeshProUGUI TargetTextMesh)
    {
        textMeshPro.text = TargetText;
        TargetTextMesh.text = draggedText;
    }
    private void SwapCurrentItem(InventorySlot targetSlot)
    {
        (targetSlot.m_CurrentItem, inventorySlot.m_CurrentItem) = (inventorySlot.m_CurrentItem, targetSlot.m_CurrentItem);
    }
    private void MoveSpriteToTarget(Image targetImage, Sprite draggedSprite)
    {
        targetImage.sprite = draggedSprite;
        SetAlphaColor(1f, targetImage);
        image.sprite = null;
        SetAlphaColor(0f, image);
    }
    private void MoveTextToTarget(TextMeshProUGUI targetTextMesh, string draggedText)
    {
        textMeshPro.text = targetTextMesh.text;
        textMeshPro.enabled = false;
        targetTextMesh.text = draggedText;
        targetTextMesh.enabled = true;
        targetTextMesh.color = new Color(1, 1, 1, 1); // Đặt màu chữ thành trắng
    }
    private void MoveCurrentItemToTarget(InventorySlot targetSlot)
    {
        targetSlot.m_CurrentItem = inventorySlot.m_CurrentItem;
        inventorySlot.m_CurrentItem = null; // Đặt lại item đang kéo về null
        inventorySlot.ClearItem(); // Xóa item trong Box hiện tại
    }
    private void SetAlphaColor(float alpha, Image image)
    {
        if (image == null) return;

        Color temp = image.color;
        temp.a = alpha;
        image.color = temp;
    }

}
