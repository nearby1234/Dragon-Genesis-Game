#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SaveManager))]
public class SaveManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Open Save Folder"))
        {
            string path = Application.persistentDataPath;
            EditorUtility.RevealInFinder(path); // hoặc RevealInExplorer(path) trên Windows
        }

        if (GUILayout.Button("Print Save JSON"))
        {
            (target as SaveManager)?.PrintSaveData();
        }
    }
}
#endif
