using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3.0f;
    public float gravity = -20f;

    [Header("Jump Settings")]
    public float jumpHeight = 2f;
    public float fallMultiplier = 2.5f;
    public float jumpBufferTime = 0.1f;
    private float jumpBufferCounter;

    [Header("Ground Settings")]
    public LayerMask groundMask;
    private bool isGrounded;

    private CharacterController controller;
    private Vector3 velocity;

    [Header("References")]
    public Transform vrCamera;
    public PlayerHealth health;

    private InputDevice leftController;
    private InputDevice rightController;

    private Vector2 moveInput;
    private bool jumpPressed;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        GetVRControllers();

        // �ڵ����� ���� ī�޶� �ν�
        if (vrCamera == null)
        {
            Camera cam = Camera.main;
            if (cam != null)
                vrCamera = cam.transform;
        }
    }

    void GetVRControllers()
    {
        var leftHandedDevices = new List<InputDevice>();
        var rightHandedDevices = new List<InputDevice>();

        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, leftHandedDevices);
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rightHandedDevices);

        if (leftHandedDevices.Count > 0)
            leftController = leftHandedDevices[0];
        if (rightHandedDevices.Count > 0)
            rightController = rightHandedDevices[0];
    }

    void Update()
    {
        if (!leftController.isValid || !rightController.isValid)
            GetVRControllers();

        if (health != null && health.isDead)
            return;

        ReadInput();
        GroundCheck();
        HandleJumpInput();
        Move();
    }

    void ReadInput()
    {
        moveInput = Vector2.zero;
        jumpPressed = false;

        // VR ��ƽ �Է�
        if (leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 axis))
            moveInput = axis;

        // VR ���� ��ư (������ A��ư)
        if (rightController.TryGetFeatureValue(CommonUsages.primaryButton, out bool jump))
            jumpPressed = jump;

        // Ű���� �Է� (WASD + Space)
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 keyboardMove = new Vector2(horizontal, vertical);
        if (keyboardMove.sqrMagnitude > 0.01f)
            moveInput = keyboardMove;

        if (Input.GetKey(KeyCode.Space))
            jumpPressed = true;
    }

    void GroundCheck()
    {
        Vector3 spherePos = new Vector3(controller.bounds.center.x,
                                        controller.bounds.min.y + 0.05f,
                                        controller.bounds.center.z);
        float checkRadius = Mathf.Max(controller.radius * 0.9f, 0.2f);
        isGrounded = Physics.CheckSphere(spherePos, checkRadius, groundMask);

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;
    }

    void HandleJumpInput()
    {
        if (jumpPressed)
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter = Mathf.Max(jumpBufferCounter - Time.deltaTime, 0);

        if (isGrounded && jumpBufferCounter > 0f)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpBufferCounter = 0;
        }
    }

    void ApplyGravity()
    {
        if (velocity.y < 0)
            velocity.y += gravity * fallMultiplier * Time.deltaTime;
        else
            velocity.y += gravity * Time.deltaTime;
    }

    void Move()
    {
        if (vrCamera == null) return;

        Vector3 camForward = vrCamera.forward;
        Vector3 camRight = vrCamera.right;
        camForward.y = 0;
        camRight.y = 0;

        Vector3 move = (camRight * moveInput.x + camForward * moveInput.y).normalized;
        controller.Move(move * moveSpeed * Time.deltaTime);

        ApplyGravity();
        controller.Move(velocity * Time.deltaTime);
    }

    void OnDrawGizmosSelected()
    {
        if (controller == null) return;
        Gizmos.color = Color.green;
        Vector3 spherePos = new Vector3(controller.bounds.center.x,
                                        controller.bounds.min.y + 0.05f,
                                        controller.bounds.center.z);
        float radius = Mathf.Max(controller.radius * 0.9f, 0.2f);
        Gizmos.DrawWireSphere(spherePos, radius);
    }
} 