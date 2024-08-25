using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// This editor window tool allows the user to convert all FBX files in a specified source folder
/// into prefabs with MeshCollider and Rigidbody components added, and save them into a specified destination
/// folder.
/// The paths for the source and destination folders can be customized through the UI.
/// </summary>
public class FBXToPrefabConverterTool : EditorWindow
{
    [Header("Source and Destination Paths")]
    [Tooltip("The folder containing the FBX files to convert.")]
    [SerializeField] private string sourceFolderPath = "Assets/ExternalAssets/kenney_food-kit/Models/FBX format/";

    [Tooltip("The folder where the prefabs will be saved.")]
    [SerializeField] private string destinationFolderPath = "Assets/TestPrefabs/";

    [MenuItem("Tools/FBX to Prefab Converter Tool")]
    public static void ShowWindow()
    {
        // Display the editor window
        GetWindow<FBXToPrefabConverterTool>("FBX to Prefab Converter");
    }

    private void OnGUI()
    {
        GUILayout.Label("FBX to Prefab Converter", EditorStyles.boldLabel);

        // Input fields for source and destination folder paths
        sourceFolderPath = EditorGUILayout.TextField("Source Folder Path", sourceFolderPath);
        destinationFolderPath = EditorGUILayout.TextField("Destination Folder Path", destinationFolderPath);

        if (GUILayout.Button("Convert FBX to Prefabs"))
        {
            ConvertFBXToPrefabs();
        }
    }

    /// <summary>
    /// Converts all FBX files in the source folder into prefabs with MeshCollider and Rigidbody components
    /// added, and saves them into the destination folder.
    /// </summary>
    private void ConvertFBXToPrefabs()
    {
        // Ensure the destination folder exists
        if (!AssetDatabase.IsValidFolder(destinationFolderPath))
        {
            AssetDatabase.CreateFolder("Assets", "TestPrefabs");
        }

        // Get all FBX files in the source folder
        string[] fbxFiles = Directory.GetFiles(sourceFolderPath, "*.fbx");

        foreach (string fbxFile in fbxFiles)
        {
            // Load the FBX model as a GameObject
            string assetPath = fbxFile.Replace(Application.dataPath, "Assets");
            GameObject fbxModel = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            // Create a new instance of the model
            GameObject instance = Instantiate(fbxModel);

            // Add MeshCollider and Rigidbody components
            MeshCollider meshCollider = instance.AddComponent<MeshCollider>();
            Rigidbody rigidbody = instance.AddComponent<Rigidbody>();

            // Set MeshCollider properties if needed
            meshCollider.convex = true;

            // Create a prefab from the instance
            string prefabPath = Path.Combine(destinationFolderPath, instance.name + ".prefab");
            prefabPath = AssetDatabase.GenerateUniqueAssetPath(prefabPath);
            PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);

            // Destroy the instance after creating the prefab
            DestroyImmediate(instance);
        }

        // Refresh the AssetDatabase to reflect the new prefabs
        AssetDatabase.Refresh();

        Debug.Log("FBX to Prefab conversion complete. Prefabs saved to " + destinationFolderPath);
    }
}