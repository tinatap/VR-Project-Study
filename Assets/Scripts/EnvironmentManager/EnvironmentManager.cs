using UnityEngine;


[System.Serializable]
public class MazeGroup
{
    public string mazeName;

    [Header("Walls")]
    public Renderer[] walls;

    [Header("Floor")]
    public Renderer floor;
}



public class EnvironmentManager : MonoBehaviour
{

    public enum EnvironmentType
    {
        Colorful,
        Neutral
    }


    [Header("Choose Environment")]
    public EnvironmentType environmentType;


    [Header("Mazes")]
    public MazeGroup[] mazes;



    [Header("Colorful Wall Materials")]
    public Material[] colorfulWallMaterials;


    [Header("Neutral Wall Material")]
    public Material grayWallMaterial;



    [Header("Floor Materials")]
    public Material colorfulFloorMaterial;
    public Material neutralFloorMaterial;



    void Start()
    {
        ApplyEnvironment();
    }



    public void ApplyEnvironment()
    {
        if (environmentType == EnvironmentType.Colorful)
        {
            ApplyColorful();
        }
        else
        {
            ApplyNeutral();
        }
    }



    void ApplyColorful()
    {
        foreach (MazeGroup maze in mazes)
        {

            // Walls
            foreach (Renderer wall in maze.walls)
            {
                wall.material =
                colorfulWallMaterials[
                Random.Range(0, colorfulWallMaterials.Length)
                ];
            }


            // Floor
            if (maze.floor != null)
            {
                maze.floor.material = colorfulFloorMaterial;
            }

        }
    }




    void ApplyNeutral()
    {
        foreach (MazeGroup maze in mazes)
        {

            // Walls
            foreach (Renderer wall in maze.walls)
            {
                wall.material = grayWallMaterial;
            }


            // Floor
            if (maze.floor != null)
            {
                maze.floor.material = neutralFloorMaterial;
            }

        }
    }

}