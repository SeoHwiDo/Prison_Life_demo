using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public enum PrisonerState { Idle, MovingToOre, Mining, Full }

public class PrisonerMiningAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private TaskManager taskManager;
    [SerializeField] private Animator animator; // 애니메이터 참조 추가
    //[SerializeField] private ItemStacker myStacker;
    [SerializeField] private Transform oreContainer; // 1번 요구사항: 광석 부모 오브젝트

    [Header("Settings")]
    [SerializeField] private float stoppingDistance = 1.5f;

    private PrisonerState currentState = PrisonerState.Idle;
    private OreReSpawn targetOre;
    private List<OreReSpawn> cachedOres = new List<OreReSpawn>(); // 캐싱된 광석 리스트
    private readonly int hashIsMoving = Animator.StringToHash("IsMoving");
    void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        // 부모 오브젝트 아래의 모든 OreReSpawn 컴포넌트를 미리 가져옵니다.
        if (oreContainer != null)
        {
            cachedOres.AddRange(oreContainer.GetComponentsInChildren<OreReSpawn>());
        }
        taskManager.SetInMineZone(true);
    }

    void Start()
    {
        StartCoroutine(AILogicRoutine());
    }

    private IEnumerator AILogicRoutine()
    {
        while (true)
        {
            switch (currentState)
            {
                case PrisonerState.Idle:
                    animator.SetBool(hashIsMoving, false);
                    FindNearestOre();
                    break;

                case PrisonerState.MovingToOre:
                    MoveToTarget();
                    break;

                case PrisonerState.Mining:
                    animator.SetBool(hashIsMoving, false);
                    CheckMiningCondition();
                    break;

                //case PrisonerState.Full:
                //    // 인벤토리가 비워질 때까지 대기하거나 수거함으로 이동하는 로직 추가 가능
                //    if (myStacker.CanStack) currentState = PrisonerState.Idle;
                //    break;
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    // 1. 가장 가까운 채굴 가능한 광석 찾기
    private void FindNearestOre()
    {
        //if (!myStacker.CanStack)
        //{
        //    currentState = PrisonerState.Full;
        //    return;
        //}

        float minDistance = Mathf.Infinity;
        OreReSpawn closest = null;

        // 캐싱된 리스트에서 루프를 돕니다.
        foreach (OreReSpawn ore in cachedOres)
        {
            // 2번 요구사항: IsCanMine 속성으로 확인
            if (ore != null && ore.IsCanMine)
            {
                float dist = Vector3.Distance(transform.position, ore.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closest = ore;
                }
            }
        }

        if (closest != null)
        {
            targetOre = closest;
            currentState = PrisonerState.MovingToOre;
        }
    }

    // 2. 광석으로 이동
    private void MoveToTarget()
    {
        // 대상이 없거나 채굴 불가능해진 경우 예외 처리
        if (targetOre == null || !targetOre.IsCanMine)
        {
            currentState = PrisonerState.Idle;
            return;
        }

        agent.SetDestination(targetOre.transform.position);
        animator.SetBool(hashIsMoving, true);
        if (Vector3.Distance(transform.position, targetOre.transform.position) <= stoppingDistance)
        {
            agent.isStopped = true;
            currentState = PrisonerState.Mining;
            taskManager.RequestAddOre(targetOre);
        }
        else
        {
            agent.isStopped = false;
        }
    }

    // 3. 채굴 상태 유지 체크
    private void CheckMiningCondition()
    {
        // 2번 요구사항: 채굴 도중 광석이 소진되었는지 확인
        if (targetOre == null || !targetOre.IsCanMine)
        {
            currentState = PrisonerState.Idle;
        }
        //else if (!myStacker.CanStack)
        //{
        //    mineHandler.StopMining();
        //    currentState = PrisonerState.Full;
        //}
    }
}