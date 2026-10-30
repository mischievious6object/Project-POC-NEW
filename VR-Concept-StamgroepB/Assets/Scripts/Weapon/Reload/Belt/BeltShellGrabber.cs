using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRSimpleInteractable))]
public class BeltShellGrabber : MonoBehaviour
{
    [Header("Shell")]
    [SerializeField] private GameObject shellPrefab;

    [Header("Safety")]
    [SerializeField] private float grabCooldown = 0.2f;

    private XRSimpleInteractable beltInteractable;

    private bool spawningShell;

    private void Awake()
    {
        beltInteractable =
            GetComponent<XRSimpleInteractable>();
    }

    private void OnEnable()
    {
        beltInteractable.selectEntered.AddListener(
            OnBeltGrabbed
        );
    }

    private void OnDisable()
    {
        beltInteractable.selectEntered.RemoveListener(
            OnBeltGrabbed
        );
    }

    private void OnBeltGrabbed(
        SelectEnterEventArgs args
    )
    {
        if (spawningShell)
            return;

        if (shellPrefab == null)
        {
            Debug.LogError(
                "BeltShellGrabber has no shell prefab."
            );

            return;
        }

        StartCoroutine(
            GiveShellToHand(
                args.interactorObject,
                args.manager
            )
        );
    }

    private IEnumerator GiveShellToHand(
        IXRSelectInteractor interactor,
        XRInteractionManager manager
    )
    {
        spawningShell = true;

        /*
         * First release the invisible belt zone.
         * Otherwise the hand may still be busy selecting it.
         */
        manager.SelectExit(
            interactor,
            beltInteractable
        );

        yield return null;

        Transform handTransform =
            interactor.transform;

        GameObject shell =
            Instantiate(
                shellPrefab,
                handTransform.position,
                handTransform.rotation
            );

        XRGrabInteractable shellGrab =
            shell.GetComponent<XRGrabInteractable>();

        if (shellGrab == null)
        {
            Debug.LogError(
                "Shell prefab needs an XRGrabInteractable."
            );

            Destroy(shell);

            spawningShell = false;

            yield break;
        }

        Rigidbody rb =
            shell.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Force the same hand to immediately grab the shell.
        manager.SelectEnter(
            interactor,
            shellGrab
        );

        Debug.Log("Shell taken from belt.");

        yield return new WaitForSeconds(
            grabCooldown
        );

        spawningShell = false;
    }
}