using UnityEngine;

namespace MWU.FilmLib
{
    // this MUST match the order of Loc.EDITBUTTONS array below
    public enum TOOLBAR_BUTTON
    {
        Edit = 0,
        Raw,
        Delete,
        Duplicate,
    }

    public enum CONTENTWINDOW
    {
        NEWCONTENTWIZARD,
        TEMPLATEEDITOR,
    }

    /// <summary>
    /// Localization table for the Editor Toolbars
    /// </summary>
    public static class NewContentWizardLoc
    {
        // menu entries
        public const string MENU_FILE_NEWSCENE = "File/New/Scene from Template";
        public const string MENU_FILE_NEWPROJECT = "File/New/Project from Template";
        public const string MENU_FILE_CONFIGUREPROJECT = "Tools/Configure Project";
        public const string MENU_ASSETS_POPULATEFOLDER = "Assets/Project/Populate Folder Structure";
        public const string MENU_GAMEOBJECT_POPULATESCENE = "GameObject/Scene/Populate Scene Structure";
        public const string MENU_TOOLS_EDITTEMPLATES = "Tools/Templates/Edit Content Templates";

        // Dialogs
        public const string DIALOG_POPSCENE_TITLE = "Populate Scene?";
        public const string DIALOG_POPSCENE_MESSSAGE = "This will populate the current scene with the template scene hierarchy. \n\nAre you sure?";
        public const string DIALOG_POPFOLDER_TITLE = "Populate Folders?";
        public const string DIALOG_POPFOLDER_MESSSAGE = "This will populate the project with the template folder hierarchy. \n\nAre you sure?";
        public const string DIALOG_NEWPROJECTSETUP_TITLE = "Setup New Project?";
        public const string DIALOG_NEWPROJECTSETUP_MESSSAGE = "This will configure the current project with the template folders and config files. \n\n Are you sure?";
        public const string DIALOG_CONFIRMOVERWRITEEXISTING_TITLE = "Confirm overwrite?";
        public const string DIALOG_CONFIRMOVERWRITEEXISTING_MESSAGE = "Config already exists. Overwrite?";
        public const string DIALOG_CONFIRMDELETE_TITLE = "Confirm Deletion?";
        public const string DIALOG_CONFIRMDELETEENTRY_MESSAGE = "Confirm deletion of entry";
        public const string DIALOG_CONFIG_DIRTY_LOSECHANGES_MESSAGE = "Active config has changed, do you want to save changes first?";
        public const string DIALOG_CONFIG_DIRTY_LOSECHANGES_TITLE = "Save Changes?";
        public const string DIALOG_CONFIG_DELETE_TITLE = "Delete Config?";
        public const string DIALOG_CONFIG_DELETE_MESSAGE = "Are you sure you want to delete the selected config?";
        public const string DIALOG_CONFIG_DELETE = "Delete Config?";
        public const string DIALOG_CONFIG_DELETE_BUILTIN = "Built in configs can't be removed.";

        // component editor
        public const string DIALOG_CONFIRMDELETECOMPONENT_MESSAGE = "Delete component ";

        // new entry name dialog
        public const string DIALOG_ADDNEWENTRY_TITLE = "Add new Entry";
        public const string DIALOG_ADDNEWENTRY_MESSAGE = "Enter a name for the new Entry";
        public const string DIALOG_ADDENTRY_DEFAULTNAME = "New Entry";

        // new template entry dialog
        public const string DIALOG_NEWCONFIG_TITLE = "Create New Template";
        public const string DIALOG_NEWCONFIG_MESSAGE = "Enter the name for the new template and choose what type it is to continue";
        public const string DIALOG_NEWCONFIG_DEFAULTNAME = "New Template";
        public const string DIALOG_NEWCONFIG_SELECTTYPE = "Choose what type of template you wish to create";
        public const string DIALOG_NEWCONFIG_HELP_FOLDER = "Folder templates allow you to define a folder structure that can be automatically populated in a project";
        public const string DIALOG_NEWCONFIG_HELP_SCENE = "Scene templates allow you to define a GameObject hierarchy and desired components that can be automatically populated in a scene";
        public const string DIALOG_NEWCONFIG_FILENAME = "File Name: ";
        public const string DIALOG_NEWCONFIG_DISPLAYNAME = "Display Name: ";

        public const string DIALOG_OK = "Ok";
        public const string DIALOG_CANCEL = "Cancel";
        public const string DIALOG_YES = "Yes";
        public const string DIALOG_NO = "No";

        // global window 
        public static string WINDOWLABEL_TEMPLATE = "Content Template Editor";
        public static string WINDOWLABEL_WIZARD = "New Scene Wizard";

        // left column
        public static string TEMPLATELIST = "Available Templates";
        public static string BUILTINTEMPLATES = "Built-in Templates";
        public static string USERTEMPLATES = "User Templates";
        public static string ADD = "Add";
        public static string REFRESH = "Refresh";
        public static string DELETE = "Delete";
        public static string DUPLICATE = "Duplicate";

        // right column
        public static string RC_DEFAULTTEXT = "Select an existing template from the left for more information, or click 'add' to create a new template.";
        public static string NCW_DEFAULTTEXT = "Select a template from the left to begin.";
        public static string[] EDITBUTTONS = { "Edit", "Raw", "Delete", "Duplicate"};
        public static string NEWCONTENT_GENERATESCENE = "Generate Scene";
        public static string NEWCONTENT_GENERATEFOLDER = "Generate Folders";

        // duplicate subtab
        public static string DUPE_TITLE = "Duplicate Config";
        public static string DUPE_HELPTEXT = "Make a copy of the selected config";
        public static string DUPE_DESTNAME = "Filename: ";
        public static string DUPE_LOCATION = "Copy will be saved into the Unity project under Assets/Settings/Templates";

        // raw subtab
        public static string RAW_TITLE = "Raw JSON Config";
        public static string RAW_HELPTEXT = "Below is the raw (read-only) JSON of the config currently";

        public static string DELETE_TITLE = "Delete Config";
        public static string DELETE_HELPTEXT = "Delete the currently selected config";

        // edit subtab
        public static string EDIT_TITLE = "Edit Template";
        public static string EDIT_TEMPLATETYPE = "Template Type : ";
        public static string EDIT_NAME = "Name: ";
        public static string EDIT_DESCRIPTION = "Description: ";
        public static string EDIT_SOURCEFILE = "Source file: ";
        public static string EDIT_HELPTEXT = "This allows you to create & configure the current template";
        public static string EDIT_FOLDERCONFIGSELECTION_TRUE = "Edit selected entry";
        public static string EDIT_FOLDERCONFIGSELECTION_FALSE = "Select an entry to view or edit details";
        public static string EDIT_VIEWONLYFOLDERDETAILS = "This config will generate the following empty folder structure in the current project";
        public static string EDIT_VIEWONLYFOLDERDETAILS_EDITOR = "This built-in config will generate the following folder structure in your project. Duplicate it to modify or change the template.";
        public static string EDIT_VIEWONLYSCENEDETAILS_EDITOR = "This built-in config will generate the following scene hierarchy in the current active scene. Select entries in the list to view the components that will be added. Duplicate it to modify or change the template.";
        public static string NCW_VIEWONLYFOLDERDETAILS = "This config will generate the following folder structure in your project.";
        public static string NCW_VIEWONLYSCENEDETAILS = "This config will generate the following scene hierarchy and components in the current active scene. Select entries in the list to view the components that will be added.";
        public static string EDIT_SAVECHANGES = "Save Changes to Template";
        public static string EDIT_NEWELEMENT = "[New Top-Level Entry]";
        public static string EDIT_SELECTEDFOLDERENTRY = "Selected Folder Entry";
        public static string EDIT_SELECTEDSCENEENTRY = "Selected Scene Entry";
        public static string EDIT_SELECTEDENTRYNAME = "Name: ";
        public static string EDIT_SELECTEDENTRYUNDO = "Discard Changes";
        public static string EDIT_SELECTEDENTRYSAVE = "Save";

        // tooltips
        public static string TOOLTIP_NEWCONTENT_GENERATESCENE = "Populate the currently active scene with the template structure?";
        public static string TOOLTIP_NEWCONTENT_GENERATEFOLDERS = "Populate project with the template folder structure?";
        public static string TOOLTIP_TEMPLATELIST_OPTIONS = "Select Template to view Options";
        public static string TOOLTIP_EDITTEMPLATE = "Edit currently selected template";
        public static string TOOLTIP_ADDTEMPLATE = "Add a new Template";
        public static string TOOLTIP_REFRESHTEMPLATE = "Refresh Template list";
        public static string TOOLTIP_DUPETEMPLATE = "Duplicate currently selected template";
        public static string TOOLTIP_ADDCHILDELEMENT = "Add new Child element?";
        public static string TOOLTIP_REMOVEELEMENT = "Remove this entry";
        public static string TOOLTIP_SAVECHANGES = "Save changes to this item";
        public static string TOOLTIP_CANCELCHANGES = "Cancel changes to this item";

        public static string EMPTYFILE_PLACEHOLDER = "Placeholder - add your content here";

        //some standard colors
        public static Color doneColor = new Color(0f, 255f, 0f);
        public static Color cancelColor = new Color(255f, 0f, 0f);
        public static Color defaultColor = new Color(128f,128f,128f);
    }
}