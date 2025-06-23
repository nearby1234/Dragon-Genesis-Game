using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest Item", menuName = "Scriptable Object/Quest/Quest Data/Quest Item")]
[System.Serializable]
public class QuestItemSO : ScriptableObject, IResettableSO , IEnumKeyed<TYPEITEM>
{
    public QuestItem questItemData;

    [SerializeField, HideInInspector] 
    private string _backupJson;

    public TYPEITEM typeItem;

    public TYPEITEM Key => typeItem;

#if UNITY_EDITOR
    private void OnValidate()
    {
        questItemData.itemID = "-" + this.name;
        typeItem = questItemData.typeItem;
    }
#endif

    public void BackupData()
    {
        _backupJson = JsonUtility.ToJson(questItemData);
    }

    public void RestoreData()
    {
        if (!string.IsNullOrEmpty(_backupJson))
        {
            JsonUtility.FromJsonOverwrite(_backupJson, questItemData);
        }
    }
}
