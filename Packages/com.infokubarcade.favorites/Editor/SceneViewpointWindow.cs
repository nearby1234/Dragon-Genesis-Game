using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace InfokubArcade.Favorites
{
    public class SceneViewpointWindow : EditorWindow
    {
        SerializedObject target;
        SerializedProperty prop;
        ToolsSettings.SceneViewpoint currentViewpoint = null;

        [MenuItem("Tools/Infokub Arcade/My Favorites/Rename viewpoint at position", false, 105)]
        public static void RenameWindow()
        {
            int idx = ToolsSettings.GetClosestViewpointIndex();
            Display(ToolsSettings.Settings.viewPoints[idx]);
        }

        [MenuItem("Tools/Infokub Arcade/My Favorites/Rename viewpoint at position", true, 105)]
        public static bool RenameWindowCheck()
        {
            return ToolsSettings.GetClosestViewpointIndex() > -1;
        }

        public static void Display(ToolsSettings.SceneViewpoint vp)
        {
            SceneViewpointWindow w = SceneViewpointWindow.GetWindow<SceneViewpointWindow>();
            Rect r = SceneView.lastActiveSceneView.position;
            w.position = new Rect(r.position.x+r.width-260, r.position.y+64, 250, EditorGUIUtility.singleLineHeight*4);
            w.SetViewpoint(vp);
            w.ShowPopup();
        }

        public void SetViewpoint(ToolsSettings.SceneViewpoint vp)
        {
            currentViewpoint = vp;
            prop = target.FindProperty("viewPoints.Array.data[" + ToolsSettings.Settings.viewPoints.FindIndex(x => x == currentViewpoint) + "].pointName");
        }

        private void OnEnable()
        {
            target = new SerializedObject(ToolsSettings.Settings);
        }

        private void OnGUI()
        {

            Undo.RecordObject(target.targetObject, "Change Viewpoint name");
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Rename Viewpoint");

            if (prop != null)
                EditorGUILayout.PropertyField(prop, GUIContent.none);

            if (EditorGUI.EndChangeCheck())
            {
                target.ApplyModifiedProperties();
            }

            if (GUILayout.Button("Close"))
                this.Close();
        }
    }

}
