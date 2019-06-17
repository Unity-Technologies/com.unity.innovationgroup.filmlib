using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using Control = MWU.FilmLib.TakeRecordingController;

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
            var window = GetWindow<TakeRecordingView>("Take System");
            TimelineUtils.GetTimelineWindow();
            window.Show();
            window.maxSize = maxWindowSize;
            window.minSize = minWindowSize;
        }

        private void OnGUI()
        {
            GUILayout.Space(15f);
            GUILayout.Label("Multi-take recording system.", EditorStyles.helpBox);

            if (activeTimeline == null)
            {
                activeTimeline = Control.GetActiveTimeline();

                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Create Timeline"))
                    {
                        GameObject go = null;
                        var timelines = GameObject.FindObjectsOfType<PlayableDirector>();
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
}