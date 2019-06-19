using UnityEngine;
using UnityEngine.Rendering;

using UnityEditor;

/*

Fur Render Pass

This requires a slightly modified version of the HD Render Pipeline containing lightweight render callbacks.
This render pass plugs into those callbacks, injecting the fur shell passes.

*/

namespace UnityEngine.Experimental.Rendering.HDPipeline
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public sealed class FurRenderPass : MonoBehaviour
    {
        // Currently shell count is internally set, in the future, this needs to be converted to per-renderer setting
        [SerializeField] int m_ShellCount = 32;
        public int ShellCount 
        {
            get { return m_ShellCount;  }
            set { m_ShellCount = value; }
        }

        static class ShaderIDs
        {
            public static readonly int _FurSystemParams = Shader.PropertyToID("_FurSystemParams");
        }

        static class ShaderPassNames
        {
            public static readonly ShaderPassName _FurShellDepthName  = new ShaderPassName("FurShellDepth");
            public static readonly ShaderPassName _FurShellOpaqueName = new ShaderPassName("FurShellOpaque");
        }

        void OnEnable()
        {
            var camera = GetComponent<Camera>();
            if(camera == null) return;

            var hdCamera = camera.GetComponent<HDAdditionalCameraData>();
            if(hdCamera == null) return;

            //TODO: Assemble list of fur renderers.
            //TODO: Warning check for HDRP asset.
#if USING_MWU_HDRP
            // Hook into HDRP passes, inject a fur shell depth + opaque pass.
            hdCamera.afterDepthPrepass  += FurShellDepthPass;
            hdCamera.afterForwardOpaque += FurShellForwardOpaquePass;
        
            //We also would like to add this renderpass to the SceneView camera.
            foreach (SceneView sv in SceneView.sceneViews)
            {
                Camera cc = sv.camera;
                if (cc.GetComponent<HDAdditionalCameraData>() == null)
                {
                    var hdSceneCamera = cc.gameObject.AddComponent<HDAdditionalCameraData>();
                    hdSceneCamera.afterDepthPrepass  += FurShellDepthPass;
                    hdSceneCamera.afterForwardOpaque += FurShellForwardOpaquePass; 
                }
            }
#endif
        }

        void OnDisable()
        {
            var camera = GetComponent<Camera>();
            if(camera == null) return;

            var hdCamera = camera.GetComponent<HDAdditionalCameraData>();
            if(hdCamera == null) return;

#if USING_MWU_HDRP
            hdCamera.afterDepthPrepass  -= FurShellDepthPass;
            hdCamera.afterForwardOpaque -= FurShellForwardOpaquePass;

            //Remove renderpass from SceneView as well.
            foreach (SceneView sv in SceneView.sceneViews)
            {
                Camera cc = sv.camera;
                if (cc.GetComponent<HDAdditionalCameraData>() != null)
                {
                    var hdSceneCamera = cc.gameObject.GetComponent<HDAdditionalCameraData>();
                    hdSceneCamera.afterDepthPrepass  -= FurShellDepthPass;
                    hdSceneCamera.afterForwardOpaque -= FurShellForwardOpaquePass; 
                }
            }
#endif
        }

        void FurShellDepthPass(ScriptableRenderContext context, HDCamera hdCamera, CullResults cull, CommandBuffer cmd,
                               RTHandleSystem.RTHandle depthStencil)
        {
            using (new ProfilingSample(cmd, "Fur Shell Pass (Depth)"))
            {
                float shellDelta = 1f / (float)m_ShellCount;

                // TODO: Per-renderer shell count.
                // NOTE: Currently skip 0th shell due to zfight issue. Bandaid fix.
                for (int s = m_ShellCount; s >= 1; --s)
                {
                    float shellLayer = (float)s / m_ShellCount;

                    cmd.SetGlobalVector(ShaderIDs._FurSystemParams, new Vector4( shellLayer, shellDelta, 0f, 0f));
                    RenderShellLayer(hdCamera, cmd, cull, context, ShaderPassNames._FurShellDepthName);
                }
            }
        }

        void FurShellForwardOpaquePass(ScriptableRenderContext context, HDCamera hdCamera, CullResults cull, CommandBuffer cmd,
                                       RTHandleSystem.RTHandle colorBuffer, RTHandleSystem.RTHandle depthStencil)
        {
            using (new ProfilingSample(cmd, "Fur Shell Pass (Opaque)"))
            {
                float shellDelta = 1f / (float)m_ShellCount;

                // TODO: Per-renderer shell count.
                // NOTE: Currently skip 0th shell due to zfight issue. Bandaid fix.
                for (int s = m_ShellCount; s >= 1; --s)
                {
                    float shellLayer = (float)s / m_ShellCount;
                    
                    cmd.SetGlobalVector(ShaderIDs._FurSystemParams, new Vector4( shellLayer, shellDelta, 0f, 0f));
                    RenderShellLayer(hdCamera, cmd, cull, context, ShaderPassNames._FurShellOpaqueName, HDUtils.k_RendererConfigurationBakedLighting);
                }
            }
        }

        //TODO: GPU Instance Dithered Fur Shells to GBuffer.
        private void RenderShellLayer(HDCamera                hdCamera,
                                      CommandBuffer           cmd,
                                      CullResults             cull,
                                      ScriptableRenderContext renderContext,
                                      ShaderPassName          passName,
                                      RendererConfiguration   configuration = 0)
        {
            renderContext.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            var drawSettings = new DrawRendererSettings(hdCamera.camera, passName)
            {
                rendererConfiguration = configuration,
                sorting = { flags = SortFlags.CommonOpaque }
            };
            
            var filterSettings = new FilterRenderersSettings(true)
            {
                renderQueueRange = HDRenderQueue.k_RenderQueue_AllOpaque,
                excludeMotionVectorObjects = false
            };

            renderContext.DrawRenderers(cull.visibleRenderers, ref drawSettings, filterSettings);
        }
    }
}