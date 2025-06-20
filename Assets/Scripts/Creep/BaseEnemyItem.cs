using UnityEngine;



public abstract class BaseEnemyItem : MonoBehaviour
{
    [SerializeField] protected QuestItemSO questItem;
    public QuestItemSO QuestItem => questItem;
    [SerializeField] protected string itemID;
    public string ItemId => itemID;
    [SerializeField] protected GameObject prefabs;
    public GameObject Prefabs => prefabs;
    [SerializeField] protected int initialSize;
    public int InitialSize => initialSize;

    protected abstract void SetItem();
   
        
}
