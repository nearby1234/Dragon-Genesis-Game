using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(DragDropItem))]
public class ItemEquip : MonoBehaviour , IItemSlot
{
    [SerializeField] private Image m_Image;
    [SerializeField] private QuestItemSO m_CurrentItem;
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private Sprite m_OriginalSprite;
    [SerializeField] private TYPEARMOR m_TypeArmor;
    public TYPEARMOR TypeArmor => m_TypeArmor;

    public QuestItemSO CurrentItem
    {
        get => m_CurrentItem;
        set
        {
            m_CurrentItem = value;
            if(m_CurrentItem != null)
            {
                SendEventItemEquip();
            } 
                
        }
    }
    private void Start()
    {
        SendEventItemEquip();
    }
    private void SendEventItemEquip()
    {
        // Kiểm tra ListenerManager.Instance đã có chưa
        if (ListenerManager.HasInstance)
        {
            // Kiểm tra m_CurrentItem và questItemData không phải null
            if (m_CurrentItem != null && m_CurrentItem.questItemData != null)
            {
                // Kiểm tra xem item có phải là ITEM_WEAPON không
                if (m_CurrentItem.questItemData.typeItem.Equals(TYPEITEM.ITEM_WEAPON))
                {
                    ListenerManager.Instance.BroadCast(ListenType.SHOWPLAYER_WEAPON_UI, m_CurrentItem);
                }

                // Kiểm tra item có phải là ARMOR không và thực hiện Broadcast tương ứng
                switch (m_CurrentItem.questItemData.typeArmor)
                {
                    case TYPEARMOR.ARMOR_CHEST:
                        ListenerManager.Instance.BroadCast(ListenType.SHOWPLAYER_ARMOR_CHEST_UI, m_CurrentItem);
                        break;
                    case TYPEARMOR.ARMOR_HEAD:
                        ListenerManager.Instance.BroadCast(ListenType.SHOWPLAYER_ARMOR_HEAD_UI, m_CurrentItem);
                        break;
                    case TYPEARMOR.ARMOR_SHOULDERS:
                        ListenerManager.Instance.BroadCast(ListenType.SHOWPLAYER_ARMOR_SHOULDERS_UI, m_CurrentItem);
                        break;
                    case TYPEARMOR.ARMOR_LEGS:
                        ListenerManager.Instance.BroadCast(ListenType.SHOWPLAYER_ARMOR_LEGS_UI, m_CurrentItem);
                        break;
                    case TYPEARMOR.ARMOR_GLOVES:
                        ListenerManager.Instance.BroadCast(ListenType.SHOWPLAYER_ARMOR_GLOVES_UI, m_CurrentItem);
                        break;
                    case TYPEARMOR.ARMOR_ARMS:
                        ListenerManager.Instance.BroadCast(ListenType.SHOWPLAYER_ARMOR_ARMS_UI, m_CurrentItem);
                        break;
                    case TYPEARMOR.ARMOR_BELT:
                        ListenerManager.Instance.BroadCast(ListenType.SHOWPLAYER_ARMOR_BELT_UI, m_CurrentItem);
                        break;
                    case TYPEARMOR.ARMOR_BOOTS:
                        ListenerManager.Instance.BroadCast(ListenType.SHOWPLAYER_ARMOR_BOOTS_UI, m_CurrentItem);
                        break;
                    default:
                        Debug.LogWarning($"Không tìm thấy {m_CurrentItem.questItemData.typeArmor}");
                        break;
                }
            }
            else
            {
                Debug.LogWarning("m_CurrentItem hoặc questItemData là null!");
            }
        }
    }



}
