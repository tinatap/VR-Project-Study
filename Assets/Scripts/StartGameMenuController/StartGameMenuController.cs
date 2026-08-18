using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class StartGameMenuController : MonoBehaviour
{
    // =====================================================
    // START ROOM
    // =====================================================

    [Header("Start Room")]

    public GameObject startRoom;
    public Transform startSpawnPoint;


    // =====================================================
    // PLAYER
    // =====================================================

    [Header("Player")]

    public Transform player;


    // =====================================================
    // START QUESTION PANEL
    // =====================================================

    [Header("Start Question Panel")]

    public GameObject startPanel;

    public Button yesButton;
    public Button noButton;


    // =====================================================
    // LEFT CONTROLLER INPUT
    // =====================================================

    [Header("Left Controller Input")]

    public InputActionReference leftTriggerAction;

    public InputActionReference leftThumbstickAction;


    // =====================================================
    // TRANSFER PANEL
    // =====================================================

    [Header("Transfer Panel")]

    public GameObject transferPanel;

    public TextMeshProUGUI transferText;

    public float transferDuration = 10f;


    // =====================================================
    // EXIT CONTROLLER
    // =====================================================

    [Header("Exit Controller")]

    public GameObject exitMenuController;


    // =====================================================
    // GAME MANAGER
    // =====================================================

    [Header("Game Manager")]

    public GameManager gameManager;


    // =====================================================
    // ANALYTICS
    // =====================================================

    [Header("Analytics")]

    public AnalyticsLogger analyticsLogger;


    // =====================================================
    // SELECTION COLORS
    // =====================================================

    [Header("Selection Colors")]

    public Color selectedColor = Color.green;

    public Color normalColor = Color.white;


    // =====================================================
    // PRIVATE VARIABLES
    // =====================================================

    private bool menuOpen = false;

    private bool yesSelected = false;

    private bool stickReady = true;

    private bool startSystemActive = true;

    private CharacterController characterController;


    // =====================================================
    // ENABLE
    // =====================================================

    private void OnEnable()
    {
        if (!startSystemActive)
            return;


        if (leftTriggerAction != null)
            leftTriggerAction.action.Enable();


        if (leftThumbstickAction != null)
            leftThumbstickAction.action.Enable();
    }


    // =====================================================
    // DISABLE
    // =====================================================

    private void OnDisable()
    {
        // Input Actionها را اینجا Disable نمی‌کنیم.
        //
        // چون ExitMenuController از همان Trigger
        // و Thumbstick استفاده می‌کند.
    }


    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        // =================================================
        // CHARACTER CONTROLLER
        // =================================================

        if (player != null)
        {
            characterController =
                player.GetComponent<CharacterController>();
        }


        // =================================================
        // MOVE PLAYER TO START ROOM
        // =================================================

        MovePlayerToStartRoom();


        // =================================================
        // CLOSE START PANEL
        // =================================================

        if (startPanel != null)
            startPanel.SetActive(false);


        // =================================================
        // CLOSE TRANSFER PANEL
        // =================================================

        if (transferPanel != null)
            transferPanel.SetActive(false);


        yesSelected = false;

        UpdateSelection();


        // =================================================
        // FIND GAME MANAGER
        // =================================================

        if (gameManager == null)
        {
            gameManager =
                FindFirstObjectByType<GameManager>();
        }


        // =================================================
        // FIND ANALYTICS LOGGER
        // =================================================

        if (analyticsLogger == null)
        {
            analyticsLogger =
                FindFirstObjectByType<AnalyticsLogger>();
        }


        // =================================================
        // START START-ROOM TIMER
        // =================================================

        if (analyticsLogger != null)
        {
            analyticsLogger.StartStartRoomTimer();
        }
        else
        {
            Debug.LogWarning(
                "StartRoomController: AnalyticsLogger is not assigned!"
            );
        }


        // =================================================
        // EXIT CONTROLLER OFF
        // =================================================

        if (exitMenuController != null)
            exitMenuController.SetActive(false);


        // =================================================
        // HIDE ALL MAZE UI
        // =================================================

        if (gameManager != null)
        {
            gameManager.SetMazeUI(false);
        }


        Debug.Log(
            "Start Room initialized. " +
            "Maze UI hidden."
        );
    }


    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        if (!startSystemActive)
            return;


        if (leftTriggerAction == null ||
            leftThumbstickAction == null)
            return;


        // =================================================
        // LEFT TRIGGER
        // =================================================

        if (leftTriggerAction.action.WasPressedThisFrame())
        {
            if (!menuOpen)
            {
                OpenStartMenu();
            }
            else
            {
                ConfirmSelection();
            }
        }


        if (!menuOpen)
            return;


        // =================================================
        // LEFT THUMBSTICK
        // =================================================

        Vector2 stick =
            leftThumbstickAction.action.ReadValue<Vector2>();


        // =================================================
        // THUMBSTICK RETURN TO CENTER
        // =================================================

        if (Mathf.Abs(stick.x) < 0.3f)
        {
            stickReady = true;
        }


        // =================================================
        // LEFT = YES
        // =================================================

        if (stickReady && stick.x < -0.5f)
        {
            yesSelected = true;

            stickReady = false;

            UpdateSelection();

            Debug.Log("YES selected");
        }


        // =================================================
        // RIGHT = NO
        // =================================================

        else if (stickReady && stick.x > 0.5f)
        {
            yesSelected = false;

            stickReady = false;

            UpdateSelection();

            Debug.Log("NO selected");
        }
    }


    // =====================================================
    // MOVE PLAYER TO START ROOM
    // =====================================================

    private void MovePlayerToStartRoom()
    {
        if (player == null)
        {
            Debug.LogWarning(
                "StartRoomController: Player is not assigned!"
            );

            return;
        }


        if (startSpawnPoint == null)
        {
            Debug.LogWarning(
                "StartRoomController: Start Spawn Point is not assigned!"
            );

            return;
        }


        if (characterController != null)
            characterController.enabled = false;


        player.SetPositionAndRotation(
            startSpawnPoint.position,
            startSpawnPoint.rotation
        );


        if (characterController != null)
            characterController.enabled = true;


        Debug.Log(
            "Player moved to Start Room Spawn Point."
        );
    }


    // =====================================================
    // OPEN START MENU
    // =====================================================

    private void OpenStartMenu()
    {
        if (!startSystemActive)
            return;


        menuOpen = true;


        if (startPanel != null)
            startPanel.SetActive(true);


        yesSelected = false;

        stickReady = true;

        UpdateSelection();


        Debug.Log(
            "Start menu opened."
        );
    }


    // =====================================================
    // UPDATE SELECTION
    // =====================================================

    private void UpdateSelection()
    {
        if (yesButton == null ||
            noButton == null)
            return;


        Image yesImage =
            yesButton.GetComponent<Image>();


        Image noImage =
            noButton.GetComponent<Image>();


        if (yesImage != null)
        {
            yesImage.color =
                yesSelected
                ? selectedColor
                : normalColor;
        }


        if (noImage != null)
        {
            noImage.color =
                yesSelected
                ? normalColor
                : selectedColor;
        }
    }


    // =====================================================
    // CONFIRM SELECTION
    // =====================================================

    private void ConfirmSelection()
    {
        if (!startSystemActive)
            return;


        if (yesSelected)
        {
            StartGame();
        }
        else
        {
            CloseStartMenu();
        }
    }


    // =====================================================
    // START GAME
    // =====================================================

    private void StartGame()
    {
        if (!startSystemActive)
            return;


        Debug.Log(
            "YES selected - preparing to start game."
        );


        // =================================================
        // SAVE START ROOM TIME
        // =================================================

        if (analyticsLogger != null)
        {
            analyticsLogger.SaveStartRoomTime();
        }


        menuOpen = false;


        // =================================================
        // CLOSE START PANEL
        // =================================================

        if (startPanel != null)
            startPanel.SetActive(false);


        // =================================================
        // SHOW TRANSFER PANEL
        // =================================================

        if (transferPanel != null)
            transferPanel.SetActive(true);


        // =================================================
        // START TRANSFER
        // =================================================

        StartCoroutine(
            TransferToMaze()
        );
    }


    // =====================================================
    // TRANSFER TO MAZE
    // =====================================================

    private IEnumerator TransferToMaze()
    {
        float remainingTime =
            transferDuration;


        // =================================================
        // COUNTDOWN
        // =================================================

        while (remainingTime > 0f)
        {
            int seconds =
                Mathf.CeilToInt(
                    remainingTime
                );


            if (transferText != null)
            {
                transferText.text =
                    "Transferring...\n" +
                    seconds.ToString();
            }


            yield return null;


            remainingTime -=
                Time.deltaTime;
        }


        // =================================================
        // SHOW ZERO
        // =================================================

        if (transferText != null)
        {
            transferText.text =
                "Transferring...\n0";
        }


        yield return new WaitForSeconds(
            0.2f
        );


        // =================================================
        // CLOSE TRANSFER PANEL
        // =================================================

        if (transferPanel != null)
            transferPanel.SetActive(false);


        // =================================================
        // LOCK START SYSTEM
        // =================================================

        startSystemActive = false;

        menuOpen = false;

        yesSelected = false;


        // =================================================
        // CLOSE START PANEL
        // =================================================

        if (startPanel != null)
            startPanel.SetActive(false);


        // =================================================
        // MAKE SURE MAZE UI IS STILL HIDDEN
        // =================================================

        if (gameManager != null)
        {
            gameManager.SetMazeUI(false);
        }


        // =================================================
        // START MAZE 1
        // =================================================

        if (gameManager != null)
        {
            gameManager.StartGameFromMaze1();
        }
        else
        {
            Debug.LogWarning(
                "StartRoomController: GameManager is not assigned!"
            );
        }


        // =================================================
        // DISABLE START ROOM
        // =================================================

        if (startRoom != null)
            startRoom.SetActive(false);


        // =================================================
        // DISABLE START CONTROLLER
        // =================================================

        enabled = false;


        // =================================================
        // ENABLE EXIT CONTROLLER
        // =================================================

        if (exitMenuController != null)
        {
            exitMenuController.SetActive(true);
        }


        // =================================================
        // IMPORTANT
        // =================================================
        //
        // Maze UI را اینجا فعال نمی‌کنیم.
        //
        // GameManager.StartGameFromMaze1()
        // خودش SetMazeUI(true) را اجرا می‌کند.
        //


        Debug.Log(
            "Maze 1 started. " +
            "Start system disabled. " +
            "Exit controller enabled. " +
            "Maze UI is controlled by GameManager."
        );
    }


    // =====================================================
    // CLOSE START MENU
    // =====================================================

    private void CloseStartMenu()
    {
        if (!startSystemActive)
            return;


        menuOpen = false;


        if (startPanel != null)
            startPanel.SetActive(false);


        Debug.Log(
            "NO selected - staying in Start Room."
        );
    }
}