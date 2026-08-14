using System.IO;
using UnityEngine;

public class AnalyticsLogger : MonoBehaviour
{
    [Header("Save Settings")]

    [Tooltip("Folder where the results file will be saved")]
    public string saveFolderPath = "";

    [Tooltip("Name of the results file")]
    public string fileName = "MazeResults.txt";


    private string filePath;


    // =====================================================
    // AWAKE
    // =====================================================

    private void Awake()
    {
        if (string.IsNullOrWhiteSpace(saveFolderPath))
        {
            Debug.LogWarning(
                "AnalyticsLogger: Save Folder Path is empty!"
            );

            return;
        }


        if (!Directory.Exists(saveFolderPath))
        {
            Directory.CreateDirectory(
                saveFolderPath
            );
        }


        filePath = Path.Combine(
            saveFolderPath,
            fileName
        );


        // ایجاد فایل جدید برای این اجرای بازی
        File.WriteAllText(
            filePath,
            "MAZE ANALYTICS\n\n"
        );


        Debug.Log(
            "Analytics file created:\n" +
            filePath
        );
    }


    // =====================================================
    // SAVE MAZE ATTEMPT
    // =====================================================

    public void SaveMazeAttempt(
        int mazeNumber,
        int attemptNumber,
        string result,
        int coins,
        int totalCoins,
        float attemptTime
    )
    {
        if (string.IsNullOrEmpty(filePath))
        {
            Debug.LogWarning(
                "AnalyticsLogger: File path is not set!"
            );

            return;
        }


        string data =
            "========================================\n" +
            "MAZE " + mazeNumber +
            " - ATTEMPT " + attemptNumber + "\n" +
            "Result: " + result + "\n" +
            "Coins: " + coins + "/" + totalCoins + "\n" +
            "Attempt Time: " +
            attemptTime.ToString("F2") +
            " seconds\n" +
            "========================================\n\n";


        File.AppendAllText(
            filePath,
            data
        );


        Debug.Log(
            "Maze attempt saved: " +
            "Maze " + mazeNumber +
            " | Attempt " + attemptNumber +
            " | " + result
        );
    }


    // =====================================================
    // SAVE FINAL GAME RESULT
    // =====================================================

    public void SaveFinalResult(
        string finalResult,
        float totalGameTime
    )
    {
        if (string.IsNullOrEmpty(filePath))
            return;


        string data =
            "\n\n" +
            "****************************************\n" +
            "GAME FINISHED\n" +
            "Final Result: " + finalResult + "\n" +
            "Total Game Time: " +
            totalGameTime.ToString("F2") +
            " seconds\n" +
            "****************************************\n";


        File.AppendAllText(
            filePath,
            data
        );


        Debug.Log(
            "Final game result saved.\n" +
            "Result: " + finalResult +
            "\nTotal Game Time: " +
            totalGameTime.ToString("F2") +
            " seconds"
        );
    }
}