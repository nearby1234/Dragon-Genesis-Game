using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
[CustomEditor(typeof(DataManager))]
public class DataManagerEditor : Editor
{
    private DataManager dataManager;
    private  string searchFilter = "";
    private Dictionary<Type, Dictionary<Enum, ScriptableObject>> dataDictionary;

    private void OnEnable()
    {
        dataManager = (DataManager)target;
        dataDictionary = dataManager.GetDataDictionary();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.LabelField("📦 Base Data Manager", EditorStyles.boldLabel);

        if (GUILayout.Button("🔄 Reload Data", GUILayout.Height(30)))
        {
            dataManager.LoadAllData();
            dataDictionary = dataManager.GetDataDictionary();
            EditorUtility.SetDirty(target);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("🔍 Search Data : ", EditorStyles.boldLabel);
        searchFilter = EditorGUILayout.TextField(searchFilter);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("📜 Loaded Data:",EditorStyles.boldLabel);
        if (dataDictionary.Count > 0)
        {
            foreach (var typeEntry in dataDictionary)
            {
                Type type = typeEntry.Key;
                Dictionary<Enum, ScriptableObject> enumDict = typeEntry.Value;
                EditorGUILayout.LabelField($"🔹 {type.Name}", EditorStyles.boldLabel);
                foreach (var kvp in enumDict)
                {
                    Enum key = kvp.Key;
                    ScriptableObject asset = kvp.Value;
                    if (asset == null || (!string.IsNullOrEmpty(searchFilter) && !asset.name.ToLower().Contains(searchFilter.ToLower())))
                        continue;
                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                    EditorGUILayout.LabelField($"🔸 {key} : {asset.name}", EditorStyles.boldLabel);
                    if (GUILayout.Button("👁️ View", GUILayout.Width(60)))
                    {
                        Selection.activeObject = asset;
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
        }
        else
        {
            EditorGUILayout.HelpBox("❌ No data loaded . Click 'Reload Data' to load assets.", MessageType.Warning);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
