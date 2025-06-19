using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class RewardItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private QuestItemSO currentItem;
    [SerializeField] private RectTransform rect;
    [SerializeField] private Image itemImg;
    [SerializeField] private CanvasGroup edgeImg;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private bool isClickItem;
    [SerializeField] private bool isFullListItem;
    public QuestItemSO CurrentItem => currentItem;
    public RectTransform RectTransform => rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }
    public void SetItem(QuestItemSO item)
    {
        this.currentItem = item;
    }
    private void Start()
    {
        if(ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.FULL_LIST_ITEM_REWARD, OnEventFullListItem);
        }
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.FULL_LIST_ITEM_REWARD, OnEventFullListItem);
        }
    }
    public void InitItem()
    {
        itemImg.sprite = currentItem.questItemData.icon;
        switch (currentItem.questItemData.typeItem)
        {
            case TYPEITEM.ITEM_EXP:
                {
                    itemName.text = $"{currentItem.questItemData.itemName} x {currentItem.questItemData.CountExp}";
                }
                break;
            default:
                {
                    itemName.text = $"{currentItem.questItemData.itemName}  x  {currentItem.questItemData.count}";
                }
                break;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Nếu item chưa được khởi tạo đầy đủ thì không làm gì
        if (currentItem == null)
            return;
        if (UIManager.HasInstance)
        {
            UIManager.Instance.ShowPopup<PopupItemToolipPanel>(currentItem, true);
        }

        //Debug.Log($"m_CurrentItem : {currentItem.questItemData.typeItem}");
        //switch (currentItem.questItemData.typeItem)
        //{
        //    case TYPEITEM.ITEM_USE:
        //        ShowItemTypeUSE();
        //        break;
        //    case TYPEITEM.ITEM_ARMOR:
        //        ShowItemTypeARMOR();
        //        break;
        //    case TYPEITEM.ITEM_WEAPON:
        //        ShowItemTypeARMOR();
        //        break;
        //    default:
        //        Debug.Log($"Không có {currentItem.questItemData.typeItem}");
        //        break;
        //}
    }
    //private void ShowItemTypeUSE()
    //{
    //    if(UIManager.HasInstance)
    //    {
    //        UIManager.Instance.ShowPopup<PopupItemToolipPanel>(currentItem,true);
    //    }
    //}
    //private void ShowItemTypeARMOR()
    //{
    //    UIManager.Instance.ShowPopup<PopupItemToolipPanel>();
    //    PopupItemToolipPanel.Instance.ShowToolTipItemArmor(currentItem);
    //}

    public void OnPointerExit(PointerEventData eventData)
    {
        //if (PopupItemToolipPanel.Instance != null)
        //    PopupItemToolipPanel.Instance.Hide();
        if (UIManager.HasInstance)
        {
            UIManager.Instance.HidePopup<PopupItemToolipPanel>();
        }
    }
    public void ShowEdge(bool ishow)
    {
        edgeImg.alpha = ishow ? 1 : 0;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        isClickItem = !isClickItem;
        if (eventData.pointerClick.TryGetComponent(out RewardItem rewardItem))
        {
            if(rewardItem.TryGetComponent<RectTransform>(out var rectTransform))
            {
                if (AudioManager.HasInstance)
                {
                    AudioManager.Instance.PlaySE("ClickSound");
                }
                if (isClickItem)
                {
                    if (isFullListItem)
                    {
                        if (UIManager.HasInstance)
                        {
                            NotifyMessageMission<RewardItem> notifyMessage = new()
                            {
                                message = "Danh sách phần thưởng đã đầy , không được chọn nữa."
                            };
                            UIManager.Instance.ShowNotify<NotifySystem>(notifyMessage, true);
                        }
                        isClickItem = false;
                        return;
                    }
                    rewardItem.ShowEdge(true);
                    if (ListenerManager.HasInstance)
                    {

                        ListenerManager.Instance.BroadCast(ListenType.ITEM_CHOSED, this);
                    }
                }
                else
                {
                    rewardItem.ShowEdge(false);
                    if (ListenerManager.HasInstance)
                    {

                        ListenerManager.Instance.BroadCast(ListenType.ITEM_DISABLE_CHOSED, this);
                    }
                }
            }
           
            
        }
    }
    private void OnEventFullListItem(object value)
    {
        if(value is bool isFullListItem)
        {
            this.isFullListItem = isFullListItem;
        }
    }
}
