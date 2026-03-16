using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    [SerializeField] private GameObject normalMesh;
    [SerializeField] private GameObject prisonerMesh;

    private NavMeshAgent agent;
    public bool IsProcessed { get; private set; } // SellNpc 작업 완료 여부

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        SetMesh(false);
    }

    public void MoveTo(Vector3 position)
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(position);
    }

    public void SetMesh(bool isPrisoner)
    {
        normalMesh.SetActive(!isPrisoner);
        prisonerMesh.SetActive(isPrisoner);
    }

    public void MarkAsProcessed()
    {
        IsProcessed = true;
        SetMesh(true); // 외형을 죄수로 변경
    }

    // 목적지에 도착했는지 확인
    public bool HasReachedDestination()
    {
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                return true;
            }
        }
        return false;
    }
}