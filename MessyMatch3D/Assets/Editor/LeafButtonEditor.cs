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
            EditorGUILayout.PropertyField(highlightImageProp);
        }
        
        // Check if _useTextEffect is true
        if (leafButton.UseTextEffect)
        {
            // Show the _highlightImage property
            SerializedProperty textProp = serializedObject.FindProperty("_buttonText");
            EditorGUILayout.PropertyField(textProp);
        }

        // Check if _useColorEffect is true
        if (leafButton.UseColorEffect)
        {
            // Show the _normalColor, _hoverColor, and _pressedColor properties
            SerializedProperty normalColorProp = serializedObject.FindProperty("_normalColor");
            SerializedProperty hoverColorProp = serializedObject.FindProperty("_hoverColor");
            SerializedProperty pressedColorProp = serializedObject.FindProperty("_pressedColor");

            EditorGUILayout.PropertyField(normalColorProp);
            EditorGUILayout.PropertyField(hoverColorProp);
            EditorGUILayout.PropertyField(pressedColorProp);
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