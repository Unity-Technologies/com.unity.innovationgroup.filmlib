using MWU.FilmLib.Extensions;
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
        private static Vector2 maxWindowSize = new Vector2(300f, 800f);
        private static Vector2 minWindowSize = new Vector2(200f, 200f);

        protected static float STANDARDBUTTONSIZE = 225f;
        protected static float STANDARDBUTTONHEIGHT = 35f;
        protected static float SMALLBUTTONSIZE = 24f;
        protected static float DEFAULT_TRACK_INDENT = 25f;
        protected static TakeRecordingView thisWindow;

        [MenuItem("Tools/Take System")]
        private static void Init()
        {
            thisWindow = GetWindow<TakeRecordingView>(Loc.WINDOW_TITLE);
            TimelineUtils.GetTimelineWindow();
            thisWindow.Show();
            //thisWindow.maxSize = maxWindowSize;
            thisWindow.minSize = minWindowSize;
        }

        private void OnGUI()
        {
            // top description
            GUILayout.Space(15f);
            GUILayout.Label(Loc.WINDOW_MAINDESCRIPTION, EditorStyles.helpBox);
            GUILayout.Space(5f);

            // timeline list
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
            //GUILayout.Space(5f);

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
                            if( TimelineUtils.GetTimelineWindowLockStatus())
                            {
                                // unlock timeline window
                                TimelineUtils.SetTimelineWindowLockStatus(false);
                            }

                            if (Control.timelineList[Control.selectedTimelineIdx] != null)
                            {
                                Selection.activeObject = Control.timelineList[Control.selectedTimelineIdx].gameObject;
                            }
                            else
                            {
                                // if the timeline was null, we should probably refresh and figure out what's up
                                Control.RefreshTimelinesInScene(true); 
                            }
                            // and lock it again
                            TimelineUtils.SetTimelineWindowLockStatus(true);

                        }
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndHorizontal();
          //  GUILayout.Space(15f);

            // timeline control
            GUILayout.BeginHorizontal();
            {
                if (Control.GetActiveTimeline() != null)
                {
                    GUILayout.BeginVertical();
                    {
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label("0", EditorStyles.helpBox, GUILayout.Width( 25f));
                            Control.GetActiveTimeline().time = GUILayout.HorizontalSlider((float)Control.GetActiveTimeline().time, 0, Control.currentTimelineDuration, GUILayout.Width(120f));
                            GUILayout.Label(Control.currentTimelineDuration.ToString(), EditorStyles.helpBox, GUILayout.Width(25f));
                            TimelineUtils.GetTimelineWindow().Repaint();
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        {
                            var begin = EditorIcons.GetIcon("SkipPrevious");
                            if( GUILayout.Button(begin, GUILayout.Width(SMALLBUTTONSIZE), GUILayout.Height(SMALLBUTTONSIZE)))
                            {
                                Control.GetActiveTimeline().time = 0f;
                            }
                            var rewind = EditorIcons.GetIcon("FastRewind");
                            if( GUILayout.Button(rewind, GUILayout.Width(SMALLBUTTONSIZE), GUILayout.Height(SMALLBUTTONSIZE)))
                            {

                            }
                            var stop = EditorIcons.GetIcon("Stop");
                            if( GUILayout.Button(stop, GUILayout.Width(SMALLBUTTONSIZE), GUILayout.Height(SMALLBUTTONSIZE)))
                            {
                                Control.GetActiveTimeline().Stop();
                            }
                            var play = EditorIcons.GetIcon("Play");
                            if( GUILayout.Button(play, GUILayout.Width(SMALLBUTTONSIZE), GUILayout.Height(SMALLBUTTONSIZE)))
                            {
                                Control.GetActiveTimeline().Play();
                            }
                            var forward = EditorIcons.GetIcon("FastForward");
                            if( GUILayout.Button(forward, GUILayout.Width(SMALLBUTTONSIZE), GUILayout.Height(SMALLBUTTONSIZE)))
                            {

                            }
                            var end = EditorIcons.GetIcon("SkipNext");
                            if( GUILayout.Button(end, GUILayout.Width(SMALLBUTTONSIZE), GUILayout.Height(SMALLBUTTONSIZE)))
                            {
                                Control.GetActiveTimeline().time = Control.currentTimelineDuration;

                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();
                }
            }
            GUILayout.EndHorizontal();
           // GUILayout.Space(15f);

            // track management
            GUILayout.BeginHorizontal();
            {
                if( Control.GetActiveTimeline() != null)
                {
                    // list all of the tracks in the currently selected timeline
                    var tracks = Control.GetTracksInActiveTimeline();

                    GUILayout.BeginVertical();
                    {
                        if (tracks.Count > 0)
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
            }
            GUILayout.EndHorizontal();
        }
        
        public static void Update()
        {
            thisWindow.Repaint();
        }

        /// <summary>
        /// draw an individual entry in the track
        /// </summary>
        /// <param name="track"></param>
        /// <param name="indent"></param>
        public static void DrawTrackEntry( TrackAsset track, float indent)
        {
            // figure out what type of track this is:
            var type = TimelineUtils.GetTrackType(track);
            
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(indent);
                var icon = Control.GetIconForType(type);
                GUILayout.Label(icon, GUILayout.Width(SMALLBUTTONSIZE), GUILayout.Height(SMALLBUTTONSIZE));
                if( type != TRACK_TYPE.TRACK_ANIMATION)
                {
                    GUILayout.Label(track.name);
                }
                else
                if( type == TRACK_TYPE.TRACK_ANIMATION)
                {
                    var sourceObject = TimelineUtils.GetSourceObjectFromTrack(track, Control.GetActiveTimeline());
                    if( sourceObject == null)
                    {
                        GUILayout.Label(track.name);
                    }
                    else
                    {
                        GUILayout.Label(sourceObject.name);
                    }

                    var recordIcon = EditorIcons.GetIcon("Record");
                    var defaultColor = GUI.backgroundColor;
                    GUI.backgroundColor = Color.red;
                    var recordContent = new GUIContent(recordIcon, "Arm Track");
                    if ( GUILayout.Button(recordContent, GUILayout.Width(SMALLBUTTONSIZE), GUILayout.Height(SMALLBUTTONSIZE)))
                    {
                        var go = new GameObject();
                        go.name = "Recorder_" + sourceObject.name;
                        var recorder = go.GetOrAddComponent<RecordTransformHierarchy>();
                        recorder.objectToRecord = sourceObject as GameObject;

                        Debug.Log("Track armed!");
                    }
                    GUI.backgroundColor = defaultColor;
                }
            }
            GUILayout.EndHorizontal();

            var childTracks = track.GetChildTracks();
            var baseIndent = indent + DEFAULT_TRACK_INDENT;

            foreach (var child in childTracks)
            {
                DrawTrackEntry(child, baseIndent);
            }
        }
    }
}