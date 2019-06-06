using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

//TODO: Cleanup.

[CustomEditor(typeof(FurRenderer))]
sealed class FurRendererEditor : Editor
{
    ReorderableList _renderers;

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
        _furDistanceField = serializedObject.FindProperty("_furDistanceField");
        _furDensity = serializedObject.FindProperty("_furDensity");
        _furAlphaRemapMin = serializedObject.FindProperty("_furAlphaRemapMin");
        _furAlphaRemapMax = serializedObject.FindProperty("_furAlphaRemapMax");
        _furCombMap = serializedObject.FindProperty("_furCombMap");
        _furCombStrength = serializedObject.FindProperty("_furCombStrength");
        _furHeightMap = serializedObject.FindProperty("_furHeightMap");
        _furHeightRemapMin = serializedObject.FindProperty("_furHeightRemapMin");
        _furHeightRemapMax = serializedObject.FindProperty("_furHeightRemapMax");

        _furSpecularShift = serializedObject.FindProperty("_furSpecularShift");
        _furSpecularColor = serializedObject.FindProperty("_furSpecularColor");
        _furSecondarySmoothness = serializedObject.FindProperty("_furSecondarySmoothness");
        _furSecondarySpecularShift = serializedObject.FindProperty("_furSecondarySpecularShift");
        _furSecondarySpecularColor = serializedObject.FindProperty("_furSecondarySpecularColor");
        _furScatter = serializedObject.FindProperty("_furScatter");
        _furScatterColor = serializedObject.FindProperty("_furScatterColor");
        _furSelfOcclusionTerm = serializedObject.FindProperty("_furSelfOcclusionTerm");
        _furAORemapMin = serializedObject.FindProperty("_furAORemapMin");
        _furAORemapMax = serializedObject.FindProperty("_furAORemapMax");

        _furOvercoatDistanceField = serializedObject.FindProperty("_furOvercoatDistanceField");
        _furOvercoatAlbedoMap = serializedObject.FindProperty("_furOvercoatAlbedoMap");
        _furOvercoatColor = serializedObject.FindProperty("_furOvercoatColor");
        _furOvercoatDensity = serializedObject.FindProperty("_furOvercoatDensity");
        _furOvercoatCombStrength = serializedObject.FindProperty("_furOvercoatCombStrength");
        _furOvercoatHeightMap = serializedObject.FindProperty("_furOvercoatHeightMap");
        _furOvercoatHeight = serializedObject.FindProperty("_furOvercoatHeight");
        _furOvercoatAlphaRemap = serializedObject.FindProperty("_furOvercoatAlphaRemap");
        _furOvercoatSilhouette = serializedObject.FindProperty("_furOvercoatSilhouette");
        _furOvercoatAORemapMin = serializedObject.FindProperty("_furOvercoatAORemapMin");
        _furOvercoatAORemapMax = serializedObject.FindProperty("_furOvercoatAORemapMax");
        _furOvercoatSelfOcclusionTerm = serializedObject.FindProperty("_furOvercoatSelfOcclusionTerm");
        _furOvercoatScatter = serializedObject.FindProperty("_furOvercoatScatter");
        _furOvercoatScatterColor = serializedObject.FindProperty("_furOvercoatScatterColor");

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

    //TODO: Overhaul cleanup of UI.
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Fur Material Properties");
        EditorGUI.indentLevel++;

        EditorGUILayout.LabelField("Grooming Inputs", EditorStyles.miniBoldLabel);

        GUI.color = Color.green;
        EditorGUILayout.PropertyField(_furDistanceField);
        GUI.color = Color.white;

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(_furDensity);

        EditorGUI.BeginChangeCheck();
        Vector2 remap = new Vector2(_furAlphaRemapMin.floatValue, _furAlphaRemapMax.floatValue);
        EditorGUILayout.MinMaxSlider(Styles.AlphaRemap, ref remap.x, ref remap.y, 0.0f, 1.0f);
        if (EditorGUI.EndChangeCheck())
        {
            _furAlphaRemapMin.floatValue = remap.x;
            _furAlphaRemapMax.floatValue = remap.y;
        }

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(_furCombMap);
        EditorGUILayout.PropertyField(_furCombStrength);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(_furHeightMap);

        EditorGUI.BeginChangeCheck();
        remap = new Vector2(_furHeightRemapMin.floatValue, _furHeightRemapMax.floatValue);
        EditorGUILayout.MinMaxSlider(Styles.HeightRemap, ref remap.x, ref remap.y, 0.0f, 1.0f);
        if (EditorGUI.EndChangeCheck())
        {
            _furHeightRemapMin.floatValue = remap.x;
            _furHeightRemapMax.floatValue = remap.y;
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Shading Inputs", EditorStyles.miniBoldLabel);

        EditorGUILayout.PropertyField(_furSpecularShift);
        EditorGUILayout.PropertyField(_furSpecularColor);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(_furSecondarySmoothness);
        EditorGUILayout.PropertyField(_furSecondarySpecularShift);
        EditorGUILayout.PropertyField(_furSecondarySpecularColor);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(_furScatter);
        EditorGUILayout.PropertyField(_furScatterColor);

        EditorGUILayout.Space();

        EditorGUI.BeginChangeCheck();
        remap = new Vector2(_furAORemapMin.floatValue, _furAORemapMax.floatValue);
        EditorGUILayout.MinMaxSlider(Styles.AORemap, ref remap.x, ref remap.y, 0.0f, 1.0f);
        if (EditorGUI.EndChangeCheck())
        {
            _furAORemapMin.floatValue = remap.x;
            _furAORemapMax.floatValue = remap.y;
        }
        EditorGUILayout.PropertyField(_furSelfOcclusionTerm, Styles.SelfOcclusionMultiplier);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Overcoat", EditorStyles.miniBoldLabel);

        GUI.color = Color.green;
        EditorGUILayout.PropertyField(_furOvercoatDistanceField);
        GUI.color = Color.white;

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(_furOvercoatAlbedoMap);
        EditorGUILayout.PropertyField(_furOvercoatColor);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(_furOvercoatDensity);
        EditorGUILayout.PropertyField(_furOvercoatCombStrength);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(_furOvercoatHeightMap);
        EditorGUILayout.PropertyField(_furOvercoatHeight);

        EditorGUI.BeginChangeCheck();
        remap = _furOvercoatAlphaRemap.vector2Value;
        EditorGUILayout.MinMaxSlider(Styles.AlphaRemap, ref remap.x, ref remap.y, 0.0f, 1.0f);
        if (EditorGUI.EndChangeCheck())
        {
            _furOvercoatAlphaRemap.vector2Value = remap;
        }

        EditorGUILayout.Space();

        EditorGUI.BeginChangeCheck();
        remap = new Vector2(_furOvercoatAORemapMin.floatValue, _furOvercoatAORemapMax.floatValue);
        EditorGUILayout.MinMaxSlider(Styles.AORemap, ref remap.x, ref remap.y, 0.0f, 1.0f);
        if (EditorGUI.EndChangeCheck())
        {
            _furOvercoatAORemapMin.floatValue = remap.x;
            _furOvercoatAORemapMax.floatValue = remap.y;
        }
        EditorGUILayout.PropertyField(_furOvercoatSelfOcclusionTerm);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(_furOvercoatScatter);
        EditorGUILayout.PropertyField(_furOvercoatScatterColor);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(_furOvercoatSilhouette);

        EditorGUI.indentLevel--;
        _renderers.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }
}