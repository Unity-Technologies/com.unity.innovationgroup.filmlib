using MWU.FilmLib.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;
using Loc = MWU.FilmLib.TakeRecordingLoc;

namespace MWU.FilmLib
{
    public enum TIMELINE_TEMPLATE
    {
        MASTER_TIMELINE,
        BEAT_TIMELINE
    }

    public class TrackRecording
    {
        public TimelineAsset thisTimeline;
        public TrackAsset thisTrack;
        public string trackName;
        public List<RecordingTake> takes = new List<RecordingTake>();
    }

    public struct RecordingTake
    {
        public string takeName;
        public TimelineClip clip;
        public AnimationClip animClip;
    }

    public class TakeRecordingController : MonoBehaviour
    {
        public static PlayableDirector activeTimeline;
        public static List<string> timelineListLabel = new List<string>();
        public static List<PlayableDirector> timelineList = new List<PlayableDirector>();
        public static int selectedTimelineIdx = 0;
        public static int activeTimelineIdx = 0;

        private void OnEnable()
        {
            EditorSceneManager.sceneOpened += SceneOpenedCallback;
            Selection.selectionChanged += SelectionChangedCallback;
        }
        private void OnDisable()
        {
            EditorSceneManager.sceneOpened -= SceneOpenedCallback;
            Selection.selectionChanged -= SelectionChangedCallback;
        }

        public static void SelectionChangedCallback()
        {
            // check if this is a timeline
            var obj = Selection.activeObject as GameObject;
            if (obj != null)
            {
                if (obj.GetComponent<PlayableDirector>() != null)
                {
                    GetActiveTimeline();
                }
            }
        }

        public static void SceneOpenedCallback(Scene newScene, OpenSceneMode _mode)
        {
            activeTimeline = null;
            RefreshTimelinesInScene(true);
        }

        /// <summary>
        /// retrieves which timeline is currently active in the timeline window
        /// </summary>
        /// <returns></returns>
        public static PlayableDirector GetActiveTimeline()
        {
            var pdAsset = TimelineUtils.GetCurrentActiveTimeline();
            if (pdAsset != null)
            {
                activeTimeline = TimelineUtils.GetDirectorFromTimeline(pdAsset);
            }
            
            return activeTimeline;
        }

        public static int GetTimelineIdx(PlayableDirector timeline)
        {
            for (int i = 0; i < timelineList.Count; i++)
            {
                if (timelineList[i].name == activeTimeline.name)
                {
                    activeTimelineIdx = i;
                }
            }
            return activeTimelineIdx;
        }

        /// <summary>
        /// Create a new timeline. First one is the 'master' timeline, the rest are children 
        /// </summary>
        /// <param name="thisName"></param>
        /// <param name="isMaster"></param>
        /// <returns></returns>
        public static GameObject CreateNewTimeline(string thisName, bool isMaster)
        {
            var go = TimelineUtils.CreatePlayableDirectorObject(thisName);
            var ta = TimelineUtils.CreateTimelineAsset(thisName);
            var pd = TimelineUtils.SetPlayableAsset(go, ta);
            pd.playOnAwake = false;         // always no play on awake, master timeline will control this
            SetActiveTimeline ( pd);

            if( !isMaster)
            {
                var masterTimeline = GameObject.Find("MasterTimeline");
                if (masterTimeline != null)
                {
                    go.transform.parent = masterTimeline.transform;
                    ApplyTimelineTemplate(pd, TIMELINE_TEMPLATE.BEAT_TIMELINE);
                }
            }
            else
            {
                ApplyTimelineTemplate(activeTimeline, TIMELINE_TEMPLATE.MASTER_TIMELINE);
            }

            RefreshTimelinesInScene(true);
            SetActiveSelection(go);
            return go;
        }

        /// <summary>
        /// Autogenerate a track template for the timeline
        /// </summary>
        /// <param name="thisPd"></param>
        /// <param name="type"></param>
        public static void ApplyTimelineTemplate(PlayableDirector pd, TIMELINE_TEMPLATE type)
        {
            switch( type)
            {
                case TIMELINE_TEMPLATE.BEAT_TIMELINE:
                    {
                        // add a control track clip to the master timeline
                        var masterPd = GetMasterTimeline();
                        var masterTa = TimelineUtils.GetTimelineAssetFromDirector(masterPd);
                        var masterControlTrack = TimelineUtils.FindTrackByName(masterTa, "BeatControl") as ControlTrack;
                        if( masterControlTrack != null)
                        {
                            var controlCip = masterControlTrack.CreateDefaultClip();
                            controlCip.displayName = pd.gameObject.name;
                            var controlAsset = controlCip.asset as ControlPlayableAsset;
                            masterPd.SetReferenceValue(controlAsset.sourceGameObject.exposedName, pd.gameObject);
                        }
                        

                        // fill in our beat track template
                        var ta = TimelineUtils.GetTimelineAssetFromDirector(pd);
                        var camera = TimelineUtils.CreateTimelineTrack(ta, TRACK_TYPE.TRACK_GROUP, "Camera", null);
                        {
                            var cmTrack = TimelineUtils.CreateTimelineTrack(ta, TRACK_TYPE.TRACK_CINEMACHINE, "CinemachineCamera", camera);
                            var mainCam = Camera.main;
                            var cmBrain = mainCam.GetComponent<Cinemachine.CinemachineBrain>();
                            pd.SetGenericBinding(cmTrack, cmBrain);
                            cmTrack.CreateDefaultClip();
                        }
                        var anim = TimelineUtils.CreateTimelineTrack(ta, TRACK_TYPE.TRACK_GROUP, "Animation", null);
                        var lighting = TimelineUtils.CreateTimelineTrack(ta, TRACK_TYPE.TRACK_GROUP, "Lighting", null);
                        var vfx = TimelineUtils.CreateTimelineTrack(ta, TRACK_TYPE.TRACK_GROUP, "VFX", null);
                        break;
                    }
                case TIMELINE_TEMPLATE.MASTER_TIMELINE:
                    {
                        var ta = TimelineUtils.GetTimelineAssetFromDirector(pd);
                        var group = TimelineUtils.CreateTimelineTrack(ta, TRACK_TYPE.TRACK_GROUP, "Beats", null);
                        var control = TimelineUtils.CreateTimelineTrack(ta, TRACK_TYPE.TRACK_CONTROL, "BeatControl", group);
                        break;
                    }
            }
        }

        public static List<TrackAsset> GetTracksInActiveTimeline()
        {
            var active = GetActiveTimeline();
            var ta = TimelineUtils.GetTimelineAssetFromDirector(active);
            if( ta != null)
            {
                var tracks = ta.GetRootTracks();
                return tracks.ToList();
            }
            return new List<TrackAsset>();
        }

        /// <summary>
        /// Scan the open scenes for any playable directors. Returns cache unless you force refresh
        /// </summary>
        /// <param name="forceRefresh"></param>
        public static void RefreshTimelinesInScene(bool forceRefresh)
        {
            if (!forceRefresh)
            {
                if (timelineList.Count > 0)
                    return;
            }

            timelineList.Clear();
            timelineListLabel.Clear();

            timelineList = GameObject.FindObjectsOfType<PlayableDirector>().ToList();
            foreach( var timeline in timelineList)
            {
                timelineListLabel.Add(timeline.name);
            }
        }

        /// <summary>
        /// Select the given timeline in our dropdown
        /// </summary>
        /// <param name="thisTimeline"></param>
        public static void SetActiveTimeline( PlayableDirector thisTimeline)
        {
            RefreshTimelinesInScene(true);
            for( int i = 0; i < timelineList.Count; i++)
            {
                if (timelineListLabel[i] == thisTimeline.name)
                {
                    selectedTimelineIdx = i;
                    activeTimeline = thisTimeline;
                    return;
                }
            }
        }

        /// <summary>
        /// Select a specific object in the scene
        /// </summary>
        /// <param name="go"></param>
        public static void SetActiveSelection(GameObject go)
        {
            Selection.activeObject = go;
        }

        /// <summary>
        /// Finds the gameobject named "MasterTimeline" and returns the playable director on it. Returns null if there isn't a master timeline yet
        /// </summary>
        /// <returns></returns>
        public static PlayableDirector GetMasterTimeline()
        {
            var go = GameObject.Find("MasterTimeline");
            if( go == null)
            {
                Debug.Log("Could not find object named 'MasterTimeline");
                return null;
            }
            return go.GetOrAddComponent<PlayableDirector>();
        }

        public static GUIContent GetIconForType(TRACK_TYPE type)
        {
            var icon = new GUIContent();
            switch( type)
            {
                case TRACK_TYPE.TRACK_ANIMATION:
                    {
                        icon = new GUIContent(EditorIcons.GetIcon("AnimationTrack"), Loc.TOOLTIP_ANIMATIONTRACK);
                        break;
                    }
                case TRACK_TYPE.TRACK_AUDIO:
                    {
                        icon = new GUIContent(EditorIcons.GetIcon("AudioTrack"), Loc.TOOLTIP_AUDIOTRACK);
                        break;
                    }
                case TRACK_TYPE.TRACK_CINEMACHINE:
                    {
                        icon = new GUIContent(EditorIcons.GetIcon("CinemachineTrack"), Loc.TOOLTIP_CINEMACHINETRACK);
                        break;
                    }
                case TRACK_TYPE.TRACK_CONTROL:
                    {
                        icon = new GUIContent(EditorIcons.GetIcon("ControlTrack"), Loc.TOOLTIP_CONTROlTRACK);
                        break;
                    }
                case TRACK_TYPE.TRACK_GROUP:
                    {
                        icon = new GUIContent(EditorIcons.GetIcon("GroupTrack"), Loc.TOOLTIP_GROUPTRACK);
                        break;
                    }
                case TRACK_TYPE.TRACK_ACTIVATION:
                    {
                        icon = new GUIContent(EditorIcons.GetIcon("ActivationTrack"), Loc.TOOLTIP_ACTIVATIONTRACK);
                        break;
                    }
                case TRACK_TYPE.TRACK_UNKNOWN:
                    {
                        icon = new GUIContent(EditorIcons.GetIcon("Unknown"), Loc.TOOLTIP_UNKNOWNTRACK);
                        break;
                    }
            }

            return icon;
        }
    }
}