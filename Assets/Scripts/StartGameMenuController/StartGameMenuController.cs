using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class StartGameMenuController : MonoBehaviour
{
    // ============================================================
    // START ROOM
    // ============================================================

    [Header("Start Room")]
    public GameObject startRoom;
    public Transform startSpawnPoint;


    // ============================================================
    // PLAYER
    // ============================================================

    [Header("Player")]
    public Transform player;


    // ============================================================
    // START QUESTION PANEL
    // ============================================================

    [Header("Start Question Panel")]
    public GameObject startPanel;
    public Button yesButton;
    public Button noButton;


    // ============================================================
    // LEFT CONTROLLER INPUT
    // ============================================================

    [Header("Left Controller Input")]
    public InputActionReference leftTriggerAction;
    public InputActionReference leftThumbstickAction;


    // ============================================================
    // TRANSFER PANEL
    // ============================================================

    [Header("Transfer Panel")]
    public GameObject transferPanel;

    public TextMeshProUGUI transferText;

    [Tooltip("Transfer duration in seconds.")]
    public float transferDuration = 10f;


    // ============================================================
    // EXIT CONTROLLER
    // ============================================================

    [Header("Exit Controller")]
    public GameObject exitMenuController;


    // ============================================================
    // GAME MANAGER
    // ============================================================

    [Header("Game Manager")]
    public GameManager gameManager;


    // ============================================================
    // TCP
    // ============================================================

    [Header("TCP")]
    public TCP tcp;


    // ============================================================
    // SELECTION COLORS
    // ============================================================

    [Header("Selection Colors")]
    public Color selectedColor = Color.green;
    public Color normalColor = Color.white;


    // ============================================================
    // PRIVATE VARIABLES
    // ============================================================

    private bool menuOpen;
    private bool yesSelected;
    private bool stickReady = true;

    private bool startSystemActive = true;
    private bool transferRunning;

    private CharacterController characterController;

    private Image yesImage;
    private Image noImage;


    // ============================================================
    // CONSTANTS
    // ============================================================

    private const float StickDeadZone = 0.3f;
    private const float StickSelectionThreshold = 0.5f;


    // ============================================================
    // ENABLE / DISABLE
    // ============================================================

    private void OnEnable()
    {
        if (!startSystemActive)
            return;

        EnableAction(leftTriggerAction);
        EnableAction(leftThumbstickAction);
    }


    private void OnDisable()
    {
        DisableAction(leftTriggerAction);
        DisableAction(leftThumbstickAction);
    }


    // ============================================================
    // START
    // ============================================================

    private void Start()
    {
        InitializeReferences();
        InitializeUIReferences();
        InitializeStartRoom();
        InitializeUI();
    }


    // ============================================================
    // INITIALIZATION
    // ============================================================

    private void InitializeReferences()
    {
        if (player != null)
            characterController = player.GetComponent<CharacterController>();

        if (gameManager == null)
            gameManager = FindFirstObjectByType<GameManager>();

        if (tcp == null)
            tcp = FindFirstObjectByType<TCP>();
    }


    private void InitializeUIReferences()
    {
        if (yesButton != null)
            yesImage = yesButton.GetComponent<Image>();

        if (noButton != null)
            noImage = noButton.GetComponent<Image>();
    }


    private void InitializeStartRoom()
    {
        MovePlayerToStartRoom();

        SetPanelActive(startPanel, false);
        SetPanelActive(transferPanel, false);

        if (exitMenuController != null)
            exitMenuController.SetActive(false);

        if (gameManager != null)
            gameManager.SetMazeUI(false);
    }


    private void InitializeUI()
    {
        menuOpen = false;
        yesSelected = false;
        stickReady = true;
        transferRunning = false;

        UpdateSelection();

        Debug.Log("Start Room initialized. Maze UI hidden.");
    }


    // ============================================================
    // UPDATE
    // ============================================================

    private void Update()
    {
        if (!startSystemActive || transferRunning)
            return;

        if (leftTriggerAction == null || leftThumbstickAction == null)
            return;

        HandleTriggerInput();

        if (!menuOpen)
            return;

        HandleThumbstickInput();
    }


    // ============================================================
    // TRIGGER INPUT
    // ============================================================

    private void HandleTriggerInput()
    {
        if (!leftTriggerAction.action.WasPressedThisFrame())
            return;

        if (!menuOpen)
        {
            OpenStartMenu();
        }
        else
        {
            ConfirmSelection();
        }
    }


    // ============================================================
    // THUMBSTICK INPUT
    // ============================================================

    private void HandleThumbstickInput()
    {
        Vector2 stick = leftThumbstickAction.action.ReadValue<Vector2>();

        float horizontalInput = stick.x;


        // Reset when stick returns to center
        if (Mathf.Abs(horizontalInput) < StickDeadZone)
            stickReady = true;


        if (!stickReady)
            return;


        // LEFT = YES
        if (horizontalInput < -StickSelectionThreshold)
        {
            yesSelected = true;
            stickReady = false;

            UpdateSelection();

            Debug.Log("YES selected");
        }


        // RIGHT = NO
        else if (horizontalInput > StickSelectionThreshold)
        {
            yesSelected = false;
            stickReady = false;

            UpdateSelection();

            Debug.Log("NO selected");
        }
    }


    // ============================================================
    // MOVE PLAYER TO START ROOM
    // ============================================================

    private void MovePlayerToStartRoom()
    {
        if (player == null)
        {
            Debug.LogWarning(
                "StartGameMenuController: Player is not assigned."
            );

            return;
        }

        if (startSpawnPoint == null)
        {
            Debug.LogWarning(
                "StartGameMenuController: Start Spawn Point is not assigned."
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


        Debug.Log("Player moved to Start Room Spawn Point.");
    }


    // ============================================================
    // OPEN START MENU
    // ============================================================

    private void OpenStartMenu()
    {
        if (!startSystemActive || transferRunning)
            return;


        menuOpen = true;
        yesSelected = false;
        stickReady = true;


        UpdateSelection();


        SetPanelActive(startPanel, true);


        if (gameManager != null)
        {
            gameManager.OpenStartQuestionPanel();
        }
        else
        {
            Debug.LogError(
                "StartGameMenuController: GameManager is NULL!"
            );
        }


        SendTCPEvent(
            "PANEL_OPENED_START_QUESTION",
            "PANEL_EVENT",
            "Start Question Panel opened."
        );


        Debug.Log(
            "========== START QUESTION PANEL OPENED =========="
        );
    }


    // ============================================================
    // UPDATE YES / NO VISUAL
    // ============================================================

    private void UpdateSelection()
    {
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


    // ============================================================
    // CONFIRM SELECTION
    // ============================================================

    private void ConfirmSelection()
    {
        if (!startSystemActive || transferRunning)
            return;


        if (yesSelected)
        {
            StartGame();
        }
        else
        {
            SelectNo();
        }
    }


    // ============================================================
    // YES / START GAME
    // ============================================================

    private void StartGame()
    {
        if (!startSystemActive || transferRunning)
            return;


        Debug.Log(
            "YES selected - preparing to start game."
        );


        if (gameManager != null)
            gameManager.RegisterStartRoomYes();


        menuOpen = false;


        SetPanelActive(
            startPanel,
            false
        );


        SendTCPEvent(
            "PANEL_CLOSED_START_QUESTION",
            "PANEL_EVENT",
            "Start Question Panel closed after YES."
        );


        transferRunning = true;


        StartCoroutine(
            TransferToMaze()
        );
    }


    // ============================================================
    // NO / STAY IN START ROOM
    // ============================================================

    private void SelectNo()
    {
        if (!startSystemActive || transferRunning)
            return;


        Debug.Log(
            "NO selected - staying in Start Room."
        );


        SendTCPEvent(
            "START_ROOM_NO",
            "BUTTON_EVENT",
            "NO button pressed."
        );


        CloseStartMenu();


        SendTCPEvent(
            "PANEL_CLOSED_START_QUESTION",
            "PANEL_EVENT",
            "Start Question Panel closed after NO."
        );
    }


    // ============================================================
    // TRANSFER TO MAZE
    // ============================================================

    private IEnumerator TransferToMaze()
    {
        SetPanelActive(
            transferPanel,
            true
        );


        SendTCPEvent(
            "PANEL_OPENED_TRANSFER",
            "PANEL_EVENT",
            "Transfer Panel opened."
        );


        float remainingTime = transferDuration;


        while (remainingTime > 0f)
        {
            int seconds =
                Mathf.CeilToInt(remainingTime);


            if (transferText != null)
            {
                transferText.text =
                    "Transferring...\n" +
                    seconds;
            }


            yield return null;


            remainingTime -=
                Time.deltaTime;
        }


        if (transferText != null)
        {
            transferText.text =
                "Transferring...\n0";
        }


        yield return new WaitForSeconds(0.2f);


        SetPanelActive(
            transferPanel,
            false
        );


        SendTCPEvent(
            "PANEL_CLOSED_TRANSFER",
            "PANEL_EVENT",
            "Transfer Panel closed."
        );


        // Disable Start Room system
        startSystemActive = false;
        menuOpen = false;
        yesSelected = false;


        SetPanelActive(
            startPanel,
            false
        );


        // Keep Maze UI controlled by GameManager
        if (gameManager != null)
            gameManager.SetMazeUI(false);


        // Start Maze 1
        if (gameManager != null)
        {
            gameManager.StartGameFromMaze1();
        }
        else
        {
            Debug.LogWarning(
                "StartGameMenuController: GameManager is not assigned!"
            );
        }


        // Disable Start Room
        SetPanelActive(
            startRoom,
            false
        );


        // Disable this Start Room system
        enabled = false;


        // Enable Exit Controller
        if (exitMenuController != null)
            exitMenuController.SetActive(true);


        transferRunning = false;


        Debug.Log(
            "Maze 1 started. " +
            "Start system disabled. " +
            "Exit controller enabled. " +
            "Maze UI is controlled by GameManager."
        );
    }


    // ============================================================
    // CLOSE START MENU
    // ============================================================

    private void CloseStartMenu()
    {
        if (!startSystemActive)
            return;


        menuOpen = false;


        SetPanelActive(
            startPanel,
            false
        );


        Debug.Log(
            "NO selected - staying in Start Room."
        );
    }


    // ============================================================
    // PANEL ACTIVE HELPER
    // ============================================================

    private void SetPanelActive(
        GameObject panel,
        bool active)
    {
        if (panel != null &&
            panel.activeSelf != active)
        {
            panel.SetActive(active);
        }
    }


    // ============================================================
    // INPUT ACTION HELPERS
    // ============================================================

    private void EnableAction(
        InputActionReference actionReference)
    {
        if (actionReference != null)
            actionReference.action.Enable();
    }


    private void DisableAction(
        InputActionReference actionReference)
    {
        if (actionReference != null)
            actionReference.action.Disable();
    }


    // ============================================================
    // TCP EVENT
    // ============================================================

    private void SendTCPEvent(
        string eventName,
        string eventType = "GAME_EVENT",
        string eventMessage = "")
    {
        if (tcp == null)
            tcp = FindFirstObjectByType<TCP>();


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
                "StartGameMenuController: " +
                "TCP reference is not assigned/found.\n" +
                "Event was not sent: " +
                eventName
            );
        }


        Debug.Log(
            "========== TCP EVENT ==========\n" +
            "Type: " + eventType + "\n" +
            "Event: " + eventName + "\n" +
            "Message: " + eventMessage
        );
    }
}