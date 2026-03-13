using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class TankController : MonoBehaviour
{
    [SerializeField] private TankData tankData;
    [Tooltip("Assign the Move action from your Input Actions (Vector2). Using InputActionReference avoids requiring PlayerInput on the tank.")]
    [SerializeField] private InputActionReference moveActionReference;

    [Tooltip("Assign the Mouse Position or Look action (Vector2). If your action provides deltas (mouse delta or stick), set Aim Input Type to Delta/Stick.")]
    [SerializeField] private InputActionReference aimActionReference;

    public enum AimInputType { ScreenPosition, PointerPosition, DeltaOrStick }
    [Tooltip("How the aim action provides data: screen position (mouse position), pointer position (Pointer.current), or delta/stick values.")]
    [SerializeField] private AimInputType aimInputType = AimInputType.ScreenPosition;

    [SerializeField] private Transform turretTransform;

    [Tooltip("Camera used to convert screen-space pointer to world. If left empty Camera.main will be used.")]
    [SerializeField] private Camera playerCamera;

    private Rigidbody rb;

    // input values
    private Vector2 moveInput = Vector2.zero; // x = turn, y = forward/back
    private Vector2 aimInput = Vector2.zero; // varies by aimInputType

    // for accumulating deltas when using delta/stick aiming
    private Vector2 accumulatedScreenPoint = Vector2.zero;

    private float currentSpeed = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // initialize accumulated point to screen center
        accumulatedScreenPoint = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
    }

    private void OnEnable()
    {
        if (moveActionReference != null && moveActionReference.action != null)
        {
            moveActionReference.action.performed += OnMove;
            moveActionReference.action.canceled += OnMove;
            moveActionReference.action.Enable();
        }

        if (aimActionReference != null && aimActionReference.action != null)
        {
            aimActionReference.action.performed += OnAim;
            aimActionReference.action.canceled += OnAim;
            aimActionReference.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (moveActionReference != null && moveActionReference.action != null)
        {
            moveActionReference.action.performed -= OnMove;
            moveActionReference.action.canceled -= OnMove;
        }

        if (aimActionReference != null && aimActionReference.action != null)
        {
            aimActionReference.action.performed -= OnAim;
            aimActionReference.action.canceled -= OnAim;
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnAim(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();

        switch (aimInputType)
        {
            case AimInputType.ScreenPosition:
                // expected: pixel coordinates (0..Screen.width, 0..Screen.height) or normalized 0..1
                if (value.x >= 0f && value.x <= 1f && value.y >= 0f && value.y <= 1f)
                    aimInput = new Vector2(value.x * Screen.width, value.y * Screen.height);
                else
                    aimInput = value;
                break;

            case AimInputType.PointerPosition:
                // ignore action value; read current pointer
                if (Pointer.current != null)
                    aimInput = Pointer.current.position.ReadValue();
                else
                    aimInput = value;
                break;

            case AimInputType.DeltaOrStick:
                // value is delta in pixels or stick direction; accumulate into a virtual cursor
                accumulatedScreenPoint += value;
                accumulatedScreenPoint.x = Mathf.Clamp(accumulatedScreenPoint.x, 0f, Screen.width);
                accumulatedScreenPoint.y = Mathf.Clamp(accumulatedScreenPoint.y, 0f, Screen.height);
                aimInput = accumulatedScreenPoint;
                break;
        }
    }

    private void Update()
    {
        // Handle turret aiming in Update for smoothness and immediate response to mouse
        if (turretTransform != null && tankData != null)
        {
            RotateTurretTowardAim(Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        if (tankData == null) return;

        // Vertical controls forward/back
        float targetSpeed = moveInput.y * tankData.MaxSpeed;

        // Smooth acceleration
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, tankData.Acceleration * Time.fixedDeltaTime);

        // MovePosition for physics-based movement
        Vector3 forwardMove = transform.forward * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + forwardMove);

        // Rotation
        float turnAmount = moveInput.x * tankData.TurnRate * Time.fixedDeltaTime;
        Quaternion turnRot = Quaternion.Euler(0f, turnAmount, 0f);
        rb.MoveRotation(rb.rotation * turnRot);
    }

    private void RotateTurretTowardAim(float deltaTime)
    {
        Camera cam = playerCamera != null ? playerCamera : Camera.main;
        if (cam == null) return;

        // Assume aimInput is a screen point in pixels now
        Vector2 screenPoint = aimInput;

        // Fallback: if aimInput is zero, try pointer
        if (screenPoint.sqrMagnitude < 0.0001f && Pointer.current != null)
            screenPoint = Pointer.current.position.ReadValue();

        Ray ray = cam.ScreenPointToRay(screenPoint);
        Plane groundPlane = new Plane(Vector3.up, transform.position);

        if (groundPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 dir = hitPoint - turretTransform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.0001f) return;

            // Use yaw angles to avoid quaternion ambiguity that can cause initial opposite rotation
            float targetYaw = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            float currentYaw = turretTransform.eulerAngles.y;

            float newYaw = Mathf.MoveTowardsAngle(currentYaw, targetYaw, tankData.TurretTurnRate * deltaTime);
            turretTransform.rotation = Quaternion.Euler(0f, newYaw, 0f);
        }
    }
}
