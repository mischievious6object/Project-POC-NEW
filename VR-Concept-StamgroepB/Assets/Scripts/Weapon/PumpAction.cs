using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class PumpAction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Shotgun shotgun;
    [SerializeField] private Transform weaponRoot;
    [SerializeField] private Transform pumpVisual;

    [Header("Pump Movement")]
    [SerializeField] private Vector3 localSlideAxis = Vector3.back;
    [SerializeField] private float travelDistance = 0.18f;

    [Header("Rack Detection")]
    [Range(0f, 1f)]
    [SerializeField] private float rearThreshold = 0.85f;

    [Range(0f, 1f)]
    [SerializeField] private float forwardThreshold = 0.15f;

    [Header("Pump Sounds")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip pumpBackSound;
    [SerializeField] private AudioClip pumpForwardSound;

    private XRSimpleInteractable pumpInteractable;
    private IXRSelectInteractor pumpInteractor;

    private Vector3 startingPumpLocalPosition;
    private Vector3 initialPumpHandLocalPosition;

    private bool isPumpGrabbed;
    private bool reachedRear;
    private bool playedRearSound;

    private void Awake()
    {
        pumpInteractable = GetComponent<XRSimpleInteractable>();

        if (pumpVisual == null)
            pumpVisual = transform;

        startingPumpLocalPosition = pumpVisual.localPosition;
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

    private void Update()
    {
        if (!isPumpGrabbed || pumpInteractor == null)
            return;

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
    }

    private void OnPumpReleased(SelectExitEventArgs args)
    {
        pumpInteractor = null;
        isPumpGrabbed = false;

        ReturnPumpForward();
    }

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