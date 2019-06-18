using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using Control = MWU.FilmLib.TakeRecordingController;
using Loc = MWU.FilmLib.TakeRecordingLoc;

namespace MWU.FilmLib
{
    public class TakeRecordingView : EditorWindow
    {
        private static Vector2 maxWindowSize = new Vector2(300f, 400f);
        private static Vector2 minWindowSize = new Vector2(200f, 200f);

        private static PlayableDirector activeTimeline;

        [MenuItem("Tools/Take System")]
        private static void Init()
        {
            var window = GetWindow<TakeRecordingView>(Loc.WINDOW_TITLE);
            TimelineUtils.GetTimelineWindow();
            window.Show();
            window.maxSize = maxWindowSize;
            window.minSize = minWindowSize;
        }

        private void OnGUI()
        {
            GUILayout.Space(15f);
            GUILayout.Label(Loc.WINDOW_MAINDESCRIPTION, EditorStyles.helpBox);

            activeTimeline = Control.GetActiveTimeline();

            GUILayout.BeginHorizontal();
            {
                var timelineCreate = Loc.TIMELINE_CREATENEWMASTER;
                if( activeTimeline != null)
                {
                    timelineCreate = Loc.TIMELINE_CREATENEWBEAT;
                }

                if (GUILayout.Button(timelineCreate))
                {
                    GameObject go = null;
                    var timelines = FindObjectsOfType<PlayableDirector>();
                    // if we don't have any timelines in the scene yet
                    if( timelines.Length < 1)
                    {
                        go = Control.CreateNewTimeline("MasterTimeline", true);
                    }
                    else
                    {
                        go = Control.CreateNewTimeline("Beat" + timelines.Length, false);
                    }

                    // select the new timeline so it becomes active in the Timeline window
                    if ( go != null)
                    {
                        Control.SetActiveSelection(go);
                    }
                }
            }
            GUILayout.EndHorizontal();
        }
    }
}