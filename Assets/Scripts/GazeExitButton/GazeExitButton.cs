using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GazeExitButton : MonoBehaviour
{
    [Header("References")]
    public Button exitButton;
    public InputActionReference leftTriggerAction;

    [Header("Gaze")]
    public float gazeTolerance = 0.05f;

    [Header("Highlight")]
    public Color highlightColor = Color.blue;
    public float outlineWidth = 5f;

    private Outline outline;
    private bool isGazing = false;

    private void Awake()
    {
        if (exitButton == null)
            exitButton = GetComponent<Button>();

        // Add Outline automatically
        outline = GetComponent<Outline>();

        if (outline == null)
            outline = gameObject.AddComponent<Outline>();

        outline.effectColor = highlightColor;
        outline.effectDistance =
            new Vector2(outlineWidth, outlineWidth);

        outline.enabled = false;
    }

    private void OnEnable()
    {
        if (leftTriggerAction != null)
        {
            leftTriggerAction.action.Enable();
            leftTriggerAction.action.performed += OnLeftTriggerPressed;
        }
    }

    private void OnDisable()
    {
        if (leftTriggerAction != null)
        {
            leftTriggerAction.action.performed -= OnLeftTriggerPressed;
            leftTriggerAction.action.Disable();
        }
    }

    private void Update()
    {
        CheckGaze();
    }

    // =====================================================
    // CHECK PLAYER GAZE
    // =====================================================

    private void CheckGaze()
    {
        if (exitButton == null)
            return;

        RectTransform rect =
            exitButton.GetComponent<RectTransform>();

        if (rect == null)
            return;

        // مرکز صفحه = جهت نگاه بازیکن
        Vector2 screenCenter =
            new Vector2(
                Screen.width * 0.5f,
                Screen.height * 0.5f
            );

        bool lookingAtButton =
            RectTransformUtility.RectangleContainsScreenPoint(
                rect,
                screenCenter,
                null
            );

        if (lookingAtButton)
        {
            if (!isGazing)
            {
                isGazing = true;

                outline.enabled = true;
            }
        }
        else
        {
            if (isGazing)
            {
                isGazing = false;

                outline.enabled = false;
            }
        }
    }

    // =====================================================
    // LEFT TRIGGER
    // =====================================================

    private void OnLeftTriggerPressed(
        InputAction.CallbackContext context)
    {
        if (!isGazing)
            return;

        if (exitButton != null)
        {
            exitButton.onClick.Invoke();
        }
    }
}