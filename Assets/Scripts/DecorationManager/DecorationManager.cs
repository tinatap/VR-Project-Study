using UnityEngine;


[System.Serializable]
public class MazeDecoration
{
    public string mazeName;

    [Header("Points near walls")]
    public Transform[] plantSpawnPoints;


    [Header("Settings")]
    [Range(0, 1)]
    public float spawnChance = 0.3f;

    public int maxPlants = 20;
}



public class DecorationManager : MonoBehaviour
{

    [Header("Plant Prefabs")]
    public GameObject[] plantPrefabs;


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

        int count = 0;


        foreach (Transform point in maze.plantSpawnPoints)
        {

            if (count >= maze.maxPlants)
                break;


            if (Random.value <= maze.spawnChance)
            {

                GameObject selectedPlant =
                plantPrefabs[
                Random.Range(0, plantPrefabs.Length)
                ];


                Instantiate(
                    selectedPlant,
                    point.position,
                    point.rotation,
                    transform
                );


                count++;
            }
        }

    }
}