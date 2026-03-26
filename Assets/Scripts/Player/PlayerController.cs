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
    private Transform mainCameraTransform;
    private Vector3 moveDirection;
    private float verticalVelocity;

    // 애니메이터 파라미터 캐싱 (성능 최적화)
    private readonly int hashSpeed = Animator.StringToHash("MoveSpeed");
    private readonly int hashIsMoving = Animator.StringToHash("IsMoving");
    private readonly int hashHasObj = Animator.StringToHash("HasObj");

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        Move();
        ApplyGravity();
        if (GameManager.Instance.PlayerHandCuffs.CurrentCount > 0)
        {
            animator.SetBool(hashHasObj, true);
        }
        else
        {
            animator.SetBool(hashHasObj, false);
        }
    }

    private void Move_legacy()
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
    private void Move()
    {
        Vector2 input = joystick.InputDirection;

        // 2. 카메라 방향을 기준으로 이동 방향 계산
        if (mainCameraTransform != null && input.sqrMagnitude > 0.01f)
        {
            // 카메라가 바라보는 방향(Forward)과 오른쪽 방향(Right)을 가져옵니다.
            Vector3 forward = mainCameraTransform.forward;
            Vector3 right = mainCameraTransform.right;

            // Y축(높이) 값을 0으로 만들어 바닥 평면상의 벡터로 고정합니다.
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            // 조이스틱의 y입력은 카메라의 앞쪽으로, x입력은 카메라의 오른쪽으로 대응시킵니다.
            moveDirection = (forward * input.y) + (right * input.x);
        }
        else
        {
            moveDirection = Vector3.zero;
        }

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            // 3. 캐릭터 회전 (카메라 기준 계산된 방향으로)
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            // 4. 실제 이동 실행
            controller.Move(moveDirection * moveSpeed * Time.deltaTime);

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