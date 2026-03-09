using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class TankController : MonoBehaviour
{
    [SerializeField] private TankData tankData;
    [Tooltip("Assign the Move action from your Input Actions (Vector2). Using InputActionReference avoids requiring PlayerInput on the tank.")]
    [SerializeField] private InputActionReference moveActionReference;

    private Rigidbody rb;

    // input values
    private Vector2 moveInput = Vector2.zero; // x = turn, y = forward/back

    private float currentSpeed = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        if (moveActionReference != null && moveActionReference.action != null)
        {
            moveActionReference.action.performed += OnMove;
            moveActionReference.action.canceled += OnMove;
            // Enable in case it's not enabled elsewhere
            moveActionReference.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (moveActionReference != null && moveActionReference.action != null)
        {
            moveActionReference.action.performed -= OnMove;
            moveActionReference.action.canceled -= OnMove;
            // Optionally disable
            // moveActionReference.action.Disable();
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
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
}
