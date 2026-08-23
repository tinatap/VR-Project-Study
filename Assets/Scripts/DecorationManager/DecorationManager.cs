using UnityEngine;

[System.Serializable]
public class MazeDecoration
{
    public string mazeName;

    [Header("Spawn Points Parent")]
    public Transform spawnPointsParent;
}

public class DecorationManager : MonoBehaviour
{
    [Header("Cactus Prefab")]
    public GameObject cactusPrefab;

    [Header("Maze Decorations")]
    public MazeDecoration[] mazes;

    void Start()
    {
        GenerateDecorations();
    }

    void GenerateDecorations()
    {
        foreach (MazeDecoration maze in mazes)
        {
            SpawnForMaze(maze);
        }
    }

    void SpawnForMaze(MazeDecoration maze)
    {
        if (maze.spawnPointsParent == null)
        {
            Debug.LogWarning($"Spawn Points Parent برای {maze.mazeName} تنظیم نشده است.");
            return;
        }

        if (cactusPrefab == null)
        {
            Debug.LogError("Cactus Prefab در DecorationManager قرار داده نشده است.");
            return;
        }

        // تمام Childهای Parent را پیدا می‌کند
        foreach (Transform point in maze.spawnPointsParent)
        {
            Instantiate(
                cactusPrefab,
                point.position,
                point.rotation,
                transform
            );
        }
    }
}