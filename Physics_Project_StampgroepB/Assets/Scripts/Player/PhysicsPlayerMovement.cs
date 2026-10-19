using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PhysicsPlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    [Header("Movement")]
    [SerializeField] private float groundAcceleration = 32f;
    [SerializeField] private float airAcceleration = 10f;
    [SerializeField] private float maxGroundSpeed = 5.5f;
    [SerializeField] private float maxCrouchSpeed = 3f;

    [Header("Jumping")]
    [SerializeField] private float jumpVelocity = 3.2f;
    [SerializeField] private float groundGraceTime = 0.08f;
    [SerializeField] private float minimumGroundNormal = 0.6f;

    [Header("Gravity")]
    [SerializeField] private float gravityMultiplier = 1.4f;
    [SerializeField] private float fallGravityMultiplier = 2.4f;

    [Header("Bunny Hopping")]
    [SerializeField] private float bunnyHopBoost = 0.45f;
    [SerializeField] private float bunnyHopMinimumSpeed = 4f;
    [SerializeField] private float bunnyHopMaxSpeed = 9f;

    [Header("Crouching")]
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float crouchingHeight = 1f;

    [SerializeField] private float standingCameraHeight = 0.75f;
    [SerializeField] private float crouchingCameraHeight = 0.25f;

    [SerializeField] private float crouchSpeed = 8f;

    [Header("Mouse Look")]
    [SerializeField] private float mouseSensitivity = 0.15f;
    [SerializeField] private float maxLookAngle = 89f;

    [Header("Physics")]
    [SerializeField] private float groundDrag = 2f;
    [SerializeField] private float airDrag = 0f;

    private Rigidbody rb;
    private CapsuleCollider capsule;

    private Vector2 moveInput;

    private bool jumpHeld;
    private bool crouchHeld;
    private bool grounded;

    private float groundCheckTimer;
    private float cameraPitch;

    private float currentHeight;
    private float currentCameraHeight;

    private Vector3 originalColliderCenter;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();

        rb.freezeRotation = true;
        rb.useGravity = true;

        originalColliderCenter = capsule.center;

        currentHeight = standingHeight;
        currentCameraHeight = standingCameraHeight;

        capsule.height = standingHeight;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        ReadInput();
        HandleMouseLook();
    }

    private void FixedUpdate()
    {
        groundCheckTimer -= Time.fixedDeltaTime;
        grounded = groundCheckTimer > 0f;

        HandleCrouch();
        HandleMovement();
        HandleJump();
        HandleExtraGravity();

        rb.linearDamping = grounded ? groundDrag : airDrag;
    }

    private void ReadInput()
    {
        if (Keyboard.current == null)
            return;

        Vector2 input = Vector2.zero;

        if (Keyboard.current.wKey.isPressed)
            input.y += 1f;

        if (Keyboard.current.sKey.isPressed)
            input.y -= 1f;

        if (Keyboard.current.dKey.isPressed)
            input.x += 1f;

        if (Keyboard.current.aKey.isPressed)
            input.x -= 1f;

        moveInput = Vector2.ClampMagnitude(input, 1f);

        jumpHeld = Keyboard.current.spaceKey.isPressed;

        crouchHeld =
            Keyboard.current.leftCtrlKey.isPressed ||
            Keyboard.current.rightCtrlKey.isPressed;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void HandleMouseLook()
    {
        if (Mouse.current == null || cameraTransform == null)
            return;

        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * mouseSensitivity;
        float mouseY = mouseDelta.y * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        cameraPitch -= mouseY;

        cameraPitch = Mathf.Clamp(
            cameraPitch,
            -maxLookAngle,
            maxLookAngle
        );

        cameraTransform.localRotation =
            Quaternion.Euler(cameraPitch, 0f, 0f);
    }

    private void HandleMovement()
    {
        Vector3 direction =
            transform.forward * moveInput.y +
            transform.right * moveInput.x;

        if (direction.sqrMagnitude > 1f)
            direction.Normalize();

        if (grounded)
            GroundMovement(direction);
        else
            AirMovement(direction);
    }

    private void GroundMovement(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.01f)
            return;

        float maxSpeed =
            crouchHeld ? maxCrouchSpeed : maxGroundSpeed;

        Vector3 horizontalVelocity = GetHorizontalVelocity();

        float speedInDirection =
            Vector3.Dot(horizontalVelocity, direction);

        if (speedInDirection < maxSpeed)
        {
            rb.AddForce(
                direction * groundAcceleration,
                ForceMode.Acceleration
            );
        }
    }

    private void AirMovement(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.01f)
            return;

        Vector3 horizontalVelocity = GetHorizontalVelocity();

        float speedInDirection =
            Vector3.Dot(horizontalVelocity, direction);

        if (speedInDirection < bunnyHopMaxSpeed)
        {
            rb.AddForce(
                direction * airAcceleration,
                ForceMode.Acceleration
            );
        }
    }

    private void HandleJump()
    {
        if (!jumpHeld || !grounded || crouchHeld)
            return;

        Vector3 velocity = rb.linearVelocity;

        if (velocity.y < 0f)
            velocity.y = 0f;

        Vector3 horizontalVelocity = new Vector3(
            velocity.x,
            0f,
            velocity.z
        );

        float horizontalSpeed = horizontalVelocity.magnitude;

        // Reward chained bunny hops.
        if (horizontalSpeed >= bunnyHopMinimumSpeed)
        {
            Vector3 boostDirection =
                horizontalVelocity.normalized;

            float remainingSpeed =
                bunnyHopMaxSpeed - horizontalSpeed;

            if (remainingSpeed > 0f)
            {
                float boostAmount =
                    Mathf.Min(
                        bunnyHopBoost,
                        remainingSpeed
                    );

                velocity +=
                    boostDirection * boostAmount;
            }
        }

        rb.linearVelocity = velocity;

        rb.AddForce(
            Vector3.up * jumpVelocity,
            ForceMode.VelocityChange
        );

        grounded = false;
        groundCheckTimer = 0f;
    }

    private void HandleExtraGravity()
    {
        if (grounded)
            return;

        float multiplier =
            rb.linearVelocity.y < 0f
                ? fallGravityMultiplier
                : gravityMultiplier;

        rb.AddForce(
            Physics.gravity * (multiplier - 1f),
            ForceMode.Acceleration
        );
    }

    private void HandleCrouch()
    {
        float targetHeight =
            crouchHeld
                ? crouchingHeight
                : standingHeight;

        float targetCameraHeight =
            crouchHeld
                ? crouchingCameraHeight
                : standingCameraHeight;

        currentHeight = Mathf.MoveTowards(
            currentHeight,
            targetHeight,
            crouchSpeed * Time.fixedDeltaTime
        );

        currentCameraHeight = Mathf.MoveTowards(
            currentCameraHeight,
            targetCameraHeight,
            crouchSpeed * Time.fixedDeltaTime
        );

        capsule.height = currentHeight;

        float heightDifference =
            standingHeight - currentHeight;

        Vector3 targetCenter;

        if (crouchHeld)
        {
            if (grounded)
            {
                /*
                 * GROUNDED CROUCH
                 *
                 * Keep the bottom of the capsule in place.
                 *
                 * The collider shrinks downward from the top,
                 * making the player's upper body dip down.
                 */
                targetCenter =
                    originalColliderCenter +
                    Vector3.down *
                    (heightDifference * 0.5f);
            }
            else
            {
                /*
                 * AIR CROUCH / CROUCH JUMP
                 *
                 * Keep the top of the capsule in place.
                 *
                 * The bottom of the collider moves upward,
                 * effectively pulling the player's legs up.
                 *
                 * This gives extra clearance over ledges.
                 */
                targetCenter =
                    originalColliderCenter +
                    Vector3.up *
                    (heightDifference * 0.5f);
            }
        }
        else
        {
            targetCenter =
                originalColliderCenter;
        }

        capsule.center = Vector3.MoveTowards(
            capsule.center,
            targetCenter,
            crouchSpeed * Time.fixedDeltaTime
        );

        if (cameraTransform != null)
        {
            Vector3 cameraPosition =
                cameraTransform.localPosition;

            cameraPosition.y =
                currentCameraHeight;

            cameraTransform.localPosition =
                cameraPosition;
        }
    }

    private Vector3 GetHorizontalVelocity()
    {
        Vector3 velocity = rb.linearVelocity;

        velocity.y = 0f;

        return velocity;
    }

    private void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.normal.y >= minimumGroundNormal)
            {
                groundCheckTimer =
                    groundGraceTime;

                grounded = true;

                return;
            }
        }
    }
}