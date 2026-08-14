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
        exitConfirmPanel.SetActive(false);

        yesSelected = false;

        UpdateSelection();

        if (gameManager == null)
            gameManager = FindFirstObjectByType<GameManager>();
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

    private void OpenExitMenu()
    {
        menuOpen = true;

        exitConfirmPanel.SetActive(true);

        yesSelected = false;

        stickReady = true;

        UpdateSelection();

        Debug.Log("Exit menu opened");
    }

    private void UpdateSelection()
    {
        if (yesButton == null || noButton == null)
            return;

        Image yesImage = yesButton.GetComponent<Image>();
        Image noImage = noButton.GetComponent<Image>();

        if (yesImage != null)
            yesImage.color =
                yesSelected ? selectedColor : normalColor;

        if (noImage != null)
            noImage.color =
                yesSelected ? normalColor : selectedColor;
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

        // اول منوی تأیید را ببند
        menuOpen = false;

        if (exitConfirmPanel != null)
            exitConfirmPanel.SetActive(false);

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

        exitConfirmPanel.SetActive(false);

        Debug.Log("NO selected - exit cancelled");
    }
}