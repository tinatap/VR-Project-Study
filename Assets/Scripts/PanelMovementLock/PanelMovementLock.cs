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

        lastMovementLocked = false;

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
        if (locked == lastMovementLocked)
            return;

        lastMovementLocked = locked;

        if (vrController != null)
        {
            vrController.SetMovementLocked(locked);

            Debug.Log(
                "========== MOVEMENT LOCK CHANGED ==========\n" +
                "Movement Locked: " + locked + "\n" +
                "Success Panel: " + IsOpen(successPanel) + "\n" +
                "Transfer Panel: " + IsOpen(transferPanel) + "\n" +
                "Time Over Panel: " + IsOpen(timeOverPanel) + "\n" +
                "Finish Panel: " + IsOpen(finishPanel) + "\n" +
                "Exit Panel: " + IsOpen(exitConfirmPanel) + "\n" +
                "Start Question: " + IsOpen(startQuestionPanel)
            );
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