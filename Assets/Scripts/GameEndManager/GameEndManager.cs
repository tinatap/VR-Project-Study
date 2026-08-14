using UnityEngine;
using TMPro;

public class GameEndManager : MonoBehaviour
{
    public GameObject finishPanel;
    public TMP_Text finishText;
    public TMP_Text scoreText;

    public int score;


    public void ExitGame()
    {
        finishPanel.SetActive(true);

        finishText.text = "END";

        scoreText.text =
            "Score: " + score;


        Time.timeScale = 0;
    }
}