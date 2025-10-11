using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 5f;        // 이동 속도
    public float rotationSpeed = 120f;  // 마우스 회전 속도
    public float gravity = -9.81f;      // 중력 가속도
    public float jumpHeight = 2.5f;     // 점프 높이

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // 카메라를 플레이어 머리 위치로 고정
        Camera cam = Camera.main;
        if (cam != null)
        {
            cam.transform.SetParent(transform);
            cam.transform.localPosition = new Vector3(0, 1.6f, 0);
            cam.transform.localRotation = Quaternion.identity;
        }
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
        ApplyGravityAndJump();
    }

    void HandleMovement()
    {
        Vector2 moveInput = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) moveInput.y += 1;
            if (Keyboard.current.sKey.isPressed) moveInput.y -= 1;
            if (Keyboard.current.aKey.isPressed) moveInput.x -= 1;
            if (Keyboard.current.dKey.isPressed) moveInput.x += 1;
        }

        Vector3 move = (transform.right * moveInput.x + transform.forward * moveInput.y).normalized;
        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    void HandleRotation()
    {
        if (Mouse.current != null)
        {
            float mouseX = Mouse.current.delta.x.ReadValue() * rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up * mouseX);
        }
    }

    void ApplyGravityAndJump()
    {
        //  바닥 감지
        isGrounded = controller.isGrounded;

        // 바닥에 닿아있으면 낙하 속도 리셋
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        //  점프 입력
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        //  중력 적용
        velocity.y += gravity * Time.deltaTime;

        //  최종 이동 (중력까지 포함)
        controller.Move(velocity * Time.deltaTime);
    }
}
