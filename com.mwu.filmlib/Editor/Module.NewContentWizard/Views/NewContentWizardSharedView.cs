using System;
using System.Text;
using UnityEditor;
using UnityEngine;
using Control = MWU.FilmLib.NewContentWizardController;
using Core = MWU.FilmLib.NewContentWizardCore;
using Loc = MWU.FilmLib.NewContentWizardLoc;                                   // string localization table

namespace MWU.FilmLib
{
    public enum EditorWindowType
    {
        TemplateEditor,
        NewSceneWizard,
    }

    /// <summary>
    /// Base window functionality, shared across NewSceneWizardView and TemplateEditorView
    /// </summary>
    public class NewContentWizardSharedView : EditorWindow
    {
        public EditorWindowType windowType;

        protected static float LEFTCOLUMNSIZE = 300f;
        protected static float STANDARDBUTTONSIZE = 225f;
        protected static float STANDARDBUTTONHEIGHT = 35f;
        protected static float HIERARCHYINDENTOFFSET = 10f;
        protected static float SMALLBUTTONSIZE = 18f;
        protected static float RIGHTPANELWIDTH = 400f;

        // window properties
        protected static Vector2 maxWindowSize = new Vector2(1920f, 50f);
        protected static Vector2 minWindowSize = new Vector2(1260f, 50f);
        protected Vector2 curWindowSize = new Vector2(1920f, 50f);
        
        // scrollbar state
        protected static Vector2 _folderEditScroll = Vector2.zero;
        protected static Vector2 _templateScroll = Vector2.zero;
        protected static Vector2 _sceneEditScroll = Vector2.zero;
        protected static Vector2 _sceneViewScroll = Vector2.zero;
        
        private static string rawTemplateText = string.Empty;           // simple entry to show the raw json config text

        // header images / styles
        private static Texture2D bannerLogoNewContent, bannerLogoTemplateEditor;
        private static GUIStyle headerBG;

        // assets for the template view listing
        private static bool assetLoadComplete = false;
        protected static Texture2D imgArrowLeft, imgArrowRight, imgArrowUp, imgArrowDown, imgAdd, imgRemove, imgClear, imgRefresh, imgEdit, imgDone, imgCreate, imgSettings, imgSave, imgCancel;

        protected void Reset()
        {
            // reset the controller
            Control.Reset();

            _folderEditScroll = Vector2.zero;
            _sceneEditScroll = Vector2.zero;
            _templateScroll = Vector2.zero;
            _sceneViewScroll = Vector2.zero;

            /*
             * Generate our template buttons
             */

            // templates built into the package
            Control.defaultTemplateButtons.Clear();
            foreach (var template in Core.defaultTemplates)
            {
                // load the config
                var config = Core.LoadGenericConfig(template.path);
                var button = new TemplateButtonEntry
                {
                    config = config,
                    displayName = config.name,
                    fileName = template.fileName,
                    description = config.description,
                    path = template.path,
                    type = config.type,
                    isPressed = false,
                    isBuiltIn = true,
                };
                Control.defaultTemplateButtons.Add(button);
            }

            // templates in user-land project space
            Control.userTemplateButtons.Clear();
            foreach (var template in Core.userTemplates)
            {
                var config = Core.LoadGenericConfig(template.path);
                var button = new TemplateButtonEntry
                {
                    config = config,
                    displayName = config.name,
                    fileName = template.fileName,
                    description = config.description,
                    path = template.path,
                    type = config.type,
                    isPressed = false,
                    isBuiltIn = false,
                };
                Control.userTemplateButtons.Add(button);
            }
        }

        /// <summary>
        /// Draws our top header
        /// </summary>
        /// <param name="windowType"></param>
        protected void DrawBanner(CONTENTWINDOW windowType)
        {
            switch (windowType)
            {
                case CONTENTWINDOW.NEWCONTENTWIZARD:
                    {
                        // load the proper banner
                        if (bannerLogoNewContent == null)
                        {
                            bannerLogoNewContent = new Texture2D(32, 32);
                            bannerLogoNewContent = AssetDatabase.LoadAssetAtPath<Texture2D>(EditorUtilities.packagePathRoot + "/Module.NewContentWizard/Content/White/Banner/NewContentWizard.png");
                        }
                        break;
                    }
                case CONTENTWINDOW.TEMPLATEEDITOR:
                    {
                        if( bannerLogoTemplateEditor == null)
                        {
                            bannerLogoTemplateEditor = new Texture2D(32, 32);
                            bannerLogoTemplateEditor = AssetDatabase.LoadAssetAtPath<Texture2D>(EditorUtilities.packagePathRoot + "/Module.NewContentWizard/Content/White/Banner/TemplateEditor.png");
                        }
                        break;
                    }
            }

            if(headerBG == null)
            {
                headerBG = new GUIStyle();
                var background = AssetDatabase.LoadAssetAtPath<Texture2D>(EditorUtilities.packagePathRoot + "/Module.NewContentWizard/Content/White/Banner/Background.png");
                headerBG.normal.background = background;
            }

            GUILayout.BeginHorizontal(headerBG, GUILayout.MinWidth(position.width));
            {
                switch( windowType)
                {
                    case CONTENTWINDOW.NEWCONTENTWIZARD:
                        {
                            GUILayout.Label(bannerLogoNewContent);
                            break;
                        }
                    case CONTENTWINDOW.TEMPLATEEDITOR:
                        {
                            GUILayout.Label(bannerLogoTemplateEditor);
                            break;
                        }
                }
            }
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Helper to generate controls in our list
        /// </summary>
        /// <param name="button"></param>
        protected void CreateTemplateButton(TemplateButtonEntry button)
        {
            var shortName = button.displayName.Split('.')[0];
            var configPath = button.path;
            var config = button.config;
            var type = button.type;

            button.isPressed = GUILayout.Toggle(button.isPressed, new GUIContent( shortName, null, Loc.TOOLTIP_TEMPLATELIST_OPTIONS), "Button", GUILayout.MaxWidth(STANDARDBUTTONSIZE), GUILayout.MaxHeight(STANDARDBUTTONHEIGHT));
            if (button.isPressed)
            {
                var actuallySwitch = Control.ConfirmSwitchActiveButton(button);
                if( !actuallySwitch)
                {
                    button.isPressed = false;
                }
                else
                {
                    button.isPressed = true;
                }
            }
        }

        /// <summary>
        /// given a config, generate a textarea to display the raw json
        /// </summary>
        /// <param name="config"></param>
        protected void ShowFolderConfigRaw(GenericHierarchyConfig config)
        {
            rawTemplateText = Core.ConfigToString(config);
            _folderEditScroll = GUILayout.BeginScrollView(_folderEditScroll, GUILayout.Width(350f), GUILayout.ExpandHeight(true));
            {
                rawTemplateText = GUILayout.TextArea(rawTemplateText, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            }
            GUILayout.EndScrollView();
        }

        /// <summary>
        /// given a config, generate a textarea to display the raw json
        /// </summary>
        /// <param name="config"></param>
        protected void ShowSceneConfigRaw(GenericHierarchyConfig config)
        {
            rawTemplateText = Core.ConfigToString(config);
            _sceneEditScroll = GUILayout.BeginScrollView(_sceneEditScroll, GUILayout.Width(RIGHTPANELWIDTH), GUILayout.ExpandHeight(true));
            {
                rawTemplateText = GUILayout.TextArea(rawTemplateText, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            }
            GUILayout.EndScrollView();
        }

        /// <summary>
        /// Generate our tree view for the template, with the optional ability to edit it
        /// </summary>
        /// <param name="config">current config that we're viewing / editing</param>
        /// <param name="canEdit">whether we are in edit mode or just read-only</param>
        protected void ViewTemplate(bool canEdit)
        {
            if (!assetLoadComplete)
            {
                LoadAssets();
            }

            GUILayout.BeginVertical();
            {
                // the top info about the template
                if( canEdit)
                {
                    if (Control.activeEntrySelection == null)
                    {
                        GUILayout.Label(Loc.EDIT_FOLDERCONFIGSELECTION_FALSE, EditorStyles.helpBox, GUILayout.MaxWidth(RIGHTPANELWIDTH));
                    }
                    else
                    {
                        GUILayout.Label(Loc.EDIT_FOLDERCONFIGSELECTION_TRUE, EditorStyles.helpBox, GUILayout.MaxWidth(RIGHTPANELWIDTH));
                    }
                }
                else
                {
                    if( windowType == EditorWindowType.TemplateEditor)
                    {
                        if (Control.activeConfig.type == TEMPLATE_TYPE.FOLDER)
                        {
                            GUILayout.Label(Loc.EDIT_VIEWONLYFOLDERDETAILS_EDITOR, EditorStyles.helpBox, GUILayout.MaxWidth(RIGHTPANELWIDTH));
                        }
                        else
                        {
                            GUILayout.Label(Loc.EDIT_VIEWONLYSCENEDETAILS_EDITOR, EditorStyles.helpBox, GUILayout.MaxWidth(RIGHTPANELWIDTH));
                        }
                    }
                    else
                    {
                        if(Control.activeConfig.type == TEMPLATE_TYPE.FOLDER)
                        {
                            GUILayout.Label(Loc.NCW_VIEWONLYFOLDERDETAILS, EditorStyles.helpBox, GUILayout.MaxWidth(RIGHTPANELWIDTH));
                        }
                        else
                        {
                            GUILayout.Label(Loc.NCW_VIEWONLYSCENEDETAILS, EditorStyles.helpBox, GUILayout.MaxWidth(RIGHTPANELWIDTH));
                        }
                    }
                    
                    // these can never be true in read only mode
                    Control.activeConfigEditDirty = false;
                    Control.entryEditDirty = false;
                }

                GUILayout.BeginHorizontal();
                {
                    // left column
                    var scrollWidth = 200f;
                    _sceneEditScroll = GUILayout.BeginScrollView(_sceneEditScroll, GUILayout.MinWidth(scrollWidth), GUILayout.MaxWidth(scrollWidth), GUILayout.ExpandHeight(true));
                    {
                        // draw our folder view
                        // can't use a foreach because we remove entries, breaks enumeration
                        for (var i = 0; i < Control.activeConfig.entries.Count; i++)
                        {
                            CreateTreeViewUI(Control.activeConfig.entries[i], "", canEdit, 0f);
                        }

                        // only need 'new element' entry if we can edit the template
                        if (canEdit)
                        {
                            // add an entry to add a new root element
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label(Loc.EDIT_NEWELEMENT);
                                CreateEntryEditControls(null);
                            }
                            GUILayout.EndHorizontal();
                        }
                    }
                    GUILayout.EndScrollView();

                    // right detail column
                    GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                    {
                        GUILayout.Space(10f);
                            
                        if (Control.activeEntrySelection != null)
                        {
                            
                            if (Control.activeConfig.type == TEMPLATE_TYPE.FOLDER)
                            {
                                GUILayout.Label(Loc.EDIT_SELECTEDFOLDERENTRY, EditorStyles.boldLabel);
                            }
                            else
                            {
                                GUILayout.Label(Loc.EDIT_SELECTEDSCENEENTRY, EditorStyles.boldLabel);
                            }
                            
                            GUILayout.BeginVertical();
                            {
                                /*
                                *
                                * display individual entry in the template
                                * 
                                */

                                // Top Row - Name 
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label(Loc.EDIT_SELECTEDENTRYNAME, GUILayout.Width(50f));
                                    // show the editable version if we're in edit mode
                                    if (canEdit)
                                    {
                                        // monitor the text field for changes
                                        EditorGUI.BeginChangeCheck();
                                        {
                                            Control.tempEntrySelectionString = GUILayout.TextField(Control.tempEntrySelectionString, GUILayout.Width(150f));
                                        }
                                        if (EditorGUI.EndChangeCheck())
                                        {
                                            Control.entryEditDirty = true;
                                        }

                                        // if they changed the entry name, prompt to save 
                                        if (Control.entryEditDirty)
                                        {
                                            // done button
                                            GUI.backgroundColor = Loc.doneColor;
                                            var done = new GUIContent(imgDone, Loc.TOOLTIP_SAVECHANGES);
                                            if (GUILayout.Button(imgDone, GUILayout.Width(SMALLBUTTONSIZE), GUILayout.Height(SMALLBUTTONSIZE)))
                                            {
                                                Control.SaveChangeToEntry(Control.tempEntrySelectionString);
                                            }
                                            // cancel button
                                            GUI.backgroundColor = Loc.cancelColor;
                                            var cancel = new GUIContent(imgCancel, Loc.TOOLTIP_CANCELCHANGES);
                                            if (GUILayout.Button(imgCancel, GUILayout.Width(SMALLBUTTONSIZE), GUILayout.Height(SMALLBUTTONSIZE)))
                                            {
                                                // reset to what it was originally
                                                Control.tempEntrySelectionString = Control.activeEntrySelection.name;
                                                Control.entryEditDirty = false;
                                            }
                                            // reset colors back to default
                                            GUI.backgroundColor = Loc.defaultColor;
                                        }
                                    }
                                    else
                                    {
                                        // otherwise just the read only version
                                        GUILayout.Label(Control.tempEntrySelectionString, EditorStyles.helpBox, GUILayout.Width(150f));
                                    }
                                }
                                GUILayout.EndHorizontal();
                               
                                // Second row - entry specific elements 
                                GUILayout.BeginHorizontal();
                                {
                                    // UI for adding components & their options 
                                    CreateComponentEditor(Control.activeEntrySelection, canEdit);
                                }
                                GUILayout.EndHorizontal();
                            }
                            GUILayout.EndVertical();
                        }
                        GUILayout.EndVertical();
                    }
                }
                GUILayout.EndHorizontal();

                // bottom 'discard / save' buttons if the user has changed the config
                if (Control.activeConfigEditDirty)
                {
                    GUILayout.BeginVertical();
                    {
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Space(170f);
                            GUILayout.Label(Loc.EDIT_SAVECHANGES);
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        {
                            var style = EditorStyles.miniButton;
                            GUI.backgroundColor = Loc.cancelColor;
                            if (GUILayout.Button(Loc.EDIT_SELECTEDENTRYUNDO, style, GUILayout.Width(165f), GUILayout.Height(STANDARDBUTTONHEIGHT)))
                            {
                                Reset();
                            }
                            GUI.backgroundColor = Loc.doneColor;
                            if (GUILayout.Button(Loc.EDIT_SELECTEDENTRYSAVE, style, GUILayout.Width(165f), GUILayout.Height(STANDARDBUTTONHEIGHT)))
                            {
                                Control.SaveConfigChanges();
                            }
                            GUI.backgroundColor = Loc.defaultColor;

                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();
                }
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Load the assets (icons etc) that we need
        /// </summary>
        protected void LoadAssets()
        {
            if (imgArrowLeft == null) {  imgArrowLeft = LoadAsset("/Module.NewContentWizard/Content/White/Icons/baseline_arrow_left_white_48dp.png"); assetLoadComplete = (imgArrowLeft != null) ? true : false;}
            if (imgArrowRight == null) { imgArrowRight = LoadAsset("/Module.NewContentWizard/Content/White/Icons/baseline_arrow_right_white_48dp.png"); assetLoadComplete = (imgArrowRight != null) ? true : false; }
            if (imgArrowUp == null) { imgArrowUp = LoadAsset("/Module.NewContentWizard/Content/White/Icons/baseline_arrow_drop_up_white_48dp.png"); assetLoadComplete = (imgArrowUp != null) ? true : false; }
            if (imgArrowDown == null) { imgArrowDown = LoadAsset("/Module.NewContentWizard/Content/White/Icons/baseline_arrow_drop_down_white_48dp.png"); assetLoadComplete = (imgArrowDown != null) ? true : false; }
            if (imgAdd == null) { imgAdd = LoadAsset("/Module.NewContentWizard/Content/White/Icons/baseline_add_white_48dp.png"); assetLoadComplete = (imgAdd != null) ? true : false; }
            if (imgRemove == null) { imgRemove = LoadAsset("/Module.NewContentWizard/Content/White/Icons/baseline_remove_white_48dp.png"); assetLoadComplete = (imgRemove != null) ? true : false; }
            if (imgRefresh == null) { imgRefresh = LoadAsset("/Module.NewContentWizard/Content/White/Icons/baseline_refresh_white_48dp.png"); assetLoadComplete = (imgRefresh != null) ? true : false; }
            if (imgClear == null) { imgClear = LoadAsset("/Module.NewContentWizard/Content/White/Icons/baseline_clear_white_48dp.png"); assetLoadComplete = (imgClear != null) ? true : false; }
            if (imgEdit == null) { imgEdit = LoadAsset("/Module.NewContentWizard/Content/White/Icons/baseline_edit_white_48dp.png"); assetLoadComplete = (imgEdit != null) ? true : false; }
            if (imgDone == null) { imgDone = LoadAsset("/Module.NewContentWizard/Content/White/Icons/baseline_done_white_48dp.png"); assetLoadComplete = (imgDone != null) ? true : false; }
            if (imgCreate == null) { imgCreate = LoadAsset("/Module.NewContentWizard/Content/White/Icons/baseline_create_white_48dp.png"); assetLoadComplete = (imgCreate != null) ? true : false; }
            if (imgSettings == null) { imgSettings = LoadAsset("/Module.NewContentWizard/Content/White/Icons/baseline_settings_white_48dp.png"); assetLoadComplete = (imgSettings != null) ? true : false; }
            if (imgSave == null) { imgSave = LoadAsset("/Module.NewContentWizard/Content/White/Icons/baseline_save_alt_white_48dp.png"); assetLoadComplete = (imgSave != null) ? true : false; }
            if (imgCancel == null) { imgCancel = LoadAsset("/Module.NewContentWizard/Content/White/Icons/baseline_clear_white_48dp.png"); assetLoadComplete = (imgCancel != null) ? true : false; }

            if( !assetLoadComplete)
            {
                Debug.LogError("Error loading assets for Content Wizard?");
            }
        }

        /// <summary>
        /// Internal individual asset loading
        /// </summary>
        private Texture2D LoadAsset( string path)
        {
            return AssetDatabase.LoadAssetAtPath<Texture2D>(EditorUtilities.packagePathRoot + path);
        }
        
        /// <summary>
        /// Delete selected config UI
        /// </summary>
        protected void ShowDeleteConfigUI()
        {
            GUILayout.Space(10f);
            GUILayout.Label(Loc.DELETE_TITLE, EditorStyles.boldLabel, GUILayout.MaxWidth(RIGHTPANELWIDTH));
            GUILayout.Label(Loc.DELETE_HELPTEXT, EditorStyles.helpBox, GUILayout.MaxWidth(RIGHTPANELWIDTH));

            GUILayout.Space(10f);

            if (!Control.activeSelection.isBuiltIn)
            {
                GUI.backgroundColor = Loc.cancelColor;
                if (GUILayout.Button(Loc.DELETE, GUILayout.MaxWidth(RIGHTPANELWIDTH), GUILayout.MaxHeight(STANDARDBUTTONHEIGHT)))
                {
                    if (EditorUtility.DisplayDialog(Loc.DIALOG_CONFIRMDELETE_TITLE,
                    Loc.DIALOG_CONFIRMDELETEENTRY_MESSAGE,
                    Loc.DIALOG_OK,
                    Loc.DIALOG_CANCEL))
                    {
                        Core.DeleteConfig(Control.activeSelection.path);
                    }
                }
                GUI.backgroundColor = Loc.defaultColor;
            }
            else
            {
                GUILayout.BeginHorizontal();
                {
                    GUI.color = Loc.cancelColor;
                    GUILayout.Label(imgClear, GUILayout.Width(SMALLBUTTONSIZE * 2), GUILayout.Height(SMALLBUTTONSIZE * 2));
                    GUI.color = Loc.defaultColor;
                    GUILayout.Label(Loc.DIALOG_CONFIG_DELETE_BUILTIN, EditorStyles.boldLabel, GUILayout.MaxWidth(RIGHTPANELWIDTH));
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.Space(10f);
        }

        /// <summary>
        /// Duplicate selected config UI
        /// </summary>
        protected void ShowDuplicateConfigUI()
        {
            GUILayout.Space(10f);
            GUILayout.Label(Loc.DUPE_TITLE, EditorStyles.boldLabel, GUILayout.MaxWidth(RIGHTPANELWIDTH));
            GUILayout.Label(Loc.DUPE_HELPTEXT, EditorStyles.helpBox, GUILayout.MaxWidth(RIGHTPANELWIDTH));
            GUILayout.Space(10f);

            GUILayout.Label(Loc.DUPE_DESTNAME, GUILayout.MaxWidth(RIGHTPANELWIDTH));

            Control.tempFileName = GUILayout.TextField(Control.tempFileName, GUILayout.MaxWidth(RIGHTPANELWIDTH));
            GUILayout.Label(Loc.DUPE_LOCATION, EditorStyles.helpBox, GUILayout.MaxWidth(RIGHTPANELWIDTH));
            GUILayout.Space(10f);

            GUI.backgroundColor = Loc.doneColor;
            if (GUILayout.Button("Duplicate", GUILayout.MaxWidth(RIGHTPANELWIDTH), GUILayout.MaxHeight(STANDARDBUTTONHEIGHT)))
            {
                // duplicate our active selection
                Control.DuplicateConfig(Control.tempFileName);
                Reset();
            }
            GUI.backgroundColor = Loc.defaultColor;
        }

        /// <summary>
        /// Recursively generate the UI required to display the treeview structure that the template will generate
        /// </summary>
        /// <param name="entry">This entry</param>
        /// <param name="parent">name of the parent (if any)</param>
        /// <param name="canEdit">whether we are in edit mode or just view</param>
        /// /// <param name="offset">Horizontal offset for the entry</param>
        protected void CreateTreeViewUI(GenericHierarchyEntry entry, string parent, bool canEdit, float offset)
        {
            var hasChildren = false;
            if (entry.children != null)
            {
                if (entry.children.Count > 0)
                {
                    hasChildren = true;
                }
            }
            if( parent != string.Empty)
            {
                offset += HIERARCHYINDENTOFFSET;
            }
            
            // draw the entry
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(offset); // indent 
                if (hasChildren)
                {
                    GUILayout.Label(imgArrowDown, GUILayout.Width(SMALLBUTTONSIZE), GUILayout.Height(SMALLBUTTONSIZE));
                }
                else
                {
                    GUILayout.Label(imgArrowRight, GUILayout.Width(SMALLBUTTONSIZE), GUILayout.Height(SMALLBUTTONSIZE));
                }

                // draw the list entry like a label, but it's actually a button, make it bold if we are in edit mode & selected
                var style = EditorStyles.label;
                if( canEdit)
                {
                    // if we're selected, make us bold
                    if (Control.activeEntrySelection == entry)
                    {
                        style = EditorStyles.boldLabel;
                    }
                }

                if( GUILayout.Button(entry.name, style, GUILayout.ExpandWidth(true)))
                {
                    Control.SetActiveEntrySelection(entry);
                }

                if (canEdit)
                {
                    // add the appropriate edit controls
                    CreateEntryEditControls(entry);
                }
            }
            GUILayout.EndHorizontal();
            if (hasChildren)
            {
                parent = entry.name;
                // can't use a foreach because we remove entries, breaks enumeration
                for(var i = 0; i < entry.children.Count; i++)
                {
                   // Debug.Log("Creating child " + child.name + " with offset: " + offset);
                    CreateTreeViewUI(entry.children[i], parent, canEdit, offset);
                }
            }
        }

        /// <summary>
        /// Adds our '+ / -' new entry / remove entry edit buttons after an entry. If entry is null, we're adding a top-level element
        /// </summary>
        /// <param name="entry"></param>
        protected void CreateEntryEditControls( GenericHierarchyEntry entry)
        {
            // show our add / remove buttons for each entry (to add a child or remove the current entry)
            GUI.backgroundColor = Loc.doneColor;
            var add = new GUIContent(imgAdd, Loc.TOOLTIP_ADDCHILDELEMENT);
            if (GUILayout.Button(add, GUILayout.Width(SMALLBUTTONSIZE), GUILayout.Height(SMALLBUTTONSIZE)))
            {
                Control.ShowNewFolderEntryDialog(entry);
            }
            GUI.backgroundColor = Loc.cancelColor;
            var remove = new GUIContent(imgRemove, Loc.TOOLTIP_REMOVEELEMENT);
            if (GUILayout.Button(remove, GUILayout.Width(SMALLBUTTONSIZE), GUILayout.Height(SMALLBUTTONSIZE)))
            {
                var entryMessage = Loc.DIALOG_CONFIRMDELETEENTRY_MESSAGE + " : " + entry.name;
                if (EditorUtility.DisplayDialog(Loc.DIALOG_CONFIRMDELETE_TITLE,
                                        entryMessage,
                                        Loc.DIALOG_OK,
                                        Loc.DIALOG_CANCEL))
                {
                    Control.FindAndRemoveConfigEntry(entry);
                }
            }
            GUI.backgroundColor = Loc.defaultColor;
        }
        
        /// <summary>
        /// Generates the component editor for an individual entry - pick component, configure etc
        /// </summary>
        /// <param name="entry"></param>
        protected void CreateComponentEditor(GenericHierarchyEntry entry, bool canEdit)
        {
            switch( Control.activeConfig.type)
            {
                case TEMPLATE_TYPE.SCENE:
                    {
                        GUILayout.BeginVertical();
                        {                            
                            GUILayout.Space(10f);
                            if( entry.components.Count > 0)
                            {
                                GUILayout.Label("Component List:");
                            }
                                
                            for( var i = 0; i < entry.components.Count; i++)
                            {
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label("Type:", GUILayout.MaxWidth( 50f));
                                    var componentType = entry.components[i].type.ToString();
                                    GUILayout.Label(componentType, EditorStyles.helpBox, GUILayout.MaxWidth(150f));

                                    if (canEdit)
                                    {
                                        GUILayout.Space(SMALLBUTTONSIZE + 5);

                                        GUI.backgroundColor = Loc.cancelColor;
                                        var remove = new GUIContent(imgRemove, Loc.TOOLTIP_REMOVEELEMENT);
                                        if (GUILayout.Button(remove, GUILayout.Width(SMALLBUTTONSIZE), GUILayout.Height(SMALLBUTTONSIZE)))
                                        {
                                            var confirmDeleteComponentMessage = new StringBuilder();
                                            confirmDeleteComponentMessage.Append(Loc.DIALOG_CONFIRMDELETECOMPONENT_MESSAGE);
                                            confirmDeleteComponentMessage.Append(componentType);
                                            confirmDeleteComponentMessage.Append(" from entry: ");
                                            confirmDeleteComponentMessage.Append(entry.name);

                                            if (EditorUtility.DisplayDialog(Loc.DIALOG_CONFIRMDELETE_TITLE,
                                                                    confirmDeleteComponentMessage.ToString(),
                                                                    Loc.DIALOG_OK,
                                                                    Loc.DIALOG_CANCEL))
                                            {
                                                Control.RemoveComponentFromEntry(entry, entry.components[i].type);
                                                break;
                                            }
                                        }
                                        GUI.backgroundColor = Loc.defaultColor;
                                    }
                                }
                                GUILayout.EndHorizontal();

                                if (canEdit)
                                {
                                    if (Control.DoesComponentRequireSubassets(entry.components[i].type))
                                    {
                                        Control.entryCreateSubAsset = EditorGUILayout.Toggle("Create Sub-Asset?", Control.entryCreateSubAsset);
                                        if (Control.entryCreateSubAsset)
                                        {
                                            GUILayout.Label("Select template for sub asset");
                                        }
                                    }
                                }
                            }

                            if (canEdit)
                            {

                                GUILayout.Space(10f);

                                GUILayout.Label("Add new Component:", GUILayout.MaxWidth(STANDARDBUTTONSIZE));

                                // dropdown of supported component types
                                Control.entrySelectedComponent = EditorGUILayout.Popup(Control.entrySelectedComponent, Control.componentsForEntry.ToArray(), GUILayout.MaxWidth(STANDARDBUTTONSIZE));
                                if( Control.entrySelectedComponent > 0)
                                {
                                    GUI.backgroundColor = Loc.doneColor;
                                    if (GUILayout.Button("[ Add ]", GUILayout.MaxWidth(STANDARDBUTTONSIZE)))
                                    {
                                        // figure out what type this is, and add it to the list
                                        var type = Enum.Parse(typeof(COMPONENT_TYPE), Control.componentsForEntry[Control.entrySelectedComponent]);

                                        Control.AddComponentToEntry(entry, (COMPONENT_TYPE) type);
                                    }
                                    GUI.backgroundColor = Loc.defaultColor;
                                }   
                            }
                        }
                        GUILayout.EndVertical();
                        break;
                    }
            }
        }

        /// <summary>
        /// Draw our left side 'template list' view with an optional filter for the type of template to show
        /// </summary>
        /// <param name="filter">Only show TEMPLATE_TYPE templates in the list</param>
        protected void GenerateTemplateListView(CONTENTWINDOW windowType, TEMPLATE_TYPE filter, bool canEdit)
        {
            GUILayout.BeginVertical(GUILayout.MaxWidth(LEFTCOLUMNSIZE), GUILayout.MinWidth(LEFTCOLUMNSIZE));
            {
                GUILayout.Space(10f);
                GUILayout.Label(Loc.TEMPLATELIST, EditorStyles.boldLabel);

                var scrollHeight = curWindowSize.y - 115;
                if( canEdit)
                {
                    scrollHeight = curWindowSize.y - 165;
                }
                _templateScroll = EditorGUILayout.BeginScrollView(_templateScroll, false, true,
                    GUILayout.MinWidth(LEFTCOLUMNSIZE),
                    GUILayout.MaxWidth(LEFTCOLUMNSIZE - 50),
                    GUILayout.MinHeight(scrollHeight));
                {
                    GUILayout.Space(10f);
                    /*
                     *  Our default templates
                     */
                    GUILayout.Label(Loc.BUILTINTEMPLATES, EditorStyles.centeredGreyMiniLabel);
                    GUILayout.Space(10f);
                    foreach (var button in Control.defaultTemplateButtons)
                    {
                        if( filter != TEMPLATE_TYPE.ALL)
                        {
                            // if it's a match, draw it
                            if( button.type == filter)
                            {
                                CreateTemplateButton(button);
                            }
                        }
                        else
                        {
                            CreateTemplateButton(button);
                        }
                    }
                    GUILayout.Space(10f);

                    /*
                     *  Any available user templates in the project
                     */
                    GUILayout.Label(Loc.USERTEMPLATES, EditorStyles.centeredGreyMiniLabel);
                    GUILayout.Space(10f);
                    foreach (var button in Control.userTemplateButtons)
                    {
                        if (filter != TEMPLATE_TYPE.ALL)
                        {
                            if (button.type == filter)
                            {
                                CreateTemplateButton(button);
                            }
                        }
                        else
                        {
                            CreateTemplateButton(button);
                        }
                    }
                    GUILayout.Space(10f);
                }
                GUILayout.EndScrollView();
                GUILayout.Space(10f);

                if (canEdit)
                {
                    // our bottom buttons (below the template list) - users can add a new template or refresh the list (in case they edited / updated them outside of Unity)
                    GUILayout.BeginHorizontal();
                    {
                        var buttonWidth = 115f;

                        GUILayout.Space(10f);
                        var style = EditorStyles.miniButton;

                        var add = new GUIContent(Loc.ADD, null, Loc.TOOLTIP_ADDTEMPLATE);
                        if (GUILayout.Button(add, style, GUILayout.Width(buttonWidth), GUILayout.Height(STANDARDBUTTONHEIGHT)))
                        {
                            // if the user has changed the current config at all, we need to verify what they want us to do before refreshing the template liste
                            if (Control.activeConfigEditDirty)
                            {
                                var action = EditorUtility.DisplayDialogComplex(Loc.DIALOG_CONFIG_DIRTY_LOSECHANGES_TITLE,
                                                Loc.DIALOG_CONFIG_DIRTY_LOSECHANGES_MESSAGE,
                                                Loc.DIALOG_YES,
                                                Loc.DIALOG_NO,
                                                Loc.DIALOG_CANCEL);
                                switch (action)
                                {
                                    // save
                                    case 0:
                                        {
                                            Control.SaveConfigChanges();        // save changes
                                            Control.CreateNewConfig(this);      // create new config
                                            break;
                                        }
                                    // don't save
                                    case 1:
                                        {
                                            Reset();                            // undo changes
                                            Control.CreateNewConfig(this);      // create new config
                                            break;
                                        }
                                    // cancel 
                                    case 2:
                                        {
                                            break;
                                        }
                                }
                            }
                            else
                            {
                                Control.CreateNewConfig(this);
                            }
                        }

                        // refresh the template list
                        var refresh = new GUIContent(Loc.REFRESH, null, Loc.TOOLTIP_REFRESHTEMPLATE);
                        if (GUILayout.Button(refresh, style, GUILayout.Width(buttonWidth), GUILayout.Height(STANDARDBUTTONHEIGHT)))
                        {
                            // if the user has changed the current config at all, we need to verify what they want us to do before refreshing the template liste
                            if (Control.activeConfigEditDirty)
                            {
                                var action = EditorUtility.DisplayDialogComplex(Loc.DIALOG_CONFIG_DIRTY_LOSECHANGES_TITLE,
                                                Loc.DIALOG_CONFIG_DIRTY_LOSECHANGES_MESSAGE,
                                                Loc.DIALOG_YES,
                                                Loc.DIALOG_NO,
                                                Loc.DIALOG_CANCEL);
                                switch (action)
                                {
                                    // save
                                    case 0:
                                        {
                                            Debug.Log("FIXME: Save changes before refresh");
                                            break;
                                        }
                                    // don't save
                                    case 1:
                                        {
                                            Debug.Log("Discard Changes, refresh template list");
                                            Reset();
                                            break;
                                        }
                                    // cancel 
                                    case 2:
                                        {
                                            Debug.Log("Cancel, don't refresh");
                                            break;
                                        }
                                }
                            }
                            else
                            {
                                Reset();
                            }
                        }
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndVertical();
        }
    }
}