using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public enum NPCName
{
    None,
    Abe,
    BatBoss,
}

public enum PlayerChoice
{
    None = 0,
    Accept,
    Deny,
    Reward,
}

[System.Serializable]
[CreateAssetMenu(fileName = "NewQuest", menuName = "Scriptable Object/Quest/Quest Data/Quest Mission")]
public class QuestData : ScriptableObject, IEnumKeyed<QuestType>, IResettableSO
{
    [InlineEditor]
    public QuestType Key => questType;
    public QuestType questType;
    public string questID;
    public string questName;
    public NPCName QuestGiver;


    public PlayerChoice playerChoice;
    public bool isAcceptMission;
    public bool isCompleteMission;

    [InlineEditor]
    public List<QuestItemSO> ItemMission;
    public QuestBonus bonus;

    // Trường này lưu dữ liệu backup dạng JSON
    [SerializeField, HideInInspector]
    private string _backupJson;

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Tự động cập nhật questID bằng cách thêm dấu gạch ngang trước tên của asset.
        questID = "-" + this.name;
    }
#endif

    /// <summary>
    /// Lưu trữ dữ liệu của QuestData ở định dạng JSON.
    /// </summary>
    public void BackupData()
    {
        _backupJson = JsonUtility.ToJson(this);
    }

    /// <summary>
    /// Khôi phục dữ liệu của QuestData từ chuỗi JSON đã backup.
    /// </summary>
    public void RestoreData()
    {
        if (!string.IsNullOrEmpty(_backupJson))
        {
            JsonUtility.FromJsonOverwrite(_backupJson, this);
        }
    }
}
