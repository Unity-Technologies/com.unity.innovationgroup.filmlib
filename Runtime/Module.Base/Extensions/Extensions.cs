using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;

namespace MWU.FilmLib.Extensions
{
    /// <summary>
    /// Our standard library of Extension / Helper Methods
    /// </summary>
    public static class Extension
    {
        public static void CopyFiles(string source, string target, bool overwrite = true)
        {
            CopyFiles(new DirectoryInfo(source), new DirectoryInfo(target), overwrite);
        }

        public static void CopyFiles(DirectoryInfo source, DirectoryInfo target, bool overwrite = true)
        {
            if (source.FullName.ToLower() == target.FullName.ToLower())
            {
                return;
            }

            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into it's new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.ToString(), fi.Name), overwrite);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                CopyFiles(diSourceSubDir, nextTargetSubDir);
            }
        }
    }

    //http://stackoverflow.com/a/2679857
    public static class DictionaryExtensions
    {
        // Works in C#3/VS2008:
        // Returns a new dictionary of this ... others merged leftward.
        // Keeps the type of 'this', which must be default-instantiable.
        // Example: 
        //   result = map.MergeLeft(other1, other2, ...)
        public static T MergeLeft<T, K, V>(this T me, params IDictionary<K, V>[] others)
            where T : IDictionary<K, V>, new()
        {
            T newMap = new T();
            foreach (IDictionary<K, V> src in
                (new List<IDictionary<K, V>> { me }).Concat(others))
            {
                // ^-- echk. Not quite there type-system.
                foreach (KeyValuePair<K, V> p in src)
                {
                    newMap[p.Key] = p.Value;
                }
            }
            return newMap;
        }

        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            TValue foundValue;
            dictionary.TryGetValue(key, out foundValue);
            if (foundValue == null)
            {
                foundValue = defaultValue;
                dictionary.Add(key, defaultValue);
            }
            return foundValue;
        }
    }
}