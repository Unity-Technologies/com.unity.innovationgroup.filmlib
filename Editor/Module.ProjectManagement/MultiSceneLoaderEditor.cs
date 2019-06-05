using UnityEngine;
using UnityEditor;

namespace MWU.FilmLib
{
	[CustomEditor(typeof(MultiSceneLoader))]
	public class MultiSceneLoaderEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			MultiSceneLoader config = (MultiSceneLoader)target;

            DrawDefaultInspector();
            GUILayout.Label("Multi-Scene Loading Config", EditorStyles.boldLabel);

            EditorGUILayout.Space();
			if (GUILayout.Button("Load Main Scenes", GUILayout.MinHeight(100), GUILayout.Height(50)))
				config.LoadAllScenes();
			
			EditorGUILayout.Space();
            if (GUILayout.Button("Load Set Scenes", GUILayout.MinHeight(100), GUILayout.Height(50)))
				config.LoadSetScenes(true);
		}
	}
}