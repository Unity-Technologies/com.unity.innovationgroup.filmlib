using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Loc = MWU.FilmLib.NewContentWizardLoc;                                   // string localization table
using Settings = MWU.FilmLib.NewContentWizardSettings;

namespace MWU.FilmLib
{
    public class TemplateEntry
    {
        public string displayName;
        public string fileName;
        public TEMPLATE_TYPE type;
        public string path;
    }

    /// <summary>
    /// Handles the low-level file mangling, json parsing etc
    /// </summary>
    public class NewContentWizardCore
    {
        public static List<TemplateEntry> defaultTemplates = new List<TemplateEntry>();
        public static List<TemplateEntry> userTemplates = new List<TemplateEntry>();

        public static void FindDefaultTemplates()
        {
            defaultTemplates.Clear();
            var configPath = Settings.TEMPLATECONFIGPATH;
            if( Directory.Exists(configPath))
            {
                var info = new DirectoryInfo(configPath);
                var fileInfo = info.GetFiles();
                foreach (var file in fileInfo)
                {
                    if (file.Extension != ".meta")
                    {
                        var entry = new TemplateEntry()
                        {
                            displayName = file.Name,
                            fileName = Path.GetFileNameWithoutExtension(file.FullName),
                            path = file.FullName,
                            type = TEMPLATE_TYPE.UNKNOWN,           // we haven't actually loaded the config at this point, so we don't know what type it is
                        };
                        defaultTemplates.Add(entry);
                    }
                }
            }
            else
            {
                Debug.Log("Package Error: Could not find default template folder?");
            }
        }

        public static void FindUserTemplates()
        {
            userTemplates.Clear();
            var configPath = Settings.USERTEMPLATECONFIGPATH;
            if( Directory.Exists(configPath))
            {
                var info = new DirectoryInfo(configPath);
                var fileInfo = info.GetFiles();
                foreach (var file in fileInfo)
                {
                    if (file.Extension != ".meta")
                    {
                        var entry = new TemplateEntry()
                        {
                            displayName = file.Name,
                            fileName = Path.GetFileNameWithoutExtension(file.FullName),
                            path = file.FullName,
                            type = TEMPLATE_TYPE.UNKNOWN,           // we haven't actually loaded the config at this point, so we don't know what type it is
                        };
                        userTemplates.Add(entry);
                    }
                }
            }
            else
            {
                CreateFolderStructure(configPath, "Assets");
            }
        }

        /// <summary>
        /// from a given parent folder, generate a hierarchy of folders. Will detect if the first folder in the list matches the parent folder
        /// </summary>
        /// <param name="inputPath">for example: Assets/Settings/Templates </param>
        /// <param name="parentFolder">for example: Assets</param>
        public static void CreateFolderStructure(string inputPath, string parentFolder)
        {
            Debug.Log("Creating folder structure: " + inputPath + " in parent folder: " + parentFolder);
            var folders = inputPath.Split('/');
            foreach (var folder in folders)
            {
                if (folder != parentFolder)
                {
                    if (!Directory.Exists(folder))
                    {
                        Debug.Log("Create folder: " + folder + " parentFolder: " + parentFolder);
                        AssetDatabase.CreateFolder(parentFolder, folder);
                    }
                    parentFolder = parentFolder + "/" + folder;
                }
            }
        }

        public static GenericHierarchyConfig LoadGenericConfig( string configPath)
        {
            var config = new GenericHierarchyConfig();

            if( File.Exists( configPath))
            {
                var json = File.ReadAllText(configPath);
                config = JsonConvert.DeserializeObject<GenericHierarchyConfig>(json);
            }
            return config;
        }

        public static string ConfigToString( GenericHierarchyConfig config)
        {
            return JsonConvert.SerializeObject(config, Formatting.Indented);
        }
        
        public static GenericHierarchyConfig LoadConfig( string configPath)
        {
            var newConfig = new GenericHierarchyConfig();
            if (File.Exists(configPath))
            {
                var sceneJson = File.ReadAllText(configPath);
                newConfig = JsonConvert.DeserializeObject<GenericHierarchyConfig>(sceneJson);
            }
            return newConfig;
        }

        public static void SaveConfig( string newFileName, GenericHierarchyConfig config)
        {
            var configTxt = ConfigToString(config);
            WriteConfigToDisk(configTxt, newFileName);
        }
        
        public static void DeleteConfig(string path)
        {
            Debug.Log("Attempting to delete file: " + path);
            if( File.Exists(path))
            {
                File.Delete(path);
                File.Delete(path + ".meta");
                AssetDatabase.Refresh();
                FindUserTemplates();
            }
        }

        // just write an empty .md placeholder (used for generating folder structures that play nice with version control)
        public static void WriteEmptyFile(string path)
        {
            var emptyFile = path + "/Placeholder.md";
            var placeholderText = Loc.EMPTYFILE_PLACEHOLDER;
            // create file & write the config
            using (StreamWriter sw = File.CreateText(emptyFile))
            {
                sw.WriteLine(placeholderText);
            }

            AssetDatabase.Refresh();
        }

        public static void WriteConfigToDisk(string configTxt, string newFileName)
        {
            var configFileName = Settings.USERTEMPLATECONFIGPATH + "/" + newFileName + ".json";
            Debug.Log("Saving template to: " + configFileName);
            if (File.Exists(configFileName))
            {
                // check if we should overwrite the file?
                if (EditorUtility.DisplayDialog(Loc.DIALOG_CONFIRMOVERWRITEEXISTING_TITLE,
                                            Loc.DIALOG_CONFIRMOVERWRITEEXISTING_MESSAGE,
                                            Loc.DIALOG_OK,
                                            Loc.DIALOG_CANCEL))
                {
                    File.Delete(configFileName);
                }
            }

            // create file & write the config
            using (StreamWriter sw = File.CreateText(configFileName))
            {
                sw.WriteLine(configTxt);
            }

            AssetDatabase.Refresh();
            FindUserTemplates();
        }
    }
}