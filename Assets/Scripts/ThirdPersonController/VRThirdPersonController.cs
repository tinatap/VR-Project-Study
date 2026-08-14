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

        [Tooltip("Right Thumbstick - Avatar Rotation")]
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

        [Header("Rotation")]

        public float turnSpeed = 90f;


        // =====================================================
        // THIRD PERSON CAMERA
        // =====================================================

        [Header("Third Person Camera")]

        [Tooltip("Distance of XR Origin behind Avatar")]
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
            if (turnAction != null)
                turnAction.action.Disable();

            if (forwardAction != null)
                forwardAction.action.Disable();
        }


        // =====================================================
        // UPDATE
        // =====================================================

        private void Update()
        {
            if (xrOrigin == null)
                return;

            MoveForward();
            RotateAvatar();
        }


        // =====================================================
        // LATE UPDATE
        // XR ORIGIN ALWAYS BEHIND AVATAR
        // =====================================================

        private void LateUpdate()
        {
            KeepXROriginBehindAvatar();
        }


        // =====================================================
        // RIGHT TRIGGER
        // MOVE FORWARD
        // =====================================================

        private void MoveForward()
        {
            // مقدار تریگر راست
            float input =
                forwardAction.action.ReadValue<float>();


            // اگر تریگر فشرده نشده
            if (input < 0.01f)
            {
                if (animator != null)
                {
                    animator.SetFloat("Speed", 0f);
                    animator.SetFloat("MotionSpeed", 0f);
                }

                return;
            }


            // حرکت فقط در جهت جلوی Avatar
            Vector3 direction =
                transform.forward;

            direction.y = 0f;

            direction.Normalize();


            // Gravity
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


            // سرعت بر اساس میزان فشار Trigger
            Vector3 velocity =
                direction *
                moveSpeed *
                input;


            velocity.y =
                verticalVelocity;


            // حرکت Avatar
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
            Vector2 input =
                turnAction.action.ReadValue<Vector2>();


            // فقط محور X تامب‌استیک
            float turn =
                input.x;


            if (Mathf.Abs(turn) < 0.01f)
                return;


            float rotation =
                turn *
                turnSpeed *
                Time.deltaTime;


            // فقط Avatar می‌چرخد
            transform.Rotate(
                0f,
                rotation,
                0f,
                Space.World
            );
        }


        // =====================================================
        // XR ORIGIN
        // ALWAYS BEHIND AVATAR
        // =====================================================

        private void KeepXROriginBehindAvatar()
        {
            if (xrOrigin == null)
                return;


            // ---------------------------------------------
            // جهت پشت Avatar
            // ---------------------------------------------

            Vector3 back =
                -transform.forward;


            back.y = 0f;


            if (back.sqrMagnitude < 0.001f)
                return;


            back.Normalize();


            // ---------------------------------------------
            // موقعیت XR Origin
            // ---------------------------------------------

            Vector3 position =
                transform.position +
                back * cameraDistance;


            // ---------------------------------------------
            // ارتفاع
            // ---------------------------------------------

            position.y =
                transform.position.y +
                cameraHeight;


            // ---------------------------------------------
            // قرار دادن XR Origin
            // ---------------------------------------------

            xrOrigin.position =
                position;


            // ---------------------------------------------
            // جهت XR Origin
            // ---------------------------------------------

            xrOrigin.rotation =
                Quaternion.Euler(
                    0f,
                    transform.eulerAngles.y,
                    0f
                );
        }
    }
}