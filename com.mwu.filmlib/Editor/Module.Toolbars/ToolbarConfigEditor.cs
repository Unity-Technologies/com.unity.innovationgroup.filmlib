using UnityEngine;
using UnityEditor;

namespace MWU.FilmLib
{
    [CustomEditor(typeof(ToolbarConfig))]
    public class ToolbarConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var config = (ToolbarConfig)target;

            GUILayout.Label("Toolbar Configuration", EditorStyles.boldLabel);

            GUILayout.Label("Configure the toolbars (available under Tools->Toolbar) for the project", EditorStyles.helpBox);

            DrawDefaultInspector();
            
        }
    }
}