using UnityEngine;
using UnityEngine.UI;

public class ItemEquip : MonoBehaviour , IItemSlot
{
    [SerializeField] private Image m_Image;
    [SerializeField] private QuestItemSO m_CurrentItem;
    [SerializeField] private CanvasGroup m_CanvasGroup;

    public QuestItemSO CurrentItem
    {
        get => m_CurrentItem;
        set
        {
            m_CurrentItem = value;
            if(m_CurrentItem != null)
            {
                if(ListenerManager.HasInstance)
                {
                    ListenerManager.Instance.BroadCast(ListenType.SHOWPLAYER_WEAPON_UI, m_CurrentItem);
                }    
            } 
                
        }
    } 
        

   
}
