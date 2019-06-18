using MWU.FilmLib.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

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
            var go = TimelineUtils.CreatePlayableDirectorObject(thisName);
            var ta = TimelineUtils.CreateTimelineAsset(thisName);
            TimelineUtils.SetPlayableAsset(go, ta);
            if( !isMaster)
            {
                var masterTimeline = GameObject.Find("MasterTimeline");
                if (masterTimeline != null)
                {
                    go.transform.parent = masterTimeline.transform;
                }
            }
            return go;
        }

        /// <summary>
        /// Autogenerate a track template for the timeline
        /// </summary>
        /// <param name="pd"></param>
        /// <param name="type"></param>
        public static void ApplyTimelineTemplae(PlayableDirector pd, TIMELINE_TEMPLATE type)
        {
            switch( type)
            {
                case TIMELINE_TEMPLATE.BEAT_TIMELINE:
                    {
                        break;
                    }
                case TIMELINE_TEMPLATE.MASTER_TIMELINE:
                    {
                        break;
                    }
            }
        }

        public static void RefreshTimelinesInScene()
        {
            timelineList = GameObject.FindObjectsOfType<PlayableDirector>().ToList();
            foreach( var timeline in timelineList)
            {
                timelineListLabel.Add(timeline.name);
            }
        }

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
            var go = GameObject.Find("MasterTimeilne");
            if( go == null)
            {
                return null;
            }
            return go.GetOrAddComponent<PlayableDirector>();
        }
    }
}