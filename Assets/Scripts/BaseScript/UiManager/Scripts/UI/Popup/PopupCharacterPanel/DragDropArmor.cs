using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDropArmor : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private GameObject DragContainer;
    private Image image;
    [SerializeField] private ItemEquip itemEquip;
    private Vector2 originalAnchoredPos;
    private GameObject OriginalParent;
    private int originalSiblingIndex;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        itemEquip = GetComponent<ItemEquip>();
        if (rectTransform == null || canvasGroup == null || image == null)
        {
            Debug.LogError("Missing required components on DragDropItem.");
        }
    }
    private void Start()
    {
        DragContainer = GameObject.Find("DragOverlay");
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if(itemEquip.CurrentItem == null)
        {
            Debug.Log("Not Item equip.");
            return;
        }    
        canvasGroup.blocksRaycasts = false;
        originalAnchoredPos = rectTransform.anchoredPosition;
        OriginalParent = rectTransform.parent.gameObject;
        originalSiblingIndex = rectTransform.GetSiblingIndex();
        //CreatePlaceholder();
        itemEquip.ShowAlphaIcon(true);
        if (DragContainer != null)
            rectTransform.SetParent(DragContainer.transform);
        if (eventData != null)
            if (PlayerManager.HasInstance)
            {
                PlayerManager.instance.isInteractingWithUI = true; // Đặt trạng thái tương tác với UI
            }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (itemEquip.CurrentItem == null)
        {
            Debug.Log("Not Item equip.");
            return;
        }
        rectTransform.anchoredPosition += eventData.delta;

        if (PlayerManager.HasInstance)
        {
            PlayerManager.instance.isInteractingWithUI = true; // Đặt trạng thái tương tác với UI
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (itemEquip.CurrentItem == null)
        {
            Debug.Log("Not Item equip.");
            return;
        }
        canvasGroup.blocksRaycasts = true;
        if (eventData.pointerEnter != null)
        {
            Debug.Log("pointerEnter: " + (eventData.pointerEnter ? eventData.pointerEnter.name : "null"));

            if (eventData.pointerEnter.TryGetComponent<InventorySlot>(out var inventorySlot))
            {
                HandlerItemInventory(inventorySlot);
                ResetDraggedItemPosition();

                if (ListenerManager.HasInstance)
                {
                    if (itemEquip.CurrentItem.questItemData.typeWeapon != TYPEWEAPON.DEFAULT)
                    {
                        ListenerManager.Instance.BroadCast(ListenType.HIDE_ITEM_WEAPON_UI, itemEquip.CurrentItem);
                    }
                    else
                    {
                        ListenerManager.Instance.BroadCast(ListenType.HIDE_ITEM_ARMOR_UI, itemEquip.CurrentItem);
                    }

                }
                itemEquip.CurrentItem = null;
                itemEquip.ShowAlphaIcon(true);
                if (AudioManager.HasInstance)
                {
                    AudioManager.Instance.PlaySE("EquipItem");
                }
            }
            else
            {
                itemEquip.ShowAlphaIcon(false);
                ResetDraggedItemPosition();
            }
        }
        else
        {
            itemEquip.ShowAlphaIcon(false);
            ResetDraggedItemPosition();
        }
    }
    private void HandlerItemInventory(InventorySlot inventorySlot)
    {
        if (inventorySlot.TryGetComponent<Image>(out var imgInventory))
        {
            Sprite sprite = image.sprite;
            SetSpriteTarget(imgInventory, sprite);
        }
        else
        {
            Debug.Log("khong tim duoc");
        }
        SetItemCurrentTarget(inventorySlot);

    }
    private void SetSpriteTarget(Image img, Sprite currentItem)
    {
        img.sprite = currentItem;
        img.color = new(1, 1, 1, 1);
        image.sprite = null;
        image.color = new(1, 1, 1, 0);
    }
    private void SetItemCurrentTarget(InventorySlot inventorySlot)
    {
        inventorySlot.CurrentItem = itemEquip.CurrentItem;
        inventorySlot.IsEmpty = false;
        inventorySlot.UpdateCountText(itemEquip.CurrentItem.questItemData.count);
        //itemEquip.CurrentItem = null;
    }
    private void ResetDraggedItemPosition()
    {
        rectTransform.SetParent(OriginalParent.transform); // Chuyển về parent gốc trước
        rectTransform.SetSiblingIndex(originalSiblingIndex); // Optional: nếu bạn muốn khôi phục thứ tự
        rectTransform.anchoredPosition = originalAnchoredPos; // Gán lại tọa độ đúng sau khi đổi parent

        Debug.Log($"anchoredPosition after reset: {rectTransform.anchoredPosition}");
    }

   
}
