using UnityEditor.SceneManagement;
using UnityEngine;

public class BatItem : BaseEnemyItem
{
    private void Start()
    {
        SetItem();
        if(ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.SEND_ITEM_INIT_POOL, this);
        }
    }
    protected override void SetItem()
    {
        itemID = questItem.questItemData.itemID;
        prefabs = questItem.questItemData.m_SwordPrefabs;
        initialSize = questItem.questItemData.initialSize;
    }

    
}
