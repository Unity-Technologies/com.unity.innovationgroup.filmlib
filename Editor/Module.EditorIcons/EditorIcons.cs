using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MWU.FilmLib
{
    /// <summary>
    /// Standard icon library - simply call 'EditorIcons.GetIcon("name")' to retrieve one of the standard icons in the set
    /// 
    /// Icons from:  https://material.io/tools/icons/?icon=edit&style=baseline
    /// </summary>
    public class EditorIcons
    {
        public const string CONTENTPATH = EditorUtilities.packagePathRoot + "/Editor/Module.EditorIcons/Content/";

        // assets for the template view listing
        private static bool assetLoadComplete = false;
        protected static Dictionary<string, Texture2D> icons = new Dictionary<string, Texture2D>();

        public static Texture GetIcon(string name)
        {
            if (!assetLoadComplete)
                LoadAssets();

            if( icons.ContainsKey(name))
            {
                return icons[name];
            }
            return null;
        }

        /// <summary>
        /// Load the assets (icons etc) that we need
        /// </summary>
        protected static void LoadAssets()
        {
            icons.Clear();
            icons = new Dictionary<string, Texture2D>()
            {
                {  "ArrowLeft", LoadAsset("White/Icons/arrow_left.png") },
                {  "ArrowRight", LoadAsset("White/Icons/arrow_right.png") },
                {  "Add", LoadAsset("White/Icons/add.png") },
                {  "Remove", LoadAsset("White/Icons/remove.png") },
                {  "Refresh", LoadAsset("White/Icons/refresh.png") },
                {  "Clear", LoadAsset("White/Icons/clear.png") },
                {  "Edit", LoadAsset("White/Icons/edit.png") },
                {  "Done", LoadAsset("White/Icons/done.png") },
                {  "Create", LoadAsset("White/Icons/create.png") },
                {  "Settings", LoadAsset("White/Icons/settings.png") },
                {  "Save", LoadAsset("White/Icons/save_alt.png") },
                {  "AnimationTrack", LoadAsset("White/Icons/play_arrow.png") },
                {  "AudioTrack", LoadAsset("White/Icons/audiotrack.png") },
                {  "CinemachineTrack", LoadAsset("White/Icons/local_see.png") },
                {  "ControlTrack", LoadAsset("White/Icons/subdirectory_arrow_right.png") },
                {  "GroupTrack", LoadAsset("White/Icons/folder.png") },
                {  "ActivationTrack", LoadAsset("White/Icons/fiber_manual_record.png") },
                {  "Unknown", LoadAsset("White/Icons/warning.png") },

            };
            assetLoadComplete = true;
        }

        /// <summary>
        /// Internal individual asset loading
        /// </summary>
        private static Texture2D LoadAsset(string path)
        {
            return AssetDatabase.LoadAssetAtPath<Texture2D>(CONTENTPATH + path);
        }

    }
}