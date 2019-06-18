using MWU.FilmLib.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace MWU.FilmLib
{
    public enum TIMELINE_TEMPLATE
    {
        MASTER_TIMELINE,
        BEAT_TIMELINE
    }

   

    public class TakeRecordingController
    {
        public static PlayableDirector activeTimeline;
        public static List<string> timelineListLabel = new List<string>();
        public static List<PlayableDirector> timelineList = new List<PlayableDirector>();
        public static int selectedTimeline = 0;
        
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

        /// <summary>
        /// Create a new timeline. First one is the 'master' timeline, the rest are children 
        /// </summary>
        /// <param name="thisName"></param>
        /// <param name="isMaster"></param>
        /// <returns></returns>
        public static GameObject CreateNewTimeline(string thisName, bool isMaster)
        {
            Debug.Log("Creating new Timeline: " + thisName);
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
                    Debug.Log("Found timeline match, selecting...");
                    selectedTimeline = i;
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
    }
}