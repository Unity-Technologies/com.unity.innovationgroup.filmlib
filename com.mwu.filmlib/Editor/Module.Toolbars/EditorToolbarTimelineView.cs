using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MWU.FilmLib
{
    public class EditorToolbarTimelineView : EditorWindow
    {
        public static Dictionary<string, string> sceneLoader = new Dictionary<string, string>();

        private static Vector2 maxWindowSize = new Vector2(1920f, 50f);
        private static Vector2 minWindowSize = new Vector2(960f, 50f);
        private Vector2 curWindowSize = new Vector2(1920f, 50f);
        private Vector2 defaultButtonSize = new Vector2(115f, 35f);
        private Vector2 _scroll = Vector2.zero;
        private static bool initialized = false;

        private static ToolbarConfig config;

        private void OnEnable()
        {
            Configure();
        }

        private static void Configure()
        {
            config = AssetDatabase.LoadAssetAtPath("Assets/Settings/Toolbar/ToolbarConfig.asset", typeof(ToolbarConfig)) as ToolbarConfig;
            if (config == null)
            {
                initialized = false;
                // FIXME: should load a default config from the package folder if we don't have one in the project
                // and/or prompt user to create a config
            }
            else
            {
                initialized = true;
            }
        }

        [MenuItem(EditorToolbarLoc.TIMELINETOOLBAR_MENULABEL, false, -100)]
        private static void Init()
        {
            var window = EditorWindow.GetWindow<EditorToolbarTimelineView>(EditorToolbarLoc.TIMELINETOOLBAR_WINDOWNAME);
            RenderSettings.SetInitialFurCount();
            window.Show();
            window.maxSize = new Vector2(maxWindowSize.x, maxWindowSize.y);
            window.minSize = new Vector2(minWindowSize.x, minWindowSize.y);
        }

        private void OnGUI()
        {
            curWindowSize.x = position.width;
            curWindowSize.y = position.height;

            var colShortcuts = new Color(1f, 0.75f, 1f, 1f);
            var colPerformance = new Color(1f, 1f, 0.75f, 1f);
            var colOverrides = new Color(0.75f, 1f, 1f, 1f);
            var style = EditorStyles.miniButton;

            //_scroll = EditorGUILayout.BeginScrollView(scrollPos, false, false, GUILayout.Width(width), GUILayout.Height(height));
            if( initialized)
            {
                GUILayout.BeginHorizontal(GUILayout.MinWidth(minWindowSize.x), GUILayout.MinHeight(minWindowSize.y));
                {
                    GUILayout.Space(10f);
                    if (config.showSceneTools)
                    {
                        // toolbar buttons
                        GUILayout.BeginVertical();
                        {
                            GUILayout.Label(EditorToolbarLoc.TIMELINETOOLBAR_SCENESHORTCUTS, EditorStyles.centeredGreyMiniLabel);

                            GUILayout.BeginHorizontal();
                            {
                                GUI.backgroundColor = colShortcuts;
                                var masterTimeline = new GUIContent(EditorToolbarLoc.TIMELINETOOLBAR_SHORTCUT_MASTERTIMELINE);
                                if (GUILayout.Button(masterTimeline, style, GUILayout.MaxWidth(style.CalcSize(masterTimeline).x), GUILayout.MaxHeight(defaultButtonSize.y)))
                                {
                                    EditorUtilities.FindSceneObject("MasterTimeline");
                                }
                                var sceneSettings = new GUIContent(EditorToolbarLoc.TIMELINETOOLBAR_SHORTCUT_SCENESETTINGS);
                                if (GUILayout.Button(sceneSettings, style, GUILayout.MaxWidth(style.CalcSize(sceneSettings).x), GUILayout.MaxHeight(defaultButtonSize.y)))
                                {
                                    EditorUtilities.FindSceneObject("SceneSettings");
                                }
                                var globalPost = new GUIContent(EditorToolbarLoc.TIMELINETOOLBAR_GLOBALPOST);
                                if (GUILayout.Button(globalPost, style, GUILayout.MaxWidth(style.CalcSize(globalPost).x), GUILayout.MaxHeight(defaultButtonSize.y)))
                                {
                                    EditorUtilities.FindSceneObject("PostVolume");
                                }
                            }
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndVertical();
                    }
                    if (RenderSettings.FindRenderSettingsObject() != null && config.showRenderSettings)
                    {
                        GUILayout.BeginVertical();
                        {
                            GUILayout.Label(EditorToolbarLoc.TIMELINETOOLBAR_RENDERSETTINGS, EditorStyles.centeredGreyMiniLabel);
                            GUI.backgroundColor = colPerformance;

                            GUILayout.BeginHorizontal();
                            {
                                var detailSettings = RenderSettings.GetRenderSettings();
                                foreach (var setting in detailSettings)
                                {
                                    var settingLabel = new GUIContent(setting);
                                    if (GUILayout.Button(setting, style, GUILayout.MaxWidth(style.CalcSize(settingLabel).x), GUILayout.MaxHeight(defaultButtonSize.y)))
                                    {
                                        RenderSettings.ActivateRenderSettings(setting);
                                    }
                                }
                                var editSettings = new GUIContent(EditorToolbarLoc.TIMELINETOOLBAR_EDITRENDERSETTINGS);
                                if (GUILayout.Button(EditorToolbarLoc.TIMELINETOOLBAR_EDITRENDERSETTINGS, style, GUILayout.MaxWidth(style.CalcSize(editSettings).x), GUILayout.MaxHeight(defaultButtonSize.y)))
                                {
                                    EditorUtilities.FindSceneObject("RenderSettings");
                                }
                            }
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndVertical();
                    }
                    if (config.showPerformance)
                    {
                        GUILayout.BeginVertical();
                        {
                            GUILayout.Label(EditorToolbarLoc.TIMELINETOOLBAR_PERFORMANCE, EditorStyles.centeredGreyMiniLabel);
                            GUI.backgroundColor = colOverrides;
                            GUILayout.BeginHorizontal();
                            {
#if HDRP_FUR
                             GUILayout.BeginVertical();
                                GUILayout.Label(EditorToolbarLoc.TIMELINETOOLBAR_FURSHELLCOUNT);
                                RenderSettings.furShellCount = (int) EditorGUILayout.Slider(RenderSettings.furShellCount, RenderSettings.furMinCount, RenderSettings.furMaxCount, GUILayout.MinWidth( 100f));
                            GUILayout.EndVertical();
                            if (GUILayout.Button(EditorToolbarLoc.TIMELINETOOLBAR_UDPATEFUR, GUILayout.MaxWidth(defaultButtonSize.x), GUILayout.MaxHeight(defaultButtonSize.y)))
                            {
                                RenderSettings.UpdateFur();
                            }
#endif
                            }
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndVertical();
                    }
                }
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.BeginHorizontal(GUILayout.MinWidth(minWindowSize.x), GUILayout.MinHeight(minWindowSize.y));
                {
                    GUILayout.Space(10f);
                    if (GUILayout.Button(EditorToolbarLoc.TOOLBAR_CREATE_CONFIG, GUILayout.MaxWidth(defaultButtonSize.x), GUILayout.MaxHeight(defaultButtonSize.y)))
                    {
                        Debug.Log("Create new scriptable toolbar config");
                        EditorToolbarController.CreateToolbarConfig();
                        Configure();
                        // refresh the layout
                        LayoutLoader.LoadFilmLayout();
                    }
                }
                GUILayout.EndHorizontal();
            }
            // EditorGUILayout.EndScrollView();
        }
    }
}