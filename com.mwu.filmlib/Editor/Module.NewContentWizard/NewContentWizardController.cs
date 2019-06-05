using Cinemachine;
using MWU.FilmLib.Extensions;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering.PostProcessing;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Timeline;
using Core = MWU.FilmLib.NewContentWizardCore;
using Settings = MWU.FilmLib.NewContentWizardSettings;
using Loc = MWU.FilmLib.NewContentWizardLoc;                                   // string localization table
using System;

namespace MWU.FilmLib
{
    public class TemplateButtonEntry
    {
        public GenericHierarchyConfig config;
        public string displayName;                  // display name
        public string description;                  // description (also available in 'config')
        public string fileName;                     // the raw file name
        public string path;                         // where the template exists on disk
        public TEMPLATE_TYPE type;                  // scene / folder
        public bool isPressed;                      // is the button pressed
        public bool isBuiltIn;                      // is this a built in config from the package or a user one?
    }

    /// <summary>
    /// This controller is shared across the NewContentWizard, TemplateEditor, 
    /// handles the core edit functionality for the wizards. 
    /// 
    /// File management is handled by NewContentWizardCore.
    /// </summary>
    public class NewContentWizardController
    {
        /// <summary>
        /// Our currently selected Config (which button is pushed currently). Changes to a template in the editor are applied to the 'activeConfig', not this
        /// </summary>
        public static TemplateButtonEntry activeSelection;
        public static string tempFileName = string.Empty;
        public static List<TemplateButtonEntry> defaultTemplateButtons = new List<TemplateButtonEntry>();
        public static List<TemplateButtonEntry> userTemplateButtons = new List<TemplateButtonEntry>();

        /// <summary>
        /// the config we loaded from disk, active changes to the template are applied to this
        /// </summary>
        public static GenericHierarchyConfig activeConfig;

        /// <summary>
        ///  for the content editor - the entry within the config that we're currently editing
        /// </summary>
        public static GenericHierarchyEntry activeEntrySelection = null;
        
        /// <summary>
        /// states to indicate whether the entry config / template has been modified and needs to be saved
        /// </summary>
        public static bool activeConfigEditDirty = false;
        public static bool configNameDirty = false;                                 // user changed the config's name?
        public static bool configDescriptionDirty = false;                          // user changed the description
        public static bool entryEditDirty = false;                                  // whether the entry name has changed inline

        // temp variables to store user changes
        public static string tempConfigNameString = string.Empty;                   // temp name 
        public static string tempConfigDescriptionString = string.Empty;            // temp description
        public static string tempEntrySelectionString = string.Empty;               // the temp name that the user has typed
        
        // sub entry component editing
        public static bool entryCreateSubAsset = false;                             
        public static List<string> supportedComponentTypes = new List<string>();    // full list of component types that can be added to an entry
        public static List<string> componentsForEntry = new List<string>();         // the sub list for the selected entry 
        public static int entrySelectedComponent = 0;                               // currently selected component from the list

        // do a reset of things
        public static void Reset()
        {
            InitComponentTypes();
            ClearSelection();
            Core.FindDefaultTemplates();
            Core.FindUserTemplates();
        }

        public static void InitComponentTypes()
        {
            supportedComponentTypes = new List<string>();

            var types = Enum.GetValues(typeof(COMPONENT_TYPE));

            foreach (var type in types)
            {
                supportedComponentTypes.Add(type.ToString());
            }
        }

        /// <summary>
        /// generates a new list of available components for the current selected entry
        /// </summary>
        /// <param name="entry"></param>
        public static void UpdateComponentsForEntry(GenericHierarchyEntry entry)
        {
            // copy our full list (deep copy)
            componentsForEntry.Clear();
            foreach( var src in supportedComponentTypes)
            {
                componentsForEntry.Add(src);
            }
            // and remove any entries that are already added on the entry
            foreach( var comp in entry.components)
            {
                if(componentsForEntry.Contains( comp.type.ToString()))
                {
                    componentsForEntry.Remove(comp.type.ToString());
                }
            }
        }

        /// <summary>
        /// We've switched to a different config, let's reset and load that config
        /// </summary>
        /// <param name="newSelection"></param>
        public static void SetActiveSelection(TemplateButtonEntry newSelection)
        {
            ClearSelection();
            activeSelection = newSelection;
            // reset template file name for copies
            tempFileName = activeSelection.displayName + " Copy";

            // load the active config
            activeConfig = Core.LoadConfig(activeSelection.path);
            // cache these, they are used in the template editor
            tempConfigNameString = activeConfig.name;
            tempConfigDescriptionString = activeConfig.description;
        }

        /// <summary>
        /// resets our selection completely
        /// </summary>
        public static void ClearSelection()
        {
            SetActiveConfigDirty(false);
            activeSelection = null;
            activeEntrySelection = null;
            activeConfig = null;
            configNameDirty = false;
            configDescriptionDirty = false;
            tempConfigDescriptionString = string.Empty;
            tempConfigNameString = string.Empty;            
        }

        /// <summary>
        /// Set which entry in our folder list we're currently editing
        /// </summary>
        /// <param name="entry"></param>
        public static void SetActiveEntrySelection( GenericHierarchyEntry entry)
        {
            // reset our 'dirty' state for the other elements
            entryEditDirty = false;

            activeEntrySelection = entry;
            tempEntrySelectionString = entry.name;          // cache the entry name for the editor UI
            UpdateComponentsForEntry(activeEntrySelection);
        }

        /// <summary>
        /// Simple check to see if the component type uses a subasset
        /// </summary>
        public static bool DoesComponentRequireSubassets(COMPONENT_TYPE type)
        {
            switch (type)
            {
                case COMPONENT_TYPE.PLAYABLE_DIRECTOR:
                case COMPONENT_TYPE.POST_VOLUME:
                case COMPONENT_TYPE.SCENE_VOLUME:
                    {
                        return true;
                    }
            }
            return false;
        }

        /// <summary>
        /// Add a new component to an entry in a template
        /// </summary>
        public static void AddComponentToEntry(GenericHierarchyEntry entry, COMPONENT_TYPE type)
        {
            if( entry != null)
            {
                var newComponent = new SceneComponentType();
                newComponent.type = type;
                newComponent.requiresSubAsset = DoesComponentRequireSubassets(type);
                entry.components.Add(newComponent);
                SetActiveConfigDirty(true);
            }
        }

        /// <summary>
        /// Remove a component from an entry in a template
        /// </summary>
        public static void RemoveComponentFromEntry(GenericHierarchyEntry entry, COMPONENT_TYPE type)
        {
            if( entry != null)
            {
                for( var i = 0; i < entry.components.Count; i++)
                {
                    if( entry.components[i].type == type)
                    {
                        entry.components.RemoveAt(i);
                        SetActiveConfigDirty(true);
                    }
                }
            }
        }


        /// <summary>
        /// The popup dialog that prompts for a name for the new Folder Entry
        /// </summary>
        /// <param name="entry">Optional parent</param>
        public static void ShowNewFolderEntryDialog(GenericHierarchyEntry entry)
        {
            // show dialog to get the desired name of the new entry
            var dialog = EditorWindow.CreateInstance<DialogGetNewEntryName>();
            {
                dialog.entry = entry;
                dialog.dialogTitle = Loc.DIALOG_ADDNEWENTRY_TITLE;
                dialog.dialogMessage = Loc.DIALOG_ADDNEWENTRY_MESSAGE;

                // make sure the dialog is centered
                var dialogPosition = dialog.position;
                dialogPosition.center = new Rect(0f, 0f, Screen.currentResolution.width, Screen.currentResolution.height).center;
                dialog.position = dialogPosition;
                dialog.maxSize = new Vector2(250f, 110f);
                dialog.minSize = new Vector2(250f, 110f);
            }
            dialog.ShowUtility();
        }

        public static void CreateNewConfig(NewContentWizardSharedView opener)
        {
            // show dialog to get the desired name of the new entry
            var dialog = EditorWindow.CreateInstance<DialogCreateNewTemplate>();
            dialog.opener = opener;
            dialog.ShowUtility();
        }

        /// <summary>
        /// Create a new entry in our folder list. If parentEntry is null, adds it as a new top-level folder
        /// </summary>
        /// <param name="parentEntry"></param>
        /// <param name="name"></param>
        public static void AddFolderEntry(GenericHierarchyEntry parentEntry, string name)
        {
            //  Debug.Log("Add new folder entry to parent: " + parentEntry.name + " name: " + name);
            var newEntry = new GenericHierarchyEntry
            {
                name = name
            };
            activeConfigEditDirty = true;

            // if we have a parent, add our new entry as a child
            if( parentEntry != null)
            {
                parentEntry.children.Add(newEntry);
            }
            else
            {
                // otherwise add it to the top level
                activeConfig.entries.Add(newEntry);
            }
            
            activeEntrySelection = null;
        }

        public static void FindAndRemoveConfigEntry(GenericHierarchyEntry thisEntry)
        {
            RemoveConfigEntry(thisEntry, activeConfig.entries);
        }

        /// <summary>
        /// Recursive find & remove
        /// </summary>
        /// <param name="thisEntry"></param>
        /// <param name="entries"></param>
        public static void RemoveConfigEntry(GenericHierarchyEntry thisEntry, List<GenericHierarchyEntry> entries)
        {
            if (entries.Contains(thisEntry))
            {
                //   Debug.Log("Found entry, removing");
                activeConfigEditDirty = true;
                entries.Remove(thisEntry);
                return;
            }
            else
            {
                for (var i = 0; i < entries.Count; i++)
                {
                    if( entries[i].children.Count > 0)
                    {
                //        Debug.Log("searching entry children : " + entries[i].name);
                        RemoveConfigEntry(thisEntry, entries[i].children);
                    }
                }
            }
        }

        /// <summary>
        /// Mark the currently active config as dirty (has been changed), so we can prompt the user to save as needed 
        /// </summary>
        /// <param name="state"></param>
        public static void SetActiveConfigDirty(bool state)
        {
            activeConfigEditDirty = state;
        }

        /// <summary>
        /// apply the changes to the current edited entry in our config entry list
        /// </summary>
        /// <param name="entry"></param>
        public static void SaveChangeToEntry(string newName)
        {
            activeConfigEditDirty = true;
            activeEntrySelection.name = newName;
        }

        public static void SaveChangeToConfigName()
        {
            configNameDirty = false;
            activeConfig.name = tempConfigNameString;
            SetActiveConfigDirty(true);
        }

        public static void SaveChangeToConfigDescription()
        {
            configDescriptionDirty = false;
            activeConfig.description = tempConfigDescriptionString;
            SetActiveConfigDirty(true);
        }

        /// <summary>
        ///  this is called every frame by the gui system, so we need to check if the button should be pressed, and 
        ///  if user is switching to a new config, confirm if they wanted to 
        /// </summary>
        public static bool ConfirmSwitchActiveButton( TemplateButtonEntry button)
        {
            // if this button is the same as our current selection then yes it should be pressed
            if (activeSelection == button)
                return true;

            // if user has selected another config, prompt them to confirm that they actually want to switch
            if (activeConfigEditDirty)
            {
                if (EditorUtility.DisplayDialog(Loc.DIALOG_CONFIG_DIRTY_LOSECHANGES_TITLE,
                                            Loc.DIALOG_CONFIG_DIRTY_LOSECHANGES_MESSAGE,
                                            Loc.DIALOG_OK,
                                            Loc.DIALOG_CANCEL))
                {
                    // save changes
                    SaveConfigChanges();
                    // and switch to the new selection
                    SwitchActiveButton_Internal(button);
                    return true;
                }
                else
                {
                    // cancel, don't save / don't change
                    return false;
                }
            }
            else
            {
                // just switch to the new config
                SwitchActiveButton_Internal(button);
                return true;
            }
        }

        /// <summary>
        /// Internal call, actually switch active config
        /// </summary>
        /// <param name="button"></param>
        private static void SwitchActiveButton_Internal( TemplateButtonEntry button)
        {
            tempFileName = activeSelection + " copy";
            ClearOtherButtons(button);
            SetActiveSelection(button);
        }

        /// <summary>
        /// make a radio button toggle out of our list of buttons
        /// </summary>
        /// <param name="activeButton"></param>
        public static void ClearOtherButtons(TemplateButtonEntry activeButton)
        {
            foreach (var button in defaultTemplateButtons)
            {
                if (button != activeButton)
                {
                    button.isPressed = false;
                }
            }
            foreach (var button in userTemplateButtons)
            {
                if (button != activeButton)
                {
                    button.isPressed = false;
                }
            }
        }

        /// <summary>
        /// Load up our folder config & return it
        /// </summary>
        public static GenericHierarchyConfig LoadFolderConfig()
        {
            var configPath = Settings.TEMPLATECONFIGPATH + "FolderConfig.json";
            return LoadConfigFromPath(configPath);
        }

        /// <summary>
        /// makes a duplicate of the active selected config and saves it to the user templates folder with the new filename
        /// </summary>
        /// <param name="path"></param>
        public static void DuplicateConfig(string newFileName)
        {
            var config = LoadConfigFromPath(activeSelection.path);

            Core.SaveConfig(newFileName, config);
            ClearSelection();
        }

        public static void SaveConfigChanges()
        {
            Debug.Log("Saving config changes to: " + activeSelection.path);
            
            Core.SaveConfig(activeSelection.fileName, activeConfig);
            ClearSelection();
        }

        /// <summary>
        /// Given a path to a config, loads the folder config and returns it
        /// </summary>
        /// <param name="configPath"></param>
        /// <returns></returns>
        public static GenericHierarchyConfig LoadConfigFromPath(string configPath)
        {
            var folderConfig = new GenericHierarchyConfig();
            folderConfig = Core.LoadConfig(configPath);
            return folderConfig;
        }

        /// <summary>
        /// Main entry point to generate a folder structure, based on a given config
        /// </summary>
        /// <param name="configPath"></param>
        public static void PopulateFolderStructure(string configPath)
        {
            var folderConfig = LoadConfigFromPath(configPath);
            CreateFolderStructure(folderConfig);
        }

        /// <summary>
        /// Main entry point to generate a scene structure, based on a given config
        /// </summary>
        public static void PopulateSceneStructure(string configPath)
        {
            var sceneConfig = LoadConfigFromPath(configPath);
            foreach (var entry in sceneConfig.entries)
            {
                CreateEntry(entry, null);
            }
        }

        /// <summary>
        /// do general project setup, make sure we're in linear space, create our toolbar config and switch to the proper editor layout
        /// </summary>
        public static void ConfigureProject()
        {
            Debug.Log("Configure project");

            // convert to Linear color space
            if(PlayerSettings.colorSpace == ColorSpace.Gamma)
            {
                PlayerSettings.colorSpace = ColorSpace.Linear;
            }

            // create our toolbar config
            EditorToolbarController.CreateToolbarConfig();
            // and switch to film layout
            LayoutLoader.LoadFilmLayout();
        }
        
        private static void CreateFolderStructure(GenericHierarchyConfig folderConfig)
        {
            foreach (var item in folderConfig.entries)
            {
                CreateFolder(item, "Assets");
            }
        }

        private static void CreateFolder( GenericHierarchyEntry entry, string parent)
        {
            var newPath = string.Empty;         // save the path in case we need to generate a placeholder file
            var created = false;
            if (!AssetDatabase.IsValidFolder(entry.name))
            {
                var guid = AssetDatabase.CreateFolder(parent, entry.name);
                newPath = AssetDatabase.GUIDToAssetPath(guid);
                Debug.Log("Created folder: " + newPath);
                created = true;
            }
            else
            {
                Debug.Log("Folder exists: " + entry.name);
                created = false;
            }
            
            if (entry.addEmptyPlaceholder && created)
            {
                Core.WriteEmptyFile(newPath);
            }

            if (entry.children != null)
            {
                foreach (var child in entry.children)
                {
                    CreateFolder((GenericHierarchyEntry) child, parent + "/" + entry.name);
                }
            }
        }

        /// <summary>
        /// Create an entry from the sceneConfig. Can be called recursively for children
        /// </summary>
        /// <param name="entry"></param>
        private static void CreateEntry(GenericHierarchyEntry entry, Transform parent)
        {
            GameObject go = null;

            // find any existing objects with the same name if we're not supposed
            // to make new ones (Main Camera for example)
            if (!entry.createNewIfAlreadyExists)
            {
                go = EditorUtilities.GetSceneObject(entry.name);
            }

            if (go == null)
            {
                go = new GameObject();
            }

            if (parent != null)
            {
                go.transform.parent = parent;
            }

            go.name = entry.name;

            // add any components that we need to the new gameobject
            if (entry.components != null)
            {
                foreach (var comp in entry.components)
                {
                    AddNewComponent(go, comp.type);
                }
            }

            // if there are children that we need to add to this gameobject, call us recursively as needed
            if (entry.children != null)
            {
                foreach (var child in entry.children)
                {
                    CreateEntry(child, go.transform);
                }
            }
        }

        /// <summary>
        /// Manages the adding of components to generated scene objects. Supports generating sub-assets for those components that need them
        /// </summary>
        private static void AddNewComponent(GameObject go, COMPONENT_TYPE type)
        {
            switch (type)
            {
                case COMPONENT_TYPE.CAMERA:
                    {
                        var cam = go.GetOrAddComponent<Camera>();
                        break;
                    }
                case COMPONENT_TYPE.AUDIO_LISTENER:
                    {
                        go.GetOrAddComponent<AudioListener>();
                        break;
                    }
                case COMPONENT_TYPE.CINEMACHINE_BRAIN:
                    {
                        go.GetOrAddComponent<CinemachineBrain>();
                        break;
                    }
                case COMPONENT_TYPE.PLAYABLE_DIRECTOR:
                    {
                        var pd = go.GetOrAddComponent<PlayableDirector>();
                        if( !AssetDatabase.IsValidFolder("Assets/Timeline"))
                        {
                            AssetDatabase.CreateFolder("Assets", "Timeline");
                        }
                        var ta = ScriptableObjectUtility.CreateAssetType<TimelineAsset>("Assets/Timeline", "MasterTimeline.asset");
                        pd.playableAsset = ta;
                        break;
                    }
                case COMPONENT_TYPE.POST_LAYER:
                    {
                        var post = go.GetOrAddComponent<PostProcessLayer>();
                        var postLayer = 1 << 8;         // post layer is 8 by default
                        post.volumeLayer = postLayer; // LayerMask.NameToLayer("PostProcessing"); <= this doesn't work for some reason
                        post.antialiasingMode = PostProcessLayer.Antialiasing.TemporalAntialiasing;
                        break;
                    }
                case COMPONENT_TYPE.POST_VOLUME:
                    {
                        var post = go.GetOrAddComponent<PostProcessVolume>();
                        post.isGlobal = true;
                        var targetName = go.name;
                        var scene = go.scene;

                        // create a new profile
                        var asset = ProfileFactory.CreatePostProcessProfile(scene, scene.name);

                        // find & load the template
                        // FIXME: should allow for user templates as well
                        var templatePath = Settings.TEMPLATECONFIGPATH + "Default_Profiles/Default_PostProfile.asset";
                        var template = AssetDatabase.LoadAssetAtPath(templatePath, typeof(PostProcessProfile)) as PostProcessProfile;

                        if( template != null)
                        {
                            // add all of the settings to the template
                            foreach (var effect in template.settings)
                            {
                                asset.AddSettings(effect);
                            }
                        }
                        else
                        {
                            Debug.Log("Could not find template post profile?");
                        }
                        
                        post.profile = asset;
                        post.isGlobal = true;
                        post.gameObject.layer = LayerMask.NameToLayer("PostProcessing");
                        break;
                    }
                case COMPONENT_TYPE.SCENE_VOLUME:
                    {
#if USING_HDRP
                        var vol = go.GetOrAddComponent<Volume>();
                        vol.isGlobal = true;
                        var targetName = go.name;
                        var scene = go.scene;

                        // FIXME: should load a volume profile from a template & 
                        // copy the components from one in the package as a base / starting point
                        var asset = VolumeProfileFactory.CreateVolumeProfile(scene, targetName);
                        
                        vol.profile = asset;
                        vol.isGlobal = true;
#endif
                        break;
                    }
                case COMPONENT_TYPE.RENDER_SETTINGS:
                    {
                        var settings = go.GetOrAddComponent<RenderSettings>();
                        // add our basic low / high detail levels to start
                        var levels = new List<DetailLevel>()
                        {
                            new DetailLevel()
                            {
                                name = "Low",
                                reflectionProbes = false,
                                planarReflectionProbes = false,
                            },
                            new DetailLevel()
                            {
                                name = "High",
                                reflectionProbes = true,
                                planarReflectionProbes = true,
                            }
                        };

                        settings.detailLevels.AddRange(levels);
                        break;
                    }
                default:
                    {
                        Debug.Log("unrecognized component type");
                        break;
                    }
            }
        }
    }
}
