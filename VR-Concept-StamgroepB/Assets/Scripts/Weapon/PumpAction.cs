using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class PumpAction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Shotgun shotgun;

    // XR Grab Interactable on the root Weapon object.
    [SerializeField] private XRGrabInteractable shotgunGrab;

    // Root transform of the complete shotgun.
    [SerializeField] private Transform weaponRoot;

    // Primary hand grip / trigger hand point.
    [SerializeField] private Transform primaryGripPoint;

    // Fixed off-hand grip point on the pump.
    [SerializeField] private Transform pumpGripPoint;

    // Actual pump mesh/bone that should visually slide.
    [SerializeField] private Transform pumpVisual;

    [Header("Pump Movement")]
    [SerializeField] private Vector3 localSlideAxis = Vector3.back;
    [SerializeField] private float travelDistance = 0.18f;

    [Header("Rack Detection")]
    [Range(0f, 1f)]
    [SerializeField] private float rearThreshold = 0.85f;

    [Range(0f, 1f)]
    [SerializeField] private float forwardThreshold = 0.15f;

    [Header("Two-Hand Aiming")]
    [SerializeField] private bool enableTwoHandAim = true;

    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip rackSound;

    private XRSimpleInteractable pumpInteractable;
    private IXRSelectInteractor pumpInteractor;

    private Vector3 startingPumpLocalPosition;
    private Vector3 initialPumpHandLocalPosition;

    private Vector3 localTwoHandDirection;
    private Vector3 localUpDirection;

    private bool isPumpGrabbed;
    private bool reachedRear;

    private void Awake()
    {
        pumpInteractable = GetComponent<XRSimpleInteractable>();

        if (weaponRoot == null && shotgun != null)
            weaponRoot = shotgun.transform;

        if (pumpVisual == null)
            pumpVisual = transform;

        startingPumpLocalPosition = pumpVisual.localPosition;

        if (
            weaponRoot != null &&
            primaryGripPoint != null &&
            pumpGripPoint != null
        )
        {
            Vector3 worldGripDirection =
                (pumpGripPoint.position - primaryGripPoint.position).normalized;

            localTwoHandDirection =
                weaponRoot.InverseTransformDirection(worldGripDirection);

            localUpDirection =
                weaponRoot.InverseTransformDirection(primaryGripPoint.up);
        }
    }

    private void OnEnable()
    {
        if (pumpInteractable == null)
            return;

        pumpInteractable.selectEntered.AddListener(OnPumpGrabbed);
        pumpInteractable.selectExited.AddListener(OnPumpReleased);
    }

    private void OnDisable()
    {
        if (pumpInteractable == null)
            return;

        pumpInteractable.selectEntered.RemoveListener(OnPumpGrabbed);
        pumpInteractable.selectExited.RemoveListener(OnPumpReleased);
    }

    private void LateUpdate()
    {
        if (!isPumpGrabbed || pumpInteractor == null)
            return;

        if (enableTwoHandAim)
        {
            UpdateTwoHandAim();
        }

        UpdatePumpSlide();
    }

    private void OnPumpGrabbed(SelectEnterEventArgs args)
    {
        pumpInteractor = args.interactorObject;
        isPumpGrabbed = true;

        Transform pumpHand =
            pumpInteractor.GetAttachTransform(pumpInteractable);

        initialPumpHandLocalPosition =
            weaponRoot.InverseTransformPoint(pumpHand.position);

        reachedRear = false;
    }

    private void OnPumpReleased(SelectExitEventArgs args)
    {
        pumpInteractor = null;
        isPumpGrabbed = false;

        ReturnPumpForward();
    }

    // --------------------------------------------------
    // TWO-HANDED AIMING
    // --------------------------------------------------

    private void UpdateTwoHandAim()
    {
        if (shotgunGrab == null)
            return;

        if (shotgunGrab.interactorsSelecting.Count == 0)
            return;

        IXRSelectInteractor primaryInteractor =
            shotgunGrab.interactorsSelecting[0];

        Transform primaryHand =
            primaryInteractor.GetAttachTransform(shotgunGrab);

        Transform secondaryHand =
            pumpInteractor.GetAttachTransform(pumpInteractable);

        Vector3 handDirection =
            secondaryHand.position - primaryHand.position;

        if (handDirection.sqrMagnitude < 0.001f)
            return;

        handDirection.Normalize();

        Quaternion localReferenceRotation =
            Quaternion.LookRotation(
                localTwoHandDirection,
                localUpDirection
            );

        Quaternion desiredWorldRotation =
            Quaternion.LookRotation(
                handDirection,
                primaryHand.up
            );

        Quaternion targetWeaponRotation =
            desiredWorldRotation *
            Quaternion.Inverse(localReferenceRotation);

        weaponRoot.rotation = targetWeaponRotation;

        // Keep the primary grip locked to the primary controller.
        Vector3 positionCorrection =
            primaryHand.position - primaryGripPoint.position;

        weaponRoot.position += positionCorrection;
    }

    // --------------------------------------------------
    // PUMP MOVEMENT
    // --------------------------------------------------

    private void UpdatePumpSlide()
    {
        Transform pumpHand =
            pumpInteractor.GetAttachTransform(pumpInteractable);

        Vector3 currentHandLocalPosition =
            weaponRoot.InverseTransformPoint(pumpHand.position);

        Vector3 handDelta =
            currentHandLocalPosition -
            initialPumpHandLocalPosition;

        Vector3 axis =
            localSlideAxis.normalized;

        float distanceAlongAxis =
            Vector3.Dot(handDelta, axis);

        float clampedDistance =
            Mathf.Clamp(
                distanceAlongAxis,
                0f,
                travelDistance
            );

        pumpVisual.localPosition =
            startingPumpLocalPosition +
            axis * clampedDistance;

        float normalizedPumpPosition =
            clampedDistance / travelDistance;

        // Pump has reached the back.
        if (normalizedPumpPosition >= rearThreshold)
        {
            reachedRear = true;
        }

        // Pump was pulled back, then pushed forward again.
        if (
            reachedRear &&
            normalizedPumpPosition <= forwardThreshold
        )
        {
            reachedRear = false;

            if (shotgun != null)
            {
                shotgun.CompletePumpCycle();
            }

            if (audioSource != null && rackSound != null)
            {
                audioSource.PlayOneShot(rackSound);
            }

            Debug.Log("Shotgun racked.");
        }
    }

    private void ReturnPumpForward()
    {
        pumpVisual.localPosition =
            startingPumpLocalPosition;

        // Releasing the pump does NOT count as completing the rack.
        reachedRear = false;
    }
}