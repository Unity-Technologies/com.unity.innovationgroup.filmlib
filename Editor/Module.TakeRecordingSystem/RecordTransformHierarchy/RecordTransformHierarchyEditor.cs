using UnityEngine;
using System.Collections;
using UnityEditor;

namespace MWU.FilmLib
{
    [CustomEditor(typeof(RecordTransformHierarchy))]
    public class RecordTransformHierarchyEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var targetScript = (RecordTransformHierarchy)target;

            // if we're playing the game, then we can do stuff
            if (Application.isPlaying)
            {
                if (targetScript.recordingActive)
                {
                    if (GUILayout.Button("Stop Recording"))
                    {
                        targetScript.StopRecording();
                    }
                }
                else
                {
                    if (GUILayout.Button("Start Recording"))
                    {
                        targetScript.StartRecording();
                    }
                }
            }
        }
    }
}