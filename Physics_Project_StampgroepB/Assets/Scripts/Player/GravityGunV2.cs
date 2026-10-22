using UnityEngine;
using UnityEngine.InputSystem;

public class GravityGunV2 : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera cam;
    [SerializeField] private Rigidbody playerRigidbody;

    [Header("Grab")]
    [SerializeField] private float maxGrabDistance = 10f;
    [SerializeField] private float holdDistance = 3f;

    [Header("Hold Feel")]
    [SerializeField] private float basePositionStrength = 22f;
    [SerializeField] private float basePositionDamping = 8f;
    [SerializeField] private float maxHoldSpeed = 15f;

    [Tooltip("How much heavier objects lag behind your aim.")]
    [SerializeField] private float massLagStrength = 0.08f;

    [Tooltip("Mass where an object starts feeling noticeably heavy.")]
    [SerializeField] private float heavyMassReference = 20f;

    [Header("Mass Limits")]
    [SerializeField] private float maxObjectMass = 150f;

    [Header("Self Lift")]
    [Tooltip("Extra pulling power while using a SelfLiftable object as an elevator.")]
    [SerializeField] private float selfLiftStrengthMultiplier = 4f;

    [Tooltip("Maximum speed of the object while self-lifting.")]
    [SerializeField] private float selfLiftMaxSpeed = 7f;

    [Tooltip("How far downward we check to see if the player is standing on the grabbed object.")]
    [SerializeField] private float standingCheckDistance = 1.5f;

    [Tooltip("How far looking up/down can move the lift target vertically.")]
    [SerializeField] private float selfLiftVerticalRange = 4f;

    [Tooltip("How strongly the player is kept horizontally aligned with the lift object.")]
    [SerializeField] private float stickyHorizontalStrength = 18f;

    [Tooltip("Damping for the player's horizontal sticking force.")]
    [SerializeField] private float stickyHorizontalDamping = 8f;

    [Header("Collision Avoidance")]
    [SerializeField] private float obstaclePadding = 0.35f;
    [SerializeField] private LayerMask environmentMask = ~0;

    [Header("Throw")]
    [SerializeField] private float throwForce = 12f;
    [SerializeField] private float heavyThrowReduction = 0.015f;

    [Header("Object Rotation")]
    [SerializeField] private float rotationSensitivity = 0.2f;

    [Tooltip("How strongly the object tries to reach the requested rotation.")]
    [SerializeField] private float rotationStrength = 25f;

    [Tooltip("Reduces rotational wobbling.")]
    [SerializeField] private float rotationDamping = 8f;

    [Tooltip("How much mass slows object rotation.")]
    [SerializeField] private float rotationMassInfluence = 0.5f;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference gravPull;
    [SerializeField] private InputActionReference gravPush;
    [SerializeField] private InputActionReference look;
    [SerializeField] private InputActionReference rotateHeldObject;

    private Rigidbody grabbedRB;

    private float grabbedOriginalLinearDamping;
    private float grabbedOriginalAngularDamping;
    private bool grabbedOriginalUseGravity;

    private Vector3 smoothedTargetPosition;
    private Quaternion targetObjectRotation;

    // Self-lift state
    private bool selfLiftActive;
    private float selfLiftStartObjectY;
    private Vector3 selfLiftPlayerHorizontalOffset;

    public bool IsRotatingHeldObject
    {
        get
        {
            return grabbedRB != null &&
                   rotateHeldObject != null &&
                   rotateHeldObject.action.IsPressed();
        }
    }

    public bool IsHoldingObject
    {
        get
        {
            return grabbedRB != null;
        }
    }

    public bool IsSelfLifting
    {
        get
        {
            return selfLiftActive;
        }
    }

    private void OnEnable()
    {
        if (gravPull != null)
            gravPull.action.Enable();

        if (gravPush != null)
            gravPush.action.Enable();

        if (look != null)
            look.action.Enable();

        if (rotateHeldObject != null)
            rotateHeldObject.action.Enable();
    }

    private void OnDisable()
    {
        if (gravPull != null)
            gravPull.action.Disable();

        if (gravPush != null)
            gravPush.action.Disable();

        if (look != null)
            look.action.Disable();

        if (rotateHeldObject != null)
            rotateHeldObject.action.Disable();

        DropObject();
    }

    private void Update()
    {
        if (gravPull != null &&
            gravPull.action.WasPressedThisFrame())
        {
            if (grabbedRB != null)
            {
                DropObject();
            }
            else
            {
                TryGrabObject();
            }
        }

        if (gravPush != null &&
            gravPush.action.WasPressedThisFrame())
        {
            ThrowObject();
        }

        if (IsRotatingHeldObject)
        {
            ReadObjectRotationInput();
        }
    }

    private void FixedUpdate()
    {
        if (grabbedRB == null)
            return;

        HoldObject();

        if (selfLiftActive)
        {
            HandleStickyLift();
        }

        if (IsRotatingHeldObject)
        {
            RotateObject();
        }
        else
        {
            StabilizeObjectRotation();
        }
    }

    // -------------------------------------------------------
    // GRABBING
    // -------------------------------------------------------

    private void TryGrabObject()
    {
        if (cam == null)
            return;

        Ray ray = cam.ViewportPointToRay(
            new Vector3(0.5f, 0.5f)
        );

        if (!Physics.Raycast(
                ray,
                out RaycastHit hit,
                maxGrabDistance,
                ~0,
                QueryTriggerInteraction.Ignore))
        {
            return;
        }

        Rigidbody rb = hit.collider.attachedRigidbody;

        if (rb == null)
            return;

        if (rb.isKinematic)
            return;

        if (rb.mass > maxObjectMass)
            return;

        grabbedRB = rb;

        grabbedOriginalLinearDamping =
            grabbedRB.linearDamping;

        grabbedOriginalAngularDamping =
            grabbedRB.angularDamping;

        grabbedOriginalUseGravity =
            grabbedRB.useGravity;

        grabbedRB.useGravity = false;

        grabbedRB.linearDamping = 0f;
        grabbedRB.angularDamping = 0f;

        smoothedTargetPosition =
            grabbedRB.worldCenterOfMass;

        targetObjectRotation =
            grabbedRB.rotation;

        TryStartSelfLift();
    }

    // -------------------------------------------------------
    // NORMAL HOLDING
    // -------------------------------------------------------

    private void HoldObject()
    {
        Vector3 rawTargetPosition;

        if (selfLiftActive)
        {
            rawTargetPosition =
                GetSelfLiftTargetPosition();
        }
        else
        {
            rawTargetPosition =
                GetNormalTargetPosition();
        }

        float massFactor =
            GetMassFactor();

        float targetFollowSpeed =
            1f /
            (1f + massFactor * massLagStrength);

        smoothedTargetPosition =
            Vector3.Lerp(
                smoothedTargetPosition,
                rawTargetPosition,
                targetFollowSpeed
            );

        Vector3 positionDifference =
            smoothedTargetPosition -
            grabbedRB.worldCenterOfMass;

        float positionStrength =
            basePositionStrength /
            Mathf.Sqrt(massFactor);

        float positionDamping =
            basePositionDamping /
            Mathf.Sqrt(massFactor);

        if (selfLiftActive)
        {
            positionStrength *=
                selfLiftStrengthMultiplier;
        }

        Vector3 desiredVelocity =
            positionDifference *
            positionStrength;

        Vector3 velocityDifference =
            desiredVelocity -
            grabbedRB.linearVelocity;

        Vector3 acceleration =
            velocityDifference *
            positionDamping;

        grabbedRB.AddForce(
            acceleration,
            ForceMode.Acceleration
        );

        float speedLimit =
            selfLiftActive
                ? selfLiftMaxSpeed
                : maxHoldSpeed;

        if (grabbedRB.linearVelocity.magnitude >
            speedLimit)
        {
            grabbedRB.linearVelocity =
                grabbedRB.linearVelocity.normalized *
                speedLimit;
        }
    }

    private Vector3 GetNormalTargetPosition()
    {
        Vector3 origin =
            cam.transform.position;

        Vector3 direction =
            cam.transform.forward;

        float desiredDistance =
            holdDistance;

        /*
         * Basic wall avoidance.
         */
        RaycastHit[] hits =
            Physics.RaycastAll(
                origin,
                direction,
                holdDistance,
                environmentMask,
                QueryTriggerInteraction.Ignore
            );

        float closestDistance =
            holdDistance;

        foreach (RaycastHit hit in hits)
        {
            if (hit.rigidbody == grabbedRB)
                continue;

            if (playerRigidbody != null &&
                hit.rigidbody == playerRigidbody)
            {
                continue;
            }

            if (hit.distance < closestDistance)
            {
                closestDistance =
                    hit.distance;
            }
        }

        if (closestDistance < holdDistance)
        {
            desiredDistance =
                Mathf.Max(
                    0.5f,
                    closestDistance -
                    obstaclePadding
                );
        }

        return
            origin +
            direction *
            desiredDistance;
    }

    // -------------------------------------------------------
    // SELF LIFT
    // -------------------------------------------------------

    private void TryStartSelfLift()
    {
        selfLiftActive = false;

        if (grabbedRB == null)
            return;

        if (playerRigidbody == null)
            return;

        /*
         * Only specifically marked objects can perform
         * the physics-elevator trick.
         */
        SelfLiftable liftable =
            grabbedRB.GetComponent<SelfLiftable>();

        if (liftable == null)
        {
            liftable =
                grabbedRB.GetComponentInParent<SelfLiftable>();
        }

        if (liftable == null)
            return;

        /*
         * Check whether the object we're grabbing is also
         * directly underneath the player.
         */
        Vector3 rayOrigin =
            playerRigidbody.worldCenterOfMass;

        RaycastHit[] hits =
            Physics.RaycastAll(
                rayOrigin,
                Vector3.down,
                standingCheckDistance,
                ~0,
                QueryTriggerInteraction.Ignore
            );

        float closestDistance =
            float.MaxValue;

        Rigidbody closestRB =
            null;

        foreach (RaycastHit hit in hits)
        {
            // Ignore our own player Rigidbody.
            if (hit.rigidbody == playerRigidbody)
                continue;

            if (hit.distance < closestDistance)
            {
                closestDistance =
                    hit.distance;

                closestRB =
                    hit.rigidbody;
            }
        }

        if (closestRB != grabbedRB)
            return;

        selfLiftActive = true;

        /*
         * Remember the object's starting height.
         *
         * Looking level = roughly this height.
         * Looking upward = lift higher.
         */
        selfLiftStartObjectY =
            grabbedRB.worldCenterOfMass.y;

        /*
         * Remember where the player is standing relative
         * to the box horizontally.
         */
        selfLiftPlayerHorizontalOffset =
            playerRigidbody.position -
            grabbedRB.position;

        selfLiftPlayerHorizontalOffset.y = 0f;

        smoothedTargetPosition =
            grabbedRB.worldCenterOfMass;
    }

    private Vector3 GetSelfLiftTargetPosition()
    {
        Vector3 current =
            grabbedRB.worldCenterOfMass;

        /*
         * IMPORTANT:
         *
         * X and Z remain where the object currently is.
         *
         * So tilting your camera upward does NOT cause
         * the box to shoot forward out from under you.
         */
        float verticalLook =
            cam.transform.forward.y;

        float targetY =
            selfLiftStartObjectY +
            verticalLook *
            selfLiftVerticalRange;

        return new Vector3(
            current.x,
            targetY,
            current.z
        );
    }

    private void HandleStickyLift()
    {
        if (!selfLiftActive)
            return;

        if (grabbedRB == null)
            return;

        if (playerRigidbody == null)
            return;

        /*
         * Figure out where the player should remain
         * horizontally relative to the object.
         */
        Vector3 desiredPlayerPosition =
            grabbedRB.position +
            selfLiftPlayerHorizontalOffset;

        Vector3 difference =
            desiredPlayerPosition -
            playerRigidbody.position;

        /*
         * Only stick horizontally.
         *
         * Vertical movement remains real physics:
         * the box physically pushes the player upward.
         */
        difference.y = 0f;

        Vector3 playerHorizontalVelocity =
            playerRigidbody.linearVelocity;

        playerHorizontalVelocity.y = 0f;

        Vector3 objectHorizontalVelocity =
            grabbedRB.linearVelocity;

        objectHorizontalVelocity.y = 0f;

        Vector3 desiredCorrectionVelocity =
            difference *
            stickyHorizontalStrength;

        Vector3 velocityDifference =
            desiredCorrectionVelocity +
            objectHorizontalVelocity -
            playerHorizontalVelocity;

        velocityDifference.y = 0f;

        playerRigidbody.AddForce(
            velocityDifference *
            stickyHorizontalDamping,
            ForceMode.Acceleration
        );
    }

    // -------------------------------------------------------
    // ROTATING HELD OBJECT
    // -------------------------------------------------------

    private void ReadObjectRotationInput()
    {
        if (look == null)
            return;

        Vector2 mouseDelta =
            look.action.ReadValue<Vector2>();

        float horizontal =
            mouseDelta.x *
            rotationSensitivity;

        float vertical =
            mouseDelta.y *
            rotationSensitivity;

        Quaternion yawRotation =
            Quaternion.AngleAxis(
                -horizontal,
                cam.transform.up
            );

        Quaternion pitchRotation =
            Quaternion.AngleAxis(
                vertical,
                cam.transform.right
            );

        targetObjectRotation =
            yawRotation *
            pitchRotation *
            targetObjectRotation;
    }

    private void RotateObject()
    {
        if (grabbedRB == null)
            return;

        Quaternion rotationDifference =
            targetObjectRotation *
            Quaternion.Inverse(
                grabbedRB.rotation
            );

        rotationDifference.ToAngleAxis(
            out float angle,
            out Vector3 axis
        );

        if (angle > 180f)
        {
            angle -= 360f;
        }

        if (Mathf.Abs(angle) < 0.01f)
            return;

        if (axis.sqrMagnitude < 0.01f)
            return;

        axis.Normalize();

        float massFactor =
            GetMassFactor();

        float massRotationScale =
            1f /
            Mathf.Pow(
                massFactor,
                rotationMassInfluence
            );

        Vector3 torque =
            axis *
            angle *
            Mathf.Deg2Rad *
            rotationStrength *
            massRotationScale;

        torque -=
            grabbedRB.angularVelocity *
            rotationDamping;

        grabbedRB.AddTorque(
            torque,
            ForceMode.Acceleration
        );
    }

    private void StabilizeObjectRotation()
    {
        if (grabbedRB == null)
            return;

        float massFactor =
            GetMassFactor();

        float damping =
            rotationDamping /
            Mathf.Sqrt(massFactor);

        grabbedRB.angularVelocity =
            Vector3.Lerp(
                grabbedRB.angularVelocity,
                Vector3.zero,
                damping *
                Time.fixedDeltaTime
            );

        targetObjectRotation =
            grabbedRB.rotation;
    }

    // -------------------------------------------------------
    // MASS
    // -------------------------------------------------------

    private float GetMassFactor()
    {
        if (grabbedRB == null)
            return 1f;

        return Mathf.Max(
            1f,
            grabbedRB.mass /
            heavyMassReference
        );
    }

    // -------------------------------------------------------
    // THROW / DROP
    // -------------------------------------------------------

    private void ThrowObject()
    {
        if (grabbedRB == null)
            return;

        Rigidbody rb =
            grabbedRB;

        float massFactor =
            GetMassFactor();

        float massReduction =
            1f /
            (
                1f +
                (massFactor - 1f) *
                heavyThrowReduction *
                heavyMassReference
            );

        float finalThrowForce =
            throwForce *
            massReduction;

        ReleaseObject();

        rb.AddForce(
            cam.transform.forward *
            finalThrowForce,
            ForceMode.VelocityChange
        );
    }

    private void DropObject()
    {
        if (grabbedRB == null)
            return;

        ReleaseObject();
    }

    private void ReleaseObject()
    {
        selfLiftActive = false;

        grabbedRB.useGravity =
            grabbedOriginalUseGravity;

        grabbedRB.linearDamping =
            grabbedOriginalLinearDamping;

        grabbedRB.angularDamping =
            grabbedOriginalAngularDamping;

        grabbedRB = null;
    }
}