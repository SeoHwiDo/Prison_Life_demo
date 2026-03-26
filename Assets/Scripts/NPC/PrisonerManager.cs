using System.Collections.Generic;
using Unity.AI.Navigation; // AI Navigation 패키지 네임스페이스
using UnityEngine;
using System.Linq;
public class PrisonerManager : MonoBehaviour
{
    [Header("Navigation")]
    [SerializeField] private NavMeshSurface navMeshSurface; // 감옥 구역의 NavMeshSurface

    [Header("BackWall Reference")]
    [SerializeField] private Transform backWall;

    [Header("Expansion Settings")]
    [SerializeField] private List<ExpansionStep> expansionSteps; // 인스펙터에서 단계별 설정
    [SerializeField] private int cumulativeCount = 0;

    [Header("Area Settings")]
    [SerializeField] private Vector2 tileSize = new Vector2(1f, 1f);
    [SerializeField] private  Transform enterTile = null; // 감옥 입구 타일 막지 않도록 참조
    
    [Header("Capacity Settings")]
    [SerializeField] private int maxPrisonerCapacity = 20; // 별도로 관리하는 수용 가능 인원
    
    private List<Transform> activeTiles = new List<Transform>();
    private List<NPCController> prisonerQueue = new List<NPCController>();
    public int ExpandLevel=> cumulativeCount;
    public System.Action OnPrisonerStatusChanged;
    void Awake()
    {
        InitializePrison();
    }
    private void InitializePrison()
    {
        activeTiles.Clear();
        // 0단계부터 현재 단계까지 모든 오브젝트 활성화 및 BackWall 위치 설정
        for (int i = 0; i <= cumulativeCount; i++)
        {
            ApplyExpansionStep(i, false); // 초기화 시에는 베이크 생략
        }
        RefreshNavMesh();
    }
    public void ExpandPrison()
    {
        if (cumulativeCount + 1 >= expansionSteps.Count)
        {
            Debug.LogWarning("최대 확장 단계에 도달했습니다.");
            return;
        }

        cumulativeCount++;
        ApplyExpansionStep(cumulativeCount, true);
    }
    private void ApplyExpansionStep(int stepIndex, bool shouldBake)
    {
        if (stepIndex < 0 || stepIndex >= expansionSteps.Count) return;

        ExpansionStep step = expansionSteps[stepIndex];

        // 1. 영역 및 좌우 벽 활성화
        if (step.internalArea)
        {
            step.internalArea.SetActive(true);
            // 핵심: 활성화된 internalArea 하위의 모든 Transform(타일들)을 리스트에 추가
            RegisterTilesFromArea(step.internalArea);
        }
        if (step.leftWall) step.leftWall.SetActive(true);
        if (step.rightWall) step.rightWall.SetActive(true);

        // 2. BackWall 위치 이동
        if (backWall != null && step.backWallTarget != null)
        {
            backWall.position = step.backWallTarget.position;
            // 필요 시 회전값도 맞춤
            backWall.rotation = step.backWallTarget.rotation;
        }

        if (shouldBake) RefreshNavMesh();
    }
    private void RegisterTilesFromArea(GameObject area)
    {
        // GetComponentsInChildren을 사용하되, 부모 본인은 제외하고 리스트에 추가
        Transform[] children = area.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child == area.transform) continue; // 부모 오브젝트는 타일이 아니므로 제외

            if (!activeTiles.Contains(child))
            {
                activeTiles.Add(child);
            }
        }
    }
    private void RefreshNavMesh()
    {
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
            Debug.Log($"NavMesh Re-baked: Step {cumulativeCount}");
        }
    }
    // 타일 범위 갱신 (확장 시 호출)
   
    public int CurrentTileCount=> activeTiles.Count;
    // --- 수용량 관련 공용 인터페이스 ---

    public int MaxCapacity => maxPrisonerCapacity;
    public int CurrentPrisonerCount => prisonerQueue.Count;

    public void SetMaxCapacity(int newCapacity)
    {
        maxPrisonerCapacity = newCapacity;
        Debug.Log($"수용 가능 인원이 {maxPrisonerCapacity}명으로 변경되었습니다.");
    }

    public bool CanAddPrisoner() => prisonerQueue.Count < maxPrisonerCapacity;

    // --------------------------------

    public void AddPrisonerToArea(NPCController npc)
    {
        if (!CanAddPrisoner()) return;

        prisonerQueue.Add(npc);

        // 타일 기반 랜덤 위치 이동
        Vector3 randomPos = GetRandomPointOnTiles();
        npc.MoveTo(randomPos);
    }

    private Vector3 GetRandomPointOnTiles()
    {
        if (activeTiles.Count == 0) return transform.position;

        // 활성화된 타일 중 랜덤 선택 -> 해당 타일 내 랜덤 좌표
        var validTiles = activeTiles.Where(t => t != enterTile).ToList();
        if (validTiles.Count == 0) validTiles = activeTiles;
        Transform randomTile = validTiles[Random.Range(0, validTiles.Count)];

        float randomX = Random.Range(-tileSize.x * 0.45f, tileSize.x * 0.45f);
        float randomZ = Random.Range(-tileSize.y * 0.45f, tileSize.y * 0.45f);

        return randomTile.position + new Vector3(randomX, 0, randomZ);
    }

}
public class PrisonRow : MonoBehaviour
{
    public GameObject internalArea;
    public GameObject leftWall;
    public GameObject rightWall;
    public GameObject backWall;

    public void SetRowActive(bool isActive, bool isLastRow)
    {
        gameObject.SetActive(isActive);
        if (isActive)
        {
            internalArea.SetActive(true);
            leftWall.SetActive(true);
            rightWall.SetActive(true);
            // 핵심 규칙: 마지막 행일 때만 BackWall을 보여줌
            backWall.SetActive(isLastRow);
        }
    }
}