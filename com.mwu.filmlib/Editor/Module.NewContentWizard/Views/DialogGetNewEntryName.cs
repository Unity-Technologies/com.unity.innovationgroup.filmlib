using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Control = MWU.FilmLib.NewContentWizardController;
using Loc = MWU.FilmLib.NewContentWizardLoc;                                   // string localization table

namespace MWU.FilmLib
{
    public enum DialogType
    {
        NewEntryName
    }

    /// <summary>
    /// Simple popup dialog design to serve as an input dialog for recieving text input (for file names / etc)
    /// 
    /// sample usage:
    /// // show dialog to get the desired name of the new entry
    ///     var dialog = EditorWindow.CreateInstance<DialogGetNewEntryName>();
    ///     {
    ///         dialog.entry = entry;
    ///         dialog.dialogTitle = Loc.DIALOG_ADDNEWENTRY_TITLE;
    ///         dialog.dialogMessage = Loc.DIALOG_ADDNEWENTRY_MESSAGE;
    ///         }
    ///     dialog.ShowUtility();
    /// </summary>
    public class DialogGetNewEntryName : EditorWindow
    {
        /// <summary>
        /// The value that the user enters on the dialog
        /// </summary>
        public static string inputValue;

        /// <summary>
        /// Title for the dialog when opened
        /// </summary>
        public string dialogTitle;

        /// <summary>
        /// Message shown on the dialog when opened
        /// </summary>
        public string dialogMessage;
        public DialogType dialogType = DialogType.NewEntryName;

        public GenericHierarchyEntry entry;
        private bool focused = false;

        private void OnEnable()
        {
            inputValue = Loc.DIALOG_ADDENTRY_DEFAULTNAME;
        }

        private void OnGUI()
        {
            GUILayout.Space(10f);
            titleContent = new GUIContent(dialogTitle);
            GUILayout.Label(dialogMessage, EditorStyles.helpBox);

            GUILayout.Space(10f);
            // show our text input and make sure that it's focused
            GUI.SetNextControlName("textInput");
            inputValue = GUILayout.TextField(inputValue, GUILayout.ExpandWidth(true));
            if (!focused)
            {
                GUI.FocusControl("textInput");
                var te = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                te.cursorIndex = 1;
                focused = true;
            }

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
                    if (inputValue != string.Empty)
                    {
                        if (entry != null)
                        {
                            Debug.Log("Adding new Entry: " + inputValue + " to parent : " + entry.name);
                        }
                        else
                        {
                            Debug.Log("Add new Top-Level Entry: " + inputValue);
                        }

                        switch (dialogType)
                        {
                            case DialogType.NewEntryName:
                                {
                                    Control.entryEditDirty = true;
                                    Control.AddFolderEntry(entry, inputValue);
                                    break;
                                }
                            default:
                                {
                                    Debug.Log("Unknown dialog type?");
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