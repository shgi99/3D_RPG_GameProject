using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public PlayerController player;

    [Header("Camera Settings")]
    public Vector3 cameraOffset = new Vector3(0, 2, -5);
    public float lookSpeed = 3f;
    public float minPitch = -30f;
    public float maxPitch = 60f;
    public LayerMask collisionLayers;
    public float cameraCollisionOffset = 0.2f;

    private PlayerInputActions inputActions;
    private Vector2 lookInput;
    private float cameraYaw;
    private float cameraPitch;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Look.canceled += ctx => lookInput = Vector2.zero;
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void LateUpdate()
    {
        HandleCamera();
        player.HandleMovement(transform);
    }

    private void HandleCamera()
    {
        // Look �Է�
        cameraYaw += lookInput.x * lookSpeed;
        cameraPitch -= lookInput.y * lookSpeed;
        cameraPitch = Mathf.Clamp(cameraPitch, minPitch, maxPitch);

        // ī�޶� ��ǥ ��ġ
        Quaternion camRot = Quaternion.Euler(cameraPitch, cameraYaw, 0);
        Vector3 desiredPos = player.transform.position + camRot * cameraOffset;

        // ��ֹ� ó��
        RaycastHit hit;
        Vector3 dir = desiredPos - (player.transform.position + Vector3.up * 1.5f);
        if (Physics.Raycast(player.transform.position + Vector3.up * 1.5f, dir.normalized, out hit, dir.magnitude, collisionLayers))
        {
            transform.position = hit.point - dir.normalized * cameraCollisionOffset;
        }
        else
        {
            transform.position = desiredPos;
        }

        // �׻� ĳ���� ��ü �ٶ󺸱�
        transform.LookAt(player.transform.position + Vector3.up * 1.5f);
    }
}
