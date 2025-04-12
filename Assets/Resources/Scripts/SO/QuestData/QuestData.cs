using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
[CreateAssetMenu(fileName = "NewQuest", menuName = "Scriptable Object/Quest/Quest Data/Quest Mission")]
public class QuestData : ScriptableObject, IEnumKeyed<QuestType>
{
    [InlineEditor]
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

    [InlineEditor]
    public List<QuestItemSO> ItemMission;
    public QuestBonus bonus;
#if UNITY_EDITOR
    private void OnValidate()
    {
        // Tự động cập nhật questID bằng cách thêm dấu gạch ngang trước tên của asset.
        questID = "-"+this.name;
    }
#endif
}
