/*using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class StartRoomController : MonoBehaviour
{
    [Header("Start Room")]
    public GameObject startRoom;
    public Transform startSpawnPoint;

    [Header("Player")]
    public Transform player;

    [Header("Start Question Panel")]
    public GameObject startPanel;

    public Button yesButton;
    public Button noButton;

    [Header("Left Controller Input")]
    public InputActionReference leftTriggerAction;
    public InputActionReference leftThumbstickAction;

    [Header("Transfer Panel")]
    public GameObject transferPanel;
    public TextMeshProUGUI transferText;
    public float transferDuration = 10f;

    [Header("Exit Controller")]
    public GameObject exitMenuController;

    [Header("Game Manager")]
    public GameManager gameManager;

    [Header("Selection Colors")]
    public Color selectedColor = Color.green;
    public Color normalColor = Color.white;

    private bool menuOpen = false;
    private bool yesSelected = false;
    private bool stickReady = true;

    // این متغیر مشخص می‌کند Start Controller هنوز فعال است یا نه
    private bool startSystemActive = true;

    private CharacterController characterController;


    // =====================================================
    // ENABLE
    // =====================================================

    private void OnEnable()
    {
        // فقط برای زمانی که Start System فعال است
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
        // عمداً Input Actionها را Disable نمی‌کنیم.
        //
        // چون ExitMenuController از همان Trigger و Thumbstick
        // استفاده می‌کند.
        //
        // با disabled شدن این Component، Update دیگر اجرا نمی‌شود.
    }


    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        // پیدا کردن Character Controller آواتار
        if (player != null)
        {
            characterController =
                player.GetComponent<CharacterController>();
        }

        // انتقال آواتار به Start Room
        MovePlayerToStartRoom();

        // Start Panel در ابتدا بسته باشد
        if (startPanel != null)
            startPanel.SetActive(false);

        // Transfer Panel در ابتدا بسته باشد
        if (transferPanel != null)
            transferPanel.SetActive(false);

        yesSelected = false;

        UpdateSelection();

        // پیدا کردن GameManager
        if (gameManager == null)
            gameManager = FindFirstObjectByType<GameManager>();

        // Exit در Start Room فعال نباشد
        if (exitMenuController != null)
            exitMenuController.SetActive(false);

        Debug.Log("Start Room initialized.");
    }


    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        // اگر Start System غیرفعال شده باشد،
        // هیچ چیزی از Start اجرا نشود.
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


        // برگشت Thumbstick به مرکز
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

        Debug.Log("Start menu opened");
    }


    // =====================================================
    // UPDATE SELECTION
    // =====================================================

    private void UpdateSelection()
    {
        if (yesButton == null || noButton == null)
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
            "YES selected - preparing to start game"
        );

        menuOpen = false;


        // بستن Start Panel
        if (startPanel != null)
            startPanel.SetActive(false);


        // نمایش Transfer Panel
        if (transferPanel != null)
            transferPanel.SetActive(true);


        // شروع شمارش معکوس
        StartCoroutine(TransferToMaze());
    }


    // =====================================================
    // TRANSFER TO MAZE
    // =====================================================

    private IEnumerator TransferToMaze()
    {
        float remainingTime = transferDuration;


        // =================================================
        // COUNTDOWN
        // =================================================

        while (remainingTime > 0f)
        {
            int seconds =
                Mathf.CeilToInt(remainingTime);


            if (transferText != null)
            {
                transferText.text =
                    "Transferring...\n" +
                    seconds.ToString();
            }


            yield return null;


            remainingTime -= Time.deltaTime;
        }


        // =================================================
        // SHOW ZERO
        // =================================================

        if (transferText != null)
        {
            transferText.text =
                "Transferring...\n0";
        }


        yield return new WaitForSeconds(0.2f);


        // =================================================
        // CLOSE TRANSFER PANEL
        // =================================================

        if (transferPanel != null)
            transferPanel.SetActive(false);


        // =================================================
        // LOCK ALL START SYSTEM
        // =================================================

        // از این لحظه Start دیگر هیچ کاری انجام نمی‌دهد.
        startSystemActive = false;

        menuOpen = false;
        yesSelected = false;


        // Start Panel حتماً بسته باشد
        if (startPanel != null)
            startPanel.SetActive(false);


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

        // Update دیگر اجرا نمی‌شود.
        enabled = false;


        // =================================================
        // ENABLE EXIT CONTROLLER
        // =================================================

        if (exitMenuController != null)
        {
            exitMenuController.SetActive(true);
        }


        Debug.Log(
            "Maze 1 started. " +
            "All Start settings disabled. " +
            "Exit controller enabled."
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
            "NO selected - staying in Start Room"
        );
    }
}*/

using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class StartRoomController : MonoBehaviour
{
    [Header("Start Room")]
    public GameObject startRoom;
    public Transform startSpawnPoint;

    [Header("Player")]
    public Transform player;

    [Header("Start Question Panel")]
    public GameObject startPanel;
    public Button yesButton;
    public Button noButton;

    [Header("Left Controller Input")]
    public InputActionReference leftTriggerAction;
    public InputActionReference leftThumbstickAction;

    [Header("Transfer Panel")]
    public GameObject transferPanel;
    public TextMeshProUGUI transferText;
    public float transferDuration = 10f;

    [Header("Exit Controller")]
    public GameObject exitMenuController;

    [Header("Maze UI")]
    public GameObject exitButton;
    public GameObject coinCounter;

    [Header("Game Manager")]
    public GameManager gameManager;

    [Header("Analytics")]
    public AnalyticsLogger analyticsLogger;

    [Header("Selection Colors")]
    public Color selectedColor = Color.green;
    public Color normalColor = Color.white;

    private bool menuOpen = false;
    private bool yesSelected = false;
    private bool stickReady = true;

    // مشخص می‌کند سیستم Start هنوز فعال است یا نه
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
        // دلیل:
        // ExitMenuController از همان Trigger و Thumbstick
        // استفاده می‌کند.
    }


    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        // پیدا کردن Character Controller آواتار
        if (player != null)
        {
            characterController =
                player.GetComponent<CharacterController>();
        }

        // انتقال آواتار به Start Room
        MovePlayerToStartRoom();


        // Start Panel در شروع بسته باشد
        if (startPanel != null)
            startPanel.SetActive(false);


        // Transfer Panel در شروع بسته باشد
        if (transferPanel != null)
            transferPanel.SetActive(false);


        yesSelected = false;

        UpdateSelection();


        // پیدا کردن GameManager
        if (gameManager == null)
            gameManager = FindFirstObjectByType<GameManager>();


        // پیدا کردن AnalyticsLogger
        if (analyticsLogger == null)
            analyticsLogger =
                FindFirstObjectByType<AnalyticsLogger>();


        // شروع ثبت زمان حضور در Start Room
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


        // Exit Controller در Start Room خاموش باشد
        if (exitMenuController != null)
            exitMenuController.SetActive(false);

        // مخفی کردن UI های مخصوص Maze در Start Room
        if (exitButton != null)
            exitButton.SetActive(false);

        if (coinCounter != null)
            coinCounter.SetActive(false);


        Debug.Log("Start Room initialized.");
    }


    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        // اگر سیستم Start غیرفعال شده،
        // دیگر هیچ ورودی‌ای دریافت نکن.
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


        // برگشت Thumbstick به مرکز
        if (Mathf.Abs(stick.x) < 0.3f)
        {
            stickReady = true;
        }


        // چپ = YES
        if (stickReady && stick.x < -0.5f)
        {
            yesSelected = true;

            stickReady = false;

            UpdateSelection();

            Debug.Log("YES selected");
        }


        // راست = NO
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


        Debug.Log("Start menu opened");
    }


    // =====================================================
    // UPDATE SELECTION
    // =====================================================

    private void UpdateSelection()
    {
        if (yesButton == null || noButton == null)
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
            "YES selected - preparing to start game"
        );


        // =================================================
        // SAVE START ROOM TIME
        // =================================================

        if (analyticsLogger != null)
        {
            analyticsLogger.SaveStartRoomTime();
        }


        menuOpen = false;


        // بستن Start Panel
        if (startPanel != null)
            startPanel.SetActive(false);


        // نمایش Transfer Panel
        if (transferPanel != null)
            transferPanel.SetActive(true);


        // شروع شمارش معکوس
        StartCoroutine(TransferToMaze());
    }


    // =====================================================
    // TRANSFER TO MAZE
    // =====================================================

    private IEnumerator TransferToMaze()
    {
        float remainingTime = transferDuration;


        // =================================================
        // COUNTDOWN
        // =================================================

        while (remainingTime > 0f)
        {
            int seconds =
                Mathf.CeilToInt(remainingTime);


            if (transferText != null)
            {
                transferText.text =
                    "Transferring...\n" +
                    seconds.ToString();
            }


            yield return null;


            remainingTime -= Time.deltaTime;
        }


        // =================================================
        // SHOW ZERO
        // =================================================

        if (transferText != null)
        {
            transferText.text =
                "Transferring...\n0";
        }


        yield return new WaitForSeconds(0.2f);


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


        // Start Panel حتماً بسته باشد
        if (startPanel != null)
            startPanel.SetActive(false);


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

        // =====================================================
        // DISABLE START ROOM
        // =====================================================

        if (startRoom != null)
            startRoom.SetActive(false);


        // =====================================================
        // DISABLE START CONTROLLER
        // =====================================================

        enabled = false;


        // =====================================================
        // ENABLE EXIT CONTROLLER
        // =====================================================

        if (exitMenuController != null)
        {
            exitMenuController.SetActive(true);
        }


        // =====================================================
        // SHOW MAZE UI
        // =====================================================

        if (exitButton != null)
            exitButton.SetActive(true);

        if (coinCounter != null)
            coinCounter.SetActive(true);


        Debug.Log(
            "Maze 1 started. " +
            "Start system disabled. " +
            "Exit controller and Maze UI enabled."
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
            "NO selected - staying in Start Room"
        );
    }
}