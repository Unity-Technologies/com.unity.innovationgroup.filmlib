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
            clip = new AnimationClip();

            recordingActive = true;
        }

        public void StopRecording()
        {
            if (clip == null)
                return;

            if (objectRecorder.isRecording)
            {
                // Save the recorded session to the clip.
                objectRecorder.SaveToClip(clip);

                AssetDatabase.CreateAsset(clip, "Assets/Test.anim");
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