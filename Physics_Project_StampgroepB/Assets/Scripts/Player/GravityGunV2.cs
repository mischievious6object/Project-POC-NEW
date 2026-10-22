using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GravityGunV2 : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera cam;

    [Header("Grab")]
    [SerializeField] private float maxGrabDistance = 10f;
    [SerializeField] private float holdDistance = 3f;
    [SerializeField] private float minHoldDistance = 1.25f;

    [Header("Hold Feel")]
    [SerializeField] private float basePositionStrength = 22f;
    [SerializeField] private float basePositionDamping = 8f;
    [SerializeField] private float maxHoldSpeed = 15f;

    [Tooltip("How strongly mass affects response delay.")]
    [SerializeField] private float massLagStrength = 0.08f;

    [Tooltip("Mass value where the object starts feeling noticeably heavy.")]
    [SerializeField] private float heavyMassReference = 20f;

    [Header("Mass Limits")]
    [SerializeField] private float maxObjectMass = 150f;

    [Header("Collision Avoidance")]
    [SerializeField] private float obstaclePadding = 0.35f;
    [SerializeField] private LayerMask environmentMask = ~0;

    [Header("Throw")]
    [SerializeField] private float throwForce = 12f;
    [SerializeField] private float heavyThrowReduction = 0.015f;

    [Header("Rotation")]
    [SerializeField] private bool stabilizeRotation = true;
    [SerializeField] private float rotationDamping = 8f;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference gravPull;
    [SerializeField] private InputActionReference gravPush;

    public event Action OnObjectGrabbed;
    public event Action OnObjectDropped;

    private Rigidbody grabbedRB;

    private float grabbedOriginalLinearDamping;
    private float grabbedOriginalAngularDamping;
    private bool grabbedOriginalUseGravity;

    private Vector3 smoothedTargetPosition;

    private void OnEnable()
    {
        gravPull.action.Enable();
        gravPush.action.Enable();
    }

    private void OnDisable()
    {
        gravPull.action.Disable();
        gravPush.action.Disable();

        DropObject();
    }

    private void Update()
    {
        if (gravPull.action.WasPressedThisFrame())
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

        if (gravPush.action.WasPressedThisFrame())
        {
            ThrowObject();
        }
    }

    private void FixedUpdate()
    {
        if (grabbedRB == null)
            return;

        HoldObject();
    }

    private void TryGrabObject()
    {
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
    }

    private void HoldObject()
    {
        Vector3 rawTargetPosition =
            GetTargetPosition();

        float massFactor =
            Mathf.Max(
                1f,
                grabbedRB.mass /
                heavyMassReference
            );

        /*
         * Heavy objects update their target more slowly.
         *
         * Light object:
         * target follows aim almost immediately.
         *
         * Heavy object:
         * target lags behind when you turn quickly.
         */
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

        /*
         * Heavy objects also react less aggressively
         * to the hold force.
         */
        float positionStrength =
            basePositionStrength /
            Mathf.Sqrt(massFactor);

        float positionDamping =
            basePositionDamping /
            Mathf.Sqrt(massFactor);

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

        if (grabbedRB.linearVelocity.magnitude >
            maxHoldSpeed)
        {
            grabbedRB.linearVelocity =
                grabbedRB.linearVelocity.normalized *
                maxHoldSpeed;
        }

        if (stabilizeRotation)
        {
            StabilizeObjectRotation(massFactor);
        }

        OnObjectGrabbed?.Invoke();
    }

    private Vector3 GetTargetPosition()
    {
        Vector3 origin =
            cam.transform.position;

        Vector3 direction =
            cam.transform.forward;

        float desiredDistance =
            holdDistance;

        if (Physics.Raycast(
                origin,
                direction,
                out RaycastHit hit,
                holdDistance,
                environmentMask,
                QueryTriggerInteraction.Ignore))
        {
            if (grabbedRB == null ||
                hit.rigidbody != grabbedRB)
            {
                desiredDistance =
                    Mathf.Max(
                        minHoldDistance,
                        hit.distance -
                        obstaclePadding
                    );
            }
        }

        return origin +
               direction *
               desiredDistance;
    }

    private void StabilizeObjectRotation(
        float massFactor)
    {
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
    }

    private void ThrowObject()
    {
        if (grabbedRB == null)
            return;

        Rigidbody rb =
            grabbedRB;

        float massFactor =
            Mathf.Max(
                1f,
                rb.mass /
                heavyMassReference
            );

        /*
         * Heavy objects can still be thrown,
         * but receive less velocity.
         */
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
        grabbedRB.useGravity =
            grabbedOriginalUseGravity;

        grabbedRB.linearDamping =
            grabbedOriginalLinearDamping;

        grabbedRB.angularDamping =
            grabbedOriginalAngularDamping;

        grabbedRB = null;
        OnObjectDropped?.Invoke();
    }
}