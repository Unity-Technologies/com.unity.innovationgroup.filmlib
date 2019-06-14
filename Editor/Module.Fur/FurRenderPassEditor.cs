using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering.HDPipeline;

namespace UnityEditor.Experimental.Rendering.HDPipeline
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(FurRenderPass))]
    class FurRenderPassEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            //TODO: Expose shell count, other fur render-specific settings.
            #if USING_MWU_HDRP

            #else
            GUILayout.Label("Fur requires Sherman Custom HDRP package. If you have it installed already, add the #USING_MWU_HDRP define in your player defines", EditorStyles.helpBox);
            #endif
        }
    }
}
