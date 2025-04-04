using UnityEngine;
[System.Serializable]
[CreateAssetMenu(fileName = "NewQuest", menuName = "Scriptable Object/Quest/Quest Data")]
public class QuestData : ScriptableObject, IEnumKeyed<QuestType>
{
    public QuestType Key => questType;
    public QuestType questType;
    public string questID;
    public string questName;
    public GameObject QuestGiver;
    [TextArea(3, 10)]
    public string description;
    [TextArea(3, 10)]
    public string descriptionElse;
    public Color m_TextColor;
    public bool isAcceptMission;
    public bool isCompleteMission;

    public QuestBonus bonus;


}
