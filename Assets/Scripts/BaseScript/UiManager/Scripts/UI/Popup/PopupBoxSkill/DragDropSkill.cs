using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDropSkill : MonoBehaviour, IPointerClickHandler , IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // Các thành phần cần thiết
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private GameObject DragContainer;
    private Image image;
    [SerializeField] private SkillSlot skillSlot;
    private Vector2 originalAnchoredPos;
    [SerializeField] private GameObject OriginalParent;
    private GameObject placeholder;
    private int originalSiblingIndex;

    private const string nameParentSkillPanel = "SkillSLotPanel";


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        image = GetComponent<Image>(); //
        skillSlot = GetComponent<SkillSlot>(); 

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
    public void OnPointerClick(PointerEventData eventData)
    {
        var popup = GetComponentInParent<PopupBoxSkill>();
        if (popup == null)
        {
            Debug.LogWarning("Không lấy được PopupBoxSkill");
            return;
        }

        GameObject clickedSlot = this.gameObject;

        // 1) Hide highlight của slot cũ, nếu khác
        if (popup.m_OriginalClickSkillSlot != null &&
            popup.m_OriginalClickSkillSlot != clickedSlot)
        {
            var oldSlot = popup.m_OriginalClickSkillSlot.GetComponent<SkillSlot>();
            oldSlot?.HideMartiralSlot();
        }

        // 2) Tính tên của parent cấp 2 (nếu cần lưu)
        string parentTwoName = transform.parent?.parent?.name ?? "(no grandparent)";
        popup.m_CurrentClickSkillSlot = clickedSlot;
        popup.m_CurrentClickSkillSlot.name = parentTwoName;
        // (Hãy thêm một biến string m_CurrentClickSkillSlotParentName trong PopupBoxSkill để lưu)

        // 3) Show highlight cho slot hiện tại
        var newSlot = clickedSlot.GetComponent<SkillSlot>();
        newSlot?.SetMaterialSkillSlot();
        newSlot.SetDeprisionSkill();

        // 4) Cập nhật slot gốc cho lần click kế tiếp
        popup.m_OriginalClickSkillSlot = clickedSlot;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        originalAnchoredPos = rectTransform.anchoredPosition; // Lưu vị trí ban đầu

        // ===========================
        OriginalParent = rectTransform.parent.gameObject;
        originalSiblingIndex = rectTransform.GetSiblingIndex();
        if (OriginalParent.name != nameParentSkillPanel)
        {
            CreatePlaceholder();
            if (DragContainer != null)
                rectTransform.SetParent(DragContainer.transform);
        }

        if (eventData != null)
            if (PlayerManager.HasInstance)
            {
                PlayerManager.instance.isInteractingWithUI = true; // Đặt trạng thái tương tác với UI
            }
    }


    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta;

        if (PlayerManager.HasInstance)
        {
            PlayerManager.instance.isInteractingWithUI = true; // Đặt trạng thái tương tác với UI
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        // Xác định Box (InventorySlot) mà con trỏ đang hướng tới.
        // Lưu ý: Đây là GameObject có chứa component InventorySlot.
        SkillSlot targetSlot = null;
        if (eventData.pointerEnter != null)
        {
            Debug.Log("Pointer Enter: " + eventData.pointerEnter.name);
            targetSlot = eventData.pointerEnter.GetComponent<SkillSlot>();
        }

        if (targetSlot != null)
        {
            // Nếu tìm thấy một SkillSlot hợp lệ, tạo một bản sao của item và đặt vào ScreenBox.
            CreateClone(targetSlot);
            if (AudioManager.HasInstance)
            {
                AudioManager.Instance.PlaySE("EquipItem");
            }
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
    }

    private void CreateClone(SkillSlot targetSlot)
    {
        // Tạo bản sao của item đang kéo
        GameObject clonedItem = Instantiate(this.gameObject);
        clonedItem.name = this.gameObject.name + "_Clone";


        // Set lại các thuộc tính cho clone (ảnh, item, v.v.)
        SkillSlot clonedSkillSlot = clonedItem.GetComponent<SkillSlot>();
        clonedSkillSlot.CurrentItem = skillSlot.CurrentItem; // Giữ lại item của skillSlot
        targetSlot.CurrentItem = clonedSkillSlot.CurrentItem;
        Image targetImage = targetSlot.GetComponent<Image>();
        targetImage.sprite = targetSlot.CurrentItem.questItemData.icon;
        targetSlot.SetAlphaColor(1.0f);
        Destroy(clonedItem);

        // Nếu cần, bạn có thể gán lại các thuộc tính khác của clone tại đây
    }
    private void CreatePlaceholder()
    {
        // Tạo placeholder ngay trước khi reparent
        placeholder = new GameObject("Placeholder");
        placeholder.transform.SetParent(OriginalParent.transform);

        // Copy layout size để giữ chỗ
        var le = placeholder.AddComponent<LayoutElement>();
        var placeImage= placeholder.AddComponent<Image>();
        var rLE = rectTransform.GetComponent<LayoutElement>();
        le.minHeight = rLE.minHeight;
        le.minWidth = rLE.minWidth;
        placeImage.sprite = image.sprite;

        // Đặt vị trí placeholder đúng chỗ item cũ
        placeholder.transform.SetSiblingIndex(originalSiblingIndex);
    }
    private void SetAlphaColor(float alpha, Image image)
    {
        if (image == null) return;

        Color temp = image.color;
        temp.a = alpha;
        image.color = temp;
    }
}
