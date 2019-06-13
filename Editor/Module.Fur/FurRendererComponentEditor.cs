using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Experimental.Rendering.HDPipeline;

using UnityEditor;
using UnityEditorInternal;

namespace UnityEditor.Experimental.Rendering.HDPipeline
{
    [CustomEditor(typeof(FurRendererComponent))]
    sealed class FurRendererComponentEditor : Editor
    {
        ReorderableList _renderers;
        ReorderableList _coatLayers;

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
        }
        
        void DrawFurCoatLayer(SerializedProperty serializedCoatLayer)
        {
            SerializedProperty geometryMode                = serializedCoatLayer.FindPropertyRelative("geometryMode");
            SerializedProperty distanceField               = serializedCoatLayer.FindPropertyRelative("distanceField");
            SerializedProperty strandOffset                = serializedCoatLayer.FindPropertyRelative("strandOffset");
            SerializedProperty strandCurl                  = serializedCoatLayer.FindPropertyRelative("strandCurl");
            SerializedProperty alphaMode                   = serializedCoatLayer.FindPropertyRelative("alphaMode");
            SerializedProperty alphaCutoff                 = serializedCoatLayer.FindPropertyRelative("alphaCutoff");
            SerializedProperty alphaFeather                = serializedCoatLayer.FindPropertyRelative("alphaFeather");
            SerializedProperty densityMap                  = serializedCoatLayer.FindPropertyRelative("densityMap");
            SerializedProperty density                     = serializedCoatLayer.FindPropertyRelative("density");       
            SerializedProperty heightMap                   = serializedCoatLayer.FindPropertyRelative("heightMap");
            SerializedProperty height                      = serializedCoatLayer.FindPropertyRelative("height");       
            SerializedProperty minimumHeight               = serializedCoatLayer.FindPropertyRelative("minimumHeight");
            SerializedProperty combMap                     = serializedCoatLayer.FindPropertyRelative("combMap");
            SerializedProperty combStrength                = serializedCoatLayer.FindPropertyRelative("combStrength");
            SerializedProperty specularShift               = serializedCoatLayer.FindPropertyRelative("specularShift");
            SerializedProperty secondarySpecularShift      = serializedCoatLayer.FindPropertyRelative("secondarySpecularShift");
            SerializedProperty specularTint                = serializedCoatLayer.FindPropertyRelative("specularTint");
            SerializedProperty secondarySpecularTint       = serializedCoatLayer.FindPropertyRelative("secondarySpecularTint");
            SerializedProperty secondarySpecularSmoothness = serializedCoatLayer.FindPropertyRelative("secondarySpecularSmoothness");
            SerializedProperty scatterAmount               = serializedCoatLayer.FindPropertyRelative("scatterAmount");
            SerializedProperty scatterTint                 = serializedCoatLayer.FindPropertyRelative("scatterTint");
            SerializedProperty rootColor                   = serializedCoatLayer.FindPropertyRelative("rootColor");
            SerializedProperty tipColor                    = serializedCoatLayer.FindPropertyRelative("tipColor");
            SerializedProperty selfShadow                  = serializedCoatLayer.FindPropertyRelative("selfShadow");

            // Render Fur Layer UI

            EditorGUILayout.LabelField("Geometry", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            
            geometryMode.enumValueIndex = (int)(FurGeometryMode)EditorGUILayout.EnumPopup(new GUIContent("Mode"), (FurGeometryMode)geometryMode.enumValueIndex);
            
            EditorGUI.indentLevel++;
            if(geometryMode.enumValueIndex == (int)FurGeometryMode.Baked)
            {
                EditorGUILayout.PropertyField(distanceField);
            }
            else
            {
                EditorGUILayout.PropertyField(strandOffset);
                EditorGUILayout.PropertyField(strandCurl);
            }
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
            
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Alpha", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            alphaMode.enumValueIndex = (int)(FurAlphaMode)EditorGUILayout.EnumPopup(new GUIContent("Mode"), (FurAlphaMode)alphaMode.enumValueIndex);
            
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(alphaCutoff);
            if(alphaMode.enumValueIndex == (int)FurAlphaMode.Dither)
            {
                EditorGUILayout.PropertyField(alphaFeather);
            }
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;

            EditorGUILayout.Space(); 

            EditorGUILayout.LabelField("Groom", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            // TODO: Consolidate map + strengths 
            EditorGUILayout.PropertyField(densityMap);
            EditorGUILayout.PropertyField(density);
            EditorGUILayout.PropertyField(heightMap);
            // TODO: Consolidate height to remaps
            EditorGUILayout.PropertyField(height);
            EditorGUILayout.PropertyField(minimumHeight);
            EditorGUILayout.PropertyField(combMap);
            EditorGUILayout.PropertyField(combStrength);
            EditorGUI.indentLevel--;
            
            EditorGUILayout.Space(); 
            
            EditorGUILayout.LabelField("Shading", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("Lobe 1", EditorStyles.miniBoldLabel);
            EditorGUILayout.PropertyField(specularShift, new GUIContent("Shift"));
            EditorGUILayout.PropertyField(specularTint, new GUIContent("Tint"));
            EditorGUILayout.Space(); 
            EditorGUILayout.LabelField("Lobe 2", EditorStyles.miniBoldLabel);
            EditorGUILayout.PropertyField(secondarySpecularSmoothness, new GUIContent("Smoothness"));
            EditorGUILayout.PropertyField(secondarySpecularShift, new GUIContent("Shift"));
            EditorGUILayout.PropertyField(secondarySpecularTint, new GUIContent("Tint"));
            EditorGUILayout.Space(); 
            EditorGUILayout.LabelField("Scatter", EditorStyles.miniBoldLabel);
            EditorGUILayout.PropertyField(scatterAmount, new GUIContent("Intensity"));
            EditorGUILayout.PropertyField(scatterTint, new GUIContent("Tint"));
            EditorGUILayout.Space(); 
            EditorGUILayout.LabelField("Albedo", EditorStyles.miniBoldLabel);
            EditorGUILayout.PropertyField(rootColor, new GUIContent("Root"));
            EditorGUILayout.PropertyField(tipColor, new GUIContent("Tip"));

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(selfShadow);
            EditorGUI.indentLevel--;
            
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
            
            EditorGUILayout.Space();
            
            _renderers.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
    }
}