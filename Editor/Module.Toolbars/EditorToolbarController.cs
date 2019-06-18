using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering.HDPipeline;
using UnityEngine.SceneManagement;
using UnityEditor.FilmTV.Toolbox;

namespace MWU.FilmLib
{

    public class EditorToolbarController : MonoBehaviour
    {
#if USING_FILMTOOLBOX
        public static void OpenMaterialRemapper()
        {
            // open the material remapper
            var window = EditorWindow.GetWindow<MaterialRemapperView>("Material Remapper");
            window.minSize = new Vector2(640, 480);
            window.Show();
        }
#endif

#if USING_MWU_HDRP
        [MenuItem("Window/Rendering/Render Window")]
        public static void OpenRenderWindow()
        {
            // open the material remapper
            var window = EditorWindow.GetWindow<RenderWindow>("Render Window");
            window.Show();
        }
#endif

        public static void CreateToolbarConfig()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Settings"))
            {
                Debug.Log("Creating folder: Settings");
                AssetDatabase.CreateFolder("Assets", "Settings");
            }
            else
            {
                Debug.Log("Folder exists: Settings");
            }

            if (!AssetDatabase.IsValidFolder("Assets/Settings/Toolbar"))
            {
                Debug.Log("Creating folder: Settings/Toolbar");
                AssetDatabase.CreateFolder("Assets/Settings", "Toolbar");
            }
            else
            {
                Debug.Log("Folder exists: Settings/Toolbar");
            }

            ScriptableObjectUtility.CreateAsset<ToolbarConfig>("Assets/Settings/Toolbar", "ToolbarConfig");
        }
    }
}