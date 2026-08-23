/*
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
*/
using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    private GameManager gameManager;
    private bool completed = false;

    private void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();

        Debug.Log("EXIT STARTED: " + gameObject.name);
        Debug.Log("GameManager = " + gameManager);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(
            "EXIT TRIGGER ENTERED: " +
            gameObject.name +
            " | Other = " +
            other.name +
            " | Tag = " +
            other.tag
        );

        if (other.CompareTag("Player") && !completed)
        {
            completed = true;

            Debug.Log("EXIT DETECTED! Calling MazeCompleted().");

            if (gameManager != null)
            {
                gameManager.MazeCompleted();
            }
            else
            {
                Debug.LogError("ExitTrigger: GameManager not found!");
            }
        }
    }
}