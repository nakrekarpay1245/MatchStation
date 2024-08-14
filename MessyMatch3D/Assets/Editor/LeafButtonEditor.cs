using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LeafButton))]
public class LeafButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Get the target object (LeafButton)
        LeafButton leafButton = (LeafButton)target;

        // Draw default inspector properties
        DrawDefaultInspector();

        // Check if _useImageEffect is true
        if (leafButton.UseHighlightEffect)
        {
            // Show the _highlightImage property
            SerializedProperty highlightImageProp = serializedObject.FindProperty("_highlightImage");
            SerializedProperty hightlightImageVisibleAlphaProp = serializedObject.FindProperty("_hightlightImageVisibleAlpha");
            SerializedProperty hightlightImageHiddenAlphaProp = serializedObject.FindProperty("_hightlightImageHiddenAlpha");
            SerializedProperty hightlightImageVisibleScaleProp = serializedObject.FindProperty("_hightlightImageVisibleScale");
            SerializedProperty hightlightImageHiddenScaleProp = serializedObject.FindProperty("_hightlightImageHiddenScale");

            EditorGUILayout.PropertyField(highlightImageProp);
            EditorGUILayout.PropertyField(hightlightImageVisibleAlphaProp);
            EditorGUILayout.PropertyField(hightlightImageHiddenAlphaProp);
            EditorGUILayout.PropertyField(hightlightImageVisibleScaleProp);
            EditorGUILayout.PropertyField(hightlightImageHiddenScaleProp);
        }

        // Check if _useTextColorEffect is true
        if (leafButton.UseTextColorEffect)
        {
            // Show the _highlightImage property
            SerializedProperty textProp = serializedObject.FindProperty("_buttonText");
            EditorGUILayout.PropertyField(textProp);

            // Show the _textNormalColor, _hoverColor, and _pressedColor properties
            SerializedProperty textNormalColorProp = serializedObject.FindProperty("_textNormalColor");
            SerializedProperty textHoverColorProp = serializedObject.FindProperty("_textHoverColor");
            SerializedProperty textPressedColorProp = serializedObject.FindProperty("_textPressedColor");

            EditorGUILayout.PropertyField(textNormalColorProp);
            EditorGUILayout.PropertyField(textHoverColorProp);
            EditorGUILayout.PropertyField(textPressedColorProp);
        }

        // Check if _useButtonColorEffect is true
        if (leafButton.UseButtonColorEffect)
        {
            // Show the _buttonNormalColor, _buttonHoverColor, and _buttonPressedColor properties
            SerializedProperty buttonormalColorProp = serializedObject.FindProperty("_buttonNormalColor");
            SerializedProperty buttonHoverColorProp = serializedObject.FindProperty("_buttonHoverColor");
            SerializedProperty buttonPressedColorProp = serializedObject.FindProperty("_buttonPressedColor");

            EditorGUILayout.PropertyField(buttonormalColorProp);
            EditorGUILayout.PropertyField(buttonHoverColorProp);
            EditorGUILayout.PropertyField(buttonPressedColorProp);
        }

        // Check if _useSoundEffect is true
        if (leafButton.UseSoundEffect)
        {
            // Show the _buttonPressClipKey and _buttonHoverClipKey properties
            SerializedProperty buttonPressClipKeyProp = serializedObject.FindProperty("_buttonPressClipKey");
            SerializedProperty buttonHoverClipKeyProp = serializedObject.FindProperty("_buttonHoverClipKey");

            EditorGUILayout.PropertyField(buttonPressClipKeyProp);
            EditorGUILayout.PropertyField(buttonHoverClipKeyProp);
        }

        // Apply changes to serialized properties
        serializedObject.ApplyModifiedProperties();
    }
}