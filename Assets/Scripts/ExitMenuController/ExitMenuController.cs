
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

    private void Start()
    {
        if (exitConfirmPanel != null)
            exitConfirmPanel.SetActive(false);

        yesSelected = false;

        UpdateSelection();

        if (gameManager == null)
            gameManager = FindFirstObjectByType<GameManager>();

        if (tcp == null)
            tcp = FindFirstObjectByType<TCP>();
    }

    private void Update()
    {
        if (leftTriggerAction == null ||
            leftThumbstickAction == null)
            return;

        // ==========================
        // LEFT TRIGGER
        // ==========================

        if (leftTriggerAction.action.WasPressedThisFrame())
        {
            if (!menuOpen)
            {
                OpenExitMenu();
            }
            else
            {
                ConfirmSelection();
            }
        }

        if (!menuOpen)
            return;


        // ==========================
        // LEFT THUMBSTICK
        // ==========================

        Vector2 stick =
            leftThumbstickAction.action.ReadValue<Vector2>();

        // برای تست
        if (stick.sqrMagnitude > 0.01f)
        {
            Debug.Log("Left Stick: " + stick);
        }

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

            // ==========================
            // TCP EVENT - YES SELECTED
            // ==========================

            SendTCPEvent(
                "EXIT_YES_SELECTED",
                "BUTTON_EVENT",
                "YES option selected in Exit confirmation panel."
            );
        }

        // راست = NO
        else if (stickReady && stick.x > 0.5f)
        {
            yesSelected = false;

            stickReady = false;

            UpdateSelection();

            Debug.Log("NO selected");

            // ==========================
            // TCP EVENT - NO SELECTED
            // ==========================

            SendTCPEvent(
                "EXIT_NO_SELECTED",
                "BUTTON_EVENT",
                "NO option selected in Exit confirmation panel."
            );
        }
    }
    private bool IsAnotherPanelOpen()
    {
        if (gameManager == null)
            gameManager = FindFirstObjectByType<GameManager>();

        if (gameManager == null)
            return false;


        // Final screens should block exit
        if (gameManager.finalGamePanel != null &&
            gameManager.finalGamePanel.activeSelf)
            return true;


        if (gameManager.finalSuccessPanel != null &&
            gameManager.finalSuccessPanel.activeSelf)
            return true;


        // Start room question should block exit
        if (gameManager.startQuestionPanel != null &&
            gameManager.startQuestionPanel.activeSelf)
            return true;


        // IMPORTANT:
        // successPanel01 and timeOverPanel are intentionally ignored.
        // Exit can open over them.


        return false;
    }

    private void OpenExitMenu()
    {
        // اگر پنل دیگری باز است، Exit Menu باز نشود
        if (IsAnotherPanelOpen())
        {
            Debug.Log(
                "Another panel is already open. " +
                "Exit menu will not open."
            );

            return;
        }

        menuOpen = true;

        if (exitConfirmPanel != null)
            exitConfirmPanel.SetActive(true);

        yesSelected = false;

        stickReady = true;

        UpdateSelection();

        Debug.Log("Exit menu opened");

        SendTCPEvent(
            "PANEL_OPENED_EXIT_CONFIRMATION",
            "PANEL_EVENT",
            "Exit confirmation panel opened."
        );
    }

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

    private void ConfirmExit()
    {
        Debug.Log("YES selected - exiting");

        // ==========================
        // TCP EVENT - YES CONFIRMED
        // ==========================

        SendTCPEvent(
            "EXIT_YES_CONFIRMED",
            "BUTTON_EVENT",
            "YES confirmed. Game exit requested."
        );

        // اول منوی تأیید را ببند
        menuOpen = false;

        if (exitConfirmPanel != null)
            exitConfirmPanel.SetActive(false);

        // ==========================
        // TCP EVENT - PANEL CLOSED
        // ==========================

        SendTCPEvent(
            "PANEL_CLOSED_EXIT_CONFIRMATION",
            "PANEL_EVENT",
            "Exit confirmation panel closed after YES."
        );

        // بعد GameManager را اجرا کن
        if (gameManager != null)
        {
            gameManager.ExitGame();
        }
        else
        {
            Debug.LogWarning("GameManager is not assigned!");
        }
    }

    private void CloseExitMenu()
    {
        menuOpen = false;

        if (exitConfirmPanel != null)
            exitConfirmPanel.SetActive(false);

        Debug.Log("NO selected - exit cancelled");

        // ==========================
        // TCP EVENT - NO CONFIRMED
        // ==========================

        SendTCPEvent(
            "EXIT_NO_CONFIRMED",
            "BUTTON_EVENT",
            "NO confirmed. Exit cancelled."
        );

        // ==========================
        // TCP EVENT - PANEL CLOSED
        // ==========================

        SendTCPEvent(
            "PANEL_CLOSED_EXIT_CONFIRMATION",
            "PANEL_EVENT",
            "Exit confirmation panel closed after NO."
        );
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