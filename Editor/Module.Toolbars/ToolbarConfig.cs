using UnityEngine;

namespace MWU.FilmLib
{
    public enum SceneLoaderType
    {
        Individual,
        MultiScene,
    }

    [CreateAssetMenu(fileName = "ToolbarConfig", menuName = "Toolbars/Create Toolbar Config", order = 1)]
    public class ToolbarConfig : ScriptableObject
    {
        // enable / disable macro sections of the toolbars
        public bool showSceneLoader = true;
        public bool showSceneTools = true;
        public bool showRenderSettings = true;
        public bool showSceneHelpers = true;
        public bool showLayoutModes = true;
        public bool showPerformance = true;
        public SceneLoaderType sceneLoaderType = SceneLoaderType.Individual;

        [SerializeField] public Object[] sceneLoaderList = null;
        [SerializeField] public Object[] windowLayouts = null;
    }
}