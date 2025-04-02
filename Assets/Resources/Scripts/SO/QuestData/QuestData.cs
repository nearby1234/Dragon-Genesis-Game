using UnityEngine;

[CreateAssetMenu(fileName = "NewQuest", menuName = "Scriptable Object/Quest/Quest Data")]
public class QuestData : ScriptableObject , IEnumKeyed<QuestType>
{
    public QuestType Key => questType;
    public QuestType questType;
    public string questID;
    public string questName;
    [TextArea(3, 10)]
    public string description;

    public QuestBonus bonus;

   
}
