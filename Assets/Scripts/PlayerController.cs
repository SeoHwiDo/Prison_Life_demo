using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private Joystick joystick;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float rotationSpeed = 720f; // 초당 회전 각도
    [SerializeField] private float gravity = 9.81f;

    private CharacterController controller;
    private Animator animator;
    private Vector3 moveDirection;
    private float verticalVelocity;

    // 애니메이터 파라미터 캐싱 (성능 최적화)
    private readonly int hashSpeed = Animator.StringToHash("MoveSpeed");
    private readonly int hashIsMoving = Animator.StringToHash("IsMoving");

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Move();
        ApplyGravity();
    }

    private void Move()
    {
        // 1. 조이스틱 입력 값 가져오기 (Vector2 -> Vector3 변환)
        Vector2 input = joystick.InputDirection;
        moveDirection = new Vector3(input.x, 0, input.y);

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            // 2. 캐릭터 회전 (입력 방향으로 부드럽게)
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            // 3. 실제 이동 실행
            controller.Move(moveDirection * moveSpeed * Time.deltaTime);

            // 4. 애니메이션 파라미터 업데이트
            if (animator != null)
            {
                animator.SetFloat(hashSpeed, input.magnitude);
                animator.SetBool(hashIsMoving, true);
            }
        }
        else
        {
            if (animator != null)
            {
                animator.SetFloat(hashSpeed, 0);
                animator.SetBool(hashIsMoving, false);
            }
        }
    }

    private void ApplyGravity()
    {
        // 바닥에 붙어있지 않을 때 중력 적용
        if (controller.isGrounded)
        {
            verticalVelocity = -0.5f; // 바닥에 붙어있도록 살짝 아래로 힘 적용
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        Vector3 gravityVector = new Vector3(0, verticalVelocity, 0);
        controller.Move(gravityVector * Time.deltaTime);
    }
}