using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MWU.FilmLib
{

    public class EditorUtilities : MonoBehaviour
    {
        /// <summary>
        /// Root path to our package
        /// </summary>
        public static string packagePathRoot = "Packages/com.unity.innovationgroup.filmlib";

#if UNITY_EDITOR
        /// <summary>
        /// Search for a specified object in the actively loaded scenes and mark it as the active selection
        /// </summary>
        /// <param name="thisObject"></param>
        public static void FindSceneObject(string thisObject)
        {
            var selected = GetSceneObject(thisObject);
            Selection.activeObject = selected;
        }

        public static GameObject GetSceneObject( string thisObject)
        {
            return GameObject.Find(thisObject);
        }
#endif

        public static GameObject SearchOpenScenesForObject(string thisObject)
        {
            var openSceneCount = SceneManager.sceneCount;
            for (int i = 0; i < openSceneCount; i++)
            {
                var thisScene = SceneManager.GetSceneAt(i);
                if (thisScene.isLoaded)
                {
                    //  Debug.Log("Scanning scene : " + thisScene.name);
                    var rootGO = thisScene.GetRootGameObjects();
                    foreach (var go in rootGO)
                    {
                        var children = go.GetComponentsInChildren<Transform>(true);
                        foreach (var child in children)
                        {
                            if (child.name == thisObject)
                                return child.gameObject;
                        }
                    }
                }
            }
            return null;
        }

#if UNITY_EDITOR
        /// <summary>
        /// open the project window and 'ping' a specific asset
        /// </summary>
        /// <param name="path"></param>
        public static void FindProjectLoader(string path)
        {
            // Check the path has no '/' at the end, if it does remove it,
            // Obviously in this example it doesn't but it might
            // if your getting the path some other way.

            if (path[path.Length - 1] == '/')
                path = path.Substring(0, path.Length - 1);

            // Load object
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));

            // Select the object in the project folder
            Selection.activeObject = obj;

            // Also flash the folder yellow to highlight it
            EditorGUIUtility.PingObject(obj);
        }
#endif
    }
}
