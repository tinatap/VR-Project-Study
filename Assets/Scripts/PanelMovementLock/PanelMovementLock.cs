using UnityEngine;
using StarterAssets;

public class PanelMovementLock : MonoBehaviour
{
    [Header("VR Player Controller")]
    public VRThirdPersonController vrController;

    [Header("Panels")]
    public GameObject successPanel;
    public GameObject transferPanel;
    public GameObject timeOverPanel;
    public GameObject finishPanel;
    public GameObject exitConfirmPanel;
    public GameObject startQuestionPanel;

    private bool lastMovementLocked;


    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        if (vrController == null)
        {
            vrController =
                FindFirstObjectByType<VRThirdPersonController>();
        }

        UpdateMovementLock(true);
    }


    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        bool anyPanelOpen =
            IsOpen(successPanel) ||
            IsOpen(transferPanel) ||
            IsOpen(timeOverPanel) ||
            IsOpen(finishPanel) ||
            IsOpen(exitConfirmPanel) ||
            IsOpen(startQuestionPanel);

        UpdateMovementLock(anyPanelOpen);
    }


    // =====================================================
    // UPDATE MOVEMENT LOCK
    // =====================================================

    private void UpdateMovementLock(bool locked)
    {
        // اگر وضعیت تغییری نکرده، نیازی به فراخوانی مجدد نیست
        if (!locked &&
            !lastMovementLocked)
        {
            return;
        }

        if (locked &&
            lastMovementLocked)
        {
            return;
        }

        lastMovementLocked = locked;

        if (vrController != null)
        {
            vrController.SetMovementLocked(locked);
        }
    }


    // =====================================================
    // PANEL CHECK
    // =====================================================

    private bool IsOpen(GameObject panel)
    {
        return panel != null &&
               panel.activeInHierarchy;
    }
}