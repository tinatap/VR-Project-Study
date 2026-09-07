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

        // قفل حرکت و چرخش Avatar
        private bool movementLocked = false;

        // آیا دوربین اجازه دارد پشت Avatar قرار بگیرد؟
        public bool allowCameraFollow = true;


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
            if (rightGripAction != null)
                rightGripAction.action.Enable();

            if (turnAction != null)
                turnAction.action.Enable();

            if (forwardAction != null)
                forwardAction.action.Enable();
        }


        // =====================================================
        // DISABLE
        // =====================================================

        private void OnDisable()
        {
            if (rightGripAction != null)
                rightGripAction.action.Disable();

            if (turnAction != null)
                turnAction.action.Disable();

            if (forwardAction != null)
                forwardAction.action.Disable();
        }


        // =====================================================
        // MOVEMENT LOCK
        // =====================================================

        public void SetMovementLocked(bool locked)
        {
            movementLocked = locked;

            if (locked)
            {
                verticalVelocity = 0f;

                if (animator != null)
                {
                    animator.SetFloat("Speed", 0f);
                    animator.SetFloat("MotionSpeed", 0f);
                }
            }
        }


        // =====================================================
        // UPDATE
        // =====================================================

        private void Update()
        {
            if (xrOrigin == null)
                return;


            // =================================================
            // اگر Start Room در حال Inspect است
            // Avatar نباید حرکت یا چرخش کند.
            // =================================================

            if (movementLocked)
                return;


            // =================================================
            // حرکت با Right Trigger
            // =================================================

            MoveForward();


            // =================================================
            // چرخش Avatar با Right Thumbstick
            // =================================================

            RotateAvatar();


            // =================================================
            // Grip = چرخش خودکار دور Avatar
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
            if (xrOrigin == null)
                return;


            // اگر StartRoom کنترل دوربین را گرفته،
            // این اسکریپت نباید XR Origin را جابه‌جا کند.

            if (!allowCameraFollow)
                return;


            // اگر Grip نگه داشته نشده،
            // دوربین پشت Avatar قرار بگیرد.

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
            if (forwardAction == null)
                return;


            float input =
                forwardAction.action.ReadValue<float>();


            // =================================================
            // GRAVITY
            // =================================================

            if (characterController.isGrounded)
            {
                if (verticalVelocity < 0f)
                    verticalVelocity = -2f;
            }
            else
            {
                verticalVelocity +=
                    Physics.gravity.y *
                    Time.deltaTime;
            }


            // =================================================
            // اگر Trigger رها شده
            // فقط Gravity اعمال شود
            // =================================================

            if (input < 0.01f)
            {
                if (animator != null)
                {
                    animator.SetFloat("Speed", 0f);
                    animator.SetFloat("MotionSpeed", 0f);
                }


                Vector3 gravityVelocity =
                    Vector3.up *
                    verticalVelocity;


                characterController.Move(
                    gravityVelocity *
                    Time.deltaTime
                );

                return;
            }


            // =================================================
            // جهت حرکت Avatar
            // =================================================

            Vector3 direction =
                transform.forward;

            direction.y = 0f;


            if (direction.sqrMagnitude > 0.001f)
                direction.Normalize();


            // =================================================
            // حرکت
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

            if (animator != null)
            {
                float animationSpeed =
                    input *
                    moveSpeed *
                    animationSpeedMultiplier;


                animator.SetFloat(
                    "Speed",
                    animationSpeed
                );


                animator.SetFloat(
                    "MotionSpeed",
                    input
                );
            }
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


            // فقط محور X استفاده می‌شود
            float turn =
                input.x;


            // Dead Zone
            if (Mathf.Abs(turn) < 0.1f)
                return;


            // محاسبه مقدار چرخش
            float rotation =
                turn *
                turnSpeed *
                Time.deltaTime;


            // چرخش Avatar
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


            float grip =
                rightGripAction.action.ReadValue<float>();


            return grip > 0.1f;
        }


        // =====================================================
        // CAMERA ORBIT
        // =====================================================

        private void OrbitCamera()
        {
            if (xrOrigin == null)
                return;


            // مرکز چرخش
            Vector3 pivot =
                transform.position;


            // =================================================
            // چرخش خودکار دور Avatar
            // =================================================

            xrOrigin.RotateAround(
                pivot,
                Vector3.up,
                orbitSpeed *
                Time.deltaTime
            );


            // =================================================
            // دوربین به Avatar نگاه کند
            // =================================================

            Vector3 direction =
                pivot -
                xrOrigin.position;


            direction.y = 0f;


            if (direction.sqrMagnitude > 0.001f)
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


            Vector3 back =
                -transform.forward;


            back.y = 0f;


            if (back.sqrMagnitude < 0.001f)
                return;


            back.Normalize();


            Vector3 position =
                transform.position +
                back *
                cameraDistance;


            position.y =
                transform.position.y +
                cameraHeight;


            xrOrigin.position =
                position;


            xrOrigin.rotation =
                Quaternion.Euler(
                    0f,
                    transform.eulerAngles.y,
                    0f
                );
        }
    }
}