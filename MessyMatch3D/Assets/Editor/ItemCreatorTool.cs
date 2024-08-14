using UnityEngine;
using UnityEditor;
using System.IO;

public class ItemCreatorEditor : EditorWindow
{
    [Header("Folders")]
    [Tooltip("The source folder containing FBX files.")]
    [SerializeField] private string sourceFolderPath = "Assets/ExternalAssets/kenney_food-kit/Models/FBX format/";

    [Tooltip("The destination folder to save the prefabs.")]
    [SerializeField] private string destinationFolderPath = "Assets/ItemPrefabs";

    [Header("Components")]
    [Tooltip("MonoScript to add as a component to the prefabs.")]
    [SerializeField] private MonoScript itemScript;

    [Header("Icons")]
    [Tooltip("Folder containing the sprites for item icons.")]
    [SerializeField] private string iconsFolderPath = "Assets/ExternalAssets/kenney_food-kit/Icons";

    private int itemIdCounter = 0; // Counter to assign unique ItemId

    [MenuItem("Tools/Item Creator")]
    public static void ShowWindow()
    {
        GetWindow<ItemCreatorEditor>("Item Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("FBX to Prefab Tool", EditorStyles.boldLabel);

        sourceFolderPath = EditorGUILayout.TextField("Source Folder", sourceFolderPath);
        destinationFolderPath = EditorGUILayout.TextField("Destination Folder", destinationFolderPath);
        iconsFolderPath = EditorGUILayout.TextField("Icons Folder", iconsFolderPath);

        itemScript = (MonoScript)EditorGUILayout.ObjectField("Item Script", itemScript, typeof(MonoScript), false);

        if (GUILayout.Button("Create Prefabs"))
        {
            CreatePrefabs();
        }
    }

    private void CreatePrefabs()
    {
        if (!Directory.Exists(sourceFolderPath))
        {
            Debug.LogError("Source folder does not exist: " + sourceFolderPath);
            return;
        }

        if (!Directory.Exists(destinationFolderPath))
        {
            Directory.CreateDirectory(destinationFolderPath);
        }

        if (!Directory.Exists(iconsFolderPath))
        {
            Debug.LogError("Icons folder does not exist: " + iconsFolderPath);
            return;
        }

        string[] fbxFiles = Directory.GetFiles(sourceFolderPath, "*.fbx");

        foreach (var fbxFile in fbxFiles)
        {
            string assetPath = fbxFile.Replace("\\", "/").Replace(Application.dataPath, "Assets");
            GameObject fbxObject = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            if (fbxObject != null)
            {
                GameObject instance = Instantiate(fbxObject);
                instance.name = fbxObject.name;

                if (instance.GetComponent<MeshCollider>() == null)
                {
                    instance.AddComponent<MeshCollider>();
                }

                if (instance.GetComponent<Rigidbody>() == null)
                {
                    instance.AddComponent<Rigidbody>();
                }

                var itemComponent = itemScript != null ? instance.AddComponent(itemScript.GetClass()) : null;

                if (itemComponent != null)
                {
                    // Assign the unique ItemId
                    var itemIdProperty = itemComponent.GetType().GetProperty("ItemId", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (itemIdProperty != null)
                    {
                        itemIdProperty.SetValue(itemComponent, itemIdCounter);
                        itemIdCounter++;
                    }

                    // Set ItemDescription to the prefab name
                    var itemDescriptionProperty = itemComponent.GetType().GetProperty("ItemDescription", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (itemDescriptionProperty != null)
                    {
                        itemDescriptionProperty.SetValue(itemComponent, instance.name);
                    }

                    // Set ItemIcon to the corresponding sprite
                    var itemIconProperty = itemComponent.GetType().GetProperty("ItemIcon", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (itemIconProperty != null)
                    {
                        string spritePath = Path.Combine(iconsFolderPath, instance.name + ".png");
                        spritePath = spritePath.Replace("\\", "/").Replace(Application.dataPath, "Assets");
                        Sprite itemIcon = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);

                        if (itemIcon != null)
                        {
                            itemIconProperty.SetValue(itemComponent, itemIcon);
                        }
                        else
                        {
                            Debug.LogWarning("Sprite not found at path: " + spritePath);
                        }
                    }
                }

                string prefabPath = Path.Combine(destinationFolderPath, instance.name + ".prefab");
                prefabPath = AssetDatabase.GenerateUniqueAssetPath(prefabPath);
                PrefabUtility.SaveAsPrefabAssetAndConnect(instance, prefabPath, InteractionMode.UserAction);

                DestroyImmediate(instance);
            }
            else
            {
                Debug.LogError("Failed to load FBX asset: " + assetPath);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Prefabs created successfully in: " + destinationFolderPath);
    }
}