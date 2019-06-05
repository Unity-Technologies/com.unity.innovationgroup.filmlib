using MWU.Shared.Utilities;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MWU.FilmLib
{
    public class EditorToolbarMainView : EditorWindow
    {
        public static Dictionary<string, string> sceneLoader = new Dictionary<string, string>();

        private static Vector2 maxWindowSize = new Vector2(1920f, 50f);
        private static Vector2 minWindowSize = new Vector2(1260f, 50f);
        private Vector2 curWindowSize = new Vector2(1920f, 50f);
        private Vector2 defaultButtonSize = new Vector2(115f, 35f);
        private Vector2 _scroll = Vector2.zero;
        private static ToolbarConfig config;
        private static List<string> sceneNames = new List<string>();
        private static bool initialized = false;

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
                // Debug.Log("Could not load toolbar config?");
            }
            else
            {
                initialized = true;
                sceneNames.Clear();
                if (config.sceneLoaderList != null)
                {
                    for (int i = 0; i < config.sceneLoaderList.Length; i++)
                    {
                        sceneNames.Add(config.sceneLoaderList[i].name);
                    }
                }
            }
        }

        [MenuItem(EditorToolbarLoc.MAINTOOLBAR_MENULABEL, false, -100)]
        private static void Init()
        {
            var window = GetWindow<EditorToolbarMainView>(EditorToolbarLoc.MAINTOOLBAR_WINDOWNAME);
            window.Show();
            window.maxSize = new Vector2(maxWindowSize.x, maxWindowSize.y);
            window.minSize = new Vector2(minWindowSize.x, minWindowSize.y);
        }

        private void OnGUI()
        {
            curWindowSize.x = position.width;
            curWindowSize.y = position.height;

            var colSceneLoader = new Color(0.75f, 1f, 1f, 1f);
            var colShortcuts = new Color(1f, 0.75f, 1f, 1f);
            var colLayout = new Color(1f, 1f, 0.75f, 1f);

            if( initialized)
            {
                // toolbar buttons
                GUILayout.BeginHorizontal(GUILayout.MinWidth(minWindowSize.x), GUILayout.MinHeight(minWindowSize.y));
                {
                    GUILayout.Space(10f);
                    if (config.showSceneLoader)
                    {
                        GUILayout.BeginVertical();
                        {
                            GUILayout.Label(EditorToolbarLoc.MAINTOOLBAR_EPISODES, EditorStyles.centeredGreyMiniLabel);
                            GUI.backgroundColor = colSceneLoader;
                            GUILayout.BeginHorizontal();
                            {
                                if (config != null)
                                {
                                    for (int i = 0; i < sceneNames.Count; i++)
                                    {
                                        if (config.sceneLoaderType == SceneLoaderType.Individual)
                                        {
                                            if (GUILayout.Button(sceneNames[i], GUILayout.MaxWidth(defaultButtonSize.x), GUILayout.MaxHeight(defaultButtonSize.y)))
                                            {
                                                var sceneFullPath = AssetDatabase.GetAssetOrScenePath(config.sceneLoaderList[i]);

                                                EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(config.sceneLoaderList[i]), OpenSceneMode.Single);

                                            }
                                        }
                                        else
                                        {
                                            foreach (var item in config.sceneLoaderList)
                                            {
                                                var buttonName = item.name;
                                                if (GUILayout.Button(buttonName, GUILayout.MaxWidth(defaultButtonSize.x), GUILayout.MaxHeight(defaultButtonSize.y)))
                                                {
                                                    var path = AssetDatabase.GetAssetOrScenePath(item);
                                                    EditorUtilities.FindProjectLoader(path);

                                                }
                                            }
                                        }

                                    }
                                }
                                //if (GUILayout.Button(EditorToolbarLoc.MAINTOOLBAR_LOADER_SHERMANEP01, GUILayout.MaxWidth(defaultButtonSize.x), GUILayout.MaxHeight(defaultButtonSize.y)))
                                //{
                                //    var path = "Assets/Scenes/ProjectB/ProjectB Scene Loader.asset";
                                //    EditorToolbarController.FindProjectLoader(path);
                                //}
                                //if (GUILayout.Button(EditorToolbarLoc.MAINTOOLBAR_LOADER_CREATENEW, GUILayout.MaxWidth(defaultButtonSize.x), GUILayout.MaxHeight(defaultButtonSize.y)))
                                //{
                                //    var path = "Assets/Scenes/ProjectB/ProjectB Scene Loader.asset";
                                //    EditorToolbarController.FindProjectLoader(path);
                                //}
                            }
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndVertical();
                    }
                    if (config.showSceneTools)
                    {
                        GUILayout.BeginVertical();
                        {
                            GUILayout.Label(EditorToolbarLoc.MAINTOOLBAR_HELPERS, EditorStyles.centeredGreyMiniLabel);

                            GUILayout.BeginHorizontal();
                            {
                                GUI.backgroundColor = colShortcuts;

                                if (GUILayout.Button(EditorToolbarLoc.MAINTOOLBAR_EDIT_SEARCHPROJECT, GUILayout.MaxWidth(defaultButtonSize.x), GUILayout.MaxHeight(defaultButtonSize.y)))
                                {
                                    Shortcuts.ProjectSearch();
                                }
                                if (GUILayout.Button(EditorToolbarLoc.MAINTOOLBAR_EDIT_POPULATESCENE, GUILayout.MaxWidth(defaultButtonSize.x), GUILayout.MaxHeight(defaultButtonSize.y)))
                                {
                                    NewContentWizardMenus.NewSceneFromTemplate();
                                }
                                if (GUILayout.Button(EditorToolbarLoc.MAINTOOLBAR_EDIT_POPULATEFOLDERS, GUILayout.MaxWidth(defaultButtonSize.x), GUILayout.MaxHeight(defaultButtonSize.y)))
                                {
                                    NewContentWizardMenus.PopulateFolderStructure();
                                }
                                if (GUILayout.Button(EditorToolbarLoc.MAINTOOLBAR_EDIT_CREATEGROUP, GUILayout.MaxWidth(defaultButtonSize.x), GUILayout.MaxHeight(defaultButtonSize.y)))
                                {
                                    Shortcuts.CreateGroup();
                                }
                                //if (GUILayout.Button(EditorToolbarLoc.MAINTOOLBAR_EDIT_CENTERGROUPONCHILDREN, GUILayout.MaxWidth(defaultButtonSize.x), GUILayout.MaxHeight(defaultButtonSize.y)))
                                //{
                                //    Shortcuts.CenterOnChildren();
                                //}
#if USING_FILMTOOLBOX
                                if (GUILayout.Button(EditorToolbarLoc.MAINTOOLBAR_EDIT_MATERIALREMAPPER, GUILayout.MaxWidth(defaultButtonSize.x), GUILayout.MaxHeight(defaultButtonSize.y)))
                                {
                                    EditorToolbarController.OpenMaterialRemapper();
                                }
#endif
#if USING_MWU_HDRP
                                if (GUILayout.Button(EditorToolbarLoc.MAINTOOLBAR_RENDERWINDOW, GUILayout.MaxWidth(defaultButtonSize.x), GUILayout.MaxHeight(defaultButtonSize.y)))
                                {
                                    EditorToolbarController.OpenRenderWindow();
                                }
#endif
                            }
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndVertical();
                    }
                    if (config.showLayoutModes)
                    {
                        GUILayout.BeginVertical();
                        {
                            GUILayout.Label("Window Layout", EditorStyles.centeredGreyMiniLabel);
                            GUILayout.BeginHorizontal();
                            {
                                GUI.backgroundColor = colLayout;
                                if (GUILayout.Button("Load Film Layout", GUILayout.MaxWidth(defaultButtonSize.x), GUILayout.MaxHeight(defaultButtonSize.y)))
                                {
                                    LayoutLoader.LoadFilmLayout();
                                }
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
                        Debug.Log("Created new toolbar config");
                        EditorToolbarController.CreateToolbarConfig();
                        Configure();
                        LayoutLoader.LoadFilmLayout();
                    }
                }
                GUILayout.EndHorizontal();
            }
            // EditorGUILayout.EndScrollView();
        }

        
    }
}