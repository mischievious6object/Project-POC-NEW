using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WeaponPickupSFX : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip pickupSound;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    private void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
    }

    private void OnEnable()
    {
        if (grabInteractable != null)
            grabInteractable.selectEntered.AddListener(OnPickedUp);
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
            grabInteractable.selectEntered.RemoveListener(OnPickedUp);
    }

    private void OnPickedUp(SelectEnterEventArgs args)
    {
        if (audioSource != null && pickupSound != null)
            audioSource.PlayOneShot(pickupSound);
    }
}