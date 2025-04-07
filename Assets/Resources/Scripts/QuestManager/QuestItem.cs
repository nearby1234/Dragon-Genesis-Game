using Sirenix.OdinInspector;
using UnityEngine;

public enum TYPEITEM
{
    DEFAULT = 0,
    ITEM_MISSION,
    ITEM_USE,
    ITEM_EQUIP,
    ITEM_COLLECT,
}

[System.Serializable]
public class QuestItem
{
    public string itemID;
    public string itemName;
    public int count;
    public int requestCount;
    public int completionCount;
    public Sprite icon;
    public TYPEITEM typeItem;

    // Ch? hi?n khi là ITEM_USE
    [ShowIf("@typeItem == TYPEITEM.ITEM_USE")]
    [BoxGroup("Item Use Stats")]
    public float percentIncrease;

}
