using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(DragDropArmor))]
public class ItemEquip : MonoBehaviour, IItemSlot
{
    [SerializeField] private Image m_Image;
    [SerializeField] private QuestItemSO m_CurrentItem;
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private Sprite m_OriginalSprite;
    [SerializeField] private TYPEARMOR m_TypeArmor;
    [SerializeField] private Image m_OriginalIcon;
    public TYPEARMOR TypeArmor => m_TypeArmor;

    private void Awake()
    {
        m_Image = GetComponent<Image>();
        m_CanvasGroup = GetComponent<CanvasGroup>();
    }
    public QuestItemSO CurrentItem
    {
        get => m_CurrentItem;
        set
        {
            m_CurrentItem = value;
            if (m_CurrentItem != null)
            {
                SendEventItemEquip();
            } 
        }
    }
    private void Start()
    {
        SendEventItemEquip();
        GetAboveSibling();
    }
    private void SendEventItemEquip()
    {
        // Kiểm tra ListenerManager.Instance đã có chưa
        if (ListenerManager.HasInstance)
        {
            // Kiểm tra m_CurrentItem và questItemData không phải null
            if (m_CurrentItem != null)
            {
                QuestItemSO questItemSO = Instantiate(m_CurrentItem);
                // Kiểm tra xem item có phải là ITEM_WEAPON không
                if (questItemSO.questItemData.typeItem.Equals(TYPEITEM.ITEM_WEAPON))
                {
                   
                    ListenerManager.Instance.BroadCast(ListenType.SHOWPLAYER_WEAPON_UI, questItemSO);
                    Debug.Log($"m_SwordPrefabs : {questItemSO.questItemData.m_SwordPrefabs}");
                    Debug.Log($"m_SwordMesh : {questItemSO.questItemData.m_SwordMesh}");
                    Debug.Log($"m_SwordMaterial : {questItemSO.questItemData.m_SwordMaterial}");
                }

                // Kiểm tra item có phải là ARMOR không và thực hiện Broadcast tương ứng
                switch (questItemSO.questItemData.typeArmor)
                {
                    case TYPEARMOR.ARMOR_CHEST:
                        ListenerManager.Instance.BroadCast(ListenType.SHOWPLAYER_ARMOR_CHEST_UI, questItemSO);
                        break;
                    case TYPEARMOR.ARMOR_HEAD:
                        ListenerManager.Instance.BroadCast(ListenType.SHOWPLAYER_ARMOR_HEAD_UI, questItemSO);
                        break;
                    case TYPEARMOR.ARMOR_SHOULDERS:
                        ListenerManager.Instance.BroadCast(ListenType.SHOWPLAYER_ARMOR_SHOULDERS_UI, questItemSO);
                        break;
                    case TYPEARMOR.ARMOR_LEGS:
                        ListenerManager.Instance.BroadCast(ListenType.SHOWPLAYER_ARMOR_LEGS_UI,questItemSO);
                        break;
                    case TYPEARMOR.ARMOR_GLOVES:
                        ListenerManager.Instance.BroadCast(ListenType.SHOWPLAYER_ARMOR_GLOVES_UI, questItemSO);
                        break;
                    case TYPEARMOR.ARMOR_ARMS:
                        ListenerManager.Instance.BroadCast(ListenType.SHOWPLAYER_ARMOR_ARMS_UI, questItemSO);
                        break;
                    case TYPEARMOR.ARMOR_BELT:
                        ListenerManager.Instance.BroadCast(ListenType.SHOWPLAYER_ARMOR_BELT_UI, questItemSO);
                        break;
                    case TYPEARMOR.ARMOR_BOOTS:
                        ListenerManager.Instance.BroadCast(ListenType.SHOWPLAYER_ARMOR_BOOTS_UI, questItemSO);
                        break;
                    default:
                        Debug.LogWarning($"Không tìm thấy {questItemSO.questItemData.typeArmor}");
                        break;
                }
            }
        }
    }
  
    public void ShowAlphaIcon(bool IsShow)
    {
        if (IsShow)
        {
            m_OriginalIcon.color = new(1, 1, 1, 0.5f);
        }
        else
        {
            m_OriginalIcon.color = new(1, 1, 1, 0);
        }
    }

    public Sprite SetOriginelSpite()
    {
        return m_OriginalSprite;
    }
    private void GetAboveSibling()
    {
        Transform parent = transform.parent;
        int myIndex = transform.GetSiblingIndex();
        if (myIndex > 0)
        {
            Transform aboveSibling = parent.GetChild(myIndex - 1);
            if(aboveSibling != null)
            {
                if(aboveSibling.TryGetComponent<Image>(out var img))
                {
                    m_OriginalIcon = img;
                }
            }
        }
        else
        {
            Debug.Log("Tôi là phần tử đầu tiên, không có sibling nào ở trên.");
        }
    }
}
