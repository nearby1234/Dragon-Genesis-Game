using System;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : BaseManager<DataManager>
{
    private readonly Dictionary<Type, Dictionary<Enum, ScriptableObject>> enumDataDictionary = new();
    private const string pathScriptableObject = "Scripts/SO";

    protected override void Awake()
    {
        base.Awake();
        LoadAllData();
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
    public Dictionary<Type,Dictionary<Enum,ScriptableObject>> GetDataDictionary()
    {
        return enumDataDictionary;
    }    
}
