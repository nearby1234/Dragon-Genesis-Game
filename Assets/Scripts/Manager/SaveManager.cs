using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : BaseManager<SaveManager>
{
    // File name của JSON
    private string SaveFilePath => Path.Combine(Application.persistentDataPath,"GameSaveData.json");
    public GameSaveData CurrentSaveData { get; set; } = new();

    [Header("Data cần lưu")]

    [SerializeField] private List<DialogSystemSO> dialogMissions;
    protected override void Awake()
    {
        base.Awake();

        LoadGameData();
    }
#if UNITY_EDITOR
    //private void OnEnable()
    //{
    //    UnityEditor.EditorApplication.playModeStateChanged += HandlePlayModeChange;
    //}

    //private void OnDisable()
    //{
    //    UnityEditor.EditorApplication.playModeStateChanged -= HandlePlayModeChange;
    //}

    //private void HandlePlayModeChange(UnityEditor.PlayModeStateChange state)
    //{
    //    if (state == UnityEditor.PlayModeStateChange.EnteredPlayMode)
    //    {
    //        ResetGameData(); // Tự reset khi vào Play để test
    //    }
    //}
    //private void OnApplicationQuit()
    //{
    //    SaveGameData(); // Lưu dữ liệu khi ứng dụng thoát
    //}
#endif
    // CHƯA DÙNG TỚI 
    //public void SaveProgress(string checkpoint)
    //{
    //    CurrentSaveData.currentCheckpoint = checkpoint;

    //    // Save nhiệm vụ
    //    CurrentSaveData.dialogSaveList.Clear();
    //    foreach (var so in dialogMissions)
    //    {
    //        CurrentSaveData.dialogSaveList.Add(DialogSaveData.FromSO(so));
    //    }

    //    // Demo: save các loại dữ liệu khác
    //    CurrentSaveData.weaponList = new List<string>(collectedWeapons);
    //    CurrentSaveData.petList = new List<string>(ownedPets);
    //    CurrentSaveData.attributePoints = currentAttributePoints;

    //    SaveGameData();
    //}

   

    /// <summary>
    /// Lưu dữ liệu hiện tại của dialogSO ra file JSON
    /// </summary>
    /// 
    public void SaveGameData()
    {
        try
        {
            string json = JsonUtility.ToJson(CurrentSaveData, true);
            File.WriteAllText(SaveFilePath, json);
            Debug.Log($"✅ Game saved to: {SaveFilePath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Save failed: {ex}");
        }
    }
    /// <summary>
    /// Generic upsert: thêm mới hoặc cập nhật newData trong list dựa trên Key
    /// </summary>
    public void SaveOrUpdate<TSaveData, TKey>(List<TSaveData> list, TSaveData newData)
        where TSaveData : ISaveKeyed<TKey>
    {
        if (newData == null) return;
        if (list == null) return;

        int idx = list.FindIndex(x => EqualityComparer<TKey>.Default.Equals(x.Key, newData.Key));
        if (idx >= 0)
            list[idx] = newData;
        else
            list.Add(newData);

        SaveGameData();
    }
    /// <summary>
    /// Chuyên dụng cho DialogSystemSO
    /// </summary>
    public void SaveOrUpdateDialog(DialogSystemSO so)
    {
        // Tạo data
        DialogSaveData data = DialogSaveData.FromSO(so);
        // Upsert
        SaveOrUpdate<DialogSaveData,DialogMission>(CurrentSaveData.dialogSaveList, data);
    }
    /// <summary>
    /// Đọc file JSON (nếu có) và overwrite lên dialogSO
    /// </summary>
    public void LoadGameData()
    {
        if (!File.Exists(SaveFilePath))
        {
            Debug.Log("No save file found. Creating new save data.");
            CurrentSaveData = new GameSaveData();
            return;
        }

        try
        {
            string json = File.ReadAllText(SaveFilePath);
            CurrentSaveData = JsonUtility.FromJson<GameSaveData>(json);
            // Load lại nhiệm vụ
            foreach (var data in CurrentSaveData.dialogSaveList)
            {
                var so = dialogMissions.Find(x => x.dialogMission == data.dialogKey);
                if (so != null)
                    DialogSaveData.OverwriteToSO(data, so);
            }

        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Load failed: {ex}");
        }
    }
    public void ResetGameData()
    {
        CurrentSaveData = new GameSaveData();
        SaveGameData();
    }
    //public bool IsCheckpoint(string checkpointName)
    //{
    //    return CurrentSaveData.currentCheckpoint == checkpointName;
    //}

    public void PrintSaveData()
    {
        string path = SaveFilePath;

        if (!File.Exists(path))
        {
            Debug.LogWarning($"[SAVE FILE] File not found at: {path}");
            return;
        }

        try
        {
            string rawJson = File.ReadAllText(path);
            Debug.Log($"[SAVE FILE CONTENT]\n{rawJson}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[SAVE FILE] Failed to read: {ex}");
        }
    }
}


