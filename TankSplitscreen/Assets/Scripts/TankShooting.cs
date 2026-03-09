using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class TankShooting : MonoBehaviour
{
    [SerializeField] private ShellData shellData;
    [Tooltip("Assign the Fire action from your Input Actions (Button). Use an InputActionReference so no PlayerInput is required on the tank.")]
    [SerializeField] private InputActionReference fireActionReference;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject shellPrefab;

    private float nextFireTime = 0f;
    private int ammoRemaining = -1;

    private void Awake()
    {
        if (shellData != null)
            ammoRemaining = shellData.AmmoCount;
    }

    private void OnEnable()
    {
        if (fireActionReference != null && fireActionReference.action != null)
        {
            fireActionReference.action.performed += OnFire;
            // Enable in case it's not enabled elsewhere
            fireActionReference.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (fireActionReference != null && fireActionReference.action != null)
        {
            fireActionReference.action.performed -= OnFire;
            // Optionally disable
            // fireActionReference.action.Disable();
        }
    }

    private void OnFire(InputAction.CallbackContext context)
    {
        if (!context.performed) return; // only on performed
        if (shellData == null || shellPrefab == null || firePoint == null) return;

        if (shellData.AmmoCount >= 0 && ammoRemaining <= 0) return;

        if (Time.time < nextFireTime) return;

        

        FireShell();
    }

    private void FireShell()
    {
        var obj = Instantiate(shellPrefab, firePoint.position, firePoint.rotation);

        // initialize shell data if Shell component supports it
        var shellComp = obj.GetComponent<Shell>();
        if (shellComp != null)
            shellComp.Initialize(shellData);

        // consume ammo if limited
        if (shellData.AmmoCount >= 0)
            ammoRemaining = Mathf.Max(0, ammoRemaining - 1);

        // set next fire time
        nextFireTime = Time.time + (1f / Mathf.Max(0.0001f, shellData.FireRate));
    }
}
