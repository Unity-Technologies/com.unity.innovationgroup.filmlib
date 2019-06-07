using UnityEditor;
using UnityEngine;
using Control = MWU.FilmLib.NewContentWizardController;
using Loc = MWU.FilmLib.NewContentWizardLoc;

namespace MWU.FilmLib
{
    public class NewContentWizardMenus
    {
        public const int DEFAULTMENUPRIORITY = -100;

#if NEWCONTENTWIZARD
        [MenuItem(Loc.MENU_FILE_NEWSCENE, false, DEFAULTMENUPRIORITY)]
#endif
        public static void NewSceneFromTemplate()
        {
            var thisWindow = EditorWindow.GetWindow<NewSceneWizardView>(false, Loc.WINDOWLABEL_WIZARD, true);
            thisWindow.SetWizardType(TEMPLATE_TYPE.SCENE);
        }

        //[MenuItem(Loc.MENU_FILE_NEWPROJECT, false, DEFAULTMENUPRIORITY)]
        //public static void NewProjectFromTemplate()
        //{
        //    Debug.Log("Create new Project from template: Sorry not supported currently");
        //}

#if NEWCONTENTWIZARD
        [MenuItem(Loc.MENU_TOOLS_EDITTEMPLATES, false, DEFAULTMENUPRIORITY)]
#endif
        public static void EditContentTemplates()
        {
            var thisWindow = EditorWindow.GetWindow<TemplateEditorView>(false, Loc.WINDOWLABEL_TEMPLATE, true);
        }

#if NEWCONTENTWIZARD
        [MenuItem(Loc.MENU_FILE_CONFIGUREPROJECT, false, DEFAULTMENUPRIORITY)]
#endif
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

#if NEWCONTENTWIZARD
        [MenuItem(Loc.MENU_ASSETS_POPULATEFOLDER, false, DEFAULTMENUPRIORITY)]
#endif
        public static void PopulateFolderStructure()
        {
            var thisWindow = EditorWindow.GetWindow<NewSceneWizardView>(false, Loc.WINDOWLABEL_WIZARD, true);
            thisWindow.SetWizardType(TEMPLATE_TYPE.FOLDER);
        }

#if NEWCONTENTWIZARD
        [MenuItem(Loc.MENU_GAMEOBJECT_POPULATESCENE, false, DEFAULTMENUPRIORITY)]
#endif
        public static void PopulateSceneStructure()
        {
            var thisWindow = EditorWindow.GetWindow<NewSceneWizardView>(false, Loc.WINDOWLABEL_WIZARD, true);
            thisWindow.SetWizardType(TEMPLATE_TYPE.SCENE);
        }
    }
}
