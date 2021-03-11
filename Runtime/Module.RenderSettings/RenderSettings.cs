using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering.HDPipeline;
using UnityEngine.SceneManagement;

namespace MWU.FilmLib
{
    [System.Serializable]
    public struct DetailLevel
    {
        public string name;                         // simple name-based lookup for detail levels so nothing is hard-coded
        public int furCount;
        public List<string> objectsToHide;          // we have string-based lookup so that we can find objects even across multi-scene boundaries
        public List<string> objectsToShow;
        public bool reflectionProbes;
        public bool planarReflectionProbes;

    }

    public class RenderSettings : MonoBehaviour
    {
        // just show them on the inspector ghetto style for now, we'll build a nicer interface for this at some point
        public List<DetailLevel> detailLevels = new List<DetailLevel>();
        private static List<ReflectionProbe> refProbeCache = new List<ReflectionProbe>();     // cache the list of refprobes
        private static List<PlanarReflectionProbe> refProbePlanarCache = new List<PlanarReflectionProbe>();     // cache the list of refprobes

        public static int furShellCount;
        public static int furMinCount = 0;
        public static int furMaxCount = 128;
        public static List<string> detailLevelsList = new List<string>();

        public static RenderSettings FindRenderSettingsObject()
        {
            var openSceneCount = SceneManager.sceneCount;
            for (int i = 0; i < openSceneCount; i++)
            {
                var thisScene = SceneManager.GetSceneAt(i);
                if (thisScene.isLoaded)
                {
                    //  Debug.Log("Scanning scene : " + thisScene.name);
                    var rootGO = thisScene.GetRootGameObjects();
                    foreach (var go in rootGO)
                    {
                        // apparently this is null sometimes
                        if( go != null)
                        {
                            var rs = go.GetComponentInChildren<RenderSettings>(true);
                            if (rs != null)
                                return rs;
                        }
                    }
                }
            }
            return null;
        }


        // activate a given render settings preset
        public static void ActivateRenderSettings(string detailLevel)
        {
            Debug.Log("Activated render setting: " + detailLevel);
            var renderSettings = RenderSettings.FindRenderSettingsObject();

            var detailLevels = renderSettings.detailLevels;
            foreach (var level in detailLevels)
            {
                if (level.name == detailLevel)
                {
                    // configure ourselves based on the detail settings.
                    furShellCount = level.furCount;

                    if( level.objectsToHide != null)
                    {
                        foreach (var toHide in level.objectsToHide)
                        {
                            var hideGo = EditorUtilities.SearchOpenScenesForObject(toHide);
                            if (hideGo != null)
                            {
                                hideGo.SetActive(false);
                            }
                            else
                            {
                                Debug.Log("RenderSettings::Could not find object: " + toHide + " to hide?");
                            }
                        }
                    }
                    
                    if( level.objectsToShow != null)
                    {
                        foreach (var toShow in level.objectsToShow)
                        {
                            var showGo = EditorUtilities.SearchOpenScenesForObject(toShow);
                            if (showGo != null)
                            {
                                showGo.SetActive(true);
                            }
                            else
                            {
                                Debug.Log("RenderSettings::Could not find object: " + toShow + " to show?");
                            }
                        }
                    }
                    
                    SearchOpenScenesForProbes();
                    UpdateFur();
                    ToggleReflectionProbes(level.reflectionProbes);
                    TogglePlanarReflectionProbes(level.reflectionProbes);
                }
            }
        }

        /// <summary>
        /// get our list of render settings so we can show them on the toolbar
        /// </summary>
        /// <returns></returns>
        public static List<string> GetRenderSettings()
        {
            var renderSettings = FindRenderSettingsObject();
            if (renderSettings == null)
            {
                // Debug.Log("RenderSettings: Could not find render settings object?");
                return new List<string>();
            }

            detailLevelsList.Clear();
            var detailLevels = renderSettings.detailLevels;
            foreach (var level in detailLevels)
            {
                detailLevelsList.Add(level.name);
            }

            return detailLevelsList;
        }

        // TODO: Hook into Film lib fur.

        public static void UpdateFur()
        {
#if USING_MWU_HDRP
            // NOTE: Currently, we assume the fur render pass to live on main camera.
            //       In the future, Fur system will be singleton (ie DecalSystem)
            var furRenderPass = Camera.main.GetComponent<FurRenderPass>();
            if (furRenderPass == null) return;

            furRenderPass.ShellCount = furShellCount;
#endif
        }

        public static void SetInitialFurCount()
        {
#if USING_MWU_HDRP
            var furRenderPass = Camera.main.GetComponent<FurRenderPass>();
            if (furRenderPass == null) return;

            furShellCount = furRenderPass.ShellCount;
#endif
        }

        private static void SearchOpenScenesForProbes()
        {
            refProbeCache.Clear();

            var openSceneCount = SceneManager.sceneCount;
            for (int i = 0; i < openSceneCount; i++)
            {
                var thisScene = SceneManager.GetSceneAt(i);
                //  Debug.Log("Scanning scene : " + thisScene.name);
                if (thisScene.isLoaded)
                {
                    var rootGO = thisScene.GetRootGameObjects();
                    foreach (var go in rootGO)
                    {
                        // find all of the reflection probes
                        var probes = go.GetComponentsInChildren<ReflectionProbe>(true);
                        //    Debug.Log("found " + probes.Length + " probes");
                        if (probes.Length > 0)
                        {
                            refProbeCache.AddRange(probes.ToList());
                        }
                        // find all of the planar reflection probes
                        var planarProbes = go.GetComponentsInChildren<PlanarReflectionProbe>(true);
                        //    Debug.Log("found " + probes.Length + " probes");
                        if (planarProbes.Length > 0)
                        {
                            refProbePlanarCache.AddRange(planarProbes);
                        }
                    }
                }
            }
        }

        public static void ToggleReflectionProbes(bool isActive)
        {
            //    Debug.Log("Have " + refProbeCache.Count + " ref probes, inverting");
            foreach (var probe in refProbeCache)
            {
                // invert whether the probe is active or not
                probe.enabled = isActive;
            }
        }

        public static void TogglePlanarReflectionProbes(bool isActive)
        {
            foreach (var probe in refProbePlanarCache)
            {
                // invert whether the probe is active or not
                probe.enabled = isActive;
            }
        }
    }
}