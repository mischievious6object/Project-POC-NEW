using UnityEngine;

public class GrabHintUI : MonoBehaviour
{
    [SerializeField] private GameObject _hint;
    private GravityGunV2 _gravityGun;

    private void Awake()
    {
        _gravityGun = FindFirstObjectByType<GravityGunV2>();
        _gravityGun.OnObjectGrabbed += Show;
        _gravityGun.OnObjectDropped += Hide;
    }

    private void OnEnable()
    {
        _gravityGun.OnObjectGrabbed += Show;
        _gravityGun.OnObjectDropped += Hide;
    }

    private void OnDisable()
    {
        _gravityGun.OnObjectGrabbed -= Show;
        _gravityGun.OnObjectDropped -= Hide;
    }

    private void Show()
    {
        _hint.SetActive(true);
    }

    private void Hide()
    {
        _hint.SetActive(false);
    }

}
