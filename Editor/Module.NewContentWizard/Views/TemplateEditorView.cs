using UnityEditor;
using UnityEngine;
using Control = MWU.FilmLib.NewContentWizardController;
using Core = MWU.FilmLib.NewContentWizardCore;
using Loc = MWU.FilmLib.NewContentWizardLoc;                                   // string localization table

namespace MWU.FilmLib
{
    /// <summary>
    /// common base because we share code with NewContentWizardView
    /// </summary>
    public class TemplateEditorView : NewContentWizardSharedView
    {
        /// <summary>
        /// which tab on our edit toolbar is selected currently
        /// </summary>
        private static int editWindowInt = 0;
        private static bool canEdit = false;
        
        public static void ShowWindow()
        {
            var thisWindow = GetWindow<TemplateEditorView>(false, Loc.WINDOWLABEL_TEMPLATE, true);
            thisWindow.windowType = EditorWindowType.TemplateEditor;
            thisWindow.Reset();
        }

        public void OnEnable()
        {
            Reset();
        }

        public new void Reset()
        {
            Control.Reset();
            // preload the configs 
            base.Reset();
            editWindowInt = 0;   
        }

        private void OnGUI()
        {
            curWindowSize.x = position.width;
            curWindowSize.y = position.height;            

            GUILayout.BeginHorizontal(GUILayout.MinWidth(minWindowSize.x), GUILayout.MinHeight(minWindowSize.y));
            {
                GUILayout.BeginVertical();
                {
                    DrawBanner(CONTENTWINDOW.TEMPLATEEDITOR);

                    GUILayout.BeginHorizontal();
                    {
                        /*
                         *
                         * generate our left column, show all templates
                         * 
                         * 
                         */
                        GenerateTemplateListView(CONTENTWINDOW.TEMPLATEEDITOR, TEMPLATE_TYPE.ALL, true);

                        /*
                         * 
                         * right column, active selection
                         * 
                         */
                        GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                        {
                            GUILayout.Space(10f);

                            // if we have a selected, then show the template
                            if( Control.activeSelection != null)
                            {
                                // whether we're in read only mode or not
                                canEdit = !Control.activeSelection.isBuiltIn;

                                // name
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label(Loc.EDIT_NAME, GUILayout.Width(50f));
                                    
                                    if(canEdit)
                                    {
                                        // monitor the text field for changes
                                        EditorGUI.BeginChangeCheck();
                                        {
                                            Control.tempConfigNameString = GUILayout.TextArea(Control.tempConfigNameString, GUILayout.Width(350f));
                                        }
                                        if (EditorGUI.EndChangeCheck())
                                        {
                                            Control.configNameDirty = true;
                                        }

                                        if (Control.configNameDirty)
                                        {
                                            // done button
                                            GUI.backgroundColor = Loc.doneColor;
                                            if (GUILayout.Button(imgDone, GUILayout.Width(SMALLBUTTONSIZE), GUILayout.Height(SMALLBUTTONSIZE)))
                                            {
                                                Debug.Log("save changes...");
                                                Control.SaveChangeToConfigName();
                                            }
                                            // cancel button
                                            GUI.backgroundColor = Loc.cancelColor;
                                            if (GUILayout.Button(imgCancel, GUILayout.Width(SMALLBUTTONSIZE), GUILayout.Height(SMALLBUTTONSIZE)))
                                            {
                                                // reset to what it was originally
                                                Control.tempConfigNameString = Control.activeConfig.name;
                                            }
                                            // reset colors back to default
                                            GUI.backgroundColor = Loc.defaultColor;
                                        }
                                    }
                                    else
                                    {
                                        GUILayout.TextArea(Control.activeConfig.name, GUILayout.Width(350f));
                                    }
                                    
                                }
                                GUILayout.EndHorizontal();

                                GUILayout.Label(Loc.EDIT_TEMPLATETYPE + Control.activeConfig.type);
                                
                                // Description 
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label(Loc.EDIT_DESCRIPTION, GUILayout.Width(50f));
                                    if (canEdit)
                                    {
                                        // monitor the text field for changes
                                        EditorGUI.BeginChangeCheck();
                                        {
                                            Control.tempConfigDescriptionString = GUILayout.TextArea(Control.tempConfigDescriptionString, GUILayout.Width(350f), GUILayout.Height(100f));
                                        }
                                        if (EditorGUI.EndChangeCheck())
                                        {
                                            Control.configDescriptionDirty = true;
                                        }

                                        if (Control.configDescriptionDirty)
                                        {
                                            // done button
                                            GUI.backgroundColor = Loc.doneColor;
                                            if (GUILayout.Button(imgDone, GUILayout.Width(SMALLBUTTONSIZE), GUILayout.Height(SMALLBUTTONSIZE)))
                                            {
                                                Control.SaveChangeToConfigDescription();
                                            }
                                            // cancel button
                                            GUI.backgroundColor = Loc.cancelColor;
                                            if (GUILayout.Button(imgCancel, GUILayout.Width(SMALLBUTTONSIZE), GUILayout.Height(SMALLBUTTONSIZE)))
                                            {
                                                // reset to what it was originally
                                                Control.tempConfigDescriptionString = Control.activeConfig.description;
                                            }
                                            // reset colors back to default
                                            GUI.backgroundColor = Loc.defaultColor;
                                        }
                                    }
                                    else
                                    {
                                        GUILayout.TextArea(Control.activeConfig.description, GUILayout.Width(350f), GUILayout.Height(100f));
                                    }
                                }
                                GUILayout.EndHorizontal();

                                GUILayout.Label(Loc.EDIT_SOURCEFILE);
                                GUILayout.Label( Control.activeSelection.path, EditorStyles.helpBox, GUILayout.MaxWidth(RIGHTPANELWIDTH));

                                /*
                                 * show our action toolbar
                                 */
                                editWindowInt = GUILayout.Toolbar(editWindowInt, Loc.EDITBUTTONS, GUILayout.MaxWidth(RIGHTPANELWIDTH));
                                {
                                    switch (editWindowInt)
                                    {
                                        case (int) TOOLBAR_BUTTON.Edit:
                                            {
                                                GUILayout.Space(10f);
                                                GUILayout.Label(Loc.EDIT_TITLE, EditorStyles.boldLabel, GUILayout.MaxWidth(RIGHTPANELWIDTH));
                                                GUILayout.Label(Loc.EDIT_HELPTEXT, EditorStyles.helpBox, GUILayout.MaxWidth(RIGHTPANELWIDTH));

                                                GUILayout.Space(10f);
                                                    
                                                // left column
                                                GUILayout.BeginHorizontal();
                                                {
                                                    /*
                                                    * 
                                                    * if it isn't a built in template, we can edit, otherwise read only
                                                    * Display the Tree view 
                                                    * 
                                                    */
                                                    ViewTemplate(canEdit);
                                                }
                                                GUILayout.EndHorizontal();
                                                    
                                                break;
                                            }
                                        case (int) TOOLBAR_BUTTON.Raw:
                                            {
                                                GUILayout.Space(10f);
                                                GUILayout.Label(Loc.RAW_TITLE, EditorStyles.boldLabel, GUILayout.MaxWidth(RIGHTPANELWIDTH));
                                                GUILayout.Label(Loc.RAW_HELPTEXT, EditorStyles.helpBox, GUILayout.MaxWidth(RIGHTPANELWIDTH));

                                                GUILayout.Space(10f);

                                                ShowFolderConfigRaw(Control.activeConfig);
                                                break;
                                            }
                                        case (int) TOOLBAR_BUTTON.Delete:
                                            {
                                                ShowDeleteConfigUI();
                                                break;
                                            }
                                        case (int) TOOLBAR_BUTTON.Duplicate:
                                            {
                                                ShowDuplicateConfigUI();
                                                break;
                                            }
                                    }
                                }
                            }
                            else
                            {
                                GUILayout.Space(10f);

                                GUILayout.Label(Loc.RC_DEFAULTTEXT, EditorStyles.helpBox, GUILayout.MaxWidth(350f));

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