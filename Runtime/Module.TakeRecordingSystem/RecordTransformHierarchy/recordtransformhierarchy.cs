using System;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace MWU.FilmLib
{
    public class RecordTransformHierarchy : MonoBehaviour
    {
        public AnimationClip clip;
        public bool recordingActive = false;
        public GameObject objectToRecord;

        private GameObjectRecorder objectRecorder;

        void Start()
        {
            // Create recorder and record the script GameObject.
            objectRecorder = new GameObjectRecorder(objectToRecord);

            // Bind all the Transforms on the GameObject and all its children.
            objectRecorder.BindComponentsOfType<Transform>(objectToRecord, true);
        }

        public void StartRecording()
        {
            if (Application.isPlaying)
            {
                clip = new AnimationClip();

                recordingActive = true;
            }
        }

        public void StopRecording()
        {
            if (clip == null)
                return;

            if (objectRecorder.isRecording)
            {
                // Save the recorded session to the clip.
                objectRecorder.SaveToClip(clip);

                var path = AssetDatabase.GenerateUniqueAssetPath("Assets/Test.anim");
                try
                {
                    AssetDatabase.CreateAsset(clip, path);

                }
                catch( Exception e)
                {
                    Debug.Log("AssetDatabase.CreateAsset() - " + e.Message);
                }
            }

            recordingActive = false;
        }

        void LateUpdate()
        {
            if (clip == null)
                return;

            if( recordingActive)
            {
                // Take a snapshot and record all the bindings values for this frame.
                objectRecorder.TakeSnapshot(Time.deltaTime);
            }
            
        }

        void OnDisable()
        {
            StopRecording();
        }
    }
}