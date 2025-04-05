using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.ShortcutManagement;
using UnityEditor.Toolbars;
using UnityEngine;

namespace InfokubArcade.Favorites
{
    [EditorToolbarElement(id, typeof(SceneView))]
    public class SceneViewpointMenu : EditorToolbarDropdown
    {
        public const string id = "InfokubArcade/Tools/SceneViewpointMenu";

        [MenuItem("Tools/Infokub Arcade/My Favorites/Move to next viewpoint", false, 115)]
        [Shortcut("My Favorites/Move to next viewpoint", KeyCode.RightArrow, ShortcutModifiers.Control)]
        public static void NextPoint()
        {
            MovePoint(1);
        }

        [MenuItem("Tools/Infokub Arcade/My Favorites/Move to previous viewpoint", false, 110)]
        [Shortcut("My Favorites/Move to previous viewpoint", KeyCode.LeftArrow, ShortcutModifiers.Control)]
        public static void PrevPoint()
        {
            MovePoint(-1);
        }

        [MenuItem("Tools/Infokub Arcade/My Favorites/Create viewpoint from position", false, 100)]
        public static void CreateViewpoint()
        {
            if (ToolsSettings.Settings == null)
            {
                Debug.LogWarning("No Favorite Settings found");
                return;
            }

            if (SceneView.lastActiveSceneView != null)
            {
                ToolsSettings.SetViewpoint(EditorSceneManager.GetActiveScene(), SceneView.lastActiveSceneView);
            }
        }

        [MenuItem("Tools/Infokub Arcade/My Favorites/Remove viewpoint at position", false, 130)]
        public static void DeletePoint()
        {
            int idx = ToolsSettings.GetClosestViewpointIndex();

            if (idx > -1)
            {
                Undo.RecordObject(ToolsSettings.Settings, "Remove viewpoint");

                ToolsSettings.Settings.viewPoints.RemoveAt(idx);
            }
        }

        public static void MovePoint(int dir)
        {
            int idx = ToolsSettings.GetClosestViewpointIndex();

            if (idx > -1)
            {
                ChangePoint(LoadPoint(idx, dir));
            }
        }

        public static void ChangePoint(ToolsSettings.SceneViewpoint vp)
        {
            if (SceneView.lastActiveSceneView != null)
            {
                SceneView.lastActiveSceneView.LookAt(vp.position, Quaternion.Euler(vp.euler), Mathf.Max(0.25f, vp.zoom));
            }
        }

        protected static ToolsSettings.SceneViewpoint LoadPoint(int baseIndex, int prog)
        {
            int len = ToolsSettings.Settings.viewPoints.Count;
            string guid = AssetDatabase.AssetPathToGUID(EditorSceneManager.GetActiveScene().path);
            ToolsSettings.SceneViewpoint view = null;
            baseIndex = (baseIndex + len + prog) % len;

            do
            {
                if (ToolsSettings.Settings.viewPoints[baseIndex].scene.guid == guid)
                    view = ToolsSettings.Settings.viewPoints[baseIndex];

                baseIndex = (baseIndex + len + prog) % len;
                Debug.Log(baseIndex);

            } while (view == null);

            return view;


        }

        List<ToolsSettings.SceneViewpoint> vps;
        public SceneViewpointMenu()
        {
            text = "Scene Viewpoints";
            tooltip = "Move to a specific viewpoint";

            if (ToolsSettings.Settings == null)
                return;

            icon = ToolsSettings.Settings.icons["viewpoint" + ToolsSettings.darkExt];
            clicked += ShowMenu;
            vps = ToolsSettings.GetSceneViewpoints(EditorSceneManager.GetActiveScene());
        }

        ~SceneViewpointMenu()
        {
            vps.Clear();
            vps = null;
        }

        void ShowMenu()
        {

            GenericMenu menu = new GenericMenu();

            vps = ToolsSettings.GetSceneViewpoints(EditorSceneManager.GetActiveScene());


            menu.AddItem(new GUIContent("Create viewpoint from position"), false, CreateViewpoint);


            if (vps.Count == 0)
            {
                menu.AddDisabledItem(new GUIContent("No viewpoint found."));
                menu.ShowAsContext();
                return;
            }

            menu.AddSeparator("");
            foreach (ToolsSettings.SceneViewpoint vp in vps)
            {
                menu.AddItem(new GUIContent("Rename viewpoint/" + vp.pointName), false, OnRenameSelected, vp);
            }

            menu.AddSeparator("");
            foreach (ToolsSettings.SceneViewpoint vp in vps)
            {
                if (vp.IsClose(SceneView.lastActiveSceneView))
                    menu.AddDisabledItem(new GUIContent("Change viewpoint position/" + vp.pointName));
                else
                    menu.AddItem(new GUIContent("Change viewpoint position/" + vp.pointName), false, OnReplaceSelected, vp);
            }

            menu.AddSeparator("");
            foreach (ToolsSettings.SceneViewpoint vp in vps)
            {
                menu.AddItem(new GUIContent("Move to "+vp.pointName, "Use Ctrl+LeftArrow or RightArrow to switch"), vp.IsClose(SceneView.lastActiveSceneView), OnPointSelected, vp);
            }

            menu.ShowAsContext();
        }

        private void OnRenameSelected(object x)
        {
            ToolsSettings.SceneViewpoint vp = x as ToolsSettings.SceneViewpoint;
            SceneViewpointWindow.Display(vp);
        }

        private void OnPointSelected(object x)
        {
            ToolsSettings.SceneViewpoint vp = x as ToolsSettings.SceneViewpoint;

            ChangePoint(vp);
        }




        public void OnReplaceSelected(object x)
        {
            if (ToolsSettings.Settings == null)
            {
                Debug.LogWarning("No Favorite Settings found");
                return;
            }

            ToolsSettings.SceneViewpoint vp = x as ToolsSettings.SceneViewpoint;
            if (SceneView.lastActiveSceneView != null)
            {
                ToolsSettings.SetViewpoint(EditorSceneManager.GetActiveScene(), SceneView.lastActiveSceneView, vp);
            }
        }

    }

}
