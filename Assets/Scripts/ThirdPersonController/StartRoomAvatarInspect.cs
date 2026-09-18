using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;

public class StartRoomAvatarInspect : MonoBehaviour
{
    // =====================================================
    // REFERENCES
    // =====================================================

    [Header("References")]

    public Transform avatar;
    public Transform xrOrigin;
    public VRThirdPersonController avatarController;


    // =====================================================
    // START ROOM
    // =====================================================

    [Header("Start Room Control")]

    public GameObject startRoom;


    // =====================================================
    // INPUT
    // =====================================================

    [Header("Input")]

    [Tooltip("Right Grip Button")]
    public InputActionReference rightGripAction;


    // =====================================================
    // CAMERA ORBIT
    // =====================================================

    [Header("Camera Orbit")]

    [Tooltip("Distance from Avatar")]
    public float orbitDistance = 3f;

    [Tooltip("Automatic rotation speed")]
    public float orbitSpeed = 30f;


    // =====================================================
    // PRIVATE
    // =====================================================

    private bool inspecting;
    private bool gripHeld;
    private float currentAngle;

    private const float GripThreshold = 0.1f;
    private const float DirectionThreshold = 0.001f;


    // =====================================================
    // ENABLE
    // =====================================================

    private void OnEnable()
    {
        if (rightGripAction != null)
            rightGripAction.action.Enable();
    }


    // =====================================================
    // DISABLE
    // =====================================================

    private void OnDisable()
    {
        if (rightGripAction != null)
            rightGripAction.action.Disable();

        // اطمینان از اینکه در صورت Disable شدن اسکریپت
        // Avatar در حالت قفل باقی نماند
        if (inspecting)
        {
            inspecting = false;
            gripHeld = false;

            if (avatarController != null)
            {
                avatarController.SetMovementLocked(false);
                avatarController.allowCameraFollow = true;
            }
        }
    }


    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        if (!IsStartRoomActive())
            return;

        if (rightGripAction == null)
            return;

        bool newGripHeld =
            rightGripAction.action.ReadValue<float>() >
            GripThreshold;


        // =================================================
        // GRIP PRESSED
        // =================================================

        if (newGripHeld && !gripHeld)
        {
            gripHeld = true;
            StartInspect();
            return;
        }


        // =================================================
        // GRIP HELD
        // =================================================

        if (!gripHeld)
            return;

        if (newGripHeld)
        {
            RotateCameraAroundAvatar();
        }
        else
        {
            gripHeld = false;
            ExitInspect();
        }
    }


    // =====================================================
    // START ROOM CHECK
    // =====================================================

    private bool IsStartRoomActive()
    {
        return
            startRoom != null &&
            startRoom.activeInHierarchy;
    }


    // =====================================================
    // START INSPECT
    // =====================================================

    private void StartInspect()
    {
        if (avatar == null ||
            xrOrigin == null)
        {
            return;
        }

        inspecting = true;


        // =================================================
        // LOCK AVATAR
        // =================================================

        if (avatarController != null)
        {
            avatarController.SetMovementLocked(true);

            // جلوگیری از Follow شدن خودکار دوربین
            // هنگام Inspect
            avatarController.allowCameraFollow = false;
        }


        // =================================================
        // CALCULATE CURRENT CAMERA ANGLE
        // =================================================

        Vector3 direction =
            xrOrigin.position -
            avatar.position;

        direction.y = 0f;

        if (direction.sqrMagnitude >
            DirectionThreshold)
        {
            currentAngle =
                Mathf.Atan2(
                    direction.x,
                    direction.z
                ) *
                Mathf.Rad2Deg;
        }
    }


    // =====================================================
    // CAMERA ROTATION
    // =====================================================

    private void RotateCameraAroundAvatar()
    {
        if (!inspecting ||
            avatar == null ||
            xrOrigin == null)
        {
            return;
        }


        // =================================================
        // UPDATE ANGLE
        // =================================================

        currentAngle +=
            orbitSpeed *
            Time.deltaTime;


        // =================================================
        // CALCULATE POSITION
        // =================================================

        Quaternion rotation =
            Quaternion.Euler(
                0f,
                currentAngle,
                0f
            );

        Vector3 offset =
            rotation *
            Vector3.forward *
            orbitDistance;

        Vector3 cameraPosition =
            avatar.position +
            offset;


        // =================================================
        // CAMERA HEIGHT
        // =================================================

        if (avatarController != null)
        {
            cameraPosition.y =
                avatar.position.y +
                avatarController.cameraHeight;
        }


        xrOrigin.position =
            cameraPosition;


        // =================================================
        // LOOK AT AVATAR
        // =================================================

        Vector3 lookDirection =
            avatar.position -
            xrOrigin.position;

        if (lookDirection.sqrMagnitude >
            DirectionThreshold)
        {
            xrOrigin.rotation =
                Quaternion.LookRotation(
                    lookDirection,
                    Vector3.up
                );
        }
    }


    // =====================================================
    // EXIT INSPECT
    // =====================================================

    private void ExitInspect()
    {
        inspecting = false;


        // =================================================
        // UNLOCK AVATAR
        // =================================================

        if (avatarController != null)
        {
            avatarController.SetMovementLocked(false);
            avatarController.allowCameraFollow = true;
        }


        // =================================================
        // RETURN CAMERA BEHIND AVATAR
        // =================================================

        ReturnBehindAvatar();
    }


    // =====================================================
    // RETURN CAMERA BEHIND AVATAR
    // =====================================================

    private void ReturnBehindAvatar()
    {
        if (avatar == null ||
            xrOrigin == null)
        {
            return;
        }


        Vector3 back =
            -avatar.forward;

        back.y = 0f;

        if (back.sqrMagnitude <
            DirectionThreshold)
        {
            return;
        }

        back.Normalize();


        // =================================================
        // POSITION
        // =================================================

        Vector3 position =
            avatar.position +
            back *
            orbitDistance;


        // =================================================
        // HEIGHT
        // =================================================

        if (avatarController != null)
        {
            position.y =
                avatar.position.y +
                avatarController.cameraHeight;
        }


        xrOrigin.position =
            position;


        // =================================================
        // ROTATION
        // =================================================

        xrOrigin.rotation =
            Quaternion.Euler(
                0f,
                avatar.eulerAngles.y,
                0f
            );
    }
}