using UnityEngine;
using System.Collections.Generic;
using ReadOnlyDrawer;
// 아이템 종류 정의

public class ItemStacker : MonoBehaviour
{
    [Header("Item Settings")]
    [SerializeField] private ItemType itemType;       // 이 스태커가 취급하는 타입
    [SerializeField,ReadOnly] private GameObject itemPrefab;   // 쌓을 아이템 모델

    [Header("Stack Layout Settings")]
    [SerializeField] private Transform stackPivot;
    [SerializeField] private int columnCount = 1;      // 가로 열 개수
    [SerializeField] private int maxPerColumn = 10;    // 한 열에 쌓을 수 있는 최대 높이

    [Header("Auto Calculated Offsets")]
    [SerializeField, ReadOnly] private Vector2 stackOffset; // X: 열 간격, Y: 높이 간격

    private string prefabPath="Items/";
    private List<GameObject> stackedItems = new List<GameObject>();

    public ItemType MyItemType => itemType;
    public bool CanStack => stackedItems.Count < (columnCount * maxPerColumn);
    public int CurrentCount => stackedItems.Count;
    public int MaxCapacity => columnCount * maxPerColumn;

    private void Awake()
    {
        // 게임 시작 시 프리팹 로드 확인
        LoadPrefabByTypeName();
    }
    private void OnValidate()
    {
        // 에디터에서 값을 바꿀 때 즉시 프리팹 탐색 및 오프셋 계산
        LoadPrefabByTypeName();
    }
    // 이제 프리팹을 외부에서 넘겨줄 필요 없이 내부 설정값을 사용합니다.
    private void LoadPrefabByTypeName()
    {
        string path = prefabPath + itemType.ToString();
        GameObject loadedPrefab = Resources.Load<GameObject>(path);

        if (loadedPrefab != null)
        {
            itemPrefab = loadedPrefab;

            // 2. 프리팹의 Mesh 크기를 가져와서 Offset 자동 설정
            MeshRenderer meshRenderer = itemPrefab.GetComponentInChildren<MeshRenderer>();
            if (meshRenderer != null)
            {
                Vector3 size = meshRenderer.bounds.size;
                stackOffset = new Vector2(size.z, size.y);
            }
        }
        else
        {
            // 폴더에 이름이 맞는 프리팹이 없을 경우 경고
            Debug.LogWarning($"[ItemStacker] '{path}' 경로에서 프리팹을 찾을 수 없습니다. 이름과 경로를 확인하세요.");
        }
    }
    // --- 일괄 추가 기능 ---
    public void AddItems(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (!CanStack) break;
            AddItem();
        }
    }

    public void AddItem()
    {
        if (!CanStack || itemPrefab == null) return;

        GameObject newItem = Instantiate(itemPrefab, stackPivot);
        newItem.transform.localPosition = CalculateLocalPosition(stackedItems.Count);
        newItem.transform.localRotation = Quaternion.identity;
        stackedItems.Add(newItem);
    }

    // --- 일괄 제거 기능 ---
    public List<GameObject> PopItems(int count)
    {
        List<GameObject> removedItems = new List<GameObject>();
        int actualPopCount = Mathf.Min(count, stackedItems.Count);

        for (int i = 0; i < actualPopCount; i++)
        {
            removedItems.Add(PopItem());
        }
        return removedItems;
    }

    public GameObject PopItem()
    {
        if (stackedItems.Count == 0) return null;
        int lastIndex = stackedItems.Count - 1;
        GameObject item = stackedItems[lastIndex];
        stackedItems.RemoveAt(lastIndex);
        return item;
    }

    // --- 좌표 계산 로직 (Grid System) ---
    private Vector3 CalculateLocalPosition(int index)
    {
        // 1. 가로(열) 위치 계산: index % columnCount
        // 2. 세로(층) 위치 계산: index / columnCount
        int col = index % columnCount;
        int row = index / columnCount;

        float posZ = col * stackOffset.x;
        float posY = row * stackOffset.y;

        // 피벗을 중심으로 정렬하고 싶다면 posX에서 (columnCount-1)*0.5f를 뺄 수도 있습니다.
        return new Vector3(0, posY, posZ);
    }
}