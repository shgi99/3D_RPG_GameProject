using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float gravity = -9.81f;
    public float rotationSmoothTime = 0.15f;

    private PlayerInputActions inputActions;
    private Vector2 moveInput;
    private bool jumpInput;

    private CharacterController controller;
    private float verticalVelocity;
    private float rotationVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;
        inputActions.Player.Jump.performed += ctx => jumpInput = true;
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    public void HandleMovement(Transform cameraTransform)
    {
        // 카메라 기준 이동
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDir = forward * moveInput.y + right * moveInput.x;

        // 캐릭터 회전 (앞으로 이동할 때만)
        if (moveDir.magnitude > 0.1f && moveInput.y >= 0f)
        {
            float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);
        }

        // 점프 & 중력
        if (controller.isGrounded)
        {
            verticalVelocity = -0.5f;
            if (jumpInput)
            {
                verticalVelocity = jumpForce;
                jumpInput = false;
            }
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        // 이동 적용
        Vector3 velocity = moveDir * moveSpeed + Vector3.up * verticalVelocity;
        controller.Move(velocity * Time.deltaTime);
    }
}
