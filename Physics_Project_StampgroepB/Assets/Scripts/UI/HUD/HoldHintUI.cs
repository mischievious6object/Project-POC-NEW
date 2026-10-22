using UnityEngine;

public class HoldHintUI : MonoBehaviour
{
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

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

}
