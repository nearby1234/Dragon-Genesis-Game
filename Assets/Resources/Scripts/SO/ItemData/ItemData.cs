using UnityEngine;
[CreateAssetMenu(fileName ="NewItemData",menuName = "Scriptable Object/Data/Itemdata")]
public class ItemData : ScriptableObject , IEnumKeyed<ItemType>
{
    public ItemType Key => itemType;
    public ItemType itemType;
    public string description;
    
}
