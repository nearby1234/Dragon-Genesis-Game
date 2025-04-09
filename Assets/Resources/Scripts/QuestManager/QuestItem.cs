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
public enum ITEMUSE
{
    DEFAULT = 0,
    ITEM_USE_HEAL,
    ITEM_USE_MANA,
    ITEM_USE_BUFF,
    ITEM_USE_DEBUFF,
}

[System.Serializable]
public class QuestItem
{
    public string itemID;
    public string itemName;
    public int count;
    public int requestCount;
    public int completionCount;
    [PreviewField(70, ObjectFieldAlignment.Left)]
    public Sprite icon;
    public TYPEITEM typeItem;

    // Chỉ hiện khi là ITEM_USE
    [ShowIf("@typeItem == TYPEITEM.ITEM_USE")]
    [BoxGroup("Item Use Stats")]
    public float percentIncrease;
    [ShowIf("@typeItem == TYPEITEM.ITEM_USE")]
    [BoxGroup("Item Use Stats")]
    public ITEMUSE itemUse;

}
