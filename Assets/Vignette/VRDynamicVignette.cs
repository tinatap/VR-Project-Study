using UnityEngine;

public class VRDynamicVignette : MonoBehaviour
{
    // =====================================================
    // REFERENCES
    // =====================================================

    [Header("References")]

    [Tooltip("CharacterController attached to the Avatar")]
    public CharacterController characterController;

    [Tooltip("Vignette material")]
    public Material vignetteMaterial;


    // =====================================================
    // MOVEMENT
    // =====================================================

    [Header("Movement Contribution")]

    [Tooltip("Maximum avatar movement speed")]
    public float maxMovementSpeed = 2f;

    [Tooltip("Speed below this value is treated as stationary")]
    public float minimumMovementSpeed = 0.05f;

    [Range(0f, 2f)]
    [Tooltip("Strength of movement contribution")]
    public float movementWeight = 1.0f;


    // =====================================================
    // ROTATION
    // =====================================================

    [Header("Rotation Contribution")]

    [Tooltip("Reference maximum rotation speed in degrees/second")]
    public float maxRotationSpeed = 90f;

    [Tooltip("Rotation below this value is ignored")]
    public float minimumRotationSpeed = 5f;

    [Range(0f, 2f)]
    [Tooltip("Strength of rotation contribution")]
    public float rotationWeight = 1.0f;


    // =====================================================
    // RESTRICTION STRENGTH
    // =====================================================

    [Header("Restriction Strength")]

    [Range(0.1f, 3f)]
    [Tooltip("Overall multiplier for vignette restriction")]
    public float restrictionGain = 2.0f;


    // =====================================================
    // VIGNETTE
    // =====================================================

    [Header("Vignette")]

    [Range(0.1f, 1.5f)]
    [Tooltip("Opening radius when completely stationary")]
    public float fullViewRadius = 1.5f;

    [Range(0.01f, 1.0f)]
    [Tooltip("Smallest opening allowed during maximum motion")]
    public float maximumRestrictionRadius = 0.01f;

    [Range(0f, 2f)]
    [Tooltip("Maximum darkness of peripheral area")]
    public float maximumIntensity = 2.0f;


    // =====================================================
    // SMOOTHING
    // =====================================================

    [Header("Smoothing")]

    [Tooltip("How quickly vignette appears")]
    public float fadeInSpeed = 5f;

    [Tooltip("How quickly vignette disappears")]
    public float fadeOutSpeed = 2.5f;

    [Tooltip("Additional smoothing of movement response")]
    public float responseSmoothTime = 0.15f;


    // =====================================================
    // DEBUG
    // =====================================================

    [Header("Debug")]

    [SerializeField]
    private float currentMovementSpeed;

    [SerializeField]
    private float currentRotationSpeed;

    [SerializeField]
    private float movementContribution;

    [SerializeField]
    private float rotationContribution;

    [SerializeField]
    private float currentRestriction;

    [SerializeField]
    private float currentRadius;

    [SerializeField]
    private float currentIntensity;


    // =====================================================
    // PRIVATE
    // =====================================================

    private float previousYaw;

    private float smoothedMovement;
    private float smoothedRotation;

    private float movementVelocity;
    private float rotationVelocity;


    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        if (vignetteMaterial == null)
        {
            Debug.LogWarning(
                "VRDynamicVignette: Vignette Material is missing."
            );

            return;
        }

        currentRadius = fullViewRadius;
        currentIntensity = 0f;

        if (characterController != null)
        {
            previousYaw =
                characterController.transform.eulerAngles.y;
        }

        ApplyVignette();
    }


    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        if (characterController == null ||
            vignetteMaterial == null)
        {
            return;
        }


        // =================================================
        // MOVEMENT SPEED
        // =================================================

        Vector3 velocity =
            characterController.velocity;

        Vector3 horizontalVelocity =
            new Vector3(
                velocity.x,
                0f,
                velocity.z
            );

        currentMovementSpeed =
            horizontalVelocity.magnitude;


        // =================================================
        // ROTATION SPEED
        // =================================================

        float currentYaw =
            characterController.transform.eulerAngles.y;

        float deltaYaw =
            Mathf.DeltaAngle(
                previousYaw,
                currentYaw
            );

        float rotationSpeed =
            Mathf.Abs(deltaYaw) /
            Mathf.Max(Time.deltaTime, 0.0001f);

        currentRotationSpeed =
            rotationSpeed;

        previousYaw =
            currentYaw;


        // =================================================
        // NORMALIZE MOVEMENT
        // =================================================

        float movementNormalized =
            Mathf.InverseLerp(
                minimumMovementSpeed,
                maxMovementSpeed,
                currentMovementSpeed
            );

        movementNormalized =
            Mathf.Clamp01(
                movementNormalized
            );


        // =================================================
        // NORMALIZE ROTATION
        // =================================================

        float rotationNormalized =
            Mathf.InverseLerp(
                minimumRotationSpeed,
                maxRotationSpeed,
                currentRotationSpeed
            );

        rotationNormalized =
            Mathf.Clamp01(
                rotationNormalized
            );


        // =================================================
        // SMOOTH INPUT
        // =================================================

        smoothedMovement =
            Mathf.SmoothDamp(
                smoothedMovement,
                movementNormalized,
                ref movementVelocity,
                responseSmoothTime
            );

        smoothedRotation =
            Mathf.SmoothDamp(
                smoothedRotation,
                rotationNormalized,
                ref rotationVelocity,
                responseSmoothTime
            );


        // =================================================
        // WEIGHTED CONTRIBUTIONS
        // =================================================

        movementContribution =
            smoothedMovement *
            movementWeight;

        rotationContribution =
            smoothedRotation *
            rotationWeight;


        // =================================================
        // COMBINE MOVEMENT + ROTATION
        // =================================================

        float combinedRestriction =
            movementContribution +
            rotationContribution;


        // =================================================
        // INCREASE OVERALL STRENGTH
        // =================================================

        combinedRestriction *=
            restrictionGain;


        currentRestriction =
            Mathf.Clamp01(
                combinedRestriction
            );


        // =================================================
        // TARGET RADIUS
        // =================================================

        float targetRadius =
            Mathf.Lerp(
                fullViewRadius,
                maximumRestrictionRadius,
                currentRestriction
            );


        // =================================================
        // TARGET INTENSITY
        // =================================================

        float targetIntensity =
            Mathf.Lerp(
                0f,
                maximumIntensity,
                currentRestriction
            );


        // =================================================
        // SMOOTH VIGNETTE
        // =================================================

        float radiusSpeed =
            targetRadius < currentRadius
                ? fadeInSpeed
                : fadeOutSpeed;

        float smoothing =
            1f -
            Mathf.Exp(
                -radiusSpeed *
                Time.deltaTime
            );


        currentRadius =
            Mathf.Lerp(
                currentRadius,
                targetRadius,
                smoothing
            );

        currentIntensity =
            Mathf.Lerp(
                currentIntensity,
                targetIntensity,
                smoothing
            );


        // =================================================
        // APPLY
        // =================================================

        ApplyVignette();
    }


    // =====================================================
    // APPLY MATERIAL
    // =====================================================

    private void ApplyVignette()
    {
        if (vignetteMaterial == null)
            return;

        vignetteMaterial.SetFloat(
            "_Radius",
            currentRadius
        );

        vignetteMaterial.SetFloat(
            "_Intensity",
            currentIntensity
        );
    }


    // =====================================================
    // PUBLIC GETTERS
    // =====================================================

    public float GetCurrentMovementSpeed()
    {
        return currentMovementSpeed;
    }

    public float GetCurrentRotationSpeed()
    {
        return currentRotationSpeed;
    }

    public float GetCurrentRestriction()
    {
        return currentRestriction;
    }

    public float GetCurrentRadius()
    {
        return currentRadius;
    }

    public float GetCurrentIntensity()
    {
        return currentIntensity;
    }
}