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
            SerializedProperty mode                        = serializedCoatLayer.FindPropertyRelative("mode");
            SerializedProperty distanceField               = serializedCoatLayer.FindPropertyRelative("distanceField");
            SerializedProperty strandOffset                = serializedCoatLayer.FindPropertyRelative("strandOffset");
            SerializedProperty strandCurl                  = serializedCoatLayer.FindPropertyRelative("strandCurl");
            SerializedProperty alphaCutoff                 = serializedCoatLayer.FindPropertyRelative("alphaCutoff");
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

            // Render Fur Layer UI

            EditorGUILayout.LabelField("Geometry", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            
            mode.enumValueIndex = (int)(FurGeometryMode)EditorGUILayout.EnumPopup(new GUIContent("Mode"), (FurGeometryMode)mode.enumValueIndex);
            
            // Expose inputs for analytic or baked distance field properties
            EditorGUI.indentLevel++;
            if(mode.enumValueIndex == (int)FurGeometryMode.Baked)
            {
                EditorGUILayout.PropertyField(distanceField);
            }
            else
            {
                EditorGUILayout.PropertyField(strandOffset);
                EditorGUILayout.PropertyField(strandCurl);
            }
            EditorGUI.indentLevel--;
            
            // TODO: Remap slider
            EditorGUILayout.PropertyField(alphaCutoff);

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
            EditorGUILayout.PropertyField(specularShift);
            EditorGUILayout.PropertyField(specularTint);
            EditorGUILayout.Space(); 
            EditorGUILayout.PropertyField(secondarySpecularShift);
            EditorGUILayout.PropertyField(secondarySpecularTint);
            EditorGUILayout.PropertyField(secondarySpecularSmoothness);
            EditorGUILayout.Space(); 
            EditorGUILayout.PropertyField(scatterAmount);
            EditorGUILayout.PropertyField(scatterTint);
            EditorGUILayout.Space(); 
            EditorGUILayout.PropertyField(rootColor);
            EditorGUILayout.PropertyField(tipColor);
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