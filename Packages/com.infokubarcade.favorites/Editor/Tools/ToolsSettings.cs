using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

// prevent old content from v1.0.0
#pragma warning disable CS0618

namespace InfokubArcade.Favorites
{
    public partial class ToolsSettings : ScriptableObject
    {
        public static event Action OnUpdateScene;
        public static ToolsSettings Settings { get { if (toolsSettings == null) Init(); return toolsSettings; } set { toolsSettings = value; } }
        public static string darkExt => EditorGUIUtility.isProSkin ? "_d" : "";

        [System.Serializable] public class SceneViewpoint
        {
            public string pointName;
            public GUIDData scene;
            public Vector3 position;
            public float zoom;
            public Vector3 euler;

            public bool IsClose(SceneView view)
            {
                if (view == null)
                    return false;

                Camera cam = view.camera;

                return Vector3.Distance(position, view.pivot) < 0.5f && Vector3.Dot(Quaternion.Euler(euler) * Vector3.forward, cam.transform.forward) > 0.75f;
            }
        }

        [System.Serializable] public class GUIDData
        {
            public string objectName { get; private set; }
            public string guid;
            public bool isDir { get; private set; }

            public string path { get; private set; }

            public GUIDData(string _guid)
            {
                guid = _guid;

                if (guid.Length == 0)
                    return;

                path = AssetDatabase.GUIDToAssetPath(guid);
                objectName = LastWord(AssetDatabase.GUIDToAssetPath(guid).Replace(".unity", ""));
                isDir = AssetDatabase.IsValidFolder(AssetDatabase.GUIDToAssetPath(guid));
            }
            public string LastWord(string objPath)
            {
                return objPath.Substring(objPath.LastIndexOf('/') + 1);
            }
        }

        static ToolsSettings toolsSettings;

        [HideInInspector] [Obsolete("Use favoriteScenesInfos instead")] public List<string> favoriteScenes;
        public List<GUIDData> favoriteScenesInfos;
        [HideInInspector]
        [Obsolete("Use prefabDirectoriesInfos instead")]
        public List<string> prefabDirectories;
        public List<GUIDData> prefabDirectoriesInfos;
        public List<SceneViewpoint> viewPoints;
        public Dictionary<string, Texture2D> icons;

        [MenuItem("Tools/Infokub Arcade/My Favorites/Refresh settings", priority =2)]
        public static void Reload()
        {
            toolsSettings = null;
            Init();
        }
        [MenuItem("Tools/Infokub Arcade/My Favorites/Select settings", priority = 5)]
        public static void SelectSettings()
        {
            Selection.activeObject = toolsSettings;
        }

        [MenuItem("Tools/Infokub Arcade/My Favorites/Convert settings from previous version", priority = 6, validate =false)]
        public static void ConvertVersion()
        {
            Undo.RecordObject(ToolsSettings.Settings, "Convert from previous version");
            Undo.IncrementCurrentGroup();
            Undo.SetCurrentGroupName("Convertion group");

            List<string> dirs = new List<string>(ToolsSettings.Settings.prefabDirectories.Count);
            foreach (string p in ToolsSettings.Settings.prefabDirectories)
            {
                if (AssetDatabase.IsValidFolder(p))
                {
                    string guid = AssetDatabase.AssetPathToGUID(p);
                    dirs.Add(guid);
                }
            }

            ToolsSettings.AddDirectories(dirs.ToArray());
            dirs.Clear();

            foreach (string p in ToolsSettings.Settings.favoriteScenes)
            {
                string guid = AssetDatabase.AssetPathToGUID((p.StartsWith("Packages") ? "" : "Assets/") + p + ".unity");

                if (guid.Length > 0)
                    ToolsSettings.AddScene(new GUIDData(guid));
            }

            ToolsSettings.Settings.prefabDirectories.Clear();
            ToolsSettings.Settings.favoriteScenes.Clear();

            AssetDatabase.SaveAssetIfDirty(ToolsSettings.Settings);
        }

        [MenuItem("Tools/Infokub Arcade/My Favorites/Convert settings from previous version", priority = 6, validate = true)]
        public static bool ConvertVersionTest()
        {
            bool ok = false;

            if (ToolsSettings.Settings != null)
            {
                if (ToolsSettings.Settings.prefabDirectories.Count > 0 || ToolsSettings.Settings.favoriteScenes.Count > 0)
                    ok = true;
            }

            return ok;
        }

        public static string DirPath(string path)
        {
            return path.Replace("Packages/", "").Replace("Assets/", "").Replace(".unity", "");
        }

        public static void Init()
        {
            if (toolsSettings != null)
                return;

            string[] guid = AssetDatabase.FindAssets("t:ToolsSettings", new string[] { "Packages/com.infokubarcade.favorites/Editor/" });

            if (guid.Length == 0)
            {
                Debug.LogWarning("No settings found for Favorites");
                return;
            }

            if (guid.Length > 1)
                Debug.LogWarning(guid.Length + " settings found for Favorites");

            string path = AssetDatabase.GUIDToAssetPath(guid[0]);
            toolsSettings = AssetDatabase.LoadAssetAtPath<ToolsSettings>(path);

            LoadIcons();
        }

        public static GUIDData PathToData(string path)
        {
            string id = AssetDatabase.AssetPathToGUID(path, AssetPathToGUIDOptions.OnlyExistingAssets);

            return new GUIDData(id);
        }

        public static SceneViewpoint SetViewpoint(Scene scn, SceneView view, SceneViewpoint vp = null)
        {
            if (Settings.viewPoints == null)
                Settings.viewPoints = new List<SceneViewpoint>();

            Vector3 pos = view.pivot;
            Camera cam = view.camera;
            Vector3 dir = cam.transform.forward;
            
            if (!Settings.viewPoints.Exists(x => Vector3.Distance(pos, x.position) < 0.5f && Vector3.Dot(Quaternion.Euler(x.euler) * Vector3.forward, dir) > 0.75f))
            {
                

                if (vp == null)
                {
                    Undo.RecordObject(ToolsSettings.Settings, "Add viewpoint to My Favorites tool Settings");
                    SceneViewpoint np = new SceneViewpoint() { scene = PathToData(scn.path), euler = cam.transform.eulerAngles, pointName = "New Position " + Settings.viewPoints.Count, position = pos, zoom = view.size };
                    Settings.viewPoints.Add(np);
                }
                else
                {
                    Undo.RecordObject(ToolsSettings.Settings, "Update viewpoint to My Favorites tool Settings");
                    vp.zoom = view.size;
                    vp.position = view.pivot;
                    vp.euler = view.camera.transform.eulerAngles;
                }

                AssetDatabase.SaveAssetIfDirty(ToolsSettings.Settings);
                return vp;
            }
            else
            {
                Debug.LogWarning("Too close to an other viewpoint.");
            }

            return null;
        }

        public static List<SceneViewpoint> GetSceneViewpoints(Scene scn)
        {
            if (Settings == null)
                return null;

            if (Settings.viewPoints == null)
                Settings.viewPoints = new List<SceneViewpoint>();

            string id = AssetDatabase.AssetPathToGUID(scn.path, AssetPathToGUIDOptions.OnlyExistingAssets);

            return Settings.viewPoints.FindAll(x => x.scene.guid == id);
        }

        public static int GetClosestViewpointIndex(List<SceneViewpoint> pts = null)
        {
            if (pts == null)
                pts = GetSceneViewpoints(EditorSceneManager.GetActiveScene());

            float lastDist = Mathf.Infinity;
            int closeIdx = -1;

            for (int i = 0; i < pts.Count; i++)
            {
                ToolsSettings.SceneViewpoint p = pts[i];
                if (p.IsClose(SceneView.lastActiveSceneView))
                {
                    closeIdx = Settings.viewPoints.IndexOf(p);
                    break;
                }
                else if (Vector3.Distance(SceneView.lastActiveSceneView.pivot, p.position) < lastDist)
                {
                    closeIdx = Settings.viewPoints.IndexOf(p);
                    lastDist = Vector3.Distance(SceneView.lastActiveSceneView.pivot, p.position);
                }
            }

            return closeIdx;
        }


        public static void AddScene(GUIDData data)
        {
            if (Settings.favoriteScenesInfos.Exists(x => x.guid == data.guid))
            {
                Debug.LogWarning("Scene '"+data.objectName+"' already in favorites");
            }
            else
            {
                Undo.RecordObject(ToolsSettings.Settings, "Add scene " + data.objectName + " to My Favorites tool Settings");
                Settings.favoriteScenesInfos.Add(data);
                AssetDatabase.SaveAssetIfDirty(ToolsSettings.Settings);
            }
        }

        public static void RemoveScene(string guid)
        {
                Undo.RecordObject(ToolsSettings.Settings, "Remove scene to My Favorites tool Settings");
                Settings.favoriteScenesInfos.RemoveAll(x => x.guid == guid);
            AssetDatabase.SaveAssetIfDirty(ToolsSettings.Settings);
        }

        public static bool SceneExists(string guid)
        {
            return Settings.favoriteScenesInfos.Exists(x => x.guid == guid);
        }

        public static void LoadScene(string path)
        {
            if (EditorSceneManager.GetActiveScene().isDirty)
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    InstantLoadScene(path);
                }
            }
            else
                InstantLoadScene(path);
        }

        private static void InstantLoadScene(string path)
        {
            EditorSceneManager.OpenScene(path);
        }

        public static void SelectDirectory(string path)
        {
            UnityEngine.Object o = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            EditorUtility.FocusProjectWindow();

            Selection.activeObject = o;
            EditorGUIUtility.PingObject(o);
        }
        public static void AddDirectories(string[] guids)
        {
            if (guids.Length > 0)
            {
                Undo.RecordObject(ToolsSettings.Settings, "Add directories to My Favorites tool Settings");
                foreach (string id in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(id);
                    string dirname = path.Substring(path.LastIndexOf('/') + 1);

                    if (AssetDatabase.IsValidFolder(path))
                    {
                        if (!ToolsSettings.Settings.prefabDirectoriesInfos.Exists(x => x.guid == id))
                        {

                            ToolsSettings.Settings.prefabDirectoriesInfos.Add(new GUIDData(id));
                            AssetDatabase.SaveAssetIfDirty(ToolsSettings.Settings);
                        }

                    }
                    else
                        Debug.LogWarning("Path " + path + " is not a folder");
                }
            }
            else
            {
                Debug.LogWarning("No Selection Found");
            }
        }

        public static bool ContainsDirectories(string[] guids)
        {
            if (guids.Length > 0)
            {
                foreach (string id in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(id);

                    if (AssetDatabase.IsValidFolder(path))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void Clear()
        {
            if (toolsSettings == null)
                return;

            Undo.RecordObject(ToolsSettings.Settings, "Clear My Favorites tool Settings");
            toolsSettings.prefabDirectories.Clear();
            toolsSettings.favoriteScenes.Clear();
            toolsSettings.prefabDirectoriesInfos.Clear();
            toolsSettings.favoriteScenesInfos.Clear();
            toolsSettings.viewPoints.Clear();
            OnUpdateScene?.Invoke();

            AssetDatabase.SaveAssetIfDirty(ToolsSettings.Settings);
        }

        private static void LoadIcons()
        {
            string[] guid = AssetDatabase.FindAssets("t:Texture2D", new string[] { "Packages/com.infokubarcade.favorites/Editor/Tools/Icons" });
            toolsSettings.icons = new Dictionary<string, Texture2D>();

            foreach (string id in guid)
            {
                string path = AssetDatabase.GUIDToAssetPath(id);
                Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                toolsSettings.icons.Add(icon.name, icon);
            }
        }

        #region Editor
#if UNITY_EDITOR
        [CustomEditor(typeof(ToolsSettings))]
        class ToolsSettingEditor : Editor
        {
            string sceneName;
            bool isIn;

            private void OnEnable()
            {
                isIn = SceneExists(FavoriteSceneMenu.currentSceneGUID);
            }

            public override void OnInspectorGUI()
            {
                if (!isIn)
                {
                    if (GUILayout.Button("Add Active Scene"))
                    {
                        FavoriteSceneMenu.AddFavorite();
                        isIn = true;
                    }
                }
                else
                {
                    if (GUILayout.Button("Remove Active Scene"))
                    {
                        FavoriteSceneMenu.RemoveFavorite();
                        isIn = false;
                    }
                }

                if (GUILayout.Button("Add Selected Directory"))
                {
                    ToolsSettings.AddDirectories(Selection.assetGUIDs);
                }

                if (GUILayout.Button("Clear Everything"))
                {
                    if (EditorUtility.DisplayDialog("Clear settings", "Are you sure ?", "Yes, clear.", "Cancel"))
                    {
                        ToolsSettings.Settings?.Clear();
                    }
                }

                if (ToolsSettings.Settings.prefabDirectories?.Count > 0 || ToolsSettings.Settings.favoriteScenes?.Count > 0)
                {
                    if (GUILayout.Button("Convert from previous version"))
                    {
                        ToolsSettings.ConvertVersion();
                    }
                }


                EditorGUILayout.PropertyField(serializedObject.FindProperty("prefabDirectoriesInfos"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("favoriteScenesInfos"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("viewPoints"));

                serializedObject.ApplyModifiedProperties();
                serializedObject.UpdateIfRequiredOrScript();
            }
        }
        [CustomPropertyDrawer(typeof(ToolsSettings.GUIDData))]
        public class GUIDDrawer : PropertyDrawer
        {

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                GUIContent icon = EditorGUIUtility.IconContent((EditorGUIUtility.isProSkin ? "d_" : "") + "animationvisibilitytoggleon");
                SerializedProperty guid = property.FindPropertyRelative("guid");
                ToolsSettings.GUIDData content = new GUIDData(guid.stringValue);
                EditorGUI.BeginProperty(position, label, property);

                // Draw label
                position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), (content.guid.Length == 0 ? label : new GUIContent(content.objectName)));

                // Don't make child fields be indented
                int indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;


                if (guid.stringValue.Length > 0)
                {

                    if (GUI.Button(position, new GUIContent(content.objectName, icon.image, content.path)))
                    {
                            ToolsSettings.SelectDirectory(content.path);
                    }
                }
                else
                {
                    GUI.Label(position, new GUIContent("Unknown"));
                }

                // Set indent back to what it was
                EditorGUI.indentLevel = indent;

                EditorGUI.EndProperty();
            }
        }

        [CustomPropertyDrawer(typeof(ToolsSettings.SceneViewpoint))]
        public class ViewpointDrawer : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                EditorGUI.BeginProperty(position, label, property);
                property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property.isExpanded, label);

                if (property.isExpanded)
                {
                    DrawProperty(position, 1, property.FindPropertyRelative("pointName"));
                    DrawProperty(position, 2, property.FindPropertyRelative("scene"));
                }
                EditorGUI.EndProperty();
                
            }

            private void DrawProperty(Rect position, int height, SerializedProperty property)
            {
                Rect fieldPos = new Rect(position.x, position.y + (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * height, position.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(fieldPos, property);
            }

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                if (!property.isExpanded)
                    return EditorGUIUtility.singleLineHeight;

                return EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing * 2;
            }
        }
#endif
        #endregion
    }
}
