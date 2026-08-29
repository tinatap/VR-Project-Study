using UnityEngine;
using UnityEngine.AI;

public class MazeShortestPath : MonoBehaviour
{
    [System.Serializable]
    public class Maze
    {
        public string mazeName;

        public Transform startPoint;
        public Transform exitPoint;

        [Header("Results")]
        [SerializeField] public float shortestPathLength;
        [SerializeField] public float minimumTime;
        [SerializeField] public bool pathFound;
    }

    [Header("All Mazes")]
    public Maze[] mazes;

    [Header("Avatar")]
    public float avatarSpeed = 2f;

    [ContextMenu("Calculate All Mazes")]
    public void CalculateAllMazes()
    {
        foreach (Maze maze in mazes)
        {
            CalculateMaze(maze);
        }

        Debug.Log("All maze calculations completed.");
    }

    private void CalculateMaze(Maze maze)
    {
        maze.pathFound = false;
        maze.shortestPathLength = 0f;
        maze.minimumTime = 0f;

        if (maze.startPoint == null || maze.exitPoint == null)
        {
            Debug.LogError(
                maze.mazeName +
                ": Start Point or Exit Point is missing."
            );

            return;
        }

        // Find Start on NavMesh
        if (!NavMesh.SamplePosition(
            maze.startPoint.position,
            out NavMeshHit startHit,
            2f,
            NavMesh.AllAreas))
        {
            Debug.LogError(
                maze.mazeName +
                ": Start Point is not near NavMesh."
            );

            return;
        }

        // Find Exit on NavMesh
        if (!NavMesh.SamplePosition(
            maze.exitPoint.position,
            out NavMeshHit exitHit,
            2f,
            NavMesh.AllAreas))
        {
            Debug.LogError(
                maze.mazeName +
                ": Exit Point is not near NavMesh."
            );

            return;
        }

        // Calculate path
        NavMeshPath path = new NavMeshPath();

        bool found = NavMesh.CalculatePath(
            startHit.position,
            exitHit.position,
            NavMesh.AllAreas,
            path
        );

        if (!found || path.corners.Length < 2)
        {
            Debug.LogError(
                maze.mazeName +
                ": No valid path found."
            );

            return;
        }

        // Calculate path length
        float length = 0f;

        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            length += Vector3.Distance(
                path.corners[i],
                path.corners[i + 1]
            );
        }

        // Save results
        maze.shortestPathLength = length;

        if (avatarSpeed > 0)
        {
            maze.minimumTime = length / avatarSpeed;
        }

        maze.pathFound = true;
    }
}