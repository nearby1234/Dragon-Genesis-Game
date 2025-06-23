using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataManager : BaseManager<DataManager>
{
    // Mỗi Type (ScriptableObject subclass) → mỗi key enum → list các SO
    private readonly Dictionary<Type, Dictionary<Enum, List<ScriptableObject>>> enumDataDictionary
        = new();

    private const string pathScriptableObject = "Scripts/SO";

    [SerializeField] private List<ScriptableObject> itemList = new();

    protected override void Awake()
    {
        base.Awake();
        LoadAllData();
    }
    private void Start()
    {
        AddListItem(typeof(QuestItemSO), TYPEITEM.ITEM_ARMOR);
    }

    /// <summary>
    /// Load tất cả ScriptableObject trong Resources/pathScriptableObject,
    /// nhóm theo loại asset (Type) rồi theo giá trị enum (IEnumKeyed.Key).
    /// </summary>
    public void LoadAllData()
    {
        enumDataDictionary.Clear();

        // Load mọi SO
        var dataAssets = Resources.LoadAll<ScriptableObject>(pathScriptableObject);
        foreach (var asset in dataAssets)
        {
            // Tìm interface IEnumKeyed<TEnum>
            var ifaces = asset.GetType().GetInterfaces();
            foreach (var iface in ifaces)
            {
                if (iface.IsGenericType &&
                    iface.GetGenericTypeDefinition() == typeof(IEnumKeyed<>))
                {
                    // Lấy giá trị Key
                    var keyProp = iface.GetProperty("Key");
                    if (keyProp == null) continue;
                    var enumKey = keyProp.GetValue(asset) as Enum;
                    if (enumKey == null) continue;

                    var assetType = asset.GetType();

                    // Lấy hoặc tạo sub-dictionary cho assetType
                    if (!enumDataDictionary.TryGetValue(assetType, out var subDict))
                    {
                        subDict = new Dictionary<Enum, List<ScriptableObject>>();
                        enumDataDictionary[assetType] = subDict;
                    }

                    // Lấy hoặc tạo list cho enumKey
                    if (!subDict.TryGetValue(enumKey, out var list))
                    {
                        list = new List<ScriptableObject>();
                        subDict[enumKey] = list;
                    }

                    // Thêm vào list
                    list.Add(asset);
                }
            }
        }

        Debug.Log($"🔄 Data Loaded! Types: {enumDataDictionary.Count}");
    }

    /// <summary>
    /// Lấy tất cả ScriptableObject gốc (chưa clone) nhóm theo giá trị enum.
    /// </summary>
    public Dictionary<TEnum, List<T>> GetEnumValuesAndDataList<T, TEnum>()
        where T : ScriptableObject, IEnumKeyed<TEnum>
        where TEnum : Enum
    {
        // Khởi tạo map với mỗi key enum một list rỗng
        var result = Enum.GetValues(typeof(TEnum))
                         .Cast<TEnum>()
                         .ToDictionary(k => k, _ => new List<T>());

        var assetType = typeof(T);
        if (enumDataDictionary.TryGetValue(assetType, out var subDict))
        {
            foreach (var kv in subDict)
            {
                // kv.Key là Enum, kv.Value là List<ScriptableObject>
                var enumKey = (TEnum)kv.Key;
                foreach (var so in kv.Value.OfType<T>())
                    result[enumKey].Add(so);
            }
        }

        return result;
    }

    /// <summary>
    /// Lấy danh sách clone của tất cả SO theo enum.
    /// </summary>
    public Dictionary<TEnum, List<T>> GetClonedEnumDataList<T, TEnum>()
        where T : ScriptableObject, IEnumKeyed<TEnum>
        where TEnum : Enum
    {
        var map = GetEnumValuesAndDataList<T, TEnum>();
        return map.ToDictionary(
            kv => kv.Key,
            kv => kv.Value.Select(so => Instantiate(so)).ToList()
        );
    }

    /// <summary>
    /// Lấy một asset gốc (first) cho key.
    /// </summary>
    public T GetData<T, TEnum>(TEnum key)
        where T : ScriptableObject, IEnumKeyed<TEnum>
        where TEnum : Enum
    {
        var listMap = GetEnumValuesAndDataList<T, TEnum>();
        var list = listMap[key];
        return list.Count > 0 ? list[0] : null;
    }

    /// <summary>
    /// Lấy clone của một asset đầu tiên cho key.
    /// </summary>
    public T GetClonedData<T, TEnum>(TEnum key)
        where T : ScriptableObject, IEnumKeyed<TEnum>
        where TEnum : Enum
    {
        var original = GetData<T, TEnum>(key);
        return original != null ? Instantiate(original) : null;
    }

    /// <summary>
    /// Lấy asset theo ID (questID or itemID), trả về clone.
    /// </summary>
    public T GetDataByID<T, TEnum>(string idValue)
        where T : ScriptableObject, IEnumKeyed<TEnum>
        where TEnum : Enum
    {
        var assetType = typeof(T);
        if (!enumDataDictionary.TryGetValue(assetType, out var subDict))
            return null;

        // Tìm field questID hoặc itemID
        var idField = assetType.GetField("questID")
                   ?? assetType.GetField("itemID");
        if (idField == null) return null;

        foreach (var list in subDict.Values)
        {
            foreach (var so in list.OfType<T>())
            {
                var val = idField.GetValue(so)?.ToString();
                if (string.Equals(val, idValue, StringComparison.Ordinal))
                    return Instantiate(so);
            }
        }
        return null;
    }

    /// <summary>
    /// Lấy tất cả clone của T (bất kể enum).
    /// </summary>
    public List<T> GetAllData<T, TEnum>()
        where T : ScriptableObject, IEnumKeyed<TEnum>
        where TEnum : Enum
    {
        var assetType = typeof(T);
        if (!enumDataDictionary.TryGetValue(assetType, out var subDict))
            return new List<T>();

        return subDict.Values
                      .SelectMany(list => list.OfType<T>())
                      .Select(so => Instantiate(so))
                      .ToList();
    }

    private List<ScriptableObject> GetAssetsByTypeAndKey(Type type, Enum key)
    {
        if (enumDataDictionary.TryGetValue(type, out var innerDict))
        {
            if (innerDict.TryGetValue(key, out var list))
            {
                return list;
            }
            else
            {
                Debug.LogWarning($"❌ Không tìm thấy key enum {key} trong type {type.Name}");
            }
        }
        else
        {
            Debug.LogWarning($"❌ Không tìm thấy type {type.Name} trong enumDataDictionary");
        }
        return new List<ScriptableObject>();
    }
    private void AddListItem(Type type, Enum key)
    {
        var list = GetAssetsByTypeAndKey(type, key);

        foreach (var item in list)
        {
            if (!itemList.Contains(item))
            {
                itemList.Add(item);
            }
            else
            {
                Debug.LogWarning("không có item để add");
            }
        }
    }

    /// <summary>
    /// Truy cập trực tiếp cấu trúc gốc (chưa clone).
    /// </summary>
    public Dictionary<Type, Dictionary<Enum, List<ScriptableObject>>> GetDataDictionary()
        => enumDataDictionary;
}
