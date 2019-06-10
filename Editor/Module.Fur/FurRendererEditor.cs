using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace UnityEditor.Experimental.Rendering.HDPipeline
{
    [CustomEditor(typeof(FurRenderer))]
    sealed class FurRendererEditor : Editor
    {
        ReorderableList _renderers;

        ReorderableList _coatLayers;

        //Grooming Inputs
        SerializedProperty _furDistanceField;
        SerializedProperty _furDensity;
        SerializedProperty _furAlphaRemapMin;
        SerializedProperty _furAlphaRemapMax;
        SerializedProperty _furCombMap;
        SerializedProperty _furCombStrength;
        SerializedProperty _furHeightMap;
        SerializedProperty _furHeightRemapMin;
        SerializedProperty _furHeightRemapMax;

        //Shading Inputs
        SerializedProperty _furSpecularShift;
        SerializedProperty _furSpecularColor;
        SerializedProperty _furSecondarySmoothness;
        SerializedProperty _furSecondarySpecularShift;
        SerializedProperty _furSecondarySpecularColor;
        SerializedProperty _furScatter;
        SerializedProperty _furScatterColor;
        SerializedProperty _furSelfOcclusionTerm;
        SerializedProperty _furAORemapMin;
        SerializedProperty _furAORemapMax;

        //Overcoat
        SerializedProperty _furOvercoatDistanceField;
        SerializedProperty _furOvercoatAlbedoMap;
        SerializedProperty _furOvercoatColor;
        SerializedProperty _furOvercoatDensity;
        SerializedProperty _furOvercoatCombStrength;
        SerializedProperty _furOvercoatHeightMap;
        SerializedProperty _furOvercoatHeight;
        SerializedProperty _furOvercoatAlphaRemap;
        SerializedProperty _furOvercoatSilhouette;
        SerializedProperty _furOvercoatAORemapMin;
        SerializedProperty _furOvercoatAORemapMax;
        SerializedProperty _furOvercoatSelfOcclusionTerm;
        SerializedProperty _furOvercoatScatter;
        SerializedProperty _furOvercoatScatterColor;

        void OnEnable()
        {
            // Coats
            _coatLayers = new ReorderableList(
                serializedObject,
                serializedObject.FindProperty("_coatLayers"),
                true, // draggable
                true, // displayHeader
                true, // displayAddButton
                true  // displayRemoveButton
            );
            
            // Layers header label.
            _coatLayers.drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField(rect, "Coat Layers");
            };

            // Layers element label.
            _coatLayers.drawElementCallback = (Rect frame, int index, bool isActive, bool isFocused) => {
                var rect = frame;
                rect.y += 2;
                rect.height = EditorGUIUtility.singleLineHeight;
                var element = _coatLayers.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.LabelField(rect, "Layer " + index);
            };

            // Renderers
            _renderers = new ReorderableList(
                serializedObject,
                serializedObject.FindProperty("_renderers"),
                true, // draggable
                true, // displayHeader
                true, // displayAddButton
                true  // displayRemoveButton
            );

            _renderers.drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField(rect, "Target Renderers");
            };

            _renderers.drawElementCallback = (Rect frame, int index, bool isActive, bool isFocused) => {
                var rect = frame;
                rect.y += 2;
                rect.height = EditorGUIUtility.singleLineHeight;
                var element = _renderers.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, element, GUIContent.none);
            };
        }

        static class Styles
        {
            public static readonly GUIContent AlphaRemap = new GUIContent("Alpha Remap");
            public static readonly GUIContent HeightRemap = new GUIContent("Height Remap");
            public static readonly GUIContent SelfOcclusionMultiplier = new GUIContent("Self Occlusion Muliplier");
            public static readonly GUIContent AORemap = new GUIContent("Self Occlusion");
        }
        
        void DrawFurCoatLayer(SerializedProperty serializedCoatLayer)
        {
            SerializedProperty distanceField = serializedCoatLayer.FindPropertyRelative("distanceField");
            SerializedProperty alphaRemap    = serializedCoatLayer.FindPropertyRelative("alphaRemap");
            SerializedProperty densityMap    = serializedCoatLayer.FindPropertyRelative("densityMap");
            SerializedProperty density       = serializedCoatLayer.FindPropertyRelative("density");       
            SerializedProperty heightMap     = serializedCoatLayer.FindPropertyRelative("heightMap");
            SerializedProperty height        = serializedCoatLayer.FindPropertyRelative("height");       
            SerializedProperty minimumHeight = serializedCoatLayer.FindPropertyRelative("minimumHeight");
            SerializedProperty combMap       = serializedCoatLayer.FindPropertyRelative("combMap");
            SerializedProperty combStrength  = serializedCoatLayer.FindPropertyRelative("combStrength");
            SerializedProperty specularShift = serializedCoatLayer.FindPropertyRelative("specularShift");
            SerializedProperty secondarySpecularShift = serializedCoatLayer.FindPropertyRelative("secondarySpecularShift");
            SerializedProperty specularTint = serializedCoatLayer.FindPropertyRelative("specularTint");
            SerializedProperty secondarySpecularTint = serializedCoatLayer.FindPropertyRelative("secondarySpecularTint");
            SerializedProperty secondarySpecularSmoothness = serializedCoatLayer.FindPropertyRelative("secondarySpecularSmoothness");
            SerializedProperty scatterAmount = serializedCoatLayer.FindPropertyRelative("scatterAmount");
            SerializedProperty scatterTint = serializedCoatLayer.FindPropertyRelative("scatterTint");
            SerializedProperty rootColor = serializedCoatLayer.FindPropertyRelative("rootColor");
            SerializedProperty tipColor = serializedCoatLayer.FindPropertyRelative("tipColor");

            // TODO: Tidy up 
            EditorGUILayout.PropertyField(distanceField);
            EditorGUILayout.PropertyField(alphaRemap);
            EditorGUILayout.PropertyField(densityMap);
            EditorGUILayout.PropertyField(density);
            EditorGUILayout.PropertyField(heightMap);
            EditorGUILayout.PropertyField(height);
            EditorGUILayout.PropertyField(minimumHeight);
            EditorGUILayout.PropertyField(combMap);
            EditorGUILayout.PropertyField(combStrength);
            EditorGUILayout.PropertyField(specularShift);
            EditorGUILayout.PropertyField(secondarySpecularShift);
            EditorGUILayout.PropertyField(specularTint);
            EditorGUILayout.PropertyField(secondarySpecularTint);
            EditorGUILayout.PropertyField(secondarySpecularSmoothness);
            EditorGUILayout.PropertyField(scatterAmount);
            EditorGUILayout.PropertyField(scatterTint);
            EditorGUILayout.PropertyField(rootColor);
            EditorGUILayout.PropertyField(tipColor);
            
        }

        //TODO: Overhaul cleanup of UI.
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();

            // Display the coat layer list, followed by section presenting layer inputs
            _coatLayers.DoLayoutList(); // TODO: Perhaps embed coat layer UI as elements here?

            // List coat layers.
            for(int i = 0; i < _coatLayers.count; i++)
            {
                SerializedProperty layer = _coatLayers.serializedProperty.GetArrayElementAtIndex(i);

                bool state = layer.isExpanded;
                state = CoreEditorUtils.DrawHeaderFoldout("Layer " + i, state);
                
                if(state)
                {
                    DrawFurCoatLayer(layer);
                }

                layer.isExpanded = state;
            } 
            
            //_renderers.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
    }
}