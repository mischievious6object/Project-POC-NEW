using UnityEngine;
using UnityEngine.InputSystem.HID;

public class HintsController : MonoBehaviour
{
    private HoldHintUI _holdHint;
    private GrabHintUI _grabHint;
    private GravityGunV2 _gravityGun;

    private void Awake()
    {
        _holdHint = FindFirstObjectByType<HoldHintUI>();
        _grabHint = FindFirstObjectByType<GrabHintUI>();
        _gravityGun = FindFirstObjectByType<GravityGunV2>();

        _gravityGun.OnObjectGrabbed += _holdHint.Show;
        _gravityGun.OnObjectDropped += _holdHint.Hide;
    }

    private void Start()
    {
        _holdHint.Hide();
        _grabHint.Hide();
    }

    private void Update()
    {
        if (_gravityGun.IsHoldingObject)
        {
            _grabHint.Hide();
        }
        else
        {
            CheckForPhysicsObjects();
        }
    }

    private void CheckForPhysicsObjects()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, _gravityGun.MaxGrabDistance))
        {
            if (hit.collider.CompareTag("PhysicsObject"))
            {
                _grabHint.Show();
                return;
            }
        }
        _grabHint.Hide();
    }

}
