#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class ResetSOOnExitPlayMode
{
    static ResetSOOnExitPlayMode()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        string[] guids = AssetDatabase.FindAssets("t:ScriptableObject");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ScriptableObject asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);

            if (asset is IResettableSO resettable)
            {
                if (state == PlayModeStateChange.EnteredPlayMode)
                {
                    resettable.BackupData();
                }
                else if (state == PlayModeStateChange.ExitingPlayMode)
                {
                    resettable.RestoreData();
                    EditorUtility.SetDirty(asset);
                }
            }
        }

        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            AssetDatabase.SaveAssets();
        }
    }
}
#endif
