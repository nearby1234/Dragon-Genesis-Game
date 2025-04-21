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
    private T GetData<T,TEnum>(TEnum key) where T : ScriptableObject , IEnumKeyed<TEnum> where TEnum : Enum
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

    public T GetDataByID<T, Tenum>(string questID) where T : ScriptableObject, IEnumKeyed<Tenum> where Tenum : Enum
    {
        Type assetType = typeof(T);

        if (enumDataDictionary.TryGetValue(assetType, out Dictionary<Enum, ScriptableObject> subDict))
        {
            //  Chỉ lấy FieldInfo một lần
            var idField = assetType.GetField("questID") ?? assetType.GetField("itemID");
            if (idField == null)
            {
                Debug.LogWarning($"⚠️ Không tìm thấy trường 'questID' hoặc 'itemID' trong {assetType.Name}");
                return null;
            }

            foreach (var item in subDict.Values)
            {
                if (item is T itemAsset)
                {
                    string idValue = idField.GetValue(itemAsset)?.ToString();
                    if (string.Equals(idValue, questID, StringComparison.Ordinal))
                    {
                        return Instantiate(itemAsset);
                    }
                }
            }
        }

        Debug.LogWarning($"⚠️ Không tìm thấy {typeof(T).Name} với ID: {questID}");
        return null;
    }


    public Dictionary<Type,Dictionary<Enum,ScriptableObject>> GetDataDictionary()
    {
        return enumDataDictionary;
    }
   

}
