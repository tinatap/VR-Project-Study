using UnityEngine;
using UnityEngine.InputSystem;

namespace StarterAssets
{
    public class VRThirdPersonController : MonoBehaviour
    {
        // =====================================================
        // REFERENCES
        // =====================================================

        [Header("References")]
        public Transform xrOrigin;


        // =====================================================
        // INPUT
        // =====================================================

        [Header("Input")]

        [Tooltip("Right Grip - Orbit Camera")]
        public InputActionReference rightGripAction;

        [Tooltip("Right Thumbstick - Rotate Avatar")]
        public InputActionReference turnAction;

        [Tooltip("Right Trigger - Move Forward")]
        public InputActionReference forwardAction;


        // =====================================================
        // MOVEMENT
        // =====================================================

        [Header("Movement")]
        public float moveSpeed = 2f;


        // =====================================================
        // ROTATION
        // =====================================================

        [Header("Avatar Rotation")]

        [Tooltip("Avatar rotation speed from Right Thumbstick")]
        public float turnSpeed = 90f;


        // =====================================================
        // CAMERA ORBIT
        // =====================================================

        [Header("Camera Orbit")]

        [Tooltip("Automatic camera orbit speed while Right Grip is held")]
        public float orbitSpeed = 30f;

        [Tooltip("Distance of XR Origin from Avatar")]
        public float cameraDistance = 3f;

        [Tooltip("Height of XR Origin")]
        public float cameraHeight = 1.5f;


        // =====================================================
        // ANIMATION
        // =====================================================

        [Header("Animation")]
        public float animationSpeedMultiplier = 1.5f;


        // =====================================================
        // PRIVATE
        // =====================================================

        private CharacterController characterController;
        private Animator animator;

        private float verticalVelocity;

        // Movement and rotation lock
        private bool movementLocked = false;

        // Allows XR Origin to follow Avatar
        public bool allowCameraFollow = true;


        // =====================================================
        // CONSTANTS
        // =====================================================

        private const float InputDeadZone = 0.01f;
        private const float TurnDeadZone = 0.1f;
        private const float GripThreshold = 0.1f;
        private const float DirectionThreshold = 0.001f;
        private const float GroundedVelocity = -2f;


        // =====================================================
        // AWAKE
        // =====================================================

        private void Awake()
        {
            characterController =
                GetComponent<CharacterController>();

            animator =
                GetComponent<Animator>();
        }


        // =====================================================
        // ENABLE
        // =====================================================

        private void OnEnable()
        {
            EnableAction(rightGripAction);
            EnableAction(turnAction);
            EnableAction(forwardAction);
        }


        // =====================================================
        // DISABLE
        // =====================================================

        private void OnDisable()
        {
            DisableAction(rightGripAction);
            DisableAction(turnAction);
            DisableAction(forwardAction);
        }


        // =====================================================
        // INPUT HELPERS
        // =====================================================

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


        // =====================================================
        // MOVEMENT LOCK
        // =====================================================

        public void SetMovementLocked(bool locked)
        {
            movementLocked = locked;

            if (!locked)
                return;

            verticalVelocity = 0f;

            SetAnimationValues(0f, 0f);
        }


        // =====================================================
        // UPDATE
        // =====================================================

        private void Update()
        {
            if (xrOrigin == null ||
                movementLocked)
            {
                return;
            }


            // =================================================
            // MOVE
            // =================================================

            MoveForward();


            // =================================================
            // ROTATE AVATAR
            // =================================================

            RotateAvatar();


            // =================================================
            // CAMERA ORBIT
            // =================================================

            if (IsRightGripHeld())
            {
                OrbitCamera();
            }
        }


        // =====================================================
        // LATE UPDATE
        // =====================================================

        private void LateUpdate()
        {
            if (xrOrigin == null ||
                !allowCameraFollow)
            {
                return;
            }


            // Keep camera behind Avatar
            // only when Grip is not held.

            if (!IsRightGripHeld())
            {
                KeepXROriginBehindAvatar();
            }
        }


        // =====================================================
        // MOVE FORWARD
        // RIGHT TRIGGER
        // =====================================================

        private void MoveForward()
        {
            if (forwardAction == null ||
                characterController == null)
            {
                return;
            }


            float input =
                forwardAction.action.ReadValue<float>();


            // =================================================
            // GRAVITY
            // =================================================

            UpdateVerticalVelocity();


            // =================================================
            // NO FORWARD INPUT
            // =================================================

            if (input < InputDeadZone)
            {
                SetAnimationValues(0f, 0f);

                ApplyGravityOnly();

                return;
            }


            // =================================================
            // MOVEMENT DIRECTION
            // =================================================

            Vector3 direction =
                transform.forward;

            direction.y = 0f;

            if (direction.sqrMagnitude >
                DirectionThreshold)
            {
                direction.Normalize();
            }


            // =================================================
            // MOVEMENT
            // =================================================

            Vector3 velocity =
                direction *
                moveSpeed *
                input;

            velocity.y =
                verticalVelocity;

            characterController.Move(
                velocity *
                Time.deltaTime
            );


            // =================================================
            // ANIMATION
            // =================================================

            float animationSpeed =
                input *
                moveSpeed *
                animationSpeedMultiplier;

            SetAnimationValues(
                animationSpeed,
                input
            );
        }


        // =====================================================
        // GRAVITY
        // =====================================================

        private void UpdateVerticalVelocity()
        {
            if (characterController.isGrounded)
            {
                if (verticalVelocity < 0f)
                    verticalVelocity =
                        GroundedVelocity;

                return;
            }

            verticalVelocity +=
                Physics.gravity.y *
                Time.deltaTime;
        }


        // =====================================================
        // GRAVITY ONLY
        // =====================================================

        private void ApplyGravityOnly()
        {
            Vector3 gravityVelocity =
                Vector3.up *
                verticalVelocity;

            characterController.Move(
                gravityVelocity *
                Time.deltaTime
            );
        }


        // =====================================================
        // ANIMATION
        // =====================================================

        private void SetAnimationValues(
            float speed,
            float motionSpeed)
        {
            if (animator == null)
                return;

            animator.SetFloat(
                "Speed",
                speed
            );

            animator.SetFloat(
                "MotionSpeed",
                motionSpeed
            );
        }


        // =====================================================
        // RIGHT THUMBSTICK
        // ROTATE AVATAR
        // =====================================================

        private void RotateAvatar()
        {
            if (turnAction == null)
                return;

            Vector2 input =
                turnAction.action.ReadValue<Vector2>();


            // Only X axis
            float turn =
                input.x;


            // Dead Zone
            if (Mathf.Abs(turn) <
                TurnDeadZone)
            {
                return;
            }


            // Rotation amount
            float rotation =
                turn *
                turnSpeed *
                Time.deltaTime;


            // Rotate Avatar
            transform.Rotate(
                0f,
                rotation,
                0f,
                Space.World
            );
        }


        // =====================================================
        // RIGHT GRIP
        // =====================================================

        private bool IsRightGripHeld()
        {
            if (rightGripAction == null)
                return false;

            return
                rightGripAction.action
                .ReadValue<float>() >
                GripThreshold;
        }


        // =====================================================
        // CAMERA ORBIT
        // =====================================================

        private void OrbitCamera()
        {
            if (xrOrigin == null)
                return;


            // Avatar position = orbit pivot
            Vector3 pivot =
                transform.position;


            // =================================================
            // AUTOMATIC ORBIT
            // =================================================

            xrOrigin.RotateAround(
                pivot,
                Vector3.up,
                orbitSpeed *
                Time.deltaTime
            );


            // =================================================
            // LOOK AT AVATAR
            // =================================================

            Vector3 direction =
                pivot -
                xrOrigin.position;

            direction.y = 0f;


            if (direction.sqrMagnitude >
                DirectionThreshold)
            {
                xrOrigin.rotation =
                    Quaternion.LookRotation(
                        direction,
                        Vector3.up
                    );
            }
        }


        // =====================================================
        // KEEP XR ORIGIN BEHIND AVATAR
        // =====================================================

        private void KeepXROriginBehindAvatar()
        {
            if (xrOrigin == null)
                return;


            // =================================================
            // BACK DIRECTION
            // =================================================

            Vector3 back =
                -transform.forward;

            back.y = 0f;


            if (back.sqrMagnitude <
                DirectionThreshold)
            {
                return;
            }

            back.Normalize();


            // =================================================
            // CAMERA POSITION
            // =================================================

            Vector3 position =
                transform.position +
                back *
                cameraDistance;

            position.y =
                transform.position.y +
                cameraHeight;


            xrOrigin.position =
                position;


            // =================================================
            // CAMERA ROTATION
            // =================================================

            xrOrigin.rotation =
                Quaternion.Euler(
                    0f,
                    transform.eulerAngles.y,
                    0f
                );
        }
    }
}