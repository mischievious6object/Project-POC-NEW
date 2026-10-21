using UnityEngine;
using UnityEngine.InputSystem;

public class GravityGun : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] Camera cam;
    [SerializeField] float maxGrabDistance = 10f, throwForce = 20f, grabSpeed = 10f;
    [SerializeField] Transform objectHolder;


    [Header("Input Actions")]
    [SerializeField] private InputActionReference gravPull;
    [SerializeField] private InputActionReference gravPush;

    Rigidbody grabbedRB;

    private void OnEnable()
    {
        gravPull.action.Enable();
        gravPush.action.Enable();
    }

    private void OnDisable()
    {
        gravPull.action.Disable();
        gravPush.action.Disable();
    }


    void Update()
    {
        if (grabbedRB)
        {
            grabbedRB.MovePosition(Vector3.Lerp(grabbedRB.position, objectHolder.transform.position, Time.deltaTime * grabSpeed));

            if (gravPush.action.WasPressedThisFrame())
            {
                grabbedRB.isKinematic = false;
                grabbedRB.AddForce(cam.transform.forward * throwForce, ForceMode.VelocityChange);
                grabbedRB = null;
            }
        }

        if (gravPull.action.WasPressedThisFrame())
        {
            if (grabbedRB)
            {
                grabbedRB.isKinematic = false;
                grabbedRB = null;
            }
            else
            {
                RaycastHit hit;
                Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
                if (Physics.Raycast(ray, out hit, maxGrabDistance))
                {
                    grabbedRB = hit.collider.gameObject.GetComponent<Rigidbody>();
                    if (grabbedRB)
                    {
                        grabbedRB.isKinematic = true;
                    }
                }
            }
        }
    }
    
}
