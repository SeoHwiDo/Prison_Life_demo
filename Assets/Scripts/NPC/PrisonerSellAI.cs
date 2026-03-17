using UnityEngine;
using UnityEngine.AI;
using System.Collections;

// 대기 상태(AtNpcSell)를 추가하여 관리합니다.
public enum LogisticsState { Idle, MovingToFactory, Collecting, MovingToSell, MovingToNpcSell, Delivering, AtNpcSell }

public class PrisonerSellAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private TaskManager taskManager;
    [SerializeField] private Animator animator;
    [SerializeField] private ItemStacker myStacker; // AI의 인벤토리

    [Header("Target Points")]
    [SerializeField] private Transform factoryOutputPoint; // 공장 Output 위치
    [SerializeField] private Transform sellInputPoint;    // 판매 Input 위치 (수갑이 쌓이는 곳)
    [SerializeField] private Transform npcSellPoint;      // NPC 판매 대기 위치

    [SerializeField] private ItemStacker sellMoneyStacker; // 판매 구역의 돈이 쌓이는 스태커 (Full 체크용)

    [Header("Settings")]
    [SerializeField] private float stoppingDistance = 1.2f;

    private LogisticsState currentState = LogisticsState.Idle;
    private readonly int hashIsMoving = Animator.StringToHash("IsMoving");
    private readonly int hashHasObj = Animator.StringToHash("HasObj");

    void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (myStacker == null) myStacker = GetComponent<ItemStacker>();
    }

    void Start()
    {
        StartCoroutine(LogisticsRoutine());
    }

    private IEnumerator LogisticsRoutine()
    {
        while (true)
        {
            // 매 루프마다 인벤토리 상태에 따라 HasObj 애니메이션 파라미터를 업데이트합니다.
            UpdateHasObjAnimation();

            switch (currentState)
            {
                case LogisticsState.Idle:
                    DetermineNextState();
                    break;

                case LogisticsState.MovingToFactory:
                    MoveTo(factoryOutputPoint.position, LogisticsState.Collecting);
                    break;

                case LogisticsState.Collecting:
                    // 가방이 꽉 차거나 공장에 더이상 수갑이 없으면 이동 결정
                    if (!myStacker.CanStack || IsTargetStackerEmpty(factoryOutputPoint))
                        currentState = LogisticsState.Idle;
                    break;

                case LogisticsState.MovingToSell:
                    MoveTo(sellInputPoint.position, LogisticsState.Delivering);
                    break;

                case LogisticsState.Delivering:
                    CheckDeliveryCondition();
                    break;

                case LogisticsState.MovingToNpcSell:
                    // NPC 판매 위치로 이동
                    MoveTo(npcSellPoint.position, LogisticsState.AtNpcSell);
                    break;

                case LogisticsState.AtNpcSell:
                    // NPC 판매 위치에서 대기하며 조건 감시
                    CheckNpcSellCondition();
                    break;
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    private void DetermineNextState()
    {
        // 1. Sell 구역(Input)에 수갑이 0보다 많으면 NPC 판매 위치로 이동
        if (GetStackCount(sellInputPoint) > 4)
        {
            currentState = LogisticsState.MovingToNpcSell;
        }
        // 2. 내 가방에 물건이 있으면 판매 구역(Input)으로 이동
        else if (myStacker.CurrentCount > 4)
        {
            currentState = LogisticsState.MovingToSell;
        }
        // 3. 공장에 수갑이 있으면 공장으로 이동
        else if (GetStackCount(factoryOutputPoint) > 0)
        {
            currentState = LogisticsState.MovingToFactory;
        }
        else
        {
            currentState = LogisticsState.Idle;
        }
    }

    private void MoveTo(Vector3 target, LogisticsState nextState)
    {
        agent.SetDestination(target);
        agent.isStopped = false;
        UpdateIsMovingAnimation(true);

        if (Vector3.Distance(transform.position, target) <= stoppingDistance)
        {
            agent.isStopped = true;
            UpdateIsMovingAnimation(false);
            currentState = nextState;
        }
    }

    private void CheckDeliveryCondition()
    {
        UpdateIsMovingAnimation(false);

        bool hasHandcuffs = myStacker.CurrentCount > 0;
        // 돈 스태커가 꽉 찼는지 확인
        bool isMoneyStackerFull = sellMoneyStacker != null && !sellMoneyStacker.CanStack;

        // 내 가방의 수갑이 떨어졌거나, 돈 자리가 꽉 차서 판매를 못 하는 상황이면 Idle로 복귀
        if (!hasHandcuffs || isMoneyStackerFull)
        {
            currentState = LogisticsState.Idle;
        }
    }

    private void CheckNpcSellCondition()
    {
        UpdateIsMovingAnimation(false);

        int sellHandcuffsCount = GetStackCount(sellInputPoint);
        bool isMoneyStackerFull = sellMoneyStacker != null && !sellMoneyStacker.CanStack;

        // Sell의 수갑이 0이 되거나, 돈 스태커에 빈자리가 없으면 다시 행동 결정(Idle)
        if (sellHandcuffsCount <= 3 || isMoneyStackerFull)
        {
            currentState = LogisticsState.Idle;
        }
    }

    private int GetStackCount(Transform point)
    {
        if (point != null && point.TryGetComponent<ItemStacker>(out var stacker))
        {
            return stacker.CurrentCount;
        }
        return 0;
    }

    private bool IsTargetStackerEmpty(Transform point)
    {
        return GetStackCount(point) == 0;
    }

    private void UpdateIsMovingAnimation(bool isMoving)
    {
        if (animator != null) animator.SetBool(hashIsMoving, isMoving);
    }

    private void UpdateHasObjAnimation()
    {
        if (animator != null)
        {
            // myStacker가 0보다 크면 HasObj는 true, 아니면 false
            animator.SetBool(hashHasObj, myStacker.CurrentCount > 0);
        }
    }
}