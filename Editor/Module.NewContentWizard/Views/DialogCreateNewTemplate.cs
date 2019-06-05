using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Control = MWU.FilmLib.NewContentWizardController;
using Loc = MWU.FilmLib.NewContentWizardLoc;                                   // string localization table
using Core = MWU.FilmLib.NewContentWizardCore;

namespace MWU.FilmLib
{
    /// <summary>
    /// New template creation dialog
    /// 
    /// sample usage:
    /// /var dialog = EditorWindow.CreateInstance<DialogCreateNewTemplate>();
    ///        dialog.opener = opener;
    ///        dialog.ShowUtility();
    /// </summary>
    public class DialogCreateNewTemplate : EditorWindow
    {
        public const float LABEL_WIDTH = 100f;
        /// <summary>
        /// The value that the user enters on the dialog
        /// </summary>
        public static string templateFileName;
        public static string templateDisplayName;
        public static TEMPLATE_TYPE templateType;
        public static string[] validTypes = new string[]
        {
            "Folder", "Scene",
        };
        public static int selectedType = 0;

        private bool focused = false;
        /// <summary>
        /// The parent window that opened us so we can attempt to refresh the template view once we're done
        /// </summary>
        public NewContentWizardSharedView opener;

        private void OnEnable()
        {
            templateFileName = Loc.DIALOG_ADDENTRY_DEFAULTNAME;
            // make sure the dialog is centered
            var dialogPosition = position;
            dialogPosition.center = new Rect(0f, 0f, Screen.currentResolution.width, Screen.currentResolution.height).center;
            position = dialogPosition;
            maxSize = new Vector2(500f, 235f);
            minSize = new Vector2(500f, 235f);
        }

        private void OnGUI()
        {
            GUILayout.Space(10f);
            titleContent = new GUIContent(Loc.DIALOG_NEWCONFIG_TITLE);
            GUILayout.Space(10f);

            GUILayout.Label(Loc.DIALOG_NEWCONFIG_MESSAGE, EditorStyles.helpBox);
            GUILayout.Space(10f);

            GUILayout.Label(Loc.DIALOG_NEWCONFIG_SELECTTYPE);
            selectedType = EditorGUILayout.Popup(selectedType, validTypes);
            GUILayout.Space(10f);

            switch( selectedType)
            {
                case (int) TEMPLATE_TYPE.FOLDER:
                    {
                        GUILayout.Label(Loc.DIALOG_NEWCONFIG_HELP_FOLDER, EditorStyles.helpBox);
                        break;
                    }
                case (int)TEMPLATE_TYPE.SCENE:
                    {
                        GUILayout.Label(Loc.DIALOG_NEWCONFIG_HELP_SCENE, EditorStyles.helpBox);
                        break;
                    }
            }

            GUILayout.Space(10f);

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(Loc.DIALOG_NEWCONFIG_FILENAME, GUILayout.Width(LABEL_WIDTH));
                // show our text input and make sure that it's focused
                GUI.SetNextControlName("textInput");
                templateFileName = GUILayout.TextField(templateFileName, GUILayout.ExpandWidth(true));
                if (!focused)
                {
                    GUI.FocusControl("textInput");
                    var te = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                    te.cursorIndex = 1;
                    focused = true;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(Loc.DIALOG_NEWCONFIG_DISPLAYNAME, GUILayout.Width(LABEL_WIDTH));
                templateDisplayName = GUILayout.TextField(templateDisplayName, GUILayout.ExpandWidth(true));
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10f);
            GUILayout.BeginHorizontal();
            {
                var style = EditorStyles.miniButton;

                GUI.backgroundColor = Loc.cancelColor;
                if (GUILayout.Button("Cancel", style, GUILayout.Height(35f)))
                {
                    Close();
                }

                GUI.backgroundColor = Loc.doneColor;
                if (GUILayout.Button("Save", style, GUILayout.Height(35f)))
                {
                    if (templateFileName != string.Empty)
                    {
                        switch (selectedType)
                        {
                            case (int)TEMPLATE_TYPE.FOLDER:
                                {
                                    var newFolderConfig = new GenericHierarchyConfig()
                                    {
                                        name = templateDisplayName,
                                        type = TEMPLATE_TYPE.FOLDER
                                    };
                                    // save the new config into the user's templates folder
                                    Core.SaveConfig(templateFileName, newFolderConfig);

                                    Control.Reset();
                                    break;
                                }
                            case (int)TEMPLATE_TYPE.SCENE:
                                {
                                    var newSceneConfig = new GenericHierarchyConfig()
                                    {
                                        name = templateDisplayName,
                                        type = TEMPLATE_TYPE.SCENE
                                    };
                                    // save the new config into the user's templates folder
                                    Core.SaveConfig(templateFileName, newSceneConfig);

                                    Control.Reset();

                                    // attempt to reset the parent window so we can refresh our file list, but doesn't seem to work, unity asset database doesn't refresh in time
                                    if (opener.windowType == EditorWindowType.NewSceneWizard)
                                    {
                                        var parentWindow = (NewSceneWizardView)opener;
                                        parentWindow.Reset();
                                    }
                                    else if (opener.windowType == EditorWindowType.TemplateEditor)
                                    {
                                        var parentWindow = (TemplateEditorView)opener;
                                        parentWindow.Reset();
                                    }

                                    break;
                                }
                        }
                        Close();
                    }
                }
                GUI.backgroundColor = Loc.defaultColor;
            }
            GUILayout.EndHorizontal();
        }
    }
}