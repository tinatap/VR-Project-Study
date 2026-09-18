using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ExitMenuController : MonoBehaviour
{
    [Header("UI")]
    public GameObject exitConfirmPanel;
    public Button yesButton;
    public Button noButton;

    [Header("Left Controller Input")]
    public InputActionReference leftTriggerAction;
    public InputActionReference leftThumbstickAction;

    [Header("Game Manager")]
    public GameManager gameManager;

    [Header("TCP")]
    public TCP tcp;

    [Header("Selection Colors")]
    public Color selectedColor = Color.green;
    public Color normalColor = Color.white;

    private bool menuOpen = false;
    private bool yesSelected = false;
    private bool stickReady = true;

    // Cached UI components
    private Image yesImage;
    private Image noImage;


    // =====================================================
    // ENABLE / DISABLE
    // =====================================================

    private void OnEnable()
    {
        if (leftTriggerAction != null)
            leftTriggerAction.action.Enable();

        if (leftThumbstickAction != null)
            leftThumbstickAction.action.Enable();
    }

    private void OnDisable()
    {
        if (leftTriggerAction != null)
            leftTriggerAction.action.Disable();

        if (leftThumbstickAction != null)
            leftThumbstickAction.action.Disable();
    }


    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        // Cache UI references once
        if (yesButton != null)
            yesImage = yesButton.GetComponent<Image>();

        if (noButton != null)
            noImage = noButton.GetComponent<Image>();

        // Find references only if not assigned in Inspector
        if (gameManager == null)
            gameManager = FindFirstObjectByType<GameManager>();

        if (tcp == null)
            tcp = FindFirstObjectByType<TCP>();

        // Initial state
        SetExitPanel(false);

        yesSelected = false;
        stickReady = true;

        UpdateSelection();
    }


    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        if (leftTriggerAction == null ||
            leftThumbstickAction == null)
        {
            return;
        }

        HandleTriggerInput();

        if (!menuOpen)
            return;

        HandleThumbstickInput();
    }


    // =====================================================
    // TRIGGER INPUT
    // =====================================================

    private void HandleTriggerInput()
    {
        if (!leftTriggerAction.action.WasPressedThisFrame())
            return;

        if (!menuOpen)
        {
            OpenExitMenu();
        }
        else
        {
            ConfirmSelection();
        }
    }


    // =====================================================
    // THUMBSTICK INPUT
    // =====================================================

    private void HandleThumbstickInput()
    {
        Vector2 stick =
            leftThumbstickAction.action.ReadValue<Vector2>();

        // Debug
        if (stick.sqrMagnitude > 0.01f)
        {
            Debug.Log("Left Stick: " + stick);
        }

        // Thumbstick returned to center
        if (Mathf.Abs(stick.x) < 0.3f)
        {
            stickReady = true;
        }

        if (!stickReady)
            return;

        // LEFT = YES
        if (stick.x < -0.5f)
        {
            SelectYes();
        }
        // RIGHT = NO
        else if (stick.x > 0.5f)
        {
            SelectNo();
        }
    }


    // =====================================================
    // SELECT YES
    // =====================================================

    private void SelectYes()
    {
        yesSelected = true;
        stickReady = false;

        UpdateSelection();

        Debug.Log("YES selected");

        SendTCPEvent(
            "EXIT_YES_SELECTED",
            "BUTTON_EVENT",
            "YES option selected in Exit confirmation panel."
        );
    }


    // =====================================================
    // SELECT NO
    // =====================================================

    private void SelectNo()
    {
        yesSelected = false;
        stickReady = false;

        UpdateSelection();

        Debug.Log("NO selected");

        SendTCPEvent(
            "EXIT_NO_SELECTED",
            "BUTTON_EVENT",
            "NO option selected in Exit confirmation panel."
        );
    }


    // =====================================================
    // CHECK OTHER PANELS
    // =====================================================

    private bool IsAnotherPanelOpen()
    {
        if (gameManager == null)
            gameManager = FindFirstObjectByType<GameManager>();

        if (gameManager == null)
            return false;

        // Final screens block Exit
        if (gameManager.finalGamePanel != null &&
            gameManager.finalGamePanel.activeSelf)
        {
            return true;
        }

        if (gameManager.finalSuccessPanel != null &&
            gameManager.finalSuccessPanel.activeSelf)
        {
            return true;
        }

        // Start room question blocks Exit
        if (gameManager.startQuestionPanel != null &&
            gameManager.startQuestionPanel.activeSelf)
        {
            return true;
        }

        // successPanel01 and timeOverPanel
        // intentionally do NOT block Exit

        return false;
    }


    // =====================================================
    // OPEN EXIT MENU
    // =====================================================

    private void OpenExitMenu()
    {
        if (IsAnotherPanelOpen())
        {
            Debug.Log(
                "Another panel is already open. " +
                "Exit menu will not open."
            );

            return;
        }

        menuOpen = true;
        yesSelected = false;
        stickReady = true;

        SetExitPanel(true);
        UpdateSelection();

        Debug.Log("Exit menu opened");

        SendTCPEvent(
            "PANEL_OPENED_EXIT_CONFIRMATION",
            "PANEL_EVENT",
            "Exit confirmation panel opened."
        );
    }


    // =====================================================
    // UPDATE SELECTION UI
    // =====================================================

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


    // =====================================================
    // CONFIRM SELECTION
    // =====================================================

    private void ConfirmSelection()
    {
        if (yesSelected)
        {
            ConfirmExit();
        }
        else
        {
            CloseExitMenu();
        }
    }


    // =====================================================
    // CONFIRM EXIT - YES
    // =====================================================

    private void ConfirmExit()
    {
        Debug.Log("YES selected - exiting");

        SendTCPEvent(
            "EXIT_YES_CONFIRMED",
            "BUTTON_EVENT",
            "YES confirmed. Game exit requested."
        );

        // Close panel first
        menuOpen = false;
        SetExitPanel(false);

        SendTCPEvent(
            "PANEL_CLOSED_EXIT_CONFIRMATION",
            "PANEL_EVENT",
            "Exit confirmation panel closed after YES."
        );

        // Then exit game
        if (gameManager != null)
        {
            gameManager.ExitGame();
        }
        else
        {
            Debug.LogWarning(
                "ExitMenuController: GameManager is not assigned!"
            );
        }
    }


    // =====================================================
    // CLOSE EXIT MENU - NO
    // =====================================================

    private void CloseExitMenu()
    {
        menuOpen = false;

        SetExitPanel(false);

        Debug.Log("NO selected - exit cancelled");

        SendTCPEvent(
            "EXIT_NO_CONFIRMED",
            "BUTTON_EVENT",
            "NO confirmed. Exit cancelled."
        );

        SendTCPEvent(
            "PANEL_CLOSED_EXIT_CONFIRMATION",
            "PANEL_EVENT",
            "Exit confirmation panel closed after NO."
        );
    }


    // =====================================================
    // PANEL STATE
    // =====================================================

    private void SetExitPanel(bool active)
    {
        if (exitConfirmPanel != null &&
            exitConfirmPanel.activeSelf != active)
        {
            exitConfirmPanel.SetActive(active);
        }
    }


    // =====================================================
    // SEND TCP EVENT
    // =====================================================

    private void SendTCPEvent(
        string eventName,
        string eventType = "GAME_EVENT",
        string eventMessage = ""
    )
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
                "ExitMenuController: TCP reference " +
                "is not assigned/found.\n" +
                "Event was not sent: " +
                eventName
            );
        }

        Debug.Log(
            "========== EXIT TCP EVENT ==========\n" +
            "Type: " + eventType + "\n" +
            "Event: " + eventName + "\n" +
            "Message: " + eventMessage
        );
    }
}