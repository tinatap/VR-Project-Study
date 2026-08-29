using TMPro;
using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;

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

    [Header("TCP Analytics")]
    public TCPAnalyticsClient tcpAnalyticsClient;

    [Header("Analytics")]
    public AnalyticsLogger analyticsLogger;

    private int[] mazeAttemptCount = new int[11];

    private float mazeStartTime;
    private float gameStartTime;

    private bool currentAttemptSaved = false;
    private bool finalResultSaved = false;


    // =====================================================
    // START ROOM TIMING
    // =====================================================

    [Header("Start Room Timing")]

    [Tooltip("Time from game launch until YES is pressed in StartQuestionPanel.")]
    [SerializeField]
    private float startRoomDuration = 0f;

    [Tooltip("Time from opening StartQuestionPanel until YES is pressed.")]
    [SerializeField]
    private float startQuestionPanelDuration = 0f;

    [Tooltip("Time when the game/application entered the Start Room.")]
    private float startRoomStartTime;

    [Tooltip("Time when StartQuestionPanel was opened.")]
    private float startQuestionPanelOpenTime;

    private bool startRoomResultSaved = false;


    // =====================================================
    // EXIT CONFIRM TIMING
    // =====================================================

    [Header("Exit Confirm History")]

    [Tooltip("All ExitConfirmPanel interactions are stored in order.")]
    [SerializeField]
    private List<ExitConfirmRecord> exitConfirmHistory =
        new List<ExitConfirmRecord>();

    private float exitConfirmPanelOpenTime;

    private bool exitConfirmPanelCurrentlyOpen = false;


    // =====================================================
    // MAZE VISIT HISTORY
    // =====================================================

    [Header("Maze Visit History")]

    [Tooltip("All maze visits are stored in the exact order they happened.")]
    [SerializeField]
    private List<MazeVisitRecord> mazeVisitHistory =
        new List<MazeVisitRecord>();

    private int mazeVisitNumber = 0;


    // =====================================================
    // MUSIC MANAGER
    // =====================================================

    [Header("Music Manager")]
    public MusicManager musicManager;


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
    // SUCCESS SOUND
    // =====================================================

    [Header("Success Sound")]

    [Tooltip("AudioSource used to play the success sound.")]
    public AudioSource successAudioSource;

    [Tooltip("Sound played when a maze is successfully completed.")]
    public AudioClip successSound;

    [Range(0f, 1f)]
    [Tooltip("Volume of the success sound.")]
    public float successSoundVolume = 1f;


    // =====================================================
    // TIME OVER UI
    // =====================================================

    [Header("Time Over UI")]

    public GameObject timeOverPanel;

    public TextMeshProUGUI timeOverMessageText;

    public TextMeshProUGUI restartCountdownText;

    public float restartDelay = 10f;


    // =====================================================
    // TIME OVER SOUND
    // =====================================================

    [Header("Time Over Sound")]

    [Tooltip("AudioSource used to play the fail sound.")]
    public AudioSource timeOverAudioSource;

    [Tooltip("Sound played when the maze timer reaches zero.")]
    public AudioClip timeOverFailSound;

    [Range(0f, 1f)]
    [Tooltip("Volume of the fail sound.")]
    public float timeOverSoundVolume = 1f;


    // =====================================================
    // TIMER UI
    // =====================================================

    [Header("Maze Timer UI")]

    public GameObject timerPanel;

    public TextMeshProUGUI timerText;


    // =====================================================
    // EXIT UI
    // =====================================================

    [Header("Exit UI")]

    public GameObject exitButton;

    [Tooltip("Exit confirmation panel.")]
    public GameObject exitConfirmPanel;


    // =====================================================
    // START QUESTION UI
    // =====================================================

    [Header("Start Question UI")]

    [Tooltip("Start question panel shown before Maze 1.")]
    public GameObject startQuestionPanel;


    // =====================================================
    // COIN UI
    // =====================================================

    [Header("Coin UI")]

    public GameObject coinCounterPanel;

    public TextMeshProUGUI coinCounterText;


    // =====================================================
    // FINAL GAME UI
    // =====================================================

    [Header("Final Game UI")]

    public GameObject finalGamePanel;

    public TextMeshProUGUI finalGameMessageText;

    public TextMeshProUGUI finalScoreText;


    // =====================================================
    // FINAL SUCCESS UI
    // =====================================================

    [Header("Final Success UI")]

    public GameObject finalSuccessPanel;

    public TextMeshProUGUI finalSuccessMessageText;

    public TextMeshProUGUI finalSuccessScoreText;


    // =====================================================
    // FINAL GAME SOUND
    // =====================================================

    [Header("Final Game Sound")]

    [Tooltip("AudioSource used to play the final game sound.")]
    public AudioSource finalGameAudioSource;

    [Tooltip("Sound played when the entire game finishes.")]
    public AudioClip finalGameSound;

    [Range(0f, 1f)]
    [Tooltip("Volume of the final game sound.")]
    public float finalGameSoundVolume = 1f;


    // =====================================================
    // FINAL SUCCESS SOUND
    // =====================================================

    [Header("Final Success Sound")]

    public AudioSource finalSuccessAudioSource;

    public AudioClip finalSuccessSound;

    [Range(0f, 1f)]
    public float finalSuccessSoundVolume = 1f;


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

    private int currentMaze = 0;

    private int totalCoins;
    private int collectedCoins;

    private int currentMazeScore;

    private int totalScore = 0;

    private float currentMazeTime;

    private bool stageCompleted = false;
    private bool waitingForRestart = false;
    private bool changingMaze = false;
    private bool gameFinished = false;


    // =====================================================
    // ANALYTICS PUBLIC DATA
    // =====================================================

    public int CurrentMaze
    {
        get
        {
            return currentMaze;
        }
    }


    public int CurrentAttempt
    {
        get
        {
            if (currentMaze >= 0 &&
                currentMaze < mazeAttemptCount.Length)
            {
                return mazeAttemptCount[currentMaze];
            }

            return 0;
        }
    }


    public int CollectedCoins
    {
        get
        {
            return collectedCoins;
        }
    }


    public int TotalCoins
    {
        get
        {
            return totalCoins;
        }
    }


    public int TotalScore
    {
        get
        {
            return totalScore;
        }
    }


    public float CurrentMazeElapsedTime
    {
        get
        {
            if (mazeStartTime <= 0f)
                return 0f;

            return Time.time - mazeStartTime;
        }
    }


    public float TotalGameElapsedTime
    {
        get
        {
            if (gameStartTime <= 0f)
                return 0f;

            return Time.time - gameStartTime;
        }
    }


    // =====================================================
    // START ROOM PUBLIC DATA
    // =====================================================

    public float StartRoomDuration
    {
        get
        {
            return startRoomDuration;
        }
    }


    public float StartQuestionPanelDuration
    {
        get
        {
            return startQuestionPanelDuration;
        }
    }


    // =====================================================
    // MAZE HISTORY PUBLIC DATA
    // =====================================================

    public List<MazeVisitRecord> MazeVisitHistory
    {
        get
        {
            return mazeVisitHistory;
        }
    }


    public int MazeVisitCount
    {
        get
        {
            return mazeVisitHistory.Count;
        }
    }


    // =====================================================
    // EXIT HISTORY PUBLIC DATA
    // =====================================================

    public List<ExitConfirmRecord> ExitConfirmHistory
    {
        get
        {
            return exitConfirmHistory;
        }
    }


    public int ExitConfirmCount
    {
        get
        {
            return exitConfirmHistory.Count;
        }
    }


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
        currentMaze = 0;

        mazeVisitHistory.Clear();
        exitConfirmHistory.Clear();

        mazeVisitNumber = 0;

        startRoomDuration = 0f;
        startQuestionPanelDuration = 0f;

        startRoomStartTime = Time.time;
        startQuestionPanelOpenTime = 0f;

        startRoomResultSaved = false;

        exitConfirmPanelOpenTime = 0f;
        exitConfirmPanelCurrentlyOpen = false;

        if (successPanel01 != null)
            successPanel01.SetActive(false);

        if (timeOverPanel != null)
            timeOverPanel.SetActive(false);

        if (finalGamePanel != null)
            finalGamePanel.SetActive(false);

        if (finalSuccessPanel != null)
            finalSuccessPanel.SetActive(false);

        if (exitConfirmPanel != null)
            exitConfirmPanel.SetActive(false);

        if (startQuestionPanel != null)
            startQuestionPanel.SetActive(false);

        SetAllMazesInactive();

        SetupCoinMode();

        SetMazeUI(false);

        Debug.Log(
            "========== GAME INITIALIZED ==========\n" +
            "Start Room Timer Started."
        );
    }


    // =====================================================
    // OPEN PANEL WITH ANALYTICS EVENT
    // =====================================================

    private void OpenPanel(
        GameObject panel,
        string panelEventType
    )
    {
        if (panel == null)
            return;

        if (panel.activeSelf)
            return;

        panel.SetActive(true);

        if (tcpAnalyticsClient != null)
        {
            tcpAnalyticsClient.SendEvent(
                panelEventType
            );
        }

        Debug.Log(
            "========== PANEL OPENED ==========\n" +
            "Panel Event: " +
            panelEventType
        );
    }


    // =====================================================
    // OPEN START QUESTION PANEL
    // =====================================================

    public void OpenStartQuestionPanel()
    {
        if (startQuestionPanel == null)
            return;

        if (startQuestionPanel.activeSelf)
            return;

        startQuestionPanelOpenTime = Time.time;

        OpenPanel(
            startQuestionPanel,
            "PANEL_OPENED_START_QUESTION"
        );
    }


    // =====================================================
    // START GAME FROM MAZE 1
    // =====================================================

    public void StartGameFromMaze1()
    {
        if (gameFinished)
            return;

        // -------------------------------------------------
        // START ROOM TIMING
        // -------------------------------------------------

        // Exact time from game start until YES.
        startRoomDuration =
            Mathf.Max(
                0f,
                Time.time - startRoomStartTime
            );

        // Exact time from StartQuestionPanel opening until YES.
        if (startQuestionPanelOpenTime > 0f)
        {
            startQuestionPanelDuration =
                Mathf.Max(
                    0f,
                    Time.time - startQuestionPanelOpenTime
                );
        }
        else
        {
            startQuestionPanelDuration = 0f;
        }

        startRoomResultSaved = true;

        Debug.Log(
            "========== START ROOM FINISHED ==========\n" +
            "Start Room Duration: " +
            startRoomDuration.ToString("F2") +
            " seconds\n" +
            "Start Question Panel Duration: " +
            startQuestionPanelDuration.ToString("F2") +
            " seconds"
        );

        if (tcpAnalyticsClient != null)
        {
            tcpAnalyticsClient.SendEvent(
                "START_ROOM_YES"
            );
        }

        // -------------------------------------------------
        // IMPORTANT:
        // The GAME TIMER starts HERE.
        //
        // Therefore:
        // Start Room time is NOT included in TotalGameElapsedTime.
        // -------------------------------------------------

        gameStartTime = Time.time;

        if (startQuestionPanel != null)
            startQuestionPanel.SetActive(false);

        if (tcpAnalyticsClient != null)
        {
            tcpAnalyticsClient.SendEvent(
                "GAME_STARTED"
            );
        }

        currentMaze = 1;

        totalScore = 0;

        collectedCoins = 0;

        stageCompleted = false;

        waitingForRestart = false;

        changingMaze = false;

        gameFinished = false;

        currentAttemptSaved = false;

        finalResultSaved = false;

        mazeVisitHistory.Clear();

        exitConfirmHistory.Clear();

        mazeVisitNumber = 0;

        for (int i = 0; i < mazeAttemptCount.Length; i++)
        {
            mazeAttemptCount[i] = 0;
        }

        SetAllMazesInactive();

        ResetAllCoins();

        if (maze01 != null)
            maze01.SetActive(true);

        if (EnvironmentManager.Instance != null)
        {
            EnvironmentManager.Instance
                .ApplyDecorationsForMaze(0);
        }

        StartMaze(1);

        SetMazeUI(true);

        if (totalGameTimerCoroutine != null)
            StopCoroutine(totalGameTimerCoroutine);

        totalGameTimerCoroutine =
            StartCoroutine(
                TotalGameTimer()
            );

        Debug.Log(
            "Game started from Maze 1."
        );
    }


    // =====================================================
    // OPEN EXIT CONFIRM PANEL
    // =====================================================

    public void OpenExitConfirmPanel()
    {
        if (gameFinished)
            return;

        if (exitConfirmPanel == null)
            return;

        if (exitConfirmPanel.activeSelf)
            return;

        // Start timing THIS specific Exit panel opening.
        exitConfirmPanelOpenTime = Time.time;

        exitConfirmPanelCurrentlyOpen = true;

        OpenPanel(
            exitConfirmPanel,
            "PANEL_OPENED_EXIT_CONFIRM"
        );

        Debug.Log(
            "ExitConfirmPanel timer started."
        );
    }


    // =====================================================
    // EXIT CONFIRM YES
    // =====================================================

    public void ConfirmExitYes()
    {
        if (gameFinished)
            return;

        if (!exitConfirmPanelCurrentlyOpen)
        {
            Debug.LogWarning(
                "ConfirmExitYes called but ExitConfirmPanel timer is not active."
            );
        }

        float panelDuration = 0f;

        if (exitConfirmPanelOpenTime > 0f)
        {
            panelDuration =
                Mathf.Max(
                    0f,
                    Time.time - exitConfirmPanelOpenTime
                );
        }

        SaveExitConfirmInteraction(
            "YES",
            panelDuration
        );

        exitConfirmPanelCurrentlyOpen = false;
        exitConfirmPanelOpenTime = 0f;

        if (exitConfirmPanel != null)
            exitConfirmPanel.SetActive(false);

        if (tcpAnalyticsClient != null)
        {
            tcpAnalyticsClient.SendEvent(
                "EXIT_CONFIRM_YES"
            );
        }

        Debug.Log(
            "Exit YES selected.\n" +
            "Exit Confirm Duration: " +
            panelDuration.ToString("F2") +
            " seconds"
        );

        // -------------------------------------------------
        // IMPORTANT:
        // ExitGame() is called at EXACTLY the moment YES
        // is selected.
        //
        // Therefore Maze Duration ends at YES.
        // -------------------------------------------------

        ExitGame();
    }


    // =====================================================
    // EXIT CONFIRM NO
    // =====================================================

    public void ConfirmExitNo()
    {
        if (gameFinished)
            return;

        float panelDuration = 0f;

        if (exitConfirmPanelCurrentlyOpen &&
            exitConfirmPanelOpenTime > 0f)
        {
            panelDuration =
                Mathf.Max(
                    0f,
                    Time.time - exitConfirmPanelOpenTime
                );
        }

        SaveExitConfirmInteraction(
            "NO",
            panelDuration
        );

        exitConfirmPanelCurrentlyOpen = false;
        exitConfirmPanelOpenTime = 0f;

        if (exitConfirmPanel != null)
            exitConfirmPanel.SetActive(false);

        if (tcpAnalyticsClient != null)
        {
            tcpAnalyticsClient.SendEvent(
                "EXIT_CONFIRM_NO"
            );
        }

        Debug.Log(
            "Exit NO selected.\n" +
            "Exit Confirm Duration: " +
            panelDuration.ToString("F2") +
            " seconds\n" +
            "Game continues."
        );
    }


    // =====================================================
    // SAVE EXIT CONFIRM INTERACTION
    // =====================================================

    private void SaveExitConfirmInteraction(
        string result,
        float duration
    )
    {
        ExitConfirmRecord record =
            new ExitConfirmRecord();

        record.interactionNumber =
            exitConfirmHistory.Count + 1;

        record.mazeNumber =
            currentMaze;

        record.attemptNumber =
            GetCurrentAttemptNumber();

        record.result =
            result;

        record.durationSeconds =
            duration;

        record.totalGameElapsedTime =
            TotalGameElapsedTime;

        exitConfirmHistory.Add(record);

        Debug.Log(
            "========== EXIT CONFIRM SAVED ==========\n" +
            "Interaction: " +
            record.interactionNumber +
            "\nMaze: " +
            record.mazeNumber +
            "\nAttempt: " +
            record.attemptNumber +
            "\nResult: " +
            record.result +
            "\nDuration: " +
            record.durationSeconds.ToString("F2") +
            " sec" +
            "\nTotal Game Time: " +
            record.totalGameElapsedTime.ToString("F2") +
            " sec"
        );
    }


    // =====================================================
    // GET CURRENT ATTEMPT NUMBER
    // =====================================================

    private int GetCurrentAttemptNumber()
    {
        if (currentMaze > 0 &&
            currentMaze < mazeAttemptCount.Length)
        {
            return mazeAttemptCount[currentMaze];
        }

        return 0;
    }


    // =====================================================
    // SET MAZE UI
    // =====================================================

    public void SetMazeUI(bool show)
    {
        if (exitButton != null)
            exitButton.SetActive(show);

        if (timerPanel != null)
            timerPanel.SetActive(show);

        if (timerText != null)
            timerText.gameObject.SetActive(show);

        if (coinCounterPanel != null)
        {
            if (show &&
                scoreMode == ScoreMode.CoinsAndMazeScore)
            {
                coinCounterPanel.SetActive(true);
            }
            else
            {
                coinCounterPanel.SetActive(false);
            }
        }

        if (coinCounterText != null)
        {
            if (show &&
                scoreMode == ScoreMode.CoinsAndMazeScore)
            {
                coinCounterText.gameObject.SetActive(true);
            }
            else
            {
                coinCounterText.gameObject.SetActive(false);
            }
        }
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
            if (coinCounterPanel != null)
                coinCounterPanel.SetActive(false);

            if (coinCounterText != null)
                coinCounterText.gameObject.SetActive(false);
        }
        else
        {
            if (coinCounterPanel != null)
                coinCounterPanel.SetActive(true);

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

        mazeAttemptCount[mazeNumber]++;

        currentAttemptSaved = false;

        // -------------------------------------------------
        // IMPORTANT:
        // Maze time starts exactly when player is moved
        // to this maze and the maze starts.
        // -------------------------------------------------

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

        if (tcpAnalyticsClient != null)
        {
            tcpAnalyticsClient.SendEvent(
                "MAZE_STARTED"
            );
        }
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
        if (scoreMode ==
            ScoreMode.MazeScoreOnly)
        {
            if (coinCounterPanel != null)
                coinCounterPanel.SetActive(false);

            return;
        }

        if (coinCounterPanel != null)
            coinCounterPanel.SetActive(true);

        if (coinCounterText != null)
        {
            coinCounterText.gameObject.SetActive(true);

            coinCounterText.text =
                "Coins: " +
                collectedCoins +
                "/" +
                totalCoins;
        }
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
        if (timerPanel != null)
            timerPanel.SetActive(true);

        if (timerText != null)
            timerText.gameObject.SetActive(true);
    }


    // =====================================================
    // HIDE MAIN TIMER
    // =====================================================

    private void HideMainTimer()
    {
        if (timerPanel != null)
            timerPanel.SetActive(false);

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

        if (currentMaze <= 0 ||
            currentMaze >= mazeAttemptCount.Length)
        {
            Debug.LogWarning(
                "Cannot save maze attempt. Invalid maze number: " +
                currentMaze
            );

            return;
        }

        // -------------------------------------------------
        // Capture values BEFORE anything gets reset.
        // -------------------------------------------------

        int mazeNumberAtEnd =
            currentMaze;

        int attemptNumberAtEnd =
            mazeAttemptCount[currentMaze];

        int coinsCollectedAtEnd =
            collectedCoins;

        int totalCoinsAtEnd =
            totalCoins;

        float attemptTime =
            Mathf.Max(
                0f,
                Time.time - mazeStartTime
            );

        // -------------------------------------------------
        // Existing AnalyticsLogger
        // -------------------------------------------------

        if (analyticsLogger != null)
        {
            analyticsLogger.SaveMazeAttempt(
                mazeNumberAtEnd,
                attemptNumberAtEnd,
                result,
                coinsCollectedAtEnd,
                totalCoinsAtEnd,
                attemptTime
            );
        }
        else
        {
            Debug.LogWarning(
                "GameManager: Analytics Logger is not assigned!"
            );
        }

        // -------------------------------------------------
        // Maze Visit History
        // -------------------------------------------------

        mazeVisitNumber++;

        MazeVisitRecord record =
            new MazeVisitRecord();

        record.visitNumber =
            mazeVisitNumber;

        record.mazeNumber =
            mazeNumberAtEnd;

        record.attemptNumber =
            attemptNumberAtEnd;

        record.durationSeconds =
            attemptTime;

        record.collectedCoins =
            coinsCollectedAtEnd;

        record.totalCoins =
            totalCoinsAtEnd;

        record.result =
            result;

        record.totalGameElapsedTime =
            TotalGameElapsedTime;

        record.startRoomDuration =
            startRoomDuration;

        record.startQuestionPanelDuration =
            startQuestionPanelDuration;

        mazeVisitHistory.Add(record);

        // -------------------------------------------------
        // TCP Maze Visit Summary
        // -------------------------------------------------

        if (tcpAnalyticsClient != null)
        {
            tcpAnalyticsClient.SendMazeVisitSummary(
                record
            );
        }

        currentAttemptSaved = true;

        Debug.Log(
            "========== MAZE VISIT SAVED ==========\n" +
            "Visit Number: " +
            record.visitNumber +
            "\nMaze: " +
            record.mazeNumber +
            "\nAttempt: " +
            record.attemptNumber +
            "\nDuration: " +
            record.durationSeconds.ToString("F2") +
            " seconds" +
            "\nCoins: " +
            record.collectedCoins +
            "/" +
            record.totalCoins +
            "\nResult: " +
            record.result +
            "\nTotal Game Time: " +
            record.totalGameElapsedTime.ToString("F2") +
            "\nStart Room Duration: " +
            record.startRoomDuration.ToString("F2") +
            "\nStart Question Duration: " +
            record.startQuestionPanelDuration.ToString("F2")
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

        SaveCurrentMazeAttempt(
            "FAILED - TIME OVER"
        );

        if (tcpAnalyticsClient != null)
        {
            tcpAnalyticsClient.SendEvent(
                "MAZE_FAILED_TIME_OVER"
            );
        }

        totalScore = 0;

        collectedCoins = 0;

        ResetAllCoins();

        SetAllMazesInactive();

        if (maze01 != null)
            maze01.SetActive(true);

        if (EnvironmentManager.Instance != null)
        {
            EnvironmentManager.Instance
                .ApplyDecorationsForMaze(0);
        }

        currentMaze = 1;

        SetMazeSettings(1);

        MovePlayer(
            maze01Spawn
        );

        UpdateCoinText();

        HideMainTimer();

        OpenPanel(
            timeOverPanel,
            "PANEL_OPENED_TIME_OVER"
        );

        PlayTimeOverSound();

        restartCoroutine =
            StartCoroutine(
                RestartFromMaze01Countdown()
            );
    }


    // =====================================================
    // PLAY TIME OVER SOUND
    // =====================================================

    private void PlayTimeOverSound()
    {
        if (timeOverAudioSource == null)
        {
            Debug.LogWarning(
                "GameManager: Time Over Audio Source is not assigned!"
            );

            return;
        }

        if (timeOverFailSound == null)
        {
            Debug.LogWarning(
                "GameManager: Time Over Fail Sound is not assigned!"
            );

            return;
        }

        timeOverAudioSource.PlayOneShot(
            timeOverFailSound,
            timeOverSoundVolume
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
        ResetAllExitTriggers();

        waitingForRestart = false;

        stageCompleted = false;

        changingMaze = false;

        if (timeOverPanel != null)
            timeOverPanel.SetActive(false);

        SetAllMazesInactive();

        if (maze01 != null)
            maze01.SetActive(true);

        if (EnvironmentManager.Instance != null)
        {
            EnvironmentManager.Instance
                .ApplyDecorationsForMaze(0);
        }

        currentMaze = 1;

        SetMazeSettings(1);

        MovePlayer(
            maze01Spawn
        );

        ResetAllExitTriggers();

        ResetAllCoins();

        ResetCoins();

        ShowMainTimer();

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
        Debug.Log(
            "========== MAZE COMPLETED ==========\n" +
            "Current Maze: " +
            currentMaze +
            "\nStage Completed: " +
            stageCompleted +
            "\nChanging Maze: " +
            changingMaze +
            "\nWaiting Restart: " +
            waitingForRestart +
            "\nGame Finished: " +
            gameFinished
        );

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

        int mazeResultScore =
            currentMazeScore;

        if (scoreMode ==
            ScoreMode.CoinsAndMazeScore)
        {
            mazeResultScore +=
                collectedCoins;
        }

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

        // -------------------------------------------------
        // IMPORTANT:
        // Maze duration ends at the exit trigger.
        // -------------------------------------------------

        SaveCurrentMazeAttempt(
            "SUCCESS"
        );

        if (tcpAnalyticsClient != null)
        {
            tcpAnalyticsClient.SendEvent(
                "MAZE_SUCCESS"
            );
        }

        PlaySuccessSound();

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
    // PLAY SUCCESS SOUND
    // =====================================================

    private void PlaySuccessSound()
    {
        if (successAudioSource == null)
        {
            Debug.LogWarning(
                "GameManager: Success Audio Source is not assigned!"
            );

            return;
        }

        if (successSound == null)
        {
            Debug.LogWarning(
                "GameManager: Success Sound is not assigned!"
            );

            return;
        }

        successAudioSource.PlayOneShot(
            successSound,
            successSoundVolume
        );
    }


    // =====================================================
    // SUCCESS → NEXT MAZE
    // =====================================================

    private IEnumerator ShowSuccessAndLoadNextMaze()
    {
        OpenPanel(
            successPanel01,
            "PANEL_OPENED_SUCCESS"
        );

        if (successMessageText != null)
        {
            successMessageText.text =
                "Level " +
                currentMaze +
                " Completed! ";
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

        if (EnvironmentManager.Instance != null)
        {
            EnvironmentManager.Instance
                .ApplyDecorationsForMaze(
                    currentMaze - 1
                );
        }

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
        if (musicManager != null)
        {
            musicManager.StopBackgroundMusic();
        }

        if (totalGameTimerCoroutine != null)
        {
            StopCoroutine(
                totalGameTimerCoroutine
            );

            totalGameTimerCoroutine = null;
        }

        PlayFinalSuccessSound();

        OpenPanel(
            finalSuccessPanel,
            "PANEL_OPENED_FINAL_SUCCESS"
        );

        if (finalSuccessMessageText != null)
        {
            finalSuccessMessageText.text =
                "Congratulations!\n\n" +
                "You successfully completed all 10 mazes!";
        }

        if (finalSuccessScoreText != null)
        {
            finalSuccessScoreText.text =
                "Final Score: " +
                totalScore;
        }

        if (!finalResultSaved)
        {
            finalResultSaved = true;

            if (analyticsLogger != null)
            {
                analyticsLogger.SaveFinalResult(
                    "SUCCESS - ALL MAZES COMPLETED",
                    Time.time - gameStartTime
                );
            }

            if (tcpAnalyticsClient != null)
            {
                tcpAnalyticsClient.SendEvent(
                    "GAME_FINISHED_SUCCESS"
                );
            }
        }

        gameFinished = true;

        Time.timeScale = 0f;

        yield break;
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

        if (!gameFinished &&
            !stageCompleted &&
            !currentAttemptSaved)
        {
            SaveCurrentMazeAttempt(
                "FAILED - TOTAL TIME OVER"
            );
        }

        FinishEntireGame(
            "FAILED - TOTAL TIME OVER",
            Mathf.Min(
                Time.time - gameStartTime,
                totalGameTime
            )
        );
    }


    // =====================================================
    // EXIT GAME
    // =====================================================

    public void ExitGame()
    {
        if (gameFinished)
            return;

        // -------------------------------------------------
        // IMPORTANT:
        // This method is called by ConfirmExitYes().
        //
        // Therefore Time.time here is EXACTLY the moment
        // YES is selected.
        //
        // Maze duration therefore ends at YES.
        // -------------------------------------------------

        if (!currentAttemptSaved &&
            !stageCompleted &&
            !waitingForRestart &&
            !changingMaze)
        {
            SaveCurrentMazeAttempt(
                "FAILED - EXIT"
            );

            if (tcpAnalyticsClient != null)
            {
                tcpAnalyticsClient.SendEvent(
                    "MAZE_EXIT"
                );
            }
        }

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

        if (musicManager != null)
        {
            musicManager.StopBackgroundMusic();
        }

        PlayFinalGameSound();

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

        OpenPanel(
            finalGamePanel,
            "PANEL_OPENED_FINAL_GAME"
        );

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

        PrintMazeVisitHistory();

        PrintExitConfirmHistory();

        PrintStartRoomTiming();

        Time.timeScale = 0f;

        if (tcpAnalyticsClient != null)
        {
            tcpAnalyticsClient.SendEvent(
                "GAME_FINISHED_FAILED"
            );
        }
    }


    // =====================================================
    // PRINT START ROOM TIMING
    // =====================================================

    private void PrintStartRoomTiming()
    {
        Debug.Log(
            "============================================\n" +
            "START ROOM TIMING\n" +
            "============================================\n" +
            "Start Room Duration: " +
            startRoomDuration.ToString("F2") +
            " sec\n" +
            "Start Question Panel Duration: " +
            startQuestionPanelDuration.ToString("F2") +
            " sec\n" +
            "============================================"
        );
    }


    // =====================================================
    // PRINT MAZE VISIT HISTORY
    // =====================================================

    private void PrintMazeVisitHistory()
    {
        Debug.Log(
            "============================================\n" +
            "COMPLETE MAZE VISIT HISTORY\n" +
            "Total Visits: " +
            mazeVisitHistory.Count +
            "\n============================================"
        );

        foreach (MazeVisitRecord record
                 in mazeVisitHistory)
        {
            Debug.Log(
                "Visit #" +
                record.visitNumber +
                " | Maze " +
                record.mazeNumber +
                " | Attempt " +
                record.attemptNumber +
                " | Time: " +
                record.durationSeconds.ToString("F2") +
                " sec" +
                " | Coins: " +
                record.collectedCoins +
                "/" +
                record.totalCoins +
                " | Result: " +
                record.result
            );
        }
    }


    // =====================================================
    // PRINT EXIT HISTORY
    // =====================================================

    private void PrintExitConfirmHistory()
    {
        Debug.Log(
            "============================================\n" +
            "COMPLETE EXIT CONFIRM HISTORY\n" +
            "Total Interactions: " +
            exitConfirmHistory.Count +
            "\n============================================"
        );

        foreach (ExitConfirmRecord record
                 in exitConfirmHistory)
        {
            Debug.Log(
                "Interaction #" +
                record.interactionNumber +
                " | Maze " +
                record.mazeNumber +
                " | Attempt " +
                record.attemptNumber +
                " | Result: " +
                record.result +
                " | Panel Duration: " +
                record.durationSeconds.ToString("F2") +
                " sec" +
                " | Total Game Time: " +
                record.totalGameElapsedTime.ToString("F2") +
                " sec"
            );
        }
    }


    // =====================================================
    // GET HISTORY COPY
    // =====================================================

    public List<MazeVisitRecord> GetMazeVisitHistory()
    {
        return new List<MazeVisitRecord>(
            mazeVisitHistory
        );
    }


    // =====================================================
    // GET EXIT HISTORY COPY
    // =====================================================

    public List<ExitConfirmRecord> GetExitConfirmHistory()
    {
        return new List<ExitConfirmRecord>(
            exitConfirmHistory
        );
    }


    // =====================================================
    // PLAY FINAL GAME SOUND
    // =====================================================

    private void PlayFinalGameSound()
    {
        if (finalGameAudioSource == null)
        {
            Debug.LogWarning(
                "GameManager: Final Game Audio Source is not assigned!"
            );

            return;
        }

        if (finalGameSound == null)
        {
            Debug.LogWarning(
                "GameManager: Final Game Sound is not assigned!"
            );

            return;
        }

        finalGameAudioSource.PlayOneShot(
            finalGameSound,
            finalGameSoundVolume
        );
    }


    // =====================================================
    // RESET ALL EXIT TRIGGERS
    // =====================================================

    private void ResetAllExitTriggers()
    {
        ExitTrigger[] exits =
            FindObjectsByType<ExitTrigger>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (ExitTrigger exit in exits)
        {
            FieldInfo field =
                typeof(ExitTrigger)
                .GetField(
                    "completed",
                    BindingFlags.NonPublic |
                    BindingFlags.Instance
                );

            if (field != null)
            {
                field.SetValue(
                    exit,
                    false
                );
            }
        }

        Debug.Log(
            "All ExitTriggers reset."
        );
    }


    // =====================================================
    // PLAY FINAL SUCCESS SOUND
    // =====================================================

    private void PlayFinalSuccessSound()
    {
        if (finalSuccessAudioSource == null)
        {
            Debug.LogWarning(
                "Final Success Audio Source not assigned!"
            );

            return;
        }

        if (finalSuccessSound == null)
        {
            Debug.LogWarning(
                "Final Success Sound not assigned!"
            );

            return;
        }

        finalSuccessAudioSource.PlayOneShot(
            finalSuccessSound,
            finalSuccessSoundVolume
        );
    }


    // =====================================================
    // SET GAME MODE
    // =====================================================

    public void SetGameMode(bool useCoins)
    {
        if (useCoins)
        {
            scoreMode =
                ScoreMode.CoinsAndMazeScore;
        }
        else
        {
            scoreMode =
                ScoreMode.MazeScoreOnly;
        }

        Debug.Log(
            "Game Mode changed. Use Coins = " +
            useCoins
        );
    }
}


// =========================================================
// MAZE VISIT RECORD
// =========================================================

[System.Serializable]
public class MazeVisitRecord
{
    // -----------------------------------------------------
    // Order in which participant entered a maze
    // -----------------------------------------------------

    public int visitNumber;


    // -----------------------------------------------------
    // Maze information
    // -----------------------------------------------------

    public int mazeNumber;

    public int attemptNumber;


    // -----------------------------------------------------
    // Time spent in THIS specific maze visit
    //
    // Starts:
    //     StartMaze()
    //
    // Ends:
    //     MazeCompleted()
    //     OR ConfirmExitYes() → ExitGame()
    //     OR Time Over
    // -----------------------------------------------------

    public float durationSeconds;


    // -----------------------------------------------------
    // Coins collected
    // -----------------------------------------------------

    public int collectedCoins;

    public int totalCoins;


    // -----------------------------------------------------
    // How this visit ended
    // -----------------------------------------------------

    public string result;


    // -----------------------------------------------------
    // Total game time at the moment this visit ended
    // -----------------------------------------------------

    public float totalGameElapsedTime;


    // -----------------------------------------------------
    // START ROOM DATA
    // -----------------------------------------------------

    public float startRoomDuration;

    public float startQuestionPanelDuration;
}


// =========================================================
// EXIT CONFIRM RECORD
// =========================================================

[System.Serializable]
public class ExitConfirmRecord
{
    // -----------------------------------------------------
    // Order of ExitConfirmPanel interactions
    // -----------------------------------------------------

    public int interactionNumber;


    // -----------------------------------------------------
    // Maze where ExitConfirmPanel was opened
    // -----------------------------------------------------

    public int mazeNumber;

    public int attemptNumber;


    // -----------------------------------------------------
    // YES or NO
    // -----------------------------------------------------

    public string result;


    // -----------------------------------------------------
    // Time from opening ExitConfirmPanel
    // until YES or NO
    // -----------------------------------------------------

    public float durationSeconds;


    // -----------------------------------------------------
    // Total game elapsed time when YES/NO was selected
    // -----------------------------------------------------

    public float totalGameElapsedTime;
}