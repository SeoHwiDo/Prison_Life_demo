using UnityEngine;
using System.Collections.Generic;

public class PrisonerManager : MonoBehaviour
{
    [SerializeField] private int maxPrisonerCapacity = 20;
    [SerializeField] private BoxCollider prisonArea; // Prison 범위

    private List<NPCController> prisonerQueue = new List<NPCController>();

    public bool CanAddPrisoner() => prisonerQueue.Count < maxPrisonerCapacity;

    public void AddPrisonerToArea(NPCController npc)
    {
        prisonerQueue.Add(npc);

        // 규칙 5: Prison 범위 안의 랜덤 위치로 이동
        Vector3 randomPos = GetRandomPointInBounds(prisonArea.bounds);
        npc.MoveTo(randomPos);

        Debug.Log("죄수 수감 완료");
    }

    private Vector3 GetRandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            bounds.min.y,
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }
}