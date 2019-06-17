using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

namespace MWU.FilmLib
{
    public class TakeRecordingController
    {
        public static PlayableDirector activeTimeline;

        public static PlayableDirector GetActiveTimeline()
        {
            if( activeTimeline != null)
            {
                var pdAsset = TimelineUtils.GetCurrentActiveTimeline();
                if (pdAsset != null)
                {
                    Debug.Log("Current timeline asset: " + pdAsset.name);
                    activeTimeline = TimelineUtils.GetDirectorFromTimeline(pdAsset);
                    Debug.Log("Current timeline: " + activeTimeline.gameObject.name);
                }
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

        public static void SetActiveSelection(GameObject go)
        {
            Selection.activeObject = go;
        }
    }
}