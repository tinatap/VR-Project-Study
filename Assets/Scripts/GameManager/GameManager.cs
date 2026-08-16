using TMPro;
using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // ===================================================== 
    // SCORE MODE 
    // ===================================================== 

    public enum ScoreMode
    {
        CoinsAndMazeScore,
        MazeScoreOnly
    }

    [Header("Score Mode")]
    public ScoreMode scoreMode = ScoreMode.CoinsAndMazeScore;


    // ===================================================== 
    // ANALYTICS 
    // ===================================================== 

    [Header("Analytics")]

    public AnalyticsLogger analyticsLogger;

    private int[] mazeAttemptCount = new int[11];

    private float mazeStartTime;

    private float gameStartTime;

    private bool currentAttemptSaved = false;

    private bool finalResultSaved = false;


    // ===================================================== 
    // PLAYER 
    // ===================================================== 

    [Header("Player")]

    public Transform player;


    // ===================================================== 
    // MAZES 
    // ===================================================== 

    [Header("Mazes")]

    public GameObject maze01;
    public GameObject maze02;
    public GameObject maze03;
    public GameObject maze04;
    public GameObject maze05;
    public GameObject maze06;
    public GameObject maze07;
    public GameObject maze08;
    public GameObject maze09;
    public GameObject maze10;


    // ===================================================== 
    // SPAWN POINTS 
    // ===================================================== 

    [Header("Spawn Points")]

    public Transform maze01Spawn;
    public Transform maze02Spawn;
    public Transform maze03Spawn;
    public Transform maze04Spawn;
    public Transform maze05Spawn;
    public Transform maze06Spawn;
    public Transform maze07Spawn;
    public Transform maze08Spawn;
    public Transform maze09Spawn;
    public Transform maze10Spawn;


    // ===================================================== 
    // SUCCESS UI 
    // ===================================================== 

    [Header("Success UI")]

    public GameObject successPanel01;

    public TextMeshProUGUI successMessageText;

    public TextMeshProUGUI successCountdownText;

    public float successPanelDuration = 3f;


    // ===================================================== 
    // TIME OVER UI 
    // ===================================================== 

    [Header("Time Over UI")]

    public GameObject timeOverPanel;

    public TextMeshProUGUI timeOverMessageText;

    public TextMeshProUGUI restartCountdownText;

    public float restartDelay = 10f;


    // ===================================================== 
    // TIMER UI 
    // ===================================================== 

    [Header("Maze Timer UI")]

    public TextMeshProUGUI timerText;


    // ===================================================== 
    // COIN UI 
    // ===================================================== 

    [Header("Coin UI")]

    public TextMeshProUGUI coinCounterText;


    // ===================================================== 
    // FINAL GAME UI 
    // ===================================================== 

    [Header("Final Game UI")]

    public GameObject finalGamePanel;

    public TextMeshProUGUI finalGameMessageText;

    public TextMeshProUGUI finalScoreText;


    // ===================================================== 
    // TOTAL GAME TIME 
    // ===================================================== 

    [Header("Total Game Time")]

    public float totalGameTime = 3600f;


    // ===================================================== 
    // MAZE 01 SETTINGS 
    // ===================================================== 

    [Header("Maze 01 Settings")]

    public int maze01TotalCoins = 10;
    public float maze01Time = 30f;
    public int maze01Score = 10;


    // ===================================================== 
    // MAZE 02 SETTINGS 
    // ===================================================== 

    [Header("Maze 02 Settings")]

    public int maze02TotalCoins = 15;
    public float maze02Time = 40f;
    public int maze02Score = 20;


    // ===================================================== 
    // MAZE 03 SETTINGS 
    // ===================================================== 

    [Header("Maze 03 Settings")]

    public int maze03TotalCoins = 20;
    public float maze03Time = 45f;
    public int maze03Score = 30;


    // ===================================================== 
    // MAZE 04 SETTINGS 
    // ===================================================== 

    [Header("Maze 04 Settings")]

    public int maze04TotalCoins = 25;
    public float maze04Time = 50f;
    public int maze04Score = 40;


    // ===================================================== 
    // MAZE 05 SETTINGS 
    // ===================================================== 

    [Header("Maze 05 Settings")]

    public int maze05TotalCoins = 30;
    public float maze05Time = 55f;
    public int maze05Score = 50;


    // ===================================================== 
    // MAZE 06 SETTINGS 
    // ===================================================== 

    [Header("Maze 06 Settings")]

    public int maze06TotalCoins = 35;
    public float maze06Time = 60f;
    public int maze06Score = 60;


    // ===================================================== 
    // MAZE 07 SETTINGS 
    // ===================================================== 

    [Header("Maze 07 Settings")]

    public int maze07TotalCoins = 40;
    public float maze07Time = 65f;
    public int maze07Score = 70;


    // ===================================================== 
    // MAZE 08 SETTINGS 
    // ===================================================== 

    [Header("Maze 08 Settings")]

    public int maze08TotalCoins = 45;
    public float maze08Time = 70f;
    public int maze08Score = 80;


    // ===================================================== 
    // MAZE 09 SETTINGS 
    // ===================================================== 

    [Header("Maze 09 Settings")]

    public int maze09TotalCoins = 50;
    public float maze09Time = 75f;
    public int maze09Score = 90;


    // ===================================================== 
    // MAZE 10 SETTINGS 
    // ===================================================== 

    [Header("Maze 10 Settings")]

    public int maze10TotalCoins = 55;
    public float maze10Time = 80f;
    public int maze10Score = 100;


    // ===================================================== 
    // PRIVATE VARIABLES 
    // ===================================================== 

    private CharacterController characterController;

    private Coroutine timerCoroutine;

    private Coroutine restartCoroutine;

    private Coroutine successCoroutine;

    private Coroutine totalGameTimerCoroutine;

    private int currentMaze = 1;

    private int totalCoins;

    private int collectedCoins;

    private int currentMazeScore;

    // فقط امتیاز Mazeهای موفق 
    private int totalScore = 0;

    private float currentMazeTime;

    private bool stageCompleted = false;

    private bool waitingForRestart = false;

    private bool changingMaze = false;

    private bool gameFinished = false;


    // ===================================================== 
    // AWAKE 
    // ===================================================== 

    private void Awake()
    {
        if (player != null)
        {
            characterController =
                player.GetComponent<CharacterController>();
        }
    }


    // ===================================================== 
    // START 
    // ===================================================== 

    private void Start()
    {
        // Game does NOT start automatically.
        // The Start Room menu will call StartGameFromMaze1()
        // after the player selects YES.

        if (successPanel01 != null)
            successPanel01.SetActive(false);


        if (timeOverPanel != null)
            timeOverPanel.SetActive(false);


        if (finalGamePanel != null)
            finalGamePanel.SetActive(false);


        SetAllMazesInactive();


        SetupCoinMode();
    }


    // =====================================================
    // START GAME FROM MAZE 1
    // =====================================================

    public void StartGameFromMaze1()
    {
        if (gameFinished)
            return;

        // Start the real game timer when the player presses YES.
        gameStartTime = Time.time;

        // Reset the current run.
        currentMaze = 1;
        totalScore = 0;
        collectedCoins = 0;

        stageCompleted = false;
        waitingForRestart = false;
        changingMaze = false;
        gameFinished = false;

        currentAttemptSaved = false;
        finalResultSaved = false;

        // Prepare Maze 1.
        SetAllMazesInactive();

        ResetAllCoins();

        if (maze01 != null)
            maze01.SetActive(true);

        // Start Maze 1.
        StartMaze(1);

        // Start the total-game timer only after YES.
        if (totalGameTimerCoroutine != null)
            StopCoroutine(totalGameTimerCoroutine);

        totalGameTimerCoroutine =
            StartCoroutine(
                TotalGameTimer()
            );

        Debug.Log("Game started from Maze 1.");
    }


    // =====================================================
    // COIN MODE SETUP 
    // ===================================================== 

    private void SetupCoinMode()
    {
        Coin[] allCoins =
            FindObjectsByType<Coin>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );


        foreach (Coin coin in allCoins)
        {
            if (coin == null)
                continue;


            if (scoreMode ==
                ScoreMode.CoinsAndMazeScore)
            {
                coin.gameObject.SetActive(true);
            }
            else
            {
                coin.gameObject.SetActive(false);
            }
        }


        if (scoreMode ==
            ScoreMode.MazeScoreOnly)
        {
            if (coinCounterText != null)
                coinCounterText.gameObject.SetActive(false);
        }
        else
        {
            if (coinCounterText != null)
                coinCounterText.gameObject.SetActive(true);
        }
    }


    // ===================================================== 
    // START MAZE 
    // ===================================================== 

    private void StartMaze(int mazeNumber)
    {
        currentMaze = mazeNumber;

        stageCompleted = false;

        waitingForRestart = false;

        changingMaze = false;


        // Attempt جدید 
        mazeAttemptCount[mazeNumber]++;

        currentAttemptSaved = false;

        mazeStartTime = Time.time;


        SetMazeSettings(mazeNumber);


        ResetCoins();


        MovePlayer(
            GetSpawnPoint(mazeNumber)
        );


        ShowMainTimer();


        StartMazeTimer();


        Debug.Log(
            "Maze " +
            mazeNumber +
            " started. Attempt: " +
            mazeAttemptCount[mazeNumber]
        );
    }


    // ===================================================== 
    // SET MAZE SETTINGS 
    // ===================================================== 

    private void SetMazeSettings(int mazeNumber)
    {
        switch (mazeNumber)
        {
            case 1:
                totalCoins = maze01TotalCoins;
                currentMazeTime = maze01Time;
                currentMazeScore = maze01Score;
                break;

            case 2:
                totalCoins = maze02TotalCoins;
                currentMazeTime = maze02Time;
                currentMazeScore = maze02Score;
                break;

            case 3:
                totalCoins = maze03TotalCoins;
                currentMazeTime = maze03Time;
                currentMazeScore = maze03Score;
                break;

            case 4:
                totalCoins = maze04TotalCoins;
                currentMazeTime = maze04Time;
                currentMazeScore = maze04Score;
                break;

            case 5:
                totalCoins = maze05TotalCoins;
                currentMazeTime = maze05Time;
                currentMazeScore = maze05Score;
                break;

            case 6:
                totalCoins = maze06TotalCoins;
                currentMazeTime = maze06Time;
                currentMazeScore = maze06Score;
                break;

            case 7:
                totalCoins = maze07TotalCoins;
                currentMazeTime = maze07Time;
                currentMazeScore = maze07Score;
                break;

            case 8:
                totalCoins = maze08TotalCoins;
                currentMazeTime = maze08Time;
                currentMazeScore = maze08Score;
                break;

            case 9:
                totalCoins = maze09TotalCoins;
                currentMazeTime = maze09Time;
                currentMazeScore = maze09Score;
                break;

            case 10:
                totalCoins = maze10TotalCoins;
                currentMazeTime = maze10Time;
                currentMazeScore = maze10Score;
                break;
        }


        if (scoreMode ==
            ScoreMode.MazeScoreOnly)
        {
            totalCoins = 0;
        }
    }


    // ===================================================== 
    // GET SPAWN POINT 
    // ===================================================== 

    private Transform GetSpawnPoint(int mazeNumber)
    {
        switch (mazeNumber)
        {
            case 1:
                return maze01Spawn;

            case 2:
                return maze02Spawn;

            case 3:
                return maze03Spawn;

            case 4:
                return maze04Spawn;

            case 5:
                return maze05Spawn;

            case 6:
                return maze06Spawn;

            case 7:
                return maze07Spawn;

            case 8:
                return maze08Spawn;

            case 9:
                return maze09Spawn;

            case 10:
                return maze10Spawn;
        }

        return null;
    }


    // ===================================================== 
    // GET MAZE 
    // ===================================================== 

    private GameObject GetMaze(int mazeNumber)
    {
        switch (mazeNumber)
        {
            case 1:
                return maze01;

            case 2:
                return maze02;

            case 3:
                return maze03;

            case 4:
                return maze04;

            case 5:
                return maze05;

            case 6:
                return maze06;

            case 7:
                return maze07;

            case 8:
                return maze08;

            case 9:
                return maze09;

            case 10:
                return maze10;
        }

        return null;
    }


    // ===================================================== 
    // DISABLE ALL MAZES 
    // ===================================================== 

    private void SetAllMazesInactive()
    {
        if (maze01 != null)
            maze01.SetActive(false);

        if (maze02 != null)
            maze02.SetActive(false);

        if (maze03 != null)
            maze03.SetActive(false);

        if (maze04 != null)
            maze04.SetActive(false);

        if (maze05 != null)
            maze05.SetActive(false);

        if (maze06 != null)
            maze06.SetActive(false);

        if (maze07 != null)
            maze07.SetActive(false);

        if (maze08 != null)
            maze08.SetActive(false);

        if (maze09 != null)
            maze09.SetActive(false);

        if (maze10 != null)
            maze10.SetActive(false);
    }


    // ===================================================== 
    // MOVE PLAYER 
    // ===================================================== 

    private void MovePlayer(Transform spawn)
    {
        if (player == null)
        {
            Debug.LogWarning(
                "GameManager: Player is not assigned!"
            );

            return;
        }


        if (spawn == null)
        {
            Debug.LogWarning(
                "GameManager: Spawn Point is not assigned!"
            );

            return;
        }


        if (characterController != null)
            characterController.enabled = false;


        player.SetPositionAndRotation(
            spawn.position,
            spawn.rotation
        );


        if (characterController != null)
            characterController.enabled = true;
    }


    // ===================================================== 
    // RESET ALL COINS 
    // ===================================================== 

    private void ResetAllCoins()
    {
        Coin[] allCoins =
            FindObjectsByType<Coin>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );


        foreach (Coin coin in allCoins)
        {
            if (coin != null)
            {
                if (scoreMode ==
                    ScoreMode.CoinsAndMazeScore)
                {
                    coin.gameObject.SetActive(true);
                }
                else
                {
                    coin.gameObject.SetActive(false);
                }
            }
        }
    }


    // ===================================================== 
    // COLLECT COIN 
    // ===================================================== 

    public void CollectCoin()
    {
        if (scoreMode ==
            ScoreMode.MazeScoreOnly)
        {
            return;
        }


        if (stageCompleted ||
            changingMaze ||
            waitingForRestart ||
            gameFinished)
        {
            return;
        }


        if (collectedCoins >= totalCoins)
            return;


        collectedCoins++;


        // مهم: 
        // اینجا totalScore تغییر نمی‌کند. 
        // سکه فقط در صورت موفقیت Maze 
        // وارد Total Score می‌شود. 

        UpdateCoinText();
    }


    // ===================================================== 
    // RESET COINS 
    // ===================================================== 

    private void ResetCoins()
    {
        collectedCoins = 0;

        UpdateCoinText();
    }


    // ===================================================== 
    // UPDATE COIN TEXT 
    // ===================================================== 

    private void UpdateCoinText()
    {
        if (coinCounterText == null)
            return;


        if (scoreMode ==
            ScoreMode.MazeScoreOnly)
        {
            coinCounterText.gameObject.SetActive(false);

            return;
        }


        coinCounterText.gameObject.SetActive(true);


        coinCounterText.text =
            "Coins: " +
            collectedCoins +
            "/" +
            totalCoins;
    }


    // ===================================================== 
    // MAIN MAZE TIMER 
    // ===================================================== 

    private void StartMazeTimer()
    {
        StopMazeTimer();


        timerCoroutine =
            StartCoroutine(
                MazeTimer()
            );
    }


    private IEnumerator MazeTimer()
    {
        float remainingTime =
            currentMazeTime;


        while (remainingTime > 0f)
        {
            if (stageCompleted ||
                waitingForRestart ||
                changingMaze ||
                gameFinished)
            {
                yield break;
            }


            UpdateTimerText(
                remainingTime
            );


            yield return null;


            remainingTime -=
                Time.deltaTime;
        }


        UpdateTimerText(0f);


        TimerFinished();
    }


    // ===================================================== 
    // UPDATE TIMER TEXT 
    // ===================================================== 

    private void UpdateTimerText(float time)
    {
        if (timerText == null)
            return;


        int seconds =
            Mathf.CeilToInt(time);


        timerText.text =
            "Time: " +
            seconds;
    }


    // ===================================================== 
    // SHOW MAIN TIMER 
    // ===================================================== 

    private void ShowMainTimer()
    {
        if (timerText != null)
            timerText.gameObject.SetActive(true);
    }


    // ===================================================== 
    // HIDE MAIN TIMER 
    // ===================================================== 

    private void HideMainTimer()
    {
        if (timerText != null)
            timerText.gameObject.SetActive(false);
    }


    // ===================================================== 
    // STOP TIMER 
    // ===================================================== 

    private void StopMazeTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(
                timerCoroutine
            );

            timerCoroutine = null;
        }
    }


    // ===================================================== 
    // SAVE CURRENT MAZE ATTEMPT 
    // ===================================================== 

    private void SaveCurrentMazeAttempt(
        string result
    )
    {
        if (currentAttemptSaved)
            return;


        if (analyticsLogger == null)
        {
            Debug.LogWarning(
                "GameManager: Analytics Logger is not assigned!"
            );

            return;
        }


        float attemptTime =
            Time.time -
            mazeStartTime;


        analyticsLogger.SaveMazeAttempt(
            currentMaze,
            mazeAttemptCount[currentMaze],
            result,
            collectedCoins,
            totalCoins,
            attemptTime
        );


        currentAttemptSaved = true;


        Debug.Log(
            "Analytics saved: " +
            "Maze " +
            currentMaze +
            " | Attempt " +
            mazeAttemptCount[currentMaze] +
            " | " +
            result +
            " | Coins: " +
            collectedCoins +
            " | Time: " +
            attemptTime.ToString("F2")
        );
    }


    // ===================================================== 
    // TIME OVER 
    // ===================================================== 

    private void TimerFinished()
    {
        if (stageCompleted ||
            waitingForRestart ||
            changingMaze ||
            gameFinished)
        {
            return;
        }


        waitingForRestart = true;


        StopMazeTimer();


        // ذخیره تلاش ناموفق 
        SaveCurrentMazeAttempt(
            "FAILED - TIME OVER"
        );


        // ================================================= 
        // RESET ENTIRE RUN SCORE 
        // ================================================= 

        totalScore = 0;


        // ================================================= 
        // RESET COINS 
        // ================================================= 

        collectedCoins = 0;

        ResetAllCoins();


        // ================================================= 
        // CLOSE ALL MAZES 
        // ================================================= 

        SetAllMazesInactive();


        // ================================================= 
        // OPEN MAZE 1 
        // ================================================= 

        if (maze01 != null)
            maze01.SetActive(true);


        currentMaze = 1;


        SetMazeSettings(1);


        MovePlayer(
            maze01Spawn
        );


        UpdateCoinText();


        // ================================================= 
        // HIDE TIMER 
        // ================================================= 

        HideMainTimer();


        // ================================================= 
        // SHOW TIME OVER PANEL 
        // ================================================= 

        if (timeOverPanel != null)
            timeOverPanel.SetActive(true);


        // ================================================= 
        // RESTART COUNTDOWN 
        // ================================================= 

        restartCoroutine =
            StartCoroutine(
                RestartFromMaze01Countdown()
            );
    }


    // ===================================================== 
    // RESTART COUNTDOWN 
    // ===================================================== 

    private IEnumerator RestartFromMaze01Countdown()
    {
        float remainingTime =
            restartDelay;


        while (remainingTime > 0f)
        {
            if (restartCountdownText != null)
            {
                int seconds =
                    Mathf.CeilToInt(
                        remainingTime
                    );


                restartCountdownText.text =
                    "Restarting from Maze 1 in " +
                    seconds +
                    " seconds";
            }


            yield return null;


            remainingTime -=
                Time.deltaTime;
        }


        RestartFromMaze01();
    }


    // ===================================================== 
    // RESTART FROM MAZE 1 
    // ===================================================== 

    private void RestartFromMaze01()
    {
        waitingForRestart = false;

        stageCompleted = false;

        changingMaze = false;


        if (timeOverPanel != null)
            timeOverPanel.SetActive(false);


        SetAllMazesInactive();


        if (maze01 != null)
            maze01.SetActive(true);


        currentMaze = 1;


        SetMazeSettings(1);


        MovePlayer(
            maze01Spawn
        );


        ResetAllCoins();


        ResetCoins();


        ShowMainTimer();


        // Attempt جدید Maze 1 
        StartMaze(1);


        Debug.Log(
            "New Run started from Maze 1."
        );
    }


    // ===================================================== 
    // MAZE COMPLETED 
    // ===================================================== 

    public void MazeCompleted()
    {
        if (stageCompleted ||
            changingMaze ||
            waitingForRestart ||
            gameFinished)
        {
            return;
        }


        stageCompleted = true;

        changingMaze = true;


        StopMazeTimer();

        HideMainTimer();


        // ================================================= 
        // CALCULATE SUCCESSFUL MAZE SCORE 
        // ================================================= 

        int mazeResultScore =
            currentMazeScore;


        if (scoreMode ==
            ScoreMode.CoinsAndMazeScore)
        {
            mazeResultScore +=
                collectedCoins;
        }


        // ================================================= 
        // ADD ONLY SUCCESSFUL MAZE 
        // ================================================= 

        totalScore +=
            mazeResultScore;


        Debug.Log(
            "Maze " +
            currentMaze +
            " completed.\n" +
            "Maze Score: " +
            currentMazeScore +
            "\nCoins: " +
            collectedCoins +
            "\nAdded Score: " +
            mazeResultScore +
            "\nTotal Score: " +
            totalScore
        );


        // ================================================= 
        // SAVE SUCCESS 
        // ================================================= 

        SaveCurrentMazeAttempt(
            "SUCCESS"
        );


        // ================================================= 
        // NEXT MAZE 
        // ================================================= 

        if (currentMaze < 10)
        {
            successCoroutine =
                StartCoroutine(
                    ShowSuccessAndLoadNextMaze()
                );
        }
        else
        {
            successCoroutine =
                StartCoroutine(
                    ShowFinalSuccess()
                );
        }
    }


    // ===================================================== 
    // SUCCESS → NEXT MAZE 
    // ===================================================== 

    private IEnumerator ShowSuccessAndLoadNextMaze()
    {
        if (successPanel01 != null)
            successPanel01.SetActive(true);


        if (successMessageText != null)
        {
            successMessageText.text =
                "Congratulations!\n" +
                "You successfully completed Maze " +
                currentMaze +
                ".";
        }


        float remainingTime =
            successPanelDuration;


        while (remainingTime > 0f)
        {
            if (successCountdownText != null)
            {
                int seconds =
                    Mathf.CeilToInt(
                        remainingTime
                    );


                successCountdownText.text =
                    "Next maze starts in: " +
                    seconds;
            }


            yield return null;


            remainingTime -=
                Time.deltaTime;
        }


        if (successPanel01 != null)
            successPanel01.SetActive(false);


        GameObject currentMazeObject =
            GetMaze(currentMaze);


        if (currentMazeObject != null)
            currentMazeObject.SetActive(false);


        currentMaze++;


        GameObject nextMaze =
            GetMaze(currentMaze);


        if (nextMaze != null)
            nextMaze.SetActive(true);


        StartMaze(
            currentMaze
        );


        successCoroutine = null;
    }


    // ===================================================== 
    // MAZE 10 COMPLETED 
    // ===================================================== 

    private IEnumerator ShowFinalSuccess()
    {
        if (successPanel01 != null)
            successPanel01.SetActive(true);


        if (successMessageText != null)
        {
            successMessageText.text =
                "Congratulations!\n" +
                "You successfully completed all 10 mazes!";
        }


        float remainingTime =
            successPanelDuration;


        while (remainingTime > 0f)
        {
            if (successCountdownText != null)
            {
                successCountdownText.text =
                    "Completed!";
            }


            yield return null;


            remainingTime -=
                Time.deltaTime;
        }


        if (successPanel01 != null)
            successPanel01.SetActive(false);


        successCoroutine = null;
    }


    // ===================================================== 
    // TOTAL GAME TIMER 
    // ===================================================== 

    private IEnumerator TotalGameTimer()
    {
        float remainingTime =
            totalGameTime;


        while (remainingTime > 0f)
        {
            if (gameFinished)
                yield break;


            yield return null;


            remainingTime -=
                Time.deltaTime;
        }


        // ================================================= 
        // SAVE CURRENT INCOMPLETE MAZE 
        // ================================================= 

        if (!gameFinished &&
            !stageCompleted &&
            !currentAttemptSaved)
        {
            SaveCurrentMazeAttempt(
                "FAILED - TOTAL TIME OVER"
            );
        }


        // ================================================= 
        // IMPORTANT: 
        // DO NOT RESET TOTAL SCORE HERE 
        // ================================================= 

        FinishEntireGame(
            "FAILED - TOTAL TIME OVER",
            totalGameTime
        );
    }


    // ===================================================== 
    // EXIT GAME 
    // ===================================================== 

    public void ExitGame()
    {
        if (gameFinished)
            return;


        // اگر در یک Maze نیمه‌تمام هستیم، 
        // آن را به عنوان EXIT ذخیره کن. 

        if (!currentAttemptSaved &&
            !stageCompleted &&
            !waitingForRestart &&
            !changingMaze)
        {
            SaveCurrentMazeAttempt(
                "FAILED - EXIT"
            );
        }


        // زمان واقعی از شروع بازی تا Exit 
        float realTotalGameTime =
            Time.time -
            gameStartTime;


        FinishEntireGame(
            "FAILED - EXIT",
            realTotalGameTime
        );
    }


    // ===================================================== 
    // FINAL GAME 
    // ===================================================== 

    private void FinishEntireGame(
        string finalResult,
        float totalGameTimeUsed
    )
    {
        if (gameFinished)
            return;


        gameFinished = true;


        StopMazeTimer();


        if (restartCoroutine != null)
        {
            StopCoroutine(
                restartCoroutine
            );

            restartCoroutine = null;
        }


        if (successCoroutine != null)
        {
            StopCoroutine(
                successCoroutine
            );

            successCoroutine = null;
        }


        if (totalGameTimerCoroutine != null)
        {
            StopCoroutine(
                totalGameTimerCoroutine
            );

            totalGameTimerCoroutine = null;
        }


        if (successPanel01 != null)
            successPanel01.SetActive(false);


        if (timeOverPanel != null)
            timeOverPanel.SetActive(false);


        HideMainTimer();


        // ================================================= 
        // SAVE FINAL ANALYTICS 
        // ================================================= 

        if (!finalResultSaved)
        {
            finalResultSaved = true;


            if (analyticsLogger != null)
            {
                analyticsLogger.SaveFinalResult(
                    finalResult,
                    totalGameTimeUsed
                );
            }
        }


        // ================================================= 
        // FINAL UI 
        // ================================================= 

        if (finalGamePanel != null)
            finalGamePanel.SetActive(true);


        if (finalGameMessageText != null)
        {
            finalGameMessageText.text =
                "Game Over";
        }


        if (finalScoreText != null)
        {
            finalScoreText.text =
                "Total Score: " +
                totalScore;
        }


        Debug.Log(
            "Game finished.\n" +
            "Result: " +
            finalResult +
            "\nTotal Game Time: " +
            totalGameTimeUsed.ToString("F2") +
            " seconds" +
            "\nFinal Score: " +
            totalScore
        );


        Time.timeScale = 0f;
    }
}
