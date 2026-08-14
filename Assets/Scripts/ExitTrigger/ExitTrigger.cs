
using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    private GameManager gameManager;

    private bool completed = false;


    private void Start()
    {
        gameManager =
            FindFirstObjectByType<GameManager>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !completed)
        {
            completed = true;

            if (gameManager != null)
            {
                gameManager.MazeCompleted();
            }
            else
            {
                Debug.LogWarning(
                    "ExitTrigger: GameManager not found!"
                );
            }
        }
    }
}

