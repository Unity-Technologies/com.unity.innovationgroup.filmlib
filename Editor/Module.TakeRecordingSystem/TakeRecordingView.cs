using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;
using Control = MWU.FilmLib.TakeRecordingController;
using Loc = MWU.FilmLib.TakeRecordingLoc;

namespace MWU.FilmLib
{
    public class TakeRecordingView : EditorWindow
    {
        private static Vector2 maxWindowSize = new Vector2(300f, 400f);
        private static Vector2 minWindowSize = new Vector2(200f, 200f);

        protected static float STANDARDBUTTONSIZE = 225f;
        protected static float STANDARDBUTTONHEIGHT = 35f;

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
            // top description
            GUILayout.Space(15f);
            GUILayout.Label(Loc.WINDOW_MAINDESCRIPTION, EditorStyles.helpBox);
            GUILayout.Space(5f);

            GUILayout.BeginVertical();
            {
                GUILayout.Label(Loc.TIMELINE_CURRENTSELECTION);
                Control.GetActiveTimeline();
                
                GUILayout.BeginHorizontal();
                {
                    var timelineName = "none";
                    if (Control.activeTimeline != null)
                    {
                        timelineName = Control.activeTimeline.name;
                    }
                    GUILayout.Label(timelineName, EditorStyles.helpBox);

                    var timelineCreate = Loc.TIMELINE_CREATENEWMASTER;

                    if (Control.timelineList.Count > 0)
                    {
                        timelineCreate = Loc.TIMELINE_CREATENEWBEAT;
                    }
                    if (GUILayout.Button(timelineCreate))
                    {
                        GameObject go = null;
                        var timelines = FindObjectsOfType<PlayableDirector>();
                        // if we don't have any timelines in the scene yet
                        if (timelines.Length < 1)
                        {
                            go = Control.CreateNewTimeline("MasterTimeline", true);
                        }
                        else
                        {
                            go = Control.CreateNewTimeline("Beat" + timelines.Length, false);
                        }

                        // select the new timeline so it becomes active in the Timeline window
                        if (go != null)
                        {
                            Control.SetActiveSelection(go);
                        }
                    }

                    if (GUILayout.Button(Loc.TIMELINE_REFRESHTIMELINES))
                    {
                        Control.RefreshTimelinesInScene(true);
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            GUILayout.Space(5f);

            // timeline management
            GUILayout.BeginHorizontal();
            {
                if (Control.timelineList.Count > 0)
                {
                    GUILayout.BeginHorizontal();
                    {
                        // dropdown of available timelines
                        Control.selectedTimelineIdx = EditorGUILayout.Popup(Control.selectedTimelineIdx, Control.timelineListLabel.ToArray(), GUILayout.MaxWidth(STANDARDBUTTONSIZE));
                        if (GUILayout.Button("Select"))
                        {
                            Selection.activeObject = Control.timelineList[Control.selectedTimelineIdx].gameObject;
                        }
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(15f);

            // track management
            GUILayout.BeginHorizontal();
            {
                // list all of the tracks in the currently selected timeline
                var tracks = Control.GetTracksInActiveTimeline();
                
                GUILayout.BeginVertical();
                {
                    if( tracks.Count > 0)
                    {
                        var baseIndent = 0;

                        foreach (var track in tracks)
                        {
                            DrawTrackEntry(track, baseIndent);
                        }
                    }
                }
                GUILayout.EndVertical();
                
            }
            GUILayout.EndHorizontal();
        }

        public static void DrawTrackEntry( TrackAsset track, float indent)
        {
            // figure out what type of track this is:
            var type = TimelineUtils.GetTrackType(track);

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(indent);
                GUILayout.Label(type.ToString() + ": " + track.name);
            }
            GUILayout.EndHorizontal();

            var childTracks = track.GetChildTracks();
            var baseIndent = indent + 10f;

            foreach (var child in childTracks)
            {
                DrawTrackEntry(child, baseIndent);
            }
        }
    }
}