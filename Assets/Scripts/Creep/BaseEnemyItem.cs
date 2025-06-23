using UnityEngine;



public abstract class BaseEnemyItem : MonoBehaviour
{
    [SerializeField] protected ItemEffectType questItemEffect;
    public ItemEffectType QuestItemEffect => questItemEffect;
    [SerializeField] protected string itemID;
    public string ItemId => itemID;
    [SerializeField] protected GameObject prefabs;
    public GameObject Prefabs => prefabs;
    [SerializeField] protected int initialSize;
    public int InitialSize => initialSize;
  

    protected abstract void SetItem();


   
        
}
