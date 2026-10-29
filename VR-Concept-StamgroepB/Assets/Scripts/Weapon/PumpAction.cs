using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class PumpAction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Shotgun shotgun;
    [SerializeField] private XRGrabInteractable shotgunGrab;
    [SerializeField] private Transform weaponRoot;
    [SerializeField] private Transform primaryGripPoint;
    [SerializeField] private Transform pumpGripPoint;
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

    [Header("Pump Sounds")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip pumpBackSound;
    [SerializeField] private AudioClip pumpForwardSound;

    private XRSimpleInteractable pumpInteractable;
    private IXRSelectInteractor pumpInteractor;

    private Vector3 startingPumpLocalPosition;
    private Vector3 initialPumpHandLocalPosition;

    private Vector3 localTwoHandDirection;
    private Vector3 localUpDirection;

    private bool isPumpGrabbed;
    private bool reachedRear;
    private bool playedRearSound;

    private bool originalTrackRotation;

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

        if (shotgunGrab != null)
        {
            originalTrackRotation = shotgunGrab.trackRotation;
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

        if (shotgunGrab != null)
        {
            shotgunGrab.trackRotation = originalTrackRotation;
        }
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
        playedRearSound = false;

        // IMPORTANT:
        // Stop XRI from forcing the shotgun to follow
        // the primary controller's rotation.
        if (shotgunGrab != null)
        {
            originalTrackRotation = shotgunGrab.trackRotation;
            shotgunGrab.trackRotation = false;
        }
    }

    private void OnPumpReleased(SelectExitEventArgs args)
    {
        pumpInteractor = null;
        isPumpGrabbed = false;

        ReturnPumpForward();

        // Give rotation control back to the normal XR grab.
        if (shotgunGrab != null)
        {
            shotgunGrab.trackRotation = originalTrackRotation;
        }
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

        // Build the rotation that points the shotgun
        // from the primary grip toward the pump hand.
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

        // After rotating, keep the trigger-hand grip
        // exactly attached to the primary controller.
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

        // Pump reaches rear.
        if (
            normalizedPumpPosition >= rearThreshold &&
            !playedRearSound
        )
        {
            reachedRear = true;
            playedRearSound = true;

            if (audioSource != null && pumpBackSound != null)
            {
                audioSource.PlayOneShot(pumpBackSound);
            }
        }

        // Pump returns fully forward.
        if (
            reachedRear &&
            normalizedPumpPosition <= forwardThreshold
        )
        {
            reachedRear = false;
            playedRearSound = false;

            if (audioSource != null && pumpForwardSound != null)
            {
                audioSource.PlayOneShot(pumpForwardSound);
            }

            if (shotgun != null)
            {
                shotgun.CompletePumpCycle();
            }

            Debug.Log("Shotgun racked.");
        }
    }

    private void ReturnPumpForward()
    {
        pumpVisual.localPosition =
            startingPumpLocalPosition;

        reachedRear = false;
        playedRearSound = false;
    }
}