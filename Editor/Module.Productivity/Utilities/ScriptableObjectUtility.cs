using UnityEngine;
using UnityEditor;
using System.IO;

public static class ScriptableObjectUtility
{
    /// <summary>
    /// Create a new ScriptableObject asset at the specified path 
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    public static void CreateAsset<T>(string path, string fileName) where T : ScriptableObject
    {
        CreateAssetType<T>(path, fileName);
    }

    /// <summary>
    /// Create a scriptable object of type T at the given path & filename. Returns the SO that was created
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static T CreateAssetType<T>(string path, string fileName) where T : ScriptableObject
    {
        var asset = ScriptableObject.CreateInstance<T>();
        string fullFileName;
        if (fileName == "")
        {
            fullFileName = "/New " + typeof(T).ToString() + ".asset";
        }
        else
        {
            fullFileName = "/" + fileName + ".asset";
        }

        var assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + fullFileName);

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;

        return asset;
    }

    /// <summary>
    //	This makes it easy to create, name and place unique new ScriptableObject asset files.
    /// </summary>
    public static void CreateAsset<T>() where T : ScriptableObject
    {
        var path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        CreateAsset<T>(path, "");
    }
}