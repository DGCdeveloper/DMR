using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float acceleration = 15f;
    [SerializeField] private float deceleration = 25f;
    [SerializeField] private float sprintDeceleration = 40f;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -20f;

    [Header("Input")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference sprintAction;



    private CharacterController controller;

    private Vector3 velocity;
    private Vector3 currentMovement;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();
        sprintAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        jumpAction.action.Disable();
        sprintAction.action.Disable();
    }

    private void Update()
    {
        HandleMovement();
        HandleJump();
        ApplyGravity();

        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleMovement()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();

        Vector3 direction =
            transform.right * input.x +
            transform.forward * input.y;

        direction.Normalize();

        bool isSprinting = sprintAction.action.IsPressed();
        bool isMoving = direction.sqrMagnitude > 0.01f;

        float targetSpeed = isSprinting
            ? sprintSpeed
            : walkSpeed;

        Vector3 targetMovement = direction * targetSpeed;

        float speedChange;

        if (!isMoving)
        {
            // Soltó las teclas de movimiento
            speedChange = deceleration;
        }
        else if (currentMovement.magnitude > walkSpeed && !isSprinting)
        {
            // Está pasando de Sprint -> Walk
            speedChange = sprintDeceleration;
        }
        else
        {
            // Movimiento normal
            speedChange = acceleration;
        }

        currentMovement = Vector3.MoveTowards(
            currentMovement,
            targetMovement,
            speedChange * Time.deltaTime
        );

        velocity.x = currentMovement.x;
        velocity.z = currentMovement.z;
    }

    private void HandleJump()
    {
        if (jumpAction.action.WasPressedThisFrame() && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
    }
}