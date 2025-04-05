using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;

namespace InfokubArcade.Favorites
{
    [EditorToolbarElement(id, typeof(SceneView))]
    public class FavoriteSceneMenu : EditorToolbarDropdown
    {
        public static event Action OnUpdateScene;
        public static string currentSceneName => EditorSceneManager.GetActiveScene().name;
        public static string currentSceneGUID => AssetDatabase.AssetPathToGUID(EditorSceneManager.GetActiveScene().path);

        public const string id = "InfokubArcade/Tools/FavoriteSceneMenu";
        GenericMenu menu;
        static string selection;

        public FavoriteSceneMenu()
        {
            text = "Favorite Scenes";
            tooltip = "Load a Favorite Scene";

            if (ToolsSettings.Settings == null)
                return;

            icon = ToolsSettings.Settings.icons["map" + ToolsSettings.darkExt];
            clicked += ShowMenu;
        }

        [MenuItem("Tools/Infokub Arcade/My Favorites/Add Scene to Favorites", false, 50)]
        public static void AddFavorite()
        {
            //ToolsSettings.Settings.favoriteScenes.Add(currentScene);
            ToolsSettings.GUIDData data = new ToolsSettings.GUIDData(currentSceneGUID);
            ToolsSettings.AddScene(data);
            OnUpdateScene?.Invoke();
        }

        [MenuItem("Tools/Infokub Arcade/My Favorites/Remove Scene from Favorites", false, 52)]
        public static void RemoveFavorite()
        {
            //ToolsSettings.Settings.favoriteScenes.Remove(currentScene);
            ToolsSettings.RemoveScene(currentSceneGUID);
            AssetDatabase.SaveAssetIfDirty(ToolsSettings.Settings);
            OnUpdateScene?.Invoke();
        }

        [MenuItem("Tools/Infokub Arcade/My Favorites/Add Scene to Favorites", true)]
        public static bool AddFavoriteCheck()
        {
            return ToolsSettings.Settings != null && !ToolsSettings.SceneExists(currentSceneGUID);
        }

        [MenuItem("Tools/Infokub Arcade/My Favorites/Remove Scene from Favorites", true)]
        public static bool RemoveFavoriteCheck()
        {
            return ToolsSettings.Settings != null && ToolsSettings.SceneExists(currentSceneGUID);
        }

        void ShowMenu()
        {
            menu = new GenericMenu();
            string curScene = currentSceneGUID;

            if (ToolsSettings.Settings.favoriteScenesInfos.Count == 0)
            {
                menu.AddDisabledItem(new GUIContent("No favorite scene found"));
            }
            else
            {
                foreach (ToolsSettings.GUIDData scene in ToolsSettings.Settings.favoriteScenesInfos)
                {
                    string path = ToolsSettings.DirPath(AssetDatabase.GUIDToAssetPath(scene.guid));

                    menu.AddItem(new GUIContent(path), curScene.Equals(scene.guid), OnSceneSelected, scene.guid);
                }
            }

            menu.ShowAsContext();
        }

        private void OnSceneSelected(object guid)
        {
            selection = (string)guid;

            ToolsSettings.LoadScene(AssetDatabase.GUIDToAssetPath(selection));

        }
    }
}
