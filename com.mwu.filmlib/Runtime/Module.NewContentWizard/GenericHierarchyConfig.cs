using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MWU.FilmLib
{
    /// <summary>
    /// List of all of the supported Unity component types. 
    /// 
    /// FIXME: should be data driven so we can add user desired / custom types easily
    /// </summary>
    public enum COMPONENT_TYPE
    {
        NONE = 0,
        CAMERA,                 // = 1,
        AUDIO_LISTENER,         // = 2,
        PLAYABLE_DIRECTOR,      // = 3,     supports subasset
        CINEMACHINE_BRAIN,      // = 4,
        SCENE_VOLUME,           // = 5,     supports subasset
        POST_VOLUME,            // = 6,     supports subasset
        POST_LAYER,             // = 7,
        RENDER_SETTINGS         // = 8
    }

    public enum TEMPLATE_TYPE
    {
        ALL = -1,
        FOLDER = 0,
        SCENE,
        UNKNOWN = 1000,
    }

    public class SceneComponentType
    {
        public COMPONENT_TYPE type = COMPONENT_TYPE.NONE;         // what kind of component do we need to add to this object
        public bool requiresSubAsset = false;                       // do we need to generate an asset for this type?
    }


    /// <summary>
    /// An entry in our config. Scenes have componets & can create new if already exists, folders can add empty placeholder. The UI displays the appropriate elements as necessary
    /// </summary>
    public class GenericHierarchyEntry
    {
        public string name;
        /// <summary>
        /// In scene configs, whether we should ignore the fact that an element exists already and create an element already or just update the existing one (for example: Main Camera)
        /// </summary>
        public bool createNewIfAlreadyExists = false;
        /// <summary>
        /// whether or not we should place an empty placeholder file in the folder for version control
        /// </summary>
        public bool addEmptyPlaceholder = false;
        /// <summary>
        /// lists components that we can add to an entry once created (for scene configs)
        /// </summary>
        public List<SceneComponentType> components = new List<SceneComponentType>();
        /// <summary>
        /// A list of child entries for the current entry
        /// </summary>
        public List<GenericHierarchyEntry> children = new List<GenericHierarchyEntry>();
    }

    /// <summary>
    /// our generic config structure, used for both scenes & folders
    /// </summary>
    public class GenericHierarchyConfig
    {
        public string name;
        public string description;
        /// <summary>
        /// Defines which UI we display to edit / manage this config
        /// </summary>
        public TEMPLATE_TYPE type;
        /// <summary>
        /// The list of entries in the config
        /// </summary>
        public List<GenericHierarchyEntry> entries = new List<GenericHierarchyEntry>();
    }
}