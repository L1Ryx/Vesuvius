using Events._Arch;
using UnityEngine;
using UnityEditor;
using _Gameplay._Arch;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class SpawnLocation : MonoBehaviour
{
    [SerializeField] private string SceneComingFrom;
    [SerializeField] private DoorController doorBackToPrevious;
    [SerializeField] private TransitionData transitionData;
    [SerializeField] private bool isVerticalUpTransition;


    [MenuItem("Tools/Update Door Transitions In Scene")]
    public static void UpdateSpawnLocations()
    {
        print("Updating");
        UpdateSpawnerTransition();
        UpdateDoorTransition();
    }

    private static void UpdateSpawnerTransition()
    {
        SpawnLocation[] components = FindObjectsByType<SpawnLocation>(FindObjectsSortMode.None);
        foreach (SpawnLocation spawnLocation in components)
        {
            //skip this iteration if not set because it will cause errors down the line.
            if (spawnLocation.SceneComingFrom == null)
            {
                Debug.LogError($"No scene data set on " + spawnLocation.gameObject.name);
                continue;
            }
            string transitionName = "To_" + EditorSceneManager.GetActiveScene().name +
                              "_From_" + spawnLocation.SceneComingFrom;
            string path = "Assets/Level Design/SceneTransitions/" +
                              transitionName + ".asset";

            //create a transition for this spawn location if one does not exist or if the data has changed
            if (spawnLocation.transitionData == null || spawnLocation.transitionData.name != transitionName)
            {
                //if for some reason the data was already created but was set to null then we can reload it
                TransitionData alreadyCreated = AssetDatabase.LoadAssetAtPath<TransitionData>(path);
                if (alreadyCreated != null)
                {
                    spawnLocation.transitionData = alreadyCreated;
                }
                else
                {
                    TransitionData asset = ScriptableObject.CreateInstance<TransitionData>();

                    path = AssetDatabase.GenerateUniqueAssetPath(path);

                    AssetDatabase.CreateAsset(asset, path);
                    AssetDatabase.SaveAssets();

                    //delete the old asset
                    if (spawnLocation.transitionData != null)
                    {
                        AssetDatabase.DeleteAsset("Assets/Level Design/SceneTransitions/" + spawnLocation.transitionData.name + ".asset");
                    }

                    spawnLocation.transitionData = asset;
                    EditorUtility.SetDirty(spawnLocation);
                }

            }

            //Update data in transition datacube if there is any change
            if (spawnLocation.transitionData.sceneToLoad != EditorSceneManager.GetActiveScene().name ||
            spawnLocation.transitionData.spawnPosition != (Vector2)spawnLocation.gameObject.transform.position ||
            spawnLocation.transitionData.isVerticalUpTransition != spawnLocation.isVerticalUpTransition)
            {
                spawnLocation.transitionData.sceneToLoad = EditorSceneManager.GetActiveScene().name;
                spawnLocation.transitionData.spawnPosition = (Vector2)spawnLocation.gameObject.transform.position;
                spawnLocation.transitionData.isVerticalUpTransition = spawnLocation.isVerticalUpTransition;
                EditorUtility.SetDirty(spawnLocation);
                EditorUtility.SetDirty(spawnLocation.transitionData);
            }
        }
    }

    private static void UpdateDoorTransition()
    {
        SpawnLocation[] components = FindObjectsByType<SpawnLocation>(FindObjectsSortMode.None);
        foreach (SpawnLocation spawnLocation in components)
        {
            //skip this iteration if not set because it will cause errors down the line.
            if (spawnLocation.SceneComingFrom == null)
            {
                Debug.LogError($"No scene data set on " + spawnLocation.gameObject.name);
                continue;
            }
            string transitionBackName = "To_" + spawnLocation.SceneComingFrom +
                              "_From_" + EditorSceneManager.GetActiveScene().name;
            string transitionBackPath = "Assets/Level Design/SceneTransitions/" +
                               transitionBackName + ".asset";

            TransitionData transitionBack = AssetDatabase.LoadAssetAtPath<TransitionData>(transitionBackPath);

            //make sure transition data exists
            if (transitionBack != null)
            {
                //if the door is null or different then update the data and set dirty
                if (spawnLocation.doorBackToPrevious.transitionData == null || spawnLocation.doorBackToPrevious.transitionData.name != transitionBackName)
                {
                    spawnLocation.doorBackToPrevious.transitionData = transitionBack;
                    EditorUtility.SetDirty(spawnLocation.doorBackToPrevious);
                }
            }
            else
            {
                Debug.LogError($"No transition data at path: {transitionBackPath}" +
                                $"\n Have you generated transitions in scene: { spawnLocation.SceneComingFrom}");
            }


        }
    }

    [MenuItem("Tools/Update all Door Transitions In Project")]
    public static void UpdateAllSpawns()
    {
        // Store the currently active scene to return to it later
        Scene activeScene = EditorSceneManager.GetActiveScene();
        string activeScenePath = activeScene.path;

        Debug.Log("Starting scene iteration...");

        // Iterate through all scenes listed in the Build Settings and create transitions
        foreach (EditorBuildSettingsScene sceneInBuildSettings in EditorBuildSettings.scenes)
        {
            if (sceneInBuildSettings.enabled) // Only process enabled scenes
            {
                string scenePath = sceneInBuildSettings.path;
                Debug.Log($"Processing scene: {scenePath}");

                Scene openedScene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

                UpdateSpawnerTransition();
            
                EditorSceneManager.SaveScene(openedScene);

                EditorSceneManager.CloseScene(openedScene, true);
            }
        }

        //Iterate through all scenes again and link doors to transitions
        foreach (EditorBuildSettingsScene sceneInBuildSettings in EditorBuildSettings.scenes)
        {
            if (sceneInBuildSettings.enabled) // Only process enabled scenes
            {
                string scenePath = sceneInBuildSettings.path;
                Debug.Log($"Processing scene: {scenePath}");

                Scene openedScene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

                UpdateDoorTransition();

                EditorSceneManager.SaveScene(openedScene);

                EditorSceneManager.CloseScene(openedScene, true);
            }
        }

        // After iteration, reload the original active scene
        if (!string.IsNullOrEmpty(activeScenePath))
        {
            EditorSceneManager.OpenScene(activeScenePath, OpenSceneMode.Single);
            Debug.Log($"Returned to original scene: {activeScenePath}");
        }

        Debug.Log("Scene iteration completed.");
    }
}
