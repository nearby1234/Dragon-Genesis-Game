using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.Toolbars;
using UnityEngine;

namespace InfokubArcade.Favorites
{
    [EditorToolbarElement(id, typeof(SceneView))]
    public class PrefabDirectoryTool : EditorToolbarDropdown
    {
        string selection;
        GenericMenu menu;
        public const string id = "InfokubArcade/Tools/PrefabDirectoryTool";

        public PrefabDirectoryTool()
        {
            tooltip = "Create a new prefab";
            text = "Prefab creator";

            if (ToolsSettings.Settings == null)
                return;

            icon = ToolsSettings.Settings.icons["delivery-box" + ToolsSettings.darkExt];
            clicked += ShowMenu;
        }

        void ShowMenu()
        {
            menu = new GenericMenu();

            if (ToolsSettings.ContainsDirectories(Selection.assetGUIDs))
            {
                menu.AddItem(new GUIContent("Add selected directories"), false, AddDirectories);
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("No Directory selected"));
            }

            if (Selection.activeGameObject != null)
            {

                foreach (ToolsSettings.GUIDData dir in ToolsSettings.Settings.prefabDirectoriesInfos)
                {
                    string path = AssetDatabase.GUIDToAssetPath(dir.guid);
                    menu.AddItem(new GUIContent("Create prefab/"+ ToolsSettings.DirPath(path).Replace("/", ">")), false, OnChoiceSelected, path);
                }
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("No GameObject selected"));
            }

            if (ToolsSettings.Settings.prefabDirectoriesInfos.Count > 0)
            {
                foreach (ToolsSettings.GUIDData dir in ToolsSettings.Settings.prefabDirectoriesInfos)
                {
                    string path = AssetDatabase.GUIDToAssetPath(dir.guid);
                    menu.AddItem(new GUIContent("Select directory/" + ToolsSettings.DirPath(path).Replace("/", ">")), false, OnSelected, path);
                }
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("No favorite directory found"));
            }

            menu.ShowAsContext();
        }

        private void OnChoiceSelected(object dirName)
        {
            selection = (string)dirName;
            string localpath = selection + "/" + Selection.activeGameObject.name + ".prefab";
            bool success;

            localpath = AssetDatabase.GenerateUniqueAssetPath(localpath);

            PrefabUtility.SaveAsPrefabAssetAndConnect(Selection.activeGameObject, localpath, InteractionMode.UserAction, out success);

            if (!success)
                Debug.LogWarning("An error has occurred for prefab creation");
        }

        private void OnSelected(object dirName)
        {
            selection = (string)dirName;

            ToolsSettings.SelectDirectory(selection);
        }

        private void AddDirectories()
        {
            ToolsSettings.AddDirectories(Selection.assetGUIDs);
        }
    }
}
