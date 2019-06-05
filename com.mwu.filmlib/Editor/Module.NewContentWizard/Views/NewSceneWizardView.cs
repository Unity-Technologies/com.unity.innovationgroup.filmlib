using UnityEditor;
using UnityEngine;
using Control = MWU.FilmLib.NewContentWizardController;
using Loc = MWU.FilmLib.NewContentWizardLoc;                                   // string localization table

namespace MWU.FilmLib
{

    public class NewSceneWizardView : NewContentWizardSharedView
    {
        public static TEMPLATE_TYPE filterType = TEMPLATE_TYPE.ALL;

        public static void ShowWindow()
        {
            var thisWindow = GetWindow<NewSceneWizardView>(false, Loc.WINDOWLABEL_WIZARD, true);
            thisWindow.Reset();
        }

        public void OnEnable()
        {
            Reset();
        }

        public new void Reset()
        {
            // default to scene unless we get told otherwise
            filterType = TEMPLATE_TYPE.SCENE;
            windowType = EditorWindowType.NewSceneWizard;
            Control.Reset();
            base.Reset();
        }

        public void SetWizardType( TEMPLATE_TYPE filter)
        {
            filterType = filter;
        }

        private void OnGUI()
        {
            curWindowSize.x = position.width;
            curWindowSize.y = position.height;

            GUILayout.BeginHorizontal(GUILayout.MinWidth(minWindowSize.x), GUILayout.MinHeight(minWindowSize.y));
            {
                GUILayout.BeginVertical();
                {
                    DrawBanner(CONTENTWINDOW.NEWCONTENTWIZARD);
                    
                    GUILayout.BeginHorizontal();
                    {
                        // generate our left column, show only scene templates
                        GenerateTemplateListView(CONTENTWINDOW.NEWCONTENTWIZARD, filterType, false);

                        GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                        {
                            GUILayout.Space(10f);

                            if (Control.activeConfig != null)
                            {
                                GUILayout.Label("Selection : " + Control.activeConfig.type);
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label("Name: ", EditorStyles.boldLabel, GUILayout.MaxWidth(150f));
                                    GUILayout.Label(Control.activeConfig.name, EditorStyles.boldLabel, GUILayout.MaxWidth(150f));
                                }
                                GUILayout.EndHorizontal();
                                
                                GUILayout.TextArea("Description: " + Control.activeConfig.description, GUILayout.MaxWidth(350f), GUILayout.Height(60f));

                                GUILayout.Space(10f);

                                ViewTemplate(false);
                                
                                GUILayout.Space(10f);

                                var buttonInfo = new GUIContent();
                                switch (filterType)
                                {
                                    case TEMPLATE_TYPE.SCENE:
                                        {
                                            buttonInfo = new GUIContent(Loc.NEWCONTENT_GENERATESCENE, null, Loc.TOOLTIP_NEWCONTENT_GENERATESCENE);
                                            break;
                                        }
                                    case TEMPLATE_TYPE.FOLDER:
                                        {
                                            buttonInfo = new GUIContent(Loc.NEWCONTENT_GENERATEFOLDER, null, Loc.TOOLTIP_NEWCONTENT_GENERATEFOLDERS);
                                            break;
                                        }
                                }

                                GUI.backgroundColor = Loc.doneColor;
                                if (GUILayout.Button(buttonInfo, GUILayout.MaxWidth(STANDARDBUTTONSIZE), GUILayout.MaxHeight(STANDARDBUTTONHEIGHT)))
                                {
                                    var dialogTitle = string.Empty;
                                    var dialogInfo = string.Empty;
                                    switch ( filterType)
                                    {
                                        case TEMPLATE_TYPE.SCENE:
                                            {
                                                dialogTitle = Loc.DIALOG_POPSCENE_TITLE;
                                                dialogInfo = Loc.DIALOG_POPSCENE_MESSSAGE;
                                                break;
                                            }
                                        case TEMPLATE_TYPE.FOLDER:
                                            {
                                                dialogTitle = Loc.DIALOG_POPFOLDER_TITLE;
                                                dialogInfo = Loc.DIALOG_POPFOLDER_MESSSAGE;
                                                break;
                                            }
                                    }
                                    if (EditorUtility.DisplayDialog(dialogTitle,
                                            dialogInfo,
                                            Loc.DIALOG_OK,
                                            Loc.DIALOG_CANCEL))
                                    {
                                        switch( filterType)
                                        {
                                            case TEMPLATE_TYPE.FOLDER:
                                                {
                                                    Control.PopulateFolderStructure(Control.activeSelection.path);
                                                    this.Close();
                                                    break;
                                                }
                                            case TEMPLATE_TYPE.SCENE:
                                                {
                                                    Control.PopulateSceneStructure(Control.activeSelection.path);
                                                    this.Close();
                                                    break;
                                                }
                                        }
                                        
                                    }
                                }
                                GUI.backgroundColor = Loc.defaultColor;
                                GUILayout.Space(10f);
                            }
                            else
                            {
                                GUILayout.Space(10f);

                                GUILayout.Label(Loc.NCW_DEFAULTTEXT, EditorStyles.helpBox, GUILayout.MaxWidth(350f));
                            }
                        }
                        GUILayout.EndVertical();
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }
    }
}