using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace InfokubArcade.Favorites
{
    [EditorToolbarElement(id, typeof(SceneView))]
    public class FavoriteSceneTool : EditorToolbarToggle
    {
        public const string id = "InfokubArcade/Tools/FavoriteSceneTool";
        public FavoriteSceneTool()
        {
            tooltip = "Add to Favorite scenes";

            if (ToolsSettings.Settings == null)
                return;

            icon = ToolsSettings.Settings.icons["bk-add"];
            offIcon = ToolsSettings.Settings.icons["bk-add" + ToolsSettings.darkExt];
            onIcon = ToolsSettings.Settings.icons["bk-remove" + ToolsSettings.darkExt];

            if (!FavoriteSceneMenu.AddFavoriteCheck())
            {
                SetValueWithoutNotify(true);
                tooltip = "Remove from Favorite scenes";
            }

            this.RegisterValueChangedCallback(ChangeValue);
            EditorSceneManager.activeSceneChanged += UpdateIcon;
            EditorSceneManager.activeSceneChangedInEditMode += UpdateIcon;
            FavoriteSceneMenu.OnUpdateScene += UpdateIcon;
            ToolsSettings.OnUpdateScene += UpdateIcon;
        }

        ~FavoriteSceneTool()
        {
            EditorSceneManager.activeSceneChanged -= UpdateIcon;
            EditorSceneManager.activeSceneChangedInEditMode -= UpdateIcon;
            FavoriteSceneMenu.OnUpdateScene -= UpdateIcon;
            ToolsSettings.OnUpdateScene -= UpdateIcon;
        }

        private void UpdateIcon(Scene lastScene, Scene newScene)
        {
            UpdateIcon();
        }

        private void UpdateIcon()
        {
            if (!FavoriteSceneMenu.AddFavoriteCheck())
            {
                SetValueWithoutNotify(true);
                tooltip = "Remove from Favorite scenes";
            }
            else
            {
                SetValueWithoutNotify(false);
                tooltip = "Add to Favorite scenes";
            }
        }

        private void ChangeValue(ChangeEvent<bool> evt)
        {
            if (evt.newValue)
            {
                FavoriteSceneMenu.AddFavorite();
                tooltip = "Remove from Favorite scenes";
            }
            else
            {
                FavoriteSceneMenu.RemoveFavorite();
                tooltip = "Add to Favorite scenes";
            }
        }
    }
}