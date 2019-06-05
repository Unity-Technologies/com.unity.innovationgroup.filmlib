using UnityEngine;
using System.IO;

namespace MWU.FilmLib.Extensions
{
    /// <summary>
    /// Our standard library of Unity Extension / Helper Methods
    /// </summary>
    public static class UnityExtension
    {
        public static void SetPositionX(this Transform t, float newX)
        {
            t.position = new Vector3(newX, t.position.y, t.position.z);
        }

        public static void SetPositionY(this Transform t, float newY)
        {
            t.position = new Vector3(t.position.x, newY, t.position.z);
        }

        public static void SetPositionZ(this Transform t, float newZ)
        {
            t.position = new Vector3(t.position.x, t.position.y, newZ);
        }

        public static float GetPositionX(this Transform t)
        {
            return t.position.x;
        }

        public static float GetPositionY(this Transform t)
        {
            return t.position.y;
        }

        
        public static GameObject LoadFromResources(this GameObject parent, string assetLocation, string layerName = "")
        {
            if (assetLocation != "" && assetLocation != null)
            {
                var loadedObject = Resources.Load(assetLocation) as GameObject;

                if (loadedObject != null)
                {
                    var instantiated = GameObject.Instantiate(loadedObject) as GameObject;
                    instantiated.name = assetLocation;
                    instantiated.transform.SetParent(parent.transform, false);
                    if (layerName != "")
                    {
                        instantiated.layer = LayerMask.NameToLayer(layerName);
                    }
                    return instantiated;
                }
                else
                {
                    Debug.LogError("Failed to load " + assetLocation);
                }
            }
            return null;
        }

        public static GameObject LoadFromResources(this GameObject parent, string assetLocation, Vector3 position, string layerName = "")
        {
            var loadedObject = parent.LoadFromResources(assetLocation);
            if (loadedObject != null)
            {
                loadedObject.transform.position = position;
            }
            return loadedObject;
        }

        public static GameObject LoadFromResources(this Transform parent, string assetLocation, string layerName = "")
        {
            return parent.gameObject.LoadFromResources(assetLocation, layerName);
        }

        public static GameObject LoadFromResources(this Transform parent, string assetLocation, Vector3 position, string layerName = "")
        {
            return parent.gameObject.LoadFromResources(assetLocation, position, layerName);
        }

        /// <summary>
        /// Instead of using [RequireComponent], we can use this method to check if a component is available
        /// </summary>
        /// <typeparam name="T">Component type we are looking for</typeparam>
        /// <param name="obj">Game Object that should have the component</param>
        /// <returns></returns>
        public static T GetSafeComponent<T>(this GameObject obj) where T : MonoBehaviour
        {
            T component = obj.GetComponent<T>();

            if (component == null)
            {
                Debug.LogError("Expected to find component of type "
                   + typeof(T) + " but found none", obj);
            }
            return component;
        }
    }

    static public class MethodExtensionForMonoBehaviourTransform
    {
        /// <summary>
        /// Gets or add a component. Usage example:
        /// BoxCollider boxCollider = transform.GetOrAddComponent<BoxCollider>();
        /// </summary>
        static public T GetOrAddComponent<T>(this Component parent) where T : Component
        {
            if (parent == null)
            {
                Debug.LogError("Failed to get or add a component due to a null parent!");
                return null;
            }

            T result = parent.GetComponent<T>();
            if (result == null)
            {
                result = parent.gameObject.AddComponent<T>();
            }
            return result;
        }

        static public T GetOrAddComponent<T>(this GameObject parent) where T : Component
        {
            if (parent == null)
            {
                Debug.LogError("Failed to get or add a component due to a null parent!");
                return null;
            }
            T result = parent.GetComponent<T>();
            if (result == null)
            {
                result = parent.AddComponent<T>();
            }
            return result;
        }

        static public T GetOrAddComponent<T>(this Transform parent) where T : Component
        {
            if (parent == null)
            {
                Debug.LogError("Failed to get or add a component due to a null parent!");
                return null;
            }
            T result = parent.GetComponent<T>();
            if (result == null)
            {
                result = parent.gameObject.AddComponent<T>();
            }
            return result;
        }
    }

}