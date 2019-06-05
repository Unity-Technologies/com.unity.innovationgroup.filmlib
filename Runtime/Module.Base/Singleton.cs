using UnityEngine;
/*
 * Borrowed from http://wiki.unity3d.com/index.php/Singleton
 */
namespace MWU.Shared.Base
{

    /// <summary>
    /// Be aware this will not prevent a non singleton constructor
    ///   such as `T myT = new T();`
    /// To prevent that, add `protected T () {}` to your singleton class.
    /// 
    /// As a note, this is made as MonoBehaviour because we need Coroutines.
    /// </summary>
    public class Singleton<T> : BaseMonoBehaviour where T : BaseMonoBehaviour
    {
        private static T _instance;

        private static object _lock = new object();

        public static T Instance
        {
            get
            {
                if ( applicationIsQuitting )
                {
                    return null;
                }

                lock ( _lock )
                {
                    if ( _instance == null )
                    {
                        _instance = (T) FindObjectOfType(typeof(T));

                        if ( FindObjectsOfType(typeof(T)).Length > 1 )
                        {
                            return _instance;
                        }

                        if ( _instance == null )
                        {
                            GameObject singleton = new GameObject();
                            _instance = singleton.AddComponent<T>();
                            singleton.name = "(singleton) " + typeof(T).ToString();

                            DontDestroyOnLoad(singleton);
                        }
                    }

                    return _instance;
                }
            }
        }

        private static bool applicationIsQuitting = false;

        /// <summary>
        /// When Unity quits, it destroys objects in a random order.
        /// In principle, a Singleton is only destroyed when application quits.
        /// If any script calls Instance after it have been destroyed, 
        ///   it will create a buggy ghost object that will stay on the Editor scene
        ///   even after stopping playing the Application. Really bad!
        /// So, this was made to be sure we're not creating that buggy ghost object.
        /// </summary>
        protected override void OnDestroy()
        {
            applicationIsQuitting = true;
            base.OnDestroy();
        }
    }
}