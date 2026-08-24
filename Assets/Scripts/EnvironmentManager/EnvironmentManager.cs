
using UnityEngine;
using System.Collections.Generic;


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

    private Dictionary<GameObject, List<Vector3>> mazeDecorationPositions
    = new Dictionary<GameObject, List<Vector3>>();

    // =================================================
    // SPAWNED DECORATIONS
    // =================================================

    private bool[] mazeDecorationsSpawned;


    // =================================================
    // AWAKE
    // =================================================

    private void Awake()
    {
        Instance = this;
    }


    // =================================================
    // START
    // =================================================

    private void Start()
    {
        if (mazes != null)
        {
            mazeDecorationsSpawned =
                new bool[mazes.Length];
        }

       // ApplyEnvironment();
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


        foreach (EnvironmentData environment
                 in environments)
        {
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


        foreach (MazeGroup mazeGroup in mazes)
        {
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


                foreach (Renderer wall in walls)
                {
                    if (wall == null)
                        continue;


                    wall.material =
                        environment.wallMaterials[
                            Random.Range(
                                0,
                                environment.wallMaterials.Length
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
        if (mazes == null ||
            mazeIndex < 0 ||
            mazeIndex >= mazes.Length)
        {
            Debug.LogWarning(
                "EnvironmentManager: Invalid maze index: " +
                mazeIndex
            );

            return;
        }


        // =============================================
        // PREVENT DUPLICATE SPAWNING
        // =============================================

        if (mazeDecorationsSpawned != null &&
            mazeDecorationsSpawned[mazeIndex])
        {
            Debug.Log(
                "Decorations already spawned for Maze " +
                (mazeIndex + 1)
            );

            return;
        }


        // =============================================
        // SELECTED ENVIRONMENT
        // =============================================

        EnvironmentData environment =
            GetSelectedEnvironment();


        if (environment == null)
        {
            Debug.LogWarning(
                "EnvironmentManager: Environment not found!"
            );

            return;
        }


        // =============================================
        // ACTUAL MAZE
        // =============================================

        GameObject currentMaze =
            mazes[mazeIndex].maze;


        if (currentMaze == null)
        {
            Debug.LogWarning(
                "EnvironmentManager: Maze GameObject is not assigned at index " +
                mazeIndex
            );

            return;
        }


        // =============================================
        // FIND ENVIRONMENT MAZE DATA
        // =============================================

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

            return;
        }


        // =============================================
        // DECORATION GROUPS
        // =============================================

        if (mazeData.decorationGroups == null ||
            mazeData.decorationGroups.Length == 0)
        {
            Debug.Log(
                "No decoration groups assigned for " +
                currentMaze.name
            );

            MarkDecorationsSpawned(mazeIndex);

            return;
        }


        foreach (
            DecorationSpawnGroup group
            in mazeData.decorationGroups)
        {
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


            SpawnRandomNearWalls(
                group,
                currentMaze,
                mazes[mazeIndex]
            );
        }


        // =============================================
        // MARK AS SPAWNED
        // =============================================

        MarkDecorationsSpawned(mazeIndex);
    }


    // =================================================
    // RANDOM NEAR WALLS
    // =================================================

    private void SpawnRandomNearWalls(
        DecorationSpawnGroup group,
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

            return;
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

            return;
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

            return;
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

            return;
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

        if (!mazeDecorationPositions.ContainsKey(currentMaze))
        {
            mazeDecorationPositions[currentMaze] = new List<Vector3>();
        }


        List<Vector3> spawnedPositions =
            mazeDecorationPositions[currentMaze];


        // =============================================
        // SPAWN
        // =============================================

        int spawnedCount = 0;

        int attempts = 0;


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
                        wallColliders.Length
                    )
                ];


            if (wall == null)
                continue;


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


            Vector3 candidate;


            // -----------------------------------------
            // Choose wall side
            // -----------------------------------------

            int side =
                Random.Range(0, 4);


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

            if (!floorBounds.Contains(
                new Vector3(
                    candidate.x,
                    floorBounds.center.y,
                    candidate.z
                )))
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
                group.maxDistanceFromWall
            )
            {
                continue;
            }


            // =========================================
            // CHECK OTHER DECORATIONS
            // =========================================

            bool tooClose = false;


            foreach (
                Vector3 existingPosition
                in spawnedPositions)
            {
                if (
                    Vector3.Distance(
                        candidate,
                        existingPosition
                    )
                    <
                    group.minDistanceBetweenDecorations
                )
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


            // =========================================
            // SCALE
            // =========================================

            decoration.transform.localScale =
                group.prefab.transform.localScale;


            if (group.randomScale)
            {
                float scale =
                    Random.Range(
                        group.minScale,
                        group.maxScale
                    );


                decoration.transform.localScale *=
                    scale;
            }


            // =========================================
            // SAVE POSITION
            // =========================================

            spawnedPositions.Add(candidate);

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
    }


    // =================================================
    // CREATE DECORATION CONTAINER
    // =================================================

    private GameObject CreateDecorationContainer(
        GameObject prefab,
        GameObject currentMaze)
    {
        GameObject container =
            new GameObject(
                prefab.name +
                "_Spawned"
            );


        container.transform.SetParent(
            currentMaze.transform
        );


        return container;
    }


    // =================================================
    // FIND ENVIRONMENT MAZE
    // =================================================

    private EnvironmentMazeData FindEnvironmentMaze(
        EnvironmentData environment,
        GameObject maze)
    {
        if (environment.mazes == null)
            return null;


        foreach (
            EnvironmentMazeData mazeData
            in environment.mazes)
        {
            if (mazeData == null)
                continue;


            if (mazeData.maze == maze)
            {
                return mazeData;
            }
        }


        return null;
    }


    // =================================================
    // MARK DECORATIONS SPAWNED
    // =================================================

    private void MarkDecorationsSpawned(
        int mazeIndex)
    {
        if (mazeDecorationsSpawned == null)
            return;


        if (
            mazeIndex < 0 ||
            mazeIndex >= mazeDecorationsSpawned.Length
        )
            return;


        mazeDecorationsSpawned[mazeIndex] = true;
    }


    // =================================================
    // RESET DECORATION STATUS
    // =================================================

    public void ResetDecorationStatus()
    {
        if (mazeDecorationsSpawned == null)
            return;


        for (
            int i = 0;
            i < mazeDecorationsSpawned.Length;
            i++)
        {
            mazeDecorationsSpawned[i] = false;
        }
    }

    public void SetEnvironment(EnvironmentType newEnvironment)
    {
        environmentType = newEnvironment;

        ApplyEnvironment();

        Debug.Log(
            "Environment changed to: " +
            newEnvironment
        );
    }
}