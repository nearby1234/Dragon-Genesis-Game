using System;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : BaseManager<DataManager>
{
    private readonly Dictionary<Type, Dictionary<Enum, ScriptableObject>> enumDataDictionary = new();
    private Dictionary<ScriptableObject, string> originalDataBackup = new();

    private const string pathScriptableObject = "Scripts/SO";

    protected override void Awake()
    {
        base.Awake();
        LoadAllData();
        //BackupAllData();
    }
    private void OnDisable()
    {
        //RestoreOriginalData();
    }

    /// <summary>
    /// Load tất cả các asset Scriptable Object từ folder Resources/Scripts/SO
    /// Chỉ load những asset implement IEnumKeyed.
    /// </summary>
    /// 
    public void LoadAllData()
    {
        // Load tất cả các asset có kiểu ScriptableObject từ folder Resources/ScriptableObjects
        ScriptableObject[] dataAssets = Resources.LoadAll<ScriptableObject>(pathScriptableObject);
        foreach (var asset in dataAssets)
        {
            var interFaces = asset.GetType().GetInterfaces();
            foreach (var iface in interFaces)
            {
                if (iface.IsGenericType && iface.GetGenericTypeDefinition().Equals(typeof(IEnumKeyed<>)))
                {
                    var keyProperty = iface.GetProperty("Key");
                    if (keyProperty != null)
                    {
                        if (keyProperty.GetValue(asset) is Enum enumKey)
                        {
                            Type assetType = asset.GetType();
                            if (!enumDataDictionary.ContainsKey(assetType))
                            {
                                enumDataDictionary[assetType] = new Dictionary<Enum, ScriptableObject>();
                            }
                            enumDataDictionary[assetType][enumKey] = asset;
                        }
                    }
                }
            }
        }
        Debug.Log("🔄 Data Loaded!");
    }

    //private void BackupAllData()
    //{
    //    // Duyệt qua toàn bộ các asset trong enumDataDictionary
    //    foreach (var subDict in enumDataDictionary.Values)
    //    {
    //        foreach (var asset in subDict.Values)
    //        {
    //            // Chỉ backup nếu asset chưa có trong backup
    //            if (!originalDataBackup.ContainsKey(asset))
    //            {
    //                string json = JsonUtility.ToJson(asset);
    //                originalDataBackup.Add(asset, json);
    //            }
    //        }
    //    }
    //    Debug.Log("🔄 Data Backup Completed!");
    //}
    //private void RestoreOriginalData()
    //{
    //    foreach (var pair in originalDataBackup)
    //    {
    //        // pair.Key là asset, pair.Value là JSON backup
    //        JsonUtility.FromJsonOverwrite(pair.Value, pair.Key);
    //    }
    //    Debug.Log("🔄 Data Restored to Original State!");
    //}



    /// <summary>
    /// Truy xuất asset theo kiểu T và giá trị khóa kiểu TEnum.
    /// Ví dụ: BaseDataSingleton.Instance.GetData<CharacterData, CharacterType>(CharacterType.Warrior)
    /// </summary>
    /// 
    public T GetData<T,TEnum>(TEnum key) where T : ScriptableObject , IEnumKeyed<TEnum> where TEnum : Enum
    {
        Type assetType = typeof(T);
        if(enumDataDictionary.TryGetValue(assetType,out Dictionary<Enum,ScriptableObject> subDict))
        {
            if(subDict.TryGetValue(key as Enum ,out ScriptableObject asset))
            {
                return asset as T;
            }    
        }
        Debug.LogWarning($"Không tìm thấy asset{assetType.Name} với key {key}");
        return null;
    }

    /// <summary>
    /// Trả về bản clone của asset được lấy qua GetData.
    /// Điều này giúp bạn tránh sửa đổi asset gốc trong runtime.
    /// Ví dụ: DataManager.Instance.GetClonedData<QuestData, QuestType>(QuestType.MainQuest)
    /// </summary>
    public T GetClonedData<T, TEnum>(TEnum key) where T : ScriptableObject, IEnumKeyed<TEnum> where TEnum : Enum
    {
        T asset = GetData<T, TEnum>(key);
        if (asset != null)
        {
            // Clone bằng Instantiate
            T clone = Instantiate(asset);
            return clone;
        }
        return null;
    }

    public QuestData GetQuestDataByID(string questID)
    {
        // Duyệt qua dictionary của DataManager để tìm QuestData có questID tương ứng.
        foreach (var dict in enumDataDictionary.Values)
        {
            foreach (var asset in dict.Values)
            {
                QuestData quest = asset as QuestData;
                if (quest != null && quest.questID.Equals(questID))
                {
                    return Instantiate(quest);
                }
            }
        }
        Debug.LogWarning($"Không tìm thấy QuestData với questID: {questID}");
        return null;
    }

    public Dictionary<Type,Dictionary<Enum,ScriptableObject>> GetDataDictionary()
    {
        return enumDataDictionary;
    }
   

}
