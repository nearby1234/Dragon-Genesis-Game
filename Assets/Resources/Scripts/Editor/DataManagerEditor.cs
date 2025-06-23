using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DataManager))]
public class DataManagerEditor : Editor
{
    private DataManager dataManager;
    private string searchFilter = "";

    // Lấy trực tiếp từ DataManager (List<SO> cho mỗi enum key)
    private Dictionary<Type, Dictionary<Enum, List<ScriptableObject>>> dataDictionary;


    private void OnEnable()
    {
        dataManager     = (DataManager)target;
        dataDictionary  = dataManager.GetDataDictionary();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // ───────── Base Data Manager ─────────
        EditorGUILayout.LabelField("📦 Base Data Manager", EditorStyles.boldLabel);
        if (GUILayout.Button("🔄 Reload Data", GUILayout.Height(30)))
        {
            
            dataManager.LoadAllData();
            dataDictionary = dataManager.GetDataDictionary();
            //questItemGroupCache = null;
            EditorUtility.SetDirty(dataManager);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("🔍 Search Data:", EditorStyles.boldLabel);
        searchFilter = EditorGUILayout.TextField(searchFilter);

        // ───────── Loaded Data (now List<SO> per enum) ─────────
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("📜 Loaded Data (SO lists):", EditorStyles.boldLabel);

        if (dataDictionary.Count > 0)
        {
            foreach (var typeEntry in dataDictionary)
            {
                var soType = typeEntry.Key;               // e.g. QuestItemSO
                var enumMap = typeEntry.Value;            // Dictionary<Enum, List<SO>>

                EditorGUILayout.LabelField($"▶ {soType.Name}", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                foreach (var kv in enumMap)
                {
                    var enumKey = kv.Key;                 // e.g. ITEM_ARMOR
                    var soList  = kv.Value;               // List<ScriptableObject>

                    // Skip empty
                    if (soList == null || soList.Count == 0) continue;

                    // Foldout style for enumKey
                    EditorGUILayout.LabelField($"── {enumKey}", EditorStyles.miniBoldLabel);
                    EditorGUI.indentLevel++;

                    // Display each asset under this key
                    foreach (var so in soList)
                    {
                        if (so == null) continue;
                        if (!string.IsNullOrEmpty(searchFilter) &&
                            !so.name.Contains(searchFilter, StringComparison.OrdinalIgnoreCase))
                            continue;

                        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                        GUILayout.Space(12);
                        EditorGUILayout.ObjectField(so, so.GetType(), false);
                        if (GUILayout.Button("👁", GUILayout.Width(30)))
                            Selection.activeObject = so;
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUI.indentLevel--;
                }

                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("❌ No data loaded. Click Reload Data.", MessageType.Warning);
        }

    }
}
