using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest Item", menuName = "Scriptable Object/Quest/Quest Data/Quest Item")]
[System.Serializable]
public class QuestItemSO : ScriptableObject
{
    // Dùng để chứa dữ liệu của QuestItem
    public QuestItem questItemData;
}
