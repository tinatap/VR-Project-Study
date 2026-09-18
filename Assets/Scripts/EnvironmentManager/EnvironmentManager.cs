using UnityEngine;
using System.Collections.Generic;
using System.IO;


// =====================================================
// DECORATION SPAWN GROUP
// =====================================================

[System.Serializable]
public class DecorationSpawnGroup
{
    [Header("Decoration Prefab")]
    public GameObject prefab;

    [Header("Random Spawn Settings")]

    [Tooltip("Number of decorations to spawn")]
    public int spawnCount = 10;

    [Tooltip("Minimum distance from wall")]
    public float minDistanceFromWall = 1f;

    [Tooltip("Maximum distance from wall")]
    public float maxDistanceFromWall = 2.5f;

    [Tooltip("Minimum distance between decorations")]
    public float minDistanceBetweenDecorations = 2f;

    [Tooltip("Maximum attempts to find valid positions")]
    public int maxSpawnAttempts = 500;

    [Header("Rotation")]

    [Tooltip("Random Y rotation")]
    public bool randomRotation = true;

    [Header("Scale")]

    [Tooltip("Randomize scale")]
    public bool randomScale = false;

    public float minScale = 0.8f;
    public float maxScale = 1.2f;
}


// =====================================================
// ENVIRONMENT MAZE DATA
// =====================================================

[System.Serializable]
public class EnvironmentMazeData
{
    [Header("Maze")]
    [Tooltip("Drag the actual Maze GameObject from the Hierarchy")]
    public GameObject maze;

    [Header("Decoration Groups")]
    public DecorationSpawnGroup[] decorationGroups;
}


// =====================================================
// ENVIRONMENT DATA
// =====================================================

[System.Serializable]
public class EnvironmentData
{
    [Header("Environment Name")]
    public string environmentName;

    [Header("Wall Materials")]
    public Material[] wallMaterials;

    [Header("Floor Material")]
    public Material floorMaterial;

    [Header("Skybox")]
    public Material skyboxMaterial;

    [Header("Maze Decorations")]
    public EnvironmentMazeData[] mazes;
}


// =====================================================
// MAZE GROUP
// =====================================================

[System.Serializable]
public class MazeGroup
{
    [Header("Maze")]
    public GameObject maze;

    [Header("Walls Parent")]
    public Transform wallsParent;

    [Header("Floor")]
    public Renderer floor;
}


// =====================================================
// SAVED DECORATION DATA
// =====================================================

[System.Serializable]
public class SavedDecoration
{
    public Vector3 localPosition;
    public Quaternion localRotation;
    public Vector3 localScale;
}


// =====================================================
// SAVED GROUP DATA
// =====================================================

[System.Serializable]
public class SavedDecorationGroup
{
    public int groupIndex;
    public string prefabName;

    public List<SavedDecoration> decorations =
        new List<SavedDecoration>();
}


// =====================================================
// SAVED MAZE DATA
// =====================================================

[System.Serializable]
public class SavedMazeDecorations
{
    public string environmentName;
    public string mazeName;
    public int mazeIndex;

    public List<SavedDecorationGroup> groups =
        new List<SavedDecorationGroup>();
}


// =====================================================
// ALL SAVED DECORATIONS
// =====================================================

[System.Serializable]
public class DecorationSaveFile
{
    public List<SavedMazeDecorations> mazes =
        new List<SavedMazeDecorations>();
}


// =====================================================
// ENVIRONMENT MANAGER
// =====================================================

public class EnvironmentManager : MonoBehaviour
{
    public static EnvironmentManager Instance;


    // =================================================
    // ENVIRONMENT TYPE
    // =================================================

    public enum EnvironmentType
    {
        Neutral,
        Desert,
        Galaxy,
        Park
    }


    [Header("Choose Environment")]
    public EnvironmentType environmentType;


    // =================================================
    // ENVIRONMENTS
    // =================================================

    [Header("Environment Settings")]
    public EnvironmentData[] environments;


    // =================================================
    // MAZES
    // =================================================

    [Header("Mazes")]
    public MazeGroup[] mazes;


    // =================================================
    // RUNTIME DECORATION POSITIONS
    // =================================================

    private Dictionary<GameObject, List<Vector3>>
        mazeDecorationPositions =
        new Dictionary<GameObject, List<Vector3>>();


    // =================================================
    // SPAWNED DECORATIONS
    // =================================================

    private bool[] mazeDecorationsSpawned;


    // =================================================
    // SAVE FILE
    // =================================================

    private DecorationSaveFile saveFile;

    private string SavePath
    {
        get
        {
            return Path.Combine(
                Application.persistentDataPath,
                "EnvironmentDecorations.json"
            );
        }
    }


    // =================================================
    // AWAKE
    // =================================================

    private void Awake()
    {
        Instance = this;

        LoadDecorationSaveFile();
    }


    // =================================================
    // START
    // =================================================

    private void Start()
    {
        InitializeMazeDecorationStatus();

        ApplyEnvironment();
    }


    // =================================================
    // INITIALIZE RUNTIME STATUS
    // =================================================

    private void InitializeMazeDecorationStatus()
    {
        int mazeCount =
            mazes != null
                ? mazes.Length
                : 0;

        mazeDecorationsSpawned =
            new bool[mazeCount];
    }


    // =================================================
    // APPLY ENVIRONMENT
    // =================================================

    public void ApplyEnvironment()
    {
        EnvironmentData environment =
            GetSelectedEnvironment();

        if (environment == null)
        {
            Debug.LogWarning(
                "EnvironmentManager: Selected environment was not found!"
            );

            return;
        }

        ApplyMaterials(environment);
        ApplySky(environment);

        Debug.Log(
            "Environment applied: " +
            environment.environmentName
        );
    }


    // =================================================
    // GET SELECTED ENVIRONMENT
    // =================================================

    private EnvironmentData GetSelectedEnvironment()
    {
        if (environments == null ||
            environments.Length == 0)
        {
            Debug.LogWarning(
                "EnvironmentManager: No environments assigned!"
            );

            return null;
        }

        string selectedName =
            environmentType.ToString();

        for (int i = 0; i < environments.Length; i++)
        {
            EnvironmentData environment =
                environments[i];

            if (environment == null)
                continue;

            if (environment.environmentName ==
                selectedName)
            {
                return environment;
            }
        }

        Debug.LogWarning(
            "Environment not found: " +
            selectedName
        );

        return null;
    }


    // =================================================
    // APPLY MATERIALS
    // =================================================

    private void ApplyMaterials(
        EnvironmentData environment)
    {
        if (mazes == null)
            return;

        for (int i = 0; i < mazes.Length; i++)
        {
            MazeGroup mazeGroup =
                mazes[i];

            if (mazeGroup == null)
                continue;


            // =========================================
            // WALLS
            // =========================================

            if (mazeGroup.wallsParent != null &&
                environment.wallMaterials != null &&
                environment.wallMaterials.Length > 0)
            {
                Renderer[] walls =
                    mazeGroup.wallsParent
                    .GetComponentsInChildren<Renderer>(true);

                Material[] wallMaterials =
                    environment.wallMaterials;

                int materialCount =
                    wallMaterials.Length;

                for (int j = 0; j < walls.Length; j++)
                {
                    Renderer wall = walls[j];

                    if (wall == null)
                        continue;

                    wall.material =
                        wallMaterials[
                            Random.Range(
                                0,
                                materialCount
                            )
                        ];
                }
            }


            // =========================================
            // FLOOR
            // =========================================

            if (mazeGroup.floor != null &&
                environment.floorMaterial != null)
            {
                mazeGroup.floor.material =
                    environment.floorMaterial;
            }
        }
    }


    // =================================================
    // APPLY SKY
    // =================================================

    private void ApplySky(
        EnvironmentData environment)
    {
        if (environment.skyboxMaterial == null)
        {
            Debug.LogWarning(
                "No Skybox assigned for environment: " +
                environment.environmentName
            );

            return;
        }

        RenderSettings.skybox =
            environment.skyboxMaterial;

        DynamicGI.UpdateEnvironment();
    }


    // =================================================
    // APPLY DECORATIONS FOR SPECIFIC MAZE
    // =================================================

    public void ApplyDecorationsForMaze(
        int mazeIndex)
    {
        if (!IsValidMazeIndex(mazeIndex))
        {
            Debug.LogWarning(
                "EnvironmentManager: Invalid maze index: " +
                mazeIndex
            );

            return;
        }

        EnvironmentData environment =
            GetSelectedEnvironment();

        if (environment == null)
        {
            Debug.LogWarning(
                "EnvironmentManager: Environment not found!"
            );

            return;
        }

        MazeGroup mazeGroup =
            mazes[mazeIndex];

        GameObject currentMaze =
            mazeGroup.maze;

        if (currentMaze == null)
        {
            Debug.LogWarning(
                "EnvironmentManager: Maze GameObject is not assigned at index " +
                mazeIndex
            );

            return;
        }


        // =================================================
        // REMOVE CURRENT ENVIRONMENT DECORATIONS
        // =================================================

        ClearSpawnedDecorations(currentMaze);


        // =================================================
        // FIND ENVIRONMENT MAZE DATA
        // =================================================

        EnvironmentMazeData mazeData =
            FindEnvironmentMaze(
                environment,
                currentMaze
            );

        if (mazeData == null)
        {
            Debug.LogWarning(
                "No decoration data found for Maze: " +
                currentMaze.name +
                " in Environment: " +
                environment.environmentName
            );

            MarkDecorationsSpawned(mazeIndex);

            return;
        }


        // =================================================
        // CHECK SAVED LAYOUT
        // =================================================

        SavedMazeDecorations savedMaze =
            FindSavedMaze(
                environment.environmentName,
                currentMaze.name,
                mazeIndex
            );

        if (savedMaze != null)
        {
            Debug.Log(
                "Loading SAVED decorations for Environment: " +
                environment.environmentName +
                " | Maze: " +
                currentMaze.name
            );

            LoadDecorationsForMaze(
                savedMaze,
                currentMaze
            );

            MarkDecorationsSpawned(mazeIndex);

            return;
        }


        // =================================================
        // FIRST TIME
        // GENERATE RANDOM DECORATIONS
        // =================================================

        Debug.Log(
            "No saved decoration layout found. " +
            "Generating NEW layout for: " +
            environment.environmentName +
            " | Maze: " +
            currentMaze.name
        );

        SavedMazeDecorations newSavedMaze =
            new SavedMazeDecorations
            {
                environmentName =
                    environment.environmentName,

                mazeName =
                    currentMaze.name,

                mazeIndex =
                    mazeIndex
            };


        if (mazeData.decorationGroups == null ||
            mazeData.decorationGroups.Length == 0)
        {
            Debug.Log(
                "No decoration groups assigned for " +
                currentMaze.name
            );

            AddSavedMaze(newSavedMaze);
            MarkDecorationsSpawned(mazeIndex);

            return;
        }


        for (
            int groupIndex = 0;
            groupIndex <
            mazeData.decorationGroups.Length;
            groupIndex++)
        {
            DecorationSpawnGroup group =
                mazeData.decorationGroups[groupIndex];

            if (group == null)
                continue;

            if (group.prefab == null)
            {
                Debug.LogWarning(
                    "Decoration prefab is missing for Maze: " +
                    currentMaze.name
                );

                continue;
            }

            SavedDecorationGroup savedGroup =
                SpawnRandomNearWallsAndSave(
                    group,
                    groupIndex,
                    currentMaze,
                    mazeGroup
                );

            if (savedGroup != null)
            {
                newSavedMaze.groups.Add(
                    savedGroup
                );
            }
        }


        // =================================================
        // SAVE EVERYTHING
        // =================================================

        AddSavedMaze(newSavedMaze);

        SaveDecorationFile();

        MarkDecorationsSpawned(mazeIndex);

        Debug.Log(
            "NEW decoration layout SAVED permanently: " +
            environment.environmentName +
            " | Maze: " +
            currentMaze.name
        );
    }


    // =====================================================
    // SPAWN RANDOM AND SAVE
    // =====================================================

    private SavedDecorationGroup
        SpawnRandomNearWallsAndSave(
            DecorationSpawnGroup group,
            int groupIndex,
            GameObject currentMaze,
            MazeGroup mazeGroup)
    {
        // =============================================
        // CHECK WALLS
        // =============================================

        if (mazeGroup.wallsParent == null)
        {
            Debug.LogWarning(
                "Walls Parent is missing for Maze: " +
                currentMaze.name
            );

            return null;
        }


        // =============================================
        // CHECK FLOOR
        // =============================================

        if (mazeGroup.floor == null)
        {
            Debug.LogWarning(
                "Floor Renderer is missing for Maze: " +
                currentMaze.name
            );

            return null;
        }


        // =============================================
        // CHECK COUNT
        // =============================================

        if (group.spawnCount <= 0)
        {
            Debug.LogWarning(
                "Spawn Count must be greater than 0 for: " +
                group.prefab.name
            );

            return null;
        }


        // =============================================
        // GET WALL COLLIDERS
        // =============================================

        Collider[] wallColliders =
            mazeGroup.wallsParent
            .GetComponentsInChildren<Collider>(true);

        if (wallColliders.Length == 0)
        {
            Debug.LogWarning(
                "No Colliders found under Walls Parent: " +
                mazeGroup.wallsParent.name
            );

            return null;
        }


        // =============================================
        // CREATE CONTAINER
        // =============================================

        GameObject decorationContainer =
            CreateDecorationContainer(
                group.prefab,
                currentMaze
            );


        // =============================================
        // FLOOR BOUNDS
        // =============================================

        Bounds floorBounds =
            mazeGroup.floor.bounds;


        // =============================================
        // SPAWNED POSITIONS
        // =============================================

        List<Vector3> spawnedPositions =
            new List<Vector3>(
                group.spawnCount
            );


        // =============================================
        // SAVED GROUP
        // =============================================

        SavedDecorationGroup savedGroup =
            new SavedDecorationGroup
            {
                groupIndex = groupIndex,
                prefabName = group.prefab.name
            };


        // =============================================
        // SPAWN
        // =============================================

        int spawnedCount = 0;
        int attempts = 0;

        int wallCount =
            wallColliders.Length;

        while (
            spawnedCount < group.spawnCount &&
            attempts < group.maxSpawnAttempts)
        {
            attempts++;


            // -----------------------------------------
            // Choose random wall
            // -----------------------------------------

            Collider wall =
                wallColliders[
                    Random.Range(
                        0,
                        wallCount
                    )
                ];

            if (wall == null)
                continue;


            // -----------------------------------------
            // Wall bounds
            // -----------------------------------------

            Bounds wallBounds =
                wall.bounds;


            // -----------------------------------------
            // Random distance
            // -----------------------------------------

            float offset =
                Random.Range(
                    group.minDistanceFromWall,
                    group.maxDistanceFromWall
                );


            // -----------------------------------------
            // Random wall side
            // -----------------------------------------

            int side =
                Random.Range(0, 4);

            Vector3 candidate;

            switch (side)
            {
                case 0:

                    candidate = new Vector3(
                        wallBounds.min.x - offset,
                        floorBounds.max.y,
                        Random.Range(
                            wallBounds.min.z,
                            wallBounds.max.z
                        )
                    );

                    break;


                case 1:

                    candidate = new Vector3(
                        wallBounds.max.x + offset,
                        floorBounds.max.y,
                        Random.Range(
                            wallBounds.min.z,
                            wallBounds.max.z
                        )
                    );

                    break;


                case 2:

                    candidate = new Vector3(
                        Random.Range(
                            wallBounds.min.x,
                            wallBounds.max.x
                        ),
                        floorBounds.max.y,
                        wallBounds.min.z - offset
                    );

                    break;


                default:

                    candidate = new Vector3(
                        Random.Range(
                            wallBounds.min.x,
                            wallBounds.max.x
                        ),
                        floorBounds.max.y,
                        wallBounds.max.z + offset
                    );

                    break;
            }


            // =========================================
            // CHECK FLOOR
            // =========================================

            Vector3 floorCheckPoint =
                new Vector3(
                    candidate.x,
                    floorBounds.center.y,
                    candidate.z
                );

            if (!floorBounds.Contains(
                floorCheckPoint))
            {
                continue;
            }


            // =========================================
            // CHECK WALL DISTANCE
            // =========================================

            Vector3 closestPoint =
                wall.ClosestPoint(candidate);

            float wallDistance =
                Vector3.Distance(
                    candidate,
                    closestPoint
                );

            if (
                wallDistance <
                group.minDistanceFromWall ||

                wallDistance >
                group.maxDistanceFromWall)
            {
                continue;
            }


            // =========================================
            // CHECK OTHER DECORATIONS
            // =========================================

            bool tooClose = false;

            float minDistance =
                group.minDistanceBetweenDecorations;

            for (
                int i = 0;
                i < spawnedPositions.Count;
                i++)
            {
                if (
                    Vector3.Distance(
                        candidate,
                        spawnedPositions[i]
                    ) < minDistance)
                {
                    tooClose = true;
                    break;
                }
            }

            if (tooClose)
                continue;


            // =========================================
            // ROTATION
            // =========================================

            Quaternion rotation =
                group.prefab.transform.rotation;

            if (group.randomRotation)
            {
                rotation =
                    Quaternion.Euler(
                        0f,
                        Random.Range(
                            0f,
                            360f
                        ),
                        0f
                    );
            }


            // =========================================
            // SCALE
            // =========================================

            Vector3 finalScale =
                group.prefab.transform.localScale;

            if (group.randomScale)
            {
                float scale =
                    Random.Range(
                        group.minScale,
                        group.maxScale
                    );

                finalScale *= scale;
            }


            // =========================================
            // CREATE DECORATION
            // =========================================

            GameObject decoration =
                Instantiate(
                    group.prefab,
                    candidate,
                    rotation
                );

            decoration.transform.SetParent(
                decorationContainer.transform,
                true
            );

            decoration.transform.localScale =
                finalScale;


            // =========================================
            // SAVE LOCAL TRANSFORM
            // =========================================

            SavedDecoration savedDecoration =
                new SavedDecoration
                {
                    localPosition =
                        decoration.transform.localPosition,

                    localRotation =
                        decoration.transform.localRotation,

                    localScale =
                        decoration.transform.localScale
                };

            savedGroup.decorations.Add(
                savedDecoration
            );


            // =========================================
            // ADD POSITION
            // =========================================

            spawnedPositions.Add(
                candidate
            );

            spawnedCount++;
        }


        Debug.Log(
            "Decoration: " +
            group.prefab.name +
            " | Spawned: " +
            spawnedCount +
            " / " +
            group.spawnCount +
            " | Attempts: " +
            attempts
        );

        return savedGroup;
    }


    // =====================================================
    // LOAD SAVED DECORATIONS
    // =====================================================

    private void LoadDecorationsForMaze(
        SavedMazeDecorations savedMaze,
        GameObject currentMaze)
    {
        if (savedMaze == null ||
            savedMaze.groups == null)
        {
            return;
        }

        foreach (
            SavedDecorationGroup savedGroup
            in savedMaze.groups)
        {
            if (savedGroup == null)
                continue;

            GameObject prefab =
                FindPrefabForSavedGroup(
                    savedMaze.environmentName,
                    savedMaze.mazeName,
                    savedGroup.groupIndex
                );

            if (prefab == null)
            {
                Debug.LogWarning(
                    "Could not find prefab for saved group: " +
                    savedGroup.prefabName
                );

                continue;
            }

            GameObject container =
                CreateDecorationContainer(
                    prefab,
                    currentMaze
                );

            if (savedGroup.decorations == null)
                continue;

            foreach (
                SavedDecoration savedDecoration
                in savedGroup.decorations)
            {
                if (savedDecoration == null)
                    continue;

                GameObject decoration =
                    Instantiate(
                        prefab,
                        container.transform
                    );

                Transform transform =
                    decoration.transform;

                transform.localPosition =
                    savedDecoration.localPosition;

                transform.localRotation =
                    savedDecoration.localRotation;

                transform.localScale =
                    savedDecoration.localScale;
            }
        }
    }


    // =====================================================
    // FIND PREFAB FROM ENVIRONMENT DATA
    // =====================================================

    private GameObject FindPrefabForSavedGroup(
        string environmentName,
        string mazeName,
        int groupIndex)
    {
        EnvironmentData environment =
            FindEnvironmentByName(environmentName);

        if (environment == null ||
            environment.mazes == null)
        {
            return null;
        }

        foreach (
            EnvironmentMazeData mazeData
            in environment.mazes)
        {
            if (mazeData == null ||
                mazeData.maze == null)
            {
                continue;
            }

            if (mazeData.maze.name != mazeName)
                continue;

            if (mazeData.decorationGroups == null)
                return null;

            if (
                groupIndex < 0 ||
                groupIndex >=
                mazeData.decorationGroups.Length)
            {
                return null;
            }

            DecorationSpawnGroup group =
                mazeData.decorationGroups[groupIndex];

            if (group == null)
                return null;

            return group.prefab;
        }

        return null;
    }


    // =====================================================
    // FIND ENVIRONMENT BY NAME
    // =====================================================

    private EnvironmentData FindEnvironmentByName(
        string environmentName)
    {
        if (environments == null)
            return null;

        for (int i = 0; i < environments.Length; i++)
        {
            EnvironmentData environment =
                environments[i];

            if (environment == null)
                continue;

            if (environment.environmentName ==
                environmentName)
            {
                return environment;
            }
        }

        return null;
    }


    // =====================================================
    // CREATE DECORATION CONTAINER
    // =====================================================

    private GameObject CreateDecorationContainer(
        GameObject prefab,
        GameObject currentMaze)
    {
        GameObject container =
            new GameObject(
                prefab.name +
                "_Spawned"
            );

        Transform containerTransform =
            container.transform;

        containerTransform.SetParent(
            currentMaze.transform,
            false
        );

        return container;
    }


    // =====================================================
    // CLEAR CURRENT DECORATIONS
    // =====================================================

    private void ClearSpawnedDecorations(
        GameObject currentMaze)
    {
        if (currentMaze == null)
            return;

        List<GameObject> containers =
            new List<GameObject>();

        Transform mazeTransform =
            currentMaze.transform;

        int childCount =
            mazeTransform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            Transform child =
                mazeTransform.GetChild(i);

            if (child == null)
                continue;

            if (child.name.EndsWith(
                "_Spawned"))
            {
                containers.Add(
                    child.gameObject
                );
            }
        }

        for (int i = 0; i < containers.Count; i++)
        {
            GameObject container =
                containers[i];

            if (Application.isPlaying)
            {
                Destroy(container);
            }
            else
            {
                DestroyImmediate(container);
            }
        }
    }


    // =====================================================
    // FIND ENVIRONMENT MAZE
    // =====================================================

    private EnvironmentMazeData FindEnvironmentMaze(
        EnvironmentData environment,
        GameObject maze)
    {
        if (environment == null ||
            environment.mazes == null)
        {
            return null;
        }

        foreach (
            EnvironmentMazeData mazeData
            in environment.mazes)
        {
            if (mazeData == null)
                continue;

            if (mazeData.maze == maze)
                return mazeData;
        }

        return null;
    }


    // =====================================================
    // FIND SAVED MAZE
    // =====================================================

    private SavedMazeDecorations FindSavedMaze(
        string environmentName,
        string mazeName,
        int mazeIndex)
    {
        if (saveFile == null ||
            saveFile.mazes == null)
        {
            return null;
        }

        foreach (
            SavedMazeDecorations savedMaze
            in saveFile.mazes)
        {
            if (savedMaze == null)
                continue;

            if (
                savedMaze.environmentName ==
                environmentName &&

                savedMaze.mazeName ==
                mazeName &&

                savedMaze.mazeIndex ==
                mazeIndex)
            {
                return savedMaze;
            }
        }

        return null;
    }


    // =====================================================
    // ADD SAVED MAZE
    // =====================================================

    private void AddSavedMaze(
        SavedMazeDecorations newMaze)
    {
        if (newMaze == null)
            return;

        if (saveFile == null)
        {
            saveFile =
                new DecorationSaveFile();
        }

        if (saveFile.mazes == null)
        {
            saveFile.mazes =
                new List<SavedMazeDecorations>();
        }


        // Remove previous version
        for (
            int i = saveFile.mazes.Count - 1;
            i >= 0;
            i--)
        {
            SavedMazeDecorations oldMaze =
                saveFile.mazes[i];

            if (oldMaze == null)
                continue;

            if (
                oldMaze.environmentName ==
                newMaze.environmentName &&

                oldMaze.mazeName ==
                newMaze.mazeName &&

                oldMaze.mazeIndex ==
                newMaze.mazeIndex)
            {
                saveFile.mazes.RemoveAt(i);
            }
        }

        saveFile.mazes.Add(newMaze);
    }


    // =====================================================
    // LOAD SAVE FILE
    // =====================================================

    private void LoadDecorationSaveFile()
    {
        try
        {
            if (!File.Exists(SavePath))
            {
                saveFile =
                    new DecorationSaveFile();

                Debug.Log(
                    "No decoration save file found. " +
                    "A new one will be created."
                );

                return;
            }

            string json =
                File.ReadAllText(SavePath);

            if (string.IsNullOrEmpty(json))
            {
                saveFile =
                    new DecorationSaveFile();

                return;
            }

            saveFile =
                JsonUtility.FromJson<DecorationSaveFile>(
                    json
                );

            if (saveFile == null)
            {
                saveFile =
                    new DecorationSaveFile();
            }

            if (saveFile.mazes == null)
            {
                saveFile.mazes =
                    new List<SavedMazeDecorations>();
            }

            Debug.Log(
                "Decoration save file loaded: " +
                SavePath
            );
        }
        catch (System.Exception e)
        {
            Debug.LogError(
                "Failed to load decoration save file: " +
                e.Message
            );

            saveFile =
                new DecorationSaveFile();
        }
    }


    // =====================================================
    // SAVE TO DISK
    // =====================================================

    private void SaveDecorationFile()
    {
        try
        {
            if (saveFile == null)
            {
                saveFile =
                    new DecorationSaveFile();
            }

            string json =
                JsonUtility.ToJson(
                    saveFile,
                    true
                );

            File.WriteAllText(
                SavePath,
                json
            );

            Debug.Log(
                "Decoration layout saved to: " +
                SavePath
            );
        }
        catch (System.Exception e)
        {
            Debug.LogError(
                "Failed to save decoration file: " +
                e.Message
            );
        }
    }


    // =====================================================
    // MARK DECORATIONS SPAWNED
    // =====================================================

    private void MarkDecorationsSpawned(
        int mazeIndex)
    {
        if (mazeDecorationsSpawned == null)
            return;

        if (
            mazeIndex < 0 ||
            mazeIndex >=
            mazeDecorationsSpawned.Length)
        {
            return;
        }

        mazeDecorationsSpawned[mazeIndex] =
            true;
    }


    // =====================================================
    // RESET RUNTIME STATUS
    // =====================================================

    public void ResetDecorationStatus()
    {
        if (mazeDecorationsSpawned == null)
            return;

        System.Array.Clear(
            mazeDecorationsSpawned,
            0,
            mazeDecorationsSpawned.Length
        );
    }


    // =====================================================
    // CHANGE ENVIRONMENT
    // =====================================================

    public void SetEnvironment(
        EnvironmentType newEnvironment)
    {
        environmentType =
            newEnvironment;


        // Clear currently visible decorations

        if (mazes != null)
        {
            for (int i = 0; i < mazes.Length; i++)
            {
                MazeGroup mazeGroup =
                    mazes[i];

                if (
                    mazeGroup != null &&
                    mazeGroup.maze != null)
                {
                    ClearSpawnedDecorations(
                        mazeGroup.maze
                    );
                }
            }
        }


        ResetDecorationStatus();

        ApplyEnvironment();

        Debug.Log(
            "Environment changed to: " +
            newEnvironment
        );
    }


    // =====================================================
    // DELETE ALL SAVED DECORATIONS
    // =====================================================

    public void DeleteAllSavedDecorationLayouts()
    {
        try
        {
            if (File.Exists(SavePath))
            {
                File.Delete(SavePath);
            }

            saveFile =
                new DecorationSaveFile();

            ResetDecorationStatus();


            if (mazes != null)
            {
                for (int i = 0; i < mazes.Length; i++)
                {
                    MazeGroup mazeGroup =
                        mazes[i];

                    if (
                        mazeGroup != null &&
                        mazeGroup.maze != null)
                    {
                        ClearSpawnedDecorations(
                            mazeGroup.maze
                        );
                    }
                }
            }

            Debug.Log(
                "ALL saved decoration layouts deleted."
            );
        }
        catch (System.Exception e)
        {
            Debug.LogError(
                "Failed to delete decoration save file: " +
                e.Message
            );
        }
    }


    // =====================================================
    // REGENERATE CURRENT ENVIRONMENT
    // =====================================================

    public void RegenerateCurrentEnvironment()
    {
        EnvironmentData environment =
            GetSelectedEnvironment();

        if (environment == null)
            return;

        if (saveFile == null ||
            saveFile.mazes == null)
        {
            saveFile =
                new DecorationSaveFile();

            return;
        }


        // Remove all saved layouts
        // belonging to current environment

        for (
            int i = saveFile.mazes.Count - 1;
            i >= 0;
            i--)
        {
            SavedMazeDecorations savedMaze =
                saveFile.mazes[i];

            if (savedMaze == null)
                continue;

            if (
                savedMaze.environmentName ==
                environment.environmentName)
            {
                saveFile.mazes.RemoveAt(i);
            }
        }


        SaveDecorationFile();

        ResetDecorationStatus();


        // Clear current visible decorations

        if (mazes != null)
        {
            for (int i = 0; i < mazes.Length; i++)
            {
                MazeGroup mazeGroup =
                    mazes[i];

                if (
                    mazeGroup != null &&
                    mazeGroup.maze != null)
                {
                    ClearSpawnedDecorations(
                        mazeGroup.maze
                    );
                }
            }
        }


        Debug.Log(
            "Decoration layouts regenerated for environment: " +
            environment.environmentName
        );
    }


    // =====================================================
    // VALIDATE MAZE INDEX
    // =====================================================

    private bool IsValidMazeIndex(int mazeIndex)
    {
        return
            mazes != null &&
            mazeIndex >= 0 &&
            mazeIndex < mazes.Length;
    }
}