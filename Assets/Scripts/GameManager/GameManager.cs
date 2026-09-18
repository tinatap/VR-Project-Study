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
    // TCP
    // =====================================================

    [Header("TCP Event Stream")]
    [Tooltip("Assign the GameObject that contains TCP.cs.")]
    public TCP tcp;

    private int[] mazeAttemptCount = new int[11];

    private float mazeStartTime;
    private float gameStartTime;

    private bool currentAttemptSaved = false;
    private bool finalResultSaved = false;
    private bool experimentSummarySent = false;


    // =====================================================
    // START ROOM TIMING
    // =====================================================

    [Header("Start Room Timing")]

    [Tooltip("Time from game launch until YES is pressed in StartQuestionPanel.")]
    [SerializeField]
    private float startRoomDuration = 0f;

    private float startRoomStartTime;
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

    public AudioSource successAudioSource;
    public AudioClip successSound;

    [Range(0f, 1f)]
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

    public AudioSource timeOverAudioSource;
    public AudioClip timeOverFailSound;

    [Range(0f, 1f)]
    public float timeOverSoundVolume = 1f;


    // =====================================================
    // TIMER UI
    // =====================================================

    [Header("Maze Timer UI")]

    public GameObject timerPanel;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI mazeNumberText;


    // =====================================================
    // EXIT UI
    // =====================================================

    [Header("Exit UI")]

    public GameObject exitButton;
    public GameObject exitConfirmPanel;


    // =====================================================
    // START QUESTION UI
    // =====================================================

    [Header("Start Question UI")]

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

    public AudioSource finalGameAudioSource;
    public AudioClip finalGameSound;

    [Range(0f, 1f)]
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
    // MAZE SETTINGS
    // =====================================================

    [Header("Maze 01 Settings")]
    public int maze01TotalCoins = 10;
    public float maze01Time = 30f;
    public int maze01Score = 10;

    [Header("Maze 02 Settings")]
    public int maze02TotalCoins = 15;
    public float maze02Time = 40f;
    public int maze02Score = 20;

    [Header("Maze 03 Settings")]
    public int maze03TotalCoins = 20;
    public float maze03Time = 45f;
    public int maze03Score = 30;

    [Header("Maze 04 Settings")]
    public int maze04TotalCoins = 25;
    public float maze04Time = 50f;
    public int maze04Score = 40;

    [Header("Maze 05 Settings")]
    public int maze05TotalCoins = 30;
    public float maze05Time = 55f;
    public int maze05Score = 50;

    [Header("Maze 06 Settings")]
    public int maze06TotalCoins = 35;
    public float maze06Time = 60f;
    public int maze06Score = 60;

    [Header("Maze 07 Settings")]
    public int maze07TotalCoins = 40;
    public float maze07Time = 65f;
    public int maze07Score = 70;

    [Header("Maze 08 Settings")]
    public int maze08TotalCoins = 45;
    public float maze08Time = 70f;
    public int maze08Score = 80;

    [Header("Maze 09 Settings")]
    public int maze09TotalCoins = 50;
    public float maze09Time = 75f;
    public int maze09Score = 90;

    [Header("Maze 10 Settings")]
    public int maze10TotalCoins = 55;
    public float maze10Time = 80f;
    public int maze10Score = 100;


    // =====================================================
    // MAZE TIME ATTEMPT MULTIPLIERS
    // =====================================================

    [Header("Maze Time Attempt Multipliers")]

    [Tooltip("Time multiplier for 1st attempt.")]
    public float attempt1TimeMultiplier = 1f;

    [Tooltip("Time multiplier for 2nd attempt.")]
    public float attempt2TimeMultiplier = 0.8f;

    [Tooltip("Time multiplier for 3rd attempt.")]
    public float attempt3TimeMultiplier = 0.6f;

    [Tooltip("Time multiplier for 4th attempt.")]
    public float attempt4TimeMultiplier = 0.5f;

    [Tooltip("Time multiplier for 5th attempt.")]
    public float attempt5TimeMultiplier = 0.4f;

    [Tooltip("Time multiplier for 6th attempt and beyond.")]
    public float attempt6PlusTimeMultiplier = 0.3f;


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

    private int highestCompletedMaze = 0;
    private float currentMazeTime;

    private bool stageCompleted = false;
    private bool waitingForRestart = false;
    private bool changingMaze = false;
    private bool gameFinished = false;

    private int finalSuccessRequestFrame = -1;

    private bool exitRequestedThisFrame = false;
    private bool exitFinishedGame = false;


    // =====================================================
    // ANALYTICS PUBLIC DATA
    // =====================================================

    public int CurrentMaze => currentMaze;

    public int CurrentAttempt
    {
        get
        {
            return GetCurrentAttemptNumber();
        }
    }

    public int CollectedCoins => collectedCoins;
    public int TotalCoins => totalCoins;
    public int TotalScore => totalScore;
    public int HighestCompletedMaze => highestCompletedMaze;

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

    public float StartRoomDuration => startRoomDuration;


    // =====================================================
    // MAZE HISTORY PUBLIC DATA
    // =====================================================

    public List<MazeVisitRecord> MazeVisitHistory =>
        mazeVisitHistory;

    public int MazeVisitCount =>
        mazeVisitHistory.Count;


    // =====================================================
    // EXIT HISTORY PUBLIC DATA
    // =====================================================

    public List<ExitConfirmRecord> ExitConfirmHistory =>
        exitConfirmHistory;

    public int ExitConfirmCount =>
        exitConfirmHistory.Count;


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

        if (tcp == null)
        {
            tcp = FindFirstObjectByType<TCP>();
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
        startRoomStartTime = Time.time;
        startRoomResultSaved = false;

        exitConfirmPanelOpenTime = 0f;
        exitConfirmPanelCurrentlyOpen = false;

        SetPanelActive(successPanel01, false);
        SetPanelActive(timeOverPanel, false);
        SetPanelActive(finalGamePanel, false);
        SetPanelActive(finalSuccessPanel, false);
        SetPanelActive(exitConfirmPanel, false);
        SetPanelActive(startQuestionPanel, false);

        SetAllMazesInactive();

        SetupCoinMode();
        SetMazeUI(false);

        Debug.Log(
            "========== GAME INITIALIZED ==========\n" +
            "Start Room Timer Started."
        );
    }


    // =====================================================
    // SEND EVENT TO TCP
    // =====================================================

    private void SendTCPEvent(
        string eventName,
        string eventType = "GAME_EVENT",
        string eventMessage = ""
    )
    {
        if (tcp == null)
        {
            tcp = FindFirstObjectByType<TCP>();
        }

        if (tcp != null)
        {
            tcp.LogEvent(
                eventType,
                eventName,
                eventMessage
            );
        }
        else
        {
            Debug.LogWarning(
                "GameManager: TCP reference is not assigned/found.\n" +
                "Event was not sent: " + eventName
            );
        }

        Debug.Log(
            "========== TCP EVENT ==========\n" +
            "Type: " + eventType +
            "\nEvent: " + eventName +
            "\nMessage: " + eventMessage +
            "\nMaze: " + currentMaze +
            "\nAttempt: " + GetCurrentAttemptNumber() +
            "\nMaze Time: " +
            CurrentMazeElapsedTime.ToString("F3") +
            " sec" +
            "\nTotal Game Time: " +
            TotalGameElapsedTime.ToString("F3") +
            " sec"
        );
    }


    // =====================================================
    // OPEN PANEL
    // =====================================================

    private void OpenPanel(
        GameObject panel,
        string panelEventName
    )
    {
        if (panel == null || panel.activeSelf)
            return;

        panel.SetActive(true);

        SendTCPEvent(
            panelEventName,
            "PANEL_OPENED"
        );
    }


    // =====================================================
    // CLOSE PANEL
    // =====================================================

    private void ClosePanel(
        GameObject panel,
        string panelEventName
    )
    {
        if (panel == null || !panel.activeSelf)
            return;

        panel.SetActive(false);

        SendTCPEvent(
            panelEventName,
            "PANEL_CLOSED"
        );
    }


    // =====================================================
    // OPEN START QUESTION PANEL
    // =====================================================

    public void OpenStartQuestionPanel()
    {
        OpenPanel(
            startQuestionPanel,
            "PANEL_OPENED_START_QUESTION"
        );
    }


    // =====================================================
    // REGISTER START ROOM YES
    // =====================================================

    public void RegisterStartRoomYes()
    {
        if (startRoomResultSaved)
            return;

        startRoomDuration =
            Mathf.Max(
                0f,
                Time.time - startRoomStartTime
            );

        startRoomResultSaved = true;

        SendTCPEvent(
            "START_ROOM_YES",
            "BUTTON_PRESSED",
            "StartRoomDuration=" +
            startRoomDuration.ToString("F3")
        );

        Debug.Log(
            "START_ROOM_YES registered immediately.\n" +
            "Start Room Duration: " +
            startRoomDuration.ToString("F3") +
            " sec"
        );
    }


    // =====================================================
    // START GAME FROM MAZE 1
    // =====================================================

    public void StartGameFromMaze1()
    {
        if (gameFinished)
            return;

        gameStartTime = Time.time;

        ClosePanel(
            startQuestionPanel,
            "PANEL_CLOSED_START_QUESTION"
        );

        SendTCPEvent(
            "GAME_STARTED",
            "GAME_EVENT"
        );

        currentMaze = 1;

        totalScore = 0;
        collectedCoins = 0;
        highestCompletedMaze = 0;

        stageCompleted = false;
        waitingForRestart = false;
        changingMaze = false;
        gameFinished = false;

        currentAttemptSaved = false;
        finalResultSaved = false;
        experimentSummarySent = false;

        mazeVisitHistory.Clear();
        exitConfirmHistory.Clear();
        mazeVisitNumber = 0;

        for (int i = 0; i < mazeAttemptCount.Length; i++)
        {
            mazeAttemptCount[i] = 0;
        }

        SetAllMazesInactive();

        SetPanelActive(maze01, true);

        if (EnvironmentManager.Instance != null)
        {
            EnvironmentManager.Instance.ApplyDecorationsForMaze(0);
        }

        StartMaze(1);

        SetMazeUI(true);

        if (totalGameTimerCoroutine != null)
            StopCoroutine(totalGameTimerCoroutine);

        totalGameTimerCoroutine =
            StartCoroutine(TotalGameTimer());

        Debug.Log("Game started from Maze 1.");
    }


    // =====================================================
    // OPEN EXIT CONFIRM PANEL
    // =====================================================

    public void OpenExitConfirmPanel()
    {
        if (gameFinished ||
            exitConfirmPanel == null ||
            exitConfirmPanel.activeSelf)
        {
            return;
        }

        exitConfirmPanelOpenTime = Time.time;
        exitConfirmPanelCurrentlyOpen = true;

        OpenPanel(
            exitConfirmPanel,
            "PANEL_OPENED_EXIT_CONFIRM"
        );
    }


    // =====================================================
    // EXIT CONFIRM YES
    // =====================================================

    public void ConfirmExitYes()
    {
        if (gameFinished)
            return;

        exitRequestedThisFrame = true;
        exitFinishedGame = true;

        float exitMazeDuration =
            CurrentMazeElapsedTime;

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

        SendTCPEvent(
            "EXIT_CONFIRM_YES",
            "BUTTON_PRESSED",
            "PanelDuration=" +
            panelDuration.ToString("F3")
        );

        exitConfirmPanelCurrentlyOpen = false;
        exitConfirmPanelOpenTime = 0f;

        ClosePanel(
            exitConfirmPanel,
            "PANEL_CLOSED_EXIT_CONFIRM"
        );

        if (!currentAttemptSaved)
        {
            SaveCurrentMazeAttempt(
                "FAILED - EXIT",
                "EXIT_YES",
                exitMazeDuration
            );

            SendTCPEvent(
                "MAZE_EXIT",
                "GAME_EVENT",
                "Result=FAILED - EXIT" +
                "; MazeDuration=" +
                exitMazeDuration.ToString("F3") +
                "; CollectedCoins=" +
                collectedCoins
            );
        }

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

        SendTCPEvent(
            "EXIT_CONFIRM_NO",
            "BUTTON_PRESSED",
            "PanelDuration=" +
            panelDuration.ToString("F3")
        );

        exitConfirmPanelCurrentlyOpen = false;
        exitConfirmPanelOpenTime = 0f;

        ClosePanel(
            exitConfirmPanel,
            "PANEL_CLOSED_EXIT_CONFIRM"
        );

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

        record.mazeNumber = currentMaze;
        record.attemptNumber = GetCurrentAttemptNumber();
        record.result = result;
        record.durationSeconds = duration;
        record.totalGameElapsedTime =
            TotalGameElapsedTime;

        exitConfirmHistory.Add(record);
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
        SetPanelActive(exitButton, show);
        SetPanelActive(timerPanel, show);

        if (timerText != null)
            timerText.gameObject.SetActive(show);

        if (mazeNumberText != null)
            mazeNumberText.gameObject.SetActive(show);

        bool showCoins =
            show &&
            scoreMode == ScoreMode.CoinsAndMazeScore;

        SetPanelActive(coinCounterPanel, showCoins);

        if (coinCounterText != null)
            coinCounterText.gameObject.SetActive(showCoins);
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

        bool enableCoins =
            scoreMode == ScoreMode.CoinsAndMazeScore;

        foreach (Coin coin in allCoins)
        {
            if (coin != null)
                coin.gameObject.SetActive(enableCoins);
        }

        SetPanelActive(
            coinCounterPanel,
            enableCoins
        );

        if (coinCounterText != null)
            coinCounterText.gameObject.SetActive(enableCoins);
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

        mazeStartTime = Time.time;

        SetMazeSettings(mazeNumber);

        if (mazeNumberText != null)
        {
            mazeNumberText.text =
                "Maze " + mazeNumber;

            mazeNumberText.gameObject.SetActive(true);
        }

        collectedCoins = 0;

        ResetCoins();

        MovePlayer(
            GetSpawnPoint(mazeNumber)
        );

        ShowMainTimer();

        StartMazeTimer();

        SendTCPEvent(
            "MAZE_STARTED",
            "GAME_EVENT",
            "Maze=" +
            mazeNumber +
            "; Attempt=" +
            mazeAttemptCount[mazeNumber]
        );

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

            default:
                totalCoins = 0;
                currentMazeTime = 0f;
                currentMazeScore = 0;
                break;
        }

        int attemptNumber =
            mazeAttemptCount[mazeNumber];

        float timeMultiplier =
            GetAttemptTimeMultiplier(attemptNumber);

        currentMazeTime *= timeMultiplier;

        if (scoreMode == ScoreMode.MazeScoreOnly)
            totalCoins = 0;

        Debug.Log(
            "========== MAZE SETTINGS ==========\n" +
            "Maze: " + mazeNumber +
            "\nAttempt: " + attemptNumber +
            "\nBase Time: " +
            GetBaseMazeTime(mazeNumber).ToString("F2") +
            " sec" +
            "\nTime Multiplier: " +
            timeMultiplier.ToString("F2") +
            "\nFinal Maze Time: " +
            currentMazeTime.ToString("F2") +
            " sec"
        );
    }


    // =====================================================
    // GET ATTEMPT TIME MULTIPLIER
    // =====================================================

    private float GetAttemptTimeMultiplier(int attemptNumber)
    {
        switch (attemptNumber)
        {
            case 1:
                return attempt1TimeMultiplier;

            case 2:
                return attempt2TimeMultiplier;

            case 3:
                return attempt3TimeMultiplier;

            case 4:
                return attempt4TimeMultiplier;

            case 5:
                return attempt5TimeMultiplier;

            default:
                return attempt6PlusTimeMultiplier;
        }
    }


    // =====================================================
    // GET BASE MAZE TIME
    // =====================================================

    private float GetBaseMazeTime(int mazeNumber)
    {
        switch (mazeNumber)
        {
            case 1: return maze01Time;
            case 2: return maze02Time;
            case 3: return maze03Time;
            case 4: return maze04Time;
            case 5: return maze05Time;
            case 6: return maze06Time;
            case 7: return maze07Time;
            case 8: return maze08Time;
            case 9: return maze09Time;
            case 10: return maze10Time;
        }

        return 0f;
    }


    // =====================================================
    // GET SPAWN POINT
    // =====================================================

    private Transform GetSpawnPoint(int mazeNumber)
    {
        switch (mazeNumber)
        {
            case 1: return maze01Spawn;
            case 2: return maze02Spawn;
            case 3: return maze03Spawn;
            case 4: return maze04Spawn;
            case 5: return maze05Spawn;
            case 6: return maze06Spawn;
            case 7: return maze07Spawn;
            case 8: return maze08Spawn;
            case 9: return maze09Spawn;
            case 10: return maze10Spawn;
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
            case 1: return maze01;
            case 2: return maze02;
            case 3: return maze03;
            case 4: return maze04;
            case 5: return maze05;
            case 6: return maze06;
            case 7: return maze07;
            case 8: return maze08;
            case 9: return maze09;
            case 10: return maze10;
        }

        return null;
    }


    // =====================================================
    // DISABLE ALL MAZES
    // =====================================================

    private void SetAllMazesInactive()
    {
        if (maze01 != null) maze01.SetActive(false);
        if (maze02 != null) maze02.SetActive(false);
        if (maze03 != null) maze03.SetActive(false);
        if (maze04 != null) maze04.SetActive(false);
        if (maze05 != null) maze05.SetActive(false);
        if (maze06 != null) maze06.SetActive(false);
        if (maze07 != null) maze07.SetActive(false);
        if (maze08 != null) maze08.SetActive(false);
        if (maze09 != null) maze09.SetActive(false);
        if (maze10 != null) maze10.SetActive(false);
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
    // COLLECT COIN
    // =====================================================

    public void CollectCoin()
    {
        if (scoreMode == ScoreMode.MazeScoreOnly)
            return;

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
        totalScore++;

        UpdateCoinText();
    }


    // =====================================================
    // RESET COINS
    // =====================================================

    private void ResetCoins()
    {
        UpdateCoinText();
    }


    // =====================================================
    // UPDATE COIN TEXT
    // =====================================================

    private void UpdateCoinText()
    {
        if (scoreMode == ScoreMode.MazeScoreOnly)
        {
            SetPanelActive(coinCounterPanel, false);

            if (coinCounterText != null)
                coinCounterText.gameObject.SetActive(false);

            return;
        }

        SetPanelActive(coinCounterPanel, true);

        if (coinCounterText != null)
        {
            coinCounterText.gameObject.SetActive(true);
            coinCounterText.text = "       " + totalScore;
        }
    }


    // =====================================================
    // MAIN MAZE TIMER
    // =====================================================

    private void StartMazeTimer()
    {
        StopMazeTimer();
        timerCoroutine = StartCoroutine(MazeTimer());
    }


    private IEnumerator MazeTimer()
    {
        float remainingTime = currentMazeTime;

        while (remainingTime > 0f)
        {
            if (stageCompleted ||
                waitingForRestart ||
                changingMaze ||
                gameFinished)
            {
                yield break;
            }

            UpdateTimerText(remainingTime);

            yield return null;

            remainingTime -= Time.deltaTime;
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

        timerText.text =
            "Time: " +
            Mathf.CeilToInt(time);
    }


    // =====================================================
    // SHOW MAIN TIMER
    // =====================================================

    private void ShowMainTimer()
    {
        SetPanelActive(timerPanel, true);

        if (timerText != null)
            timerText.gameObject.SetActive(true);
    }


    // =====================================================
    // HIDE MAIN TIMER
    // =====================================================

    private void HideMainTimer()
    {
        SetPanelActive(timerPanel, false);

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
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
    }


    // =====================================================
    // SAVE CURRENT MAZE ATTEMPT
    // =====================================================

    private void SaveCurrentMazeAttempt(
        string result,
        string endReason,
        float duration
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

        int mazeNumberAtEnd = currentMaze;
        int attemptNumberAtEnd =
            mazeAttemptCount[currentMaze];

        int coinsCollectedAtEnd = collectedCoins;
        int totalCoinsAtEnd = totalCoins;

        float attemptTime =
            Mathf.Max(0f, duration);

        mazeVisitNumber++;

        MazeVisitRecord record =
            new MazeVisitRecord();

        record.visitNumber = mazeVisitNumber;
        record.mazeNumber = mazeNumberAtEnd;
        record.attemptNumber = attemptNumberAtEnd;
        record.durationSeconds = attemptTime;
        record.collectedCoins = coinsCollectedAtEnd;
        record.totalCoins = totalCoinsAtEnd;
        record.result = result;
        record.endReason = endReason;
        record.totalGameElapsedTime =
            TotalGameElapsedTime;
        record.startRoomDuration =
            startRoomDuration;

        mazeVisitHistory.Add(record);

        currentAttemptSaved = true;

        Debug.Log(
            "========== MAZE VISIT SAVED ==========\n" +
            "Visit Number: " + record.visitNumber +
            "\nMaze: " + record.mazeNumber +
            "\nAttempt: " + record.attemptNumber +
            "\nDuration: " +
            record.durationSeconds.ToString("F3") +
            " seconds" +
            "\nCoins: " +
            record.collectedCoins +
            "/" +
            record.totalCoins +
            "\nResult: " +
            record.result +
            "\nEnd Reason: " +
            record.endReason +
            "\nTotal Game Time: " +
            record.totalGameElapsedTime.ToString("F3") +
            " seconds"
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

        float failedMazeTime =
            CurrentMazeElapsedTime;

        SendTCPEvent(
            "MAZE_FAILED_TIME_OVER",
            "GAME_EVENT",
            "Result=FAILED - TIME OVER" +
            "; MazeDuration=" +
            failedMazeTime.ToString("F3") +
            "; CollectedCoins=" +
            collectedCoins
        );

        SetAllMazesInactive();

        SetPanelActive(maze01, true);

        if (EnvironmentManager.Instance != null)
        {
            EnvironmentManager.Instance
                .ApplyDecorationsForMaze(0);
        }

        // Keep the failed maze number until its record is saved.
        SetMazeSettings(currentMaze);

        MovePlayer(maze01Spawn);

        UpdateCoinText();
        HideMainTimer();

        OpenPanel(
            timeOverPanel,
            "PANEL_OPENED_TIME_OVER"
        );

        SaveCurrentMazeAttempt(
            "FAILED - TIME OVER",
            "TIME_OVER_PANEL",
            failedMazeTime
        );

        currentMaze = 1;

        SetMazeSettings(1);

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
        if (timeOverAudioSource == null ||
            timeOverFailSound == null)
        {
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
        float remainingTime = restartDelay;

        while (remainingTime > 0f)
        {
            if (restartCountdownText != null)
            {
                restartCountdownText.text =
                    "Restarting from Maze 1 in " +
                    Mathf.CeilToInt(remainingTime) +
                    " seconds";
            }

            yield return null;

            remainingTime -= Time.deltaTime;
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

        ClosePanel(
            timeOverPanel,
            "PANEL_CLOSED_TIME_OVER"
        );

        SetAllMazesInactive();

        SetPanelActive(maze01, true);

        if (EnvironmentManager.Instance != null)
        {
            EnvironmentManager.Instance
                .ApplyDecorationsForMaze(0);
        }

        currentMaze = 1;

        SetMazeSettings(1);

        MovePlayer(maze01Spawn);

        ResetAllExitTriggers();
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

        int mazeResultScore = collectedCoins;

        if (currentMaze > highestCompletedMaze)
        {
            highestCompletedMaze = currentMaze;
        }

        float successfulMazeTime =
            CurrentMazeElapsedTime;

        SendTCPEvent(
            "MAZE_SUCCESS",
            "GAME_EVENT",
            "Result=SUCCESS" +
            "; MazeDuration=" +
            successfulMazeTime.ToString("F3") +
            "; CollectedCoins=" +
            collectedCoins +
            "; MazeScore=" +
            mazeResultScore +
            "; TotalScore=" +
            totalScore +
            "; HighestCompletedMaze=" +
            highestCompletedMaze
        );

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
            finalSuccessRequestFrame =
                Time.frameCount;

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
        if (successAudioSource == null ||
            successSound == null)
        {
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

        SaveCurrentMazeAttempt(
            "SUCCESS",
            "SUCCESS_PANEL",
            CurrentMazeElapsedTime
        );

        if (successMessageText != null)
        {
            successMessageText.text =
                "Level " +
                currentMaze +
                " Completed!";
        }

        float remainingTime =
            successPanelDuration;

        while (remainingTime > 0f)
        {
            if (successCountdownText != null)
            {
                successCountdownText.text =
                    "Next maze starts in: " +
                    Mathf.CeilToInt(remainingTime);
            }

            yield return null;

            remainingTime -= Time.deltaTime;
        }

        ClosePanel(
            successPanel01,
            "PANEL_CLOSED_SUCCESS"
        );

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

        StartMaze(currentMaze);

        successCoroutine = null;
    }


    // =====================================================
    // MAZE 10 COMPLETED
    // =====================================================

    private IEnumerator ShowFinalSuccess()
    {
        if (musicManager != null)
            musicManager.StopBackgroundMusic();

        if (totalGameTimerCoroutine != null)
        {
            StopCoroutine(totalGameTimerCoroutine);
            totalGameTimerCoroutine = null;
        }

        PlayFinalSuccessSound();

        OpenPanel(
            finalSuccessPanel,
            "PANEL_OPENED_FINAL_SUCCESS"
        );

        SaveCurrentMazeAttempt(
            "SUCCESS - ALL MAZES COMPLETED",
            "FINAL_SUCCESS_PANEL",
            CurrentMazeElapsedTime
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

        float finalTime =
            Time.time - gameStartTime;

        SendExperimentSummary(
            "SUCCESS - ALL MAZES COMPLETED",
            finalTime
        );

        SendTCPEvent(
            "GAME_FINISHED_SUCCESS",
            "GAME_EVENT",
            "FinalResult=SUCCESS - ALL MAZES COMPLETED" +
            "; TotalGameTime=" +
            finalTime.ToString("F3") +
            "; TotalScore=" +
            totalScore +
            "; TotalCoinsCollected=" +
            totalScore +
            "; HighestCompletedMaze=" +
            highestCompletedMaze
        );

        gameFinished = true;

        Time.timeScale = 0f;

        yield break;
    }


    // =====================================================
    // TOTAL GAME TIMER
    // =====================================================

    private IEnumerator TotalGameTimer()
    {
        float remainingTime = totalGameTime;

        while (remainingTime > 0f)
        {
            if (gameFinished)
                yield break;

            yield return null;

            remainingTime -= Time.deltaTime;
        }

        // Final Maze Success has priority.
        if (finalSuccessRequestFrame == Time.frameCount)
        {
            Debug.Log(
                "============================================\n" +
                "SIMULTANEOUS FINAL EVENTS\n" +
                "Maze 10 SUCCESS and TOTAL GAME TIME OVER\n" +
                "FINAL SUCCESS has priority.\n" +
                "============================================"
            );

            yield break;
        }

        // Exit has priority if it happens in the same frame.
        if (exitRequestedThisFrame)
        {
            Debug.Log(
                "============================================\n" +
                "SIMULTANEOUS EXIT AND TOTAL TIME OVER\n" +
                "EXIT HAS PRIORITY.\n" +
                "============================================"
            );

            exitRequestedThisFrame = false;
            yield break;
        }

        if (exitFinishedGame)
        {
            Debug.Log(
                "============================================\n" +
                "EXIT CONFIRM YES OCCURRED AT TOTAL TIME END.\n" +
                "RESULT = FAILED - EXIT\n" +
                "============================================"
            );

            yield break;
        }

        if (gameFinished)
            yield break;

        ClosePanel(
            successPanel01,
            "PANEL_CLOSED_SUCCESS_TOTAL_TIME_OVER"
        );

        ClosePanel(
            timeOverPanel,
            "PANEL_CLOSED_TIME_OVER_TOTAL_TIME_OVER"
        );

        ClosePanel(
            exitConfirmPanel,
            "PANEL_CLOSED_EXIT_CONFIRM_TOTAL_TIME_OVER"
        );

        exitConfirmPanelCurrentlyOpen = false;
        exitConfirmPanelOpenTime = 0f;

        StopMazeTimer();

        StopCoroutineIfRunning(
            ref restartCoroutine
        );

        StopCoroutineIfRunning(
            ref successCoroutine
        );

        stageCompleted = false;
        waitingForRestart = false;
        changingMaze = false;

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

        exitRequestedThisFrame = true;
        exitFinishedGame = true;

        if (!currentAttemptSaved)
        {
            float exitMazeTime =
                CurrentMazeElapsedTime;

            SaveCurrentMazeAttempt(
                "FAILED - EXIT",
                "EXIT_YES",
                exitMazeTime
            );

            SendTCPEvent(
                "MAZE_EXIT",
                "GAME_EVENT",
                "Result=FAILED - EXIT" +
                "; MazeDuration=" +
                exitMazeTime.ToString("F3") +
                "; CollectedCoins=" +
                collectedCoins
            );
        }

        float realTotalGameTime =
            Time.time - gameStartTime;

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

        if (finalSuccessRequestFrame == Time.frameCount)
        {
            Debug.Log(
                "============================================\n" +
                "FINAL SUCCESS HAS PRIORITY\n" +
                "Maze 10 Success and Final Game request\n" +
                "occurred in the same frame.\n" +
                "Final Game panel will NOT open.\n" +
                "============================================"
            );

            return;
        }

        gameFinished = true;

        if (musicManager != null)
            musicManager.StopBackgroundMusic();

        PlayFinalGameSound();

        StopMazeTimer();

        StopCoroutineIfRunning(
            ref restartCoroutine
        );

        StopCoroutineIfRunning(
            ref successCoroutine
        );

        StopCoroutineIfRunning(
            ref totalGameTimerCoroutine
        );

        ClosePanel(
            successPanel01,
            "PANEL_CLOSED_SUCCESS"
        );

        ClosePanel(
            timeOverPanel,
            "PANEL_CLOSED_TIME_OVER"
        );

        ClosePanel(
            exitConfirmPanel,
            "PANEL_CLOSED_EXIT_CONFIRM"
        );

        exitConfirmPanelCurrentlyOpen = false;
        exitConfirmPanelOpenTime = 0f;

        HideMainTimer();

        OpenPanel(
            finalGamePanel,
            "PANEL_OPENED_FINAL_GAME"
        );

        if (!currentAttemptSaved)
        {
            SaveCurrentMazeAttempt(
                finalResult,
                "FINAL_GAME_PANEL",
                CurrentMazeElapsedTime
            );
        }

        SendExperimentSummary(
            finalResult,
            totalGameTimeUsed
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

        PrintMazeVisitHistory();
        PrintExitConfirmHistory();
        PrintStartRoomTiming();

        SendTCPEvent(
            "GAME_FINISHED_FAILED",
            "GAME_EVENT",
            "FinalResult=" +
            finalResult +
            "; TotalGameTime=" +
            totalGameTimeUsed.ToString("F3") +
            "; TotalScore=" +
            totalScore +
            "; TotalCoinsCollected=" +
            totalScore +
            "; HighestCompletedMaze=" +
            highestCompletedMaze
        );

        Time.timeScale = 0f;
    }


    // =====================================================
    // SEND FINAL EXPERIMENT SUMMARY
    // =====================================================

    private void SendExperimentSummary(
        string finalResult,
        float totalGameTimeUsed
    )
    {
        if (experimentSummarySent)
            return;

        if (tcp == null)
            tcp = FindFirstObjectByType<TCP>();

        if (tcp == null)
        {
            Debug.LogError(
                "GameManager: TCP not found. " +
                "Experiment summary was NOT sent."
            );

            return;
        }

        experimentSummarySent = true;

        tcp.SendExperimentSummary(
            finalResult,
            totalGameTimeUsed,
            totalScore,
            highestCompletedMaze,
            startRoomDuration,
            mazeVisitHistory
        );

        Debug.Log(
            "============================================\n" +
            "EXPERIMENT SUMMARY REQUESTED\n" +
            "Final Result: " + finalResult +
            "\nTotal Game Time: " +
            totalGameTimeUsed.ToString("F3") +
            "\nFinal Score: " +
            totalScore +
            "\nHighest Completed Maze: " +
            highestCompletedMaze +
            "\nMaze Visits: " +
            mazeVisitHistory.Count +
            "\n============================================"
        );
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

        foreach (MazeVisitRecord record in mazeVisitHistory)
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

        foreach (ExitConfirmRecord record in exitConfirmHistory)
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
        if (finalGameAudioSource == null ||
            finalGameSound == null)
        {
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

        FieldInfo completedField =
            typeof(ExitTrigger).GetField(
                "completed",
                BindingFlags.NonPublic |
                BindingFlags.Instance
            );

        if (completedField != null)
        {
            foreach (ExitTrigger exit in exits)
            {
                if (exit != null)
                {
                    completedField.SetValue(
                        exit,
                        false
                    );
                }
            }
        }

        Debug.Log("All ExitTriggers reset.");
    }


    // =====================================================
    // PLAY FINAL SUCCESS SOUND
    // =====================================================

    private void PlayFinalSuccessSound()
    {
        if (finalSuccessAudioSource == null ||
            finalSuccessSound == null)
        {
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
        scoreMode =
            useCoins
                ? ScoreMode.CoinsAndMazeScore
                : ScoreMode.MazeScoreOnly;

        Debug.Log(
            "Game Mode changed. Use Coins = " +
            useCoins
        );
    }


    // =====================================================
    // MANUAL BUTTON EVENT
    // =====================================================

    public void LogButtonPress(string buttonName)
    {
        if (string.IsNullOrEmpty(buttonName))
            return;

        SendTCPEvent(
            "BUTTON_PRESSED_" + buttonName,
            "BUTTON_PRESSED"
        );
    }


    // =====================================================
    // MANUAL PANEL OPEN EVENT
    // =====================================================

    public void LogPanelOpened(string panelName)
    {
        if (string.IsNullOrEmpty(panelName))
            return;

        SendTCPEvent(
            "PANEL_OPENED_" + panelName,
            "PANEL_OPENED"
        );
    }


    // =====================================================
    // MANUAL PANEL CLOSE EVENT
    // =====================================================

    public void LogPanelClosed(string panelName)
    {
        if (string.IsNullOrEmpty(panelName))
            return;

        SendTCPEvent(
            "PANEL_CLOSED_" + panelName,
            "PANEL_CLOSED"
        );
    }


    // =====================================================
    // SET PANEL ACTIVE
    // =====================================================

    private void SetPanelActive(
        GameObject panel,
        bool active
    )
    {
        if (panel != null &&
            panel.activeSelf != active)
        {
            panel.SetActive(active);
        }
    }


    // =====================================================
    // STOP COROUTINE IF RUNNING
    // =====================================================

    private void StopCoroutineIfRunning(
        ref Coroutine coroutine
    )
    {
        if (coroutine == null)
            return;

        StopCoroutine(coroutine);
        coroutine = null;
    }
}


// =========================================================
// MAZE VISIT RECORD
// =========================================================

[System.Serializable]
public class MazeVisitRecord
{
    public int visitNumber;
    public int mazeNumber;
    public int attemptNumber;

    public float durationSeconds;

    public int collectedCoins;
    public int totalCoins;

    public string result;
    public string endReason;

    public float totalGameElapsedTime;
    public float startRoomDuration;
}


// =========================================================
// EXIT CONFIRM RECORD
// =========================================================

[System.Serializable]
public class ExitConfirmRecord
{
    public int interactionNumber;

    public int mazeNumber;
    public int attemptNumber;

    public string result;

    public float durationSeconds;
    public float totalGameElapsedTime;
}