


using UnityEditor;
using UnityEditor.Overlays;

namespace InfokubArcade.Favorites
{
    [Overlay(typeof(SceneView), "Favorites Panel")]
    public class InfokubOverlay : ToolbarOverlay
    {
        public InfokubOverlay() : base(
            FavoriteSceneTool.id,
            FavoriteSceneMenu.id,
            PrefabDirectoryTool.id,
            SceneViewpointMenu.id
            )
        {

        }
    }
}