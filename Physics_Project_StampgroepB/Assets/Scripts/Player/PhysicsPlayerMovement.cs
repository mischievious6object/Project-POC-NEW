using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(PlayerInput))]
public class PhysicsPlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private GravityGunV2 gravityGun;

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
    [SerializeField] private float controllerLookSensitivity = 120f;
    [SerializeField] private float maxLookAngle = 89f;

    [Header("Physics")]
    [SerializeField] private float groundDrag = 2f;
    [SerializeField] private float airDrag = 0f;

    private Rigidbody rb;
    private CapsuleCollider capsule;
    private PlayerInput playerInput;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction crouchAction;

    private Vector2 moveInput;
    private Vector2 lookInput;

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
        playerInput = GetComponent<PlayerInput>();

        moveAction =
            playerInput.actions["Move"];

        lookAction =
            playerInput.actions["Look"];

        jumpAction =
            playerInput.actions["Jump"];

        crouchAction =
            playerInput.actions["Crouch"];

        rb.freezeRotation = true;
        rb.useGravity = true;

        originalColliderCenter =
            capsule.center;

        currentHeight =
            standingHeight;

        currentCameraHeight =
            standingCameraHeight;

        capsule.height =
            standingHeight;

        Cursor.lockState =
            CursorLockMode.Locked;

        Cursor.visible = false;
    }

    private void Update()
    {
        ReadInput();
        HandleLook();
        HandleCursor();
    }

    private void FixedUpdate()
    {
        groundCheckTimer -=
            Time.fixedDeltaTime;

        grounded =
            groundCheckTimer > 0f;

        HandleCrouch();
        HandleMovement();
        HandleJump();
        HandleExtraGravity();

        rb.linearDamping =
            grounded
                ? groundDrag
                : airDrag;
    }

    private void ReadInput()
    {
        moveInput =
            moveAction.ReadValue<Vector2>();

        lookInput =
            lookAction.ReadValue<Vector2>();

        jumpHeld =
            jumpAction.IsPressed();

        crouchHeld =
            crouchAction.IsPressed();
    }

    private void HandleLook()
    {
        if (cameraTransform == null)
            return;

        if (Cursor.lockState !=
            CursorLockMode.Locked)
            return;

        /*
         * IMPORTANT:
         *
         * If R is being held while carrying an object,
         * the gravity gun owns the Look input.
         *
         * Player/camera rotation completely stops.
         */
        if (gravityGun != null &&
            gravityGun.IsRotatingHeldObject)
        {
            return;
        }

        Vector2 look =
            lookInput;

        if (playerInput.currentControlScheme ==
            "Gamepad")
        {
            look *=
                controllerLookSensitivity *
                Time.deltaTime;
        }
        else
        {
            look *=
                mouseSensitivity;
        }

        transform.Rotate(
            Vector3.up *
            look.x
        );

        cameraPitch -=
            look.y;

        cameraPitch =
            Mathf.Clamp(
                cameraPitch,
                -maxLookAngle,
                maxLookAngle
            );

        cameraTransform.localRotation =
            Quaternion.Euler(
                cameraPitch,
                0f,
                0f
            );
    }

    private void HandleMovement()
    {
        Vector3 direction =
            transform.forward *
            moveInput.y +
            transform.right *
            moveInput.x;

        if (direction.sqrMagnitude > 1f)
        {
            direction.Normalize();
        }

        if (grounded)
        {
            GroundMovement(direction);
        }
        else
        {
            AirMovement(direction);
        }
    }

    private void GroundMovement(
        Vector3 direction)
    {
        if (direction.sqrMagnitude <
            0.01f)
        {
            return;
        }

        float maxSpeed =
            crouchHeld
                ? maxCrouchSpeed
                : maxGroundSpeed;

        Vector3 horizontalVelocity =
            GetHorizontalVelocity();

        float speedInDirection =
            Vector3.Dot(
                horizontalVelocity,
                direction
            );

        if (speedInDirection <
            maxSpeed)
        {
            rb.AddForce(
                direction *
                groundAcceleration,
                ForceMode.Acceleration
            );
        }
    }

    private void AirMovement(
        Vector3 direction)
    {
        if (direction.sqrMagnitude <
            0.01f)
        {
            return;
        }

        Vector3 horizontalVelocity =
            GetHorizontalVelocity();

        float speedInDirection =
            Vector3.Dot(
                horizontalVelocity,
                direction
            );

        if (speedInDirection <
            bunnyHopMaxSpeed)
        {
            rb.AddForce(
                direction *
                airAcceleration,
                ForceMode.Acceleration
            );
        }
    }

    private void HandleJump()
    {
        if (!jumpHeld ||
            !grounded ||
            crouchHeld)
        {
            return;
        }

        Vector3 velocity =
            rb.linearVelocity;

        if (velocity.y < 0f)
        {
            velocity.y = 0f;
        }

        Vector3 horizontalVelocity =
            new Vector3(
                velocity.x,
                0f,
                velocity.z
            );

        float horizontalSpeed =
            horizontalVelocity.magnitude;

        if (horizontalSpeed >=
            bunnyHopMinimumSpeed)
        {
            Vector3 boostDirection =
                horizontalVelocity.normalized;

            float remainingSpeed =
                bunnyHopMaxSpeed -
                horizontalSpeed;

            if (remainingSpeed > 0f)
            {
                float boostAmount =
                    Mathf.Min(
                        bunnyHopBoost,
                        remainingSpeed
                    );

                velocity +=
                    boostDirection *
                    boostAmount;
            }
        }

        rb.linearVelocity =
            velocity;

        rb.AddForce(
            Vector3.up *
            jumpVelocity,
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
            Physics.gravity *
            (multiplier - 1f),
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

        currentHeight =
            Mathf.MoveTowards(
                currentHeight,
                targetHeight,
                crouchSpeed *
                Time.fixedDeltaTime
            );

        currentCameraHeight =
            Mathf.MoveTowards(
                currentCameraHeight,
                targetCameraHeight,
                crouchSpeed *
                Time.fixedDeltaTime
            );

        capsule.height =
            currentHeight;

        float heightDifference =
            standingHeight -
            currentHeight;

        Vector3 targetCenter;

        if (crouchHeld)
        {
            if (grounded)
            {
                // Feet stay planted.
                targetCenter =
                    originalColliderCenter +
                    Vector3.down *
                    (heightDifference * 0.5f);
            }
            else
            {
                // Legs tuck upward while airborne.
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

        capsule.center =
            Vector3.MoveTowards(
                capsule.center,
                targetCenter,
                crouchSpeed *
                Time.fixedDeltaTime
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
        Vector3 velocity =
            rb.linearVelocity;

        velocity.y = 0f;

        return velocity;
    }

    private void HandleCursor()
    {
        if (Keyboard.current != null &&
            Keyboard.current.escapeKey
                .wasPressedThisFrame)
        {
            Cursor.lockState =
                CursorLockMode.None;

            Cursor.visible = true;
        }

        if (Mouse.current != null &&
            Mouse.current.leftButton
                .wasPressedThisFrame)
        {
            Cursor.lockState =
                CursorLockMode.Locked;

            Cursor.visible = false;
        }
    }

    private void OnCollisionStay(
        Collision collision)
    {
        foreach (
            ContactPoint contact
            in collision.contacts)
        {
            if (contact.normal.y >=
                minimumGroundNormal)
            {
                groundCheckTimer =
                    groundGraceTime;

                grounded = true;

                return;
            }
        }
    }
}