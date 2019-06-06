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
        }
    }
}
