
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
        if(questItemEffect != null)
        {
            itemID = questItemEffect.QuestItemSO.questItemData.itemID;
            prefabs = questItemEffect.QuestItemSO.questItemData.m_SwordPrefabs;
            initialSize = questItemEffect.QuestItemSO.questItemData.initialSize;
        }
    }
}
