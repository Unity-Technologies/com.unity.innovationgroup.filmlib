using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Loc = MWU.FilmLib.NewContentWizardLoc;
using Control = MWU.FilmLib.NewContentWizardController;

namespace MWU.FilmLib
{
    public class NewContentWizardMenus
    {
        public const int DEFAULTMENUPRIORITY = -100;

        [MenuItem(Loc.MENU_FILE_NEWSCENE, false, DEFAULTMENUPRIORITY)]
        public static void NewSceneFromTemplate()
        {
            var thisWindow = EditorWindow.GetWindow<NewSceneWizardView>(false, Loc.WINDOWLABEL_WIZARD, true);
            thisWindow.SetWizardType(TEMPLATE_TYPE.SCENE);
        }

        [MenuItem(Loc.MENU_FILE_NEWPROJECT, false, DEFAULTMENUPRIORITY)]
        public static void NewProjectFromTemplate()
        {
            Debug.Log("Create new Project from template: Sorry not supported currently");
        }

        [MenuItem(Loc.MENU_TOOLS_EDITTEMPLATES, false, DEFAULTMENUPRIORITY)]
        public static void EditContentTemplates()
        {
            var thisWindow = EditorWindow.GetWindow<TemplateEditorView>(false, Loc.WINDOWLABEL_TEMPLATE, true);
        }

        [MenuItem(Loc.MENU_FILE_CONFIGUREPROJECT, false, DEFAULTMENUPRIORITY)]
        public static void ConfigureProject()
        {
            if (EditorUtility.DisplayDialog(Loc.DIALOG_NEWPROJECTSETUP_TITLE,
                                            Loc.DIALOG_NEWPROJECTSETUP_MESSSAGE,
                                            Loc.DIALOG_OK,
                                            Loc.DIALOG_CANCEL))
            {
                Control.ConfigureProject();
            }
        }

        [MenuItem(Loc.MENU_ASSETS_POPULATEFOLDER, false, DEFAULTMENUPRIORITY)]
        public static void PopulateFolderStructure()
        {
            var thisWindow = EditorWindow.GetWindow<NewSceneWizardView>(false, Loc.WINDOWLABEL_WIZARD, true);
            thisWindow.SetWizardType(TEMPLATE_TYPE.FOLDER);
        }

        [MenuItem(Loc.MENU_GAMEOBJECT_POPULATESCENE, false, DEFAULTMENUPRIORITY)]
        public static void PopulateSceneStructure()
        {
            var thisWindow = EditorWindow.GetWindow<NewSceneWizardView>(false, Loc.WINDOWLABEL_WIZARD, true);
            thisWindow.SetWizardType(TEMPLATE_TYPE.SCENE);
        }
    }
}