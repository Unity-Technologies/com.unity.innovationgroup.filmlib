using UnityEditor;
using UnityEngine;
#if USING_MWU_HDRP
using System.IO;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Experimental.Rendering.HDPipeline;
#endif

public class RenderWindow : EditorWindow
{
    protected static float STANDARDBUTTONHEIGHT = 35f;
#if USING_MWU_HDRP
    // RTs
    RenderTexture m_RenderView = null;

    // Viewer
    // Currently does nothing, in future may be able to add progress bar.
    Material m_RenderWindowMaterial = null;

    private Camera m_RenderCamera = null;

    private static int kPreviewWidth  = 960;
    private static int kPreviewHeight = 480;
#endif
    private static Vector2 curWindowSize = Vector2.zero;
    
    //[MenuItem("DEV/RenderWindow")]
    //static void CreateWizard()
    //{
    //    RenderWindow window = ScriptableObject.CreateInstance(typeof(RenderWindow)) as RenderWindow;
    //    window.titleContent = new GUIContent("Render Window");
    //    window.ShowUtility();
    //}

    private void OnEnable()
    {
#if USING_MWU_HDRP
        m_RenderWindowMaterial = CoreUtils.CreateEngineMaterial(Shader.Find("Hidden/RenderWindow"));

        m_RenderCamera = Camera.main;
        
        CreatePreviewBuffer();
        Graphics.Blit(Texture2D.blackTexture, m_RenderView);
#endif
    }

#if USING_MWU_HDRP
    private void CreatePreviewBuffer()
    {
        CoreUtils.Destroy(m_RenderView);
        m_RenderView = new RenderTexture(m_RenderCamera.pixelWidth, m_RenderCamera.pixelHeight, 0, RenderTextureFormat.ARGBHalf)
        {
            filterMode = FilterMode.Bilinear 
        };
    }
#endif

    void OnGUI()
    {
        //minSize = new Vector2(kPreviewWidth, kPreviewHeight);
        //maxSize = new Vector2(kPreviewWidth, kPreviewHeight);

        curWindowSize.x = position.width;
        curWindowSize.y = position.height;
#if USING_MWU_HDRP
        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Converge", GUILayout.Height(STANDARDBUTTONHEIGHT)))
            {
                //Recreate preview buffer on resolution change.
                if (m_RenderCamera.pixelWidth != m_RenderView.width ||
                   m_RenderCamera.pixelHeight != m_RenderView.height)
                {
                    CreatePreviewBuffer();
                }

                //Request the final picture RT frame.
                RenderTexture convertedFrameRT = FilmicMotionBlurManager.Converge();

                //Blit RT to presentation buffer.
                Graphics.Blit(convertedFrameRT, m_RenderView);

                //Destory RT
                RenderTexture.DestroyImmediate(convertedFrameRT);
                
                // Hack: Currently the final converged from does not include bloom/final grading. Run it through post here...
                PostProcessLayer postProcessing = FilmicMotionBlurManager.GetPostLayer();
                
                //Supply Depth for DoF.
                CommandBuffer cmd = new CommandBuffer() { name = "Render Window" };
                cmd.SetGlobalTexture(HDShaderIDs._CameraDepthTexture, HDRPColorDepthCapture.instance.DepthTexture); //Hack: Ask VFX script for depth...
                
                HDCamera hdCamera = HDCamera.Get(m_RenderCamera);
                var postProcessRenderContext = hdCamera.postprocessRenderContext;
                postProcessRenderContext.Reset();
                postProcessRenderContext.source = m_RenderView;
                postProcessRenderContext.destination = m_RenderView;
                postProcessRenderContext.command = cmd;
                postProcessRenderContext.camera = Camera.main;
                postProcessRenderContext.sourceFormat = RenderTextureFormat.ARGBHalf;
                postProcessRenderContext.flip = false;
                postProcessRenderContext.skipDepthOfField = true;
                postProcessing.Render(postProcessRenderContext);
                
                Graphics.ExecuteCommandBuffer(cmd);
                cmd.Dispose();
            }

            if (GUILayout.Button("Save", GUILayout.Height(STANDARDBUTTONHEIGHT)))
            {
                string path = EditorUtility.SaveFilePanel("Save Render Preview", "", "RenderPreview", "png");

                int w = m_RenderView.width;
                int h = m_RenderView.height;
                Texture2D t = new Texture2D(w, h, TextureFormat.RGB24, false);

                RenderTexture pRT = RenderTexture.active;
                RenderTexture.active = m_RenderView;
                t.ReadPixels(new Rect(0, 0, w, h), 0, 0);
                RenderTexture.active = pRT;

                //Brute-force Gamma Correct
                for(int x = 0; x < w; x++)
                {
                    for(int y = 0; y < h; ++y)
                    {
                        Color currentColor = t.GetPixel(x, y);
                        t.SetPixel(x, y, currentColor.gamma);
                    }
                }

                byte[] bytes = t.EncodeToPNG();
                CoreUtils.Destroy(t);

                File.WriteAllBytes(path, bytes);
            }

            //if (GUILayout.Button("Close", GUILayout.Height(STANDARDBUTTONHEIGHT)))
            //{
            //    Close();
            //    return;
            //}
        }
        GUILayout.EndHorizontal();

        //Present final picture
        EditorGUI.DrawPreviewTexture(GUILayoutUtility.GetRect(curWindowSize.x, curWindowSize.y), m_RenderView, m_RenderWindowMaterial, ScaleMode.ScaleToFit, 0f);
#else
        GUILayout.Label("Render Window requires Sherman Custom HDRP package. If you have it installed already, add the #USING_MWU_HDRP define in your player defines", EditorStyles.helpBox);
#endif

    }

#if USING_MWU_HDRP
    private void OnDestroy()
    {
        CoreUtils.Destroy(m_RenderView);
        CoreUtils.Destroy(m_RenderWindowMaterial);
    }
#endif

    private void OnInspectorUpdate()
    {
        Repaint();
    }
}