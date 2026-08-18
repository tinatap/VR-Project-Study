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

    private void Update()
    {
        bool anyPanelOpen =
            IsOpen(successPanel) ||
            IsOpen(transferPanel) ||
            IsOpen(timeOverPanel) ||
            IsOpen(finishPanel) ||
            IsOpen(exitConfirmPanel) ||
            IsOpen(startQuestionPanel);

        if (vrController != null)
        {
            vrController.SetMovementLocked(anyPanelOpen);
        }
    }

    private bool IsOpen(GameObject panel)
    {
        return panel != null && panel.activeInHierarchy;
    }
}