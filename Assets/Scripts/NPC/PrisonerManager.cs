using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation; // AI Navigation 패키지 네임스페이스

public class PrisonerManager : MonoBehaviour
{
    [Header("Navigation")]
    [SerializeField] private NavMeshSurface navMeshSurface; // 감옥 구역의 NavMeshSurface

    [Header("Area Settings")]
    [SerializeField] private Transform tileParent;      // 타일 오브젝트들의 부모
    [SerializeField] private Vector2 tileSize = new Vector2(1f, 1f);
    [SerializeField] private  Transform EnterTile = null; // 감옥 입구 타일 막지 않도록 참조
    [Header("Capacity Settings")]
    [SerializeField] private int maxPrisonerCapacity = 5; // 별도로 관리하는 수용 가능 인원

    private List<Transform> activeTiles = new List<Transform>();
    private List<NPCController> prisonerQueue = new List<NPCController>();

    void Awake()
    {
        RefreshActiveTiles();
    }

    // 타일 범위 갱신 (확장 시 호출)
    public void RefreshActiveTiles()
    {
        activeTiles.Clear();
        foreach (Transform child in tileParent)
        {
            if (child.gameObject.activeSelf) activeTiles.Add(child);
        }
        Debug.Log($"감옥 영역 갱신: 타일 {activeTiles.Count}개 활성화됨");
        // --- 실시간 NavMesh 베이크 실행 ---
        if (navMeshSurface != null)
        {
            // BuildNavMesh()는 런타임에 내비게이션 데이터를 다시 생성합니다.
            navMeshSurface.BuildNavMesh();
            Debug.Log("NavMesh 실시간 베이크 완료");
        }
    }
    public int CurrentTileCount=> activeTiles.Count;
    // --- 수용량 관련 공용 인터페이스 ---

    public int MaxCapacity => maxPrisonerCapacity;

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
        Transform randomTile= EnterTile;
        while (randomTile == EnterTile)
        {
           randomTile = activeTiles[Random.Range(0, activeTiles.Count)];
        }

        float randomX = Random.Range(-tileSize.x * 0.5f, tileSize.x * 0.5f);
        float randomZ = Random.Range(-tileSize.y * 0.5f, tileSize.y * 0.5f);
        
        return new Vector3(randomTile.position.x + randomX, randomTile.position.y, randomTile.position.z + randomZ);
    }

    /// <summary>
    /// 감옥 외형 확장 (수용량과 무관하게 시각적/물리적 범위만 확장)
    /// </summary>
    public void ExpandPrisonArea(int tileCount)
    {
        Debug.Log("확장");
        int activated = 0;
        foreach (Transform child in tileParent)
        {
            if (!child.gameObject.activeSelf)
            {
                child.gameObject.SetActive(true);
                activated++;
                if (activated >= tileCount) break;
            }
        }
        RefreshActiveTiles();
    }
}