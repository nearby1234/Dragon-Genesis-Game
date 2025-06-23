using UnityEngine;

public class ItemEffectType : MonoBehaviour
{
    [SerializeField] private GameObject itemEffectPrefabs;
    public GameObject ItemEffectPrefabs=> itemEffectPrefabs;
    [SerializeField] private QuestItemSO questItemSO;
    public QuestItemSO QuestItemSO=>questItemSO;
}
