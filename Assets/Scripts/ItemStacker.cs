using UnityEngine;
using System.Collections.Generic;
using ReadOnlyDrawer;
// 아이템 종류 정의
public enum ItemType { StackOre, StackHandCuffs, StackMoney }

public class ItemStacker : MonoBehaviour
{
    [Header("Item Settings")]
    [SerializeField] private ItemType itemType;       // 이 스태커가 취급하는 타입
    [SerializeField,ReadOnly] private GameObject itemPrefab;   // 쌓을 아이템 모델

    [Header("Stack Settings")]
    [SerializeField] private Transform stackPivot;
    [SerializeField, ReadOnly] private Vector3 stackOffset;
    [SerializeField] private int maxStack = 10;

    private string prefabPath="Items/";
    private List<GameObject> stackedItems = new List<GameObject>();
    
    public ItemType MyItemType => itemType;
    public bool CanStack => stackedItems.Count < maxStack;
    public int CurrentCount => stackedItems.Count;
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
        // 1. Resources 폴더 내의 "Items/Enum이름" 경로로 프리팹 로드
        // 예: ItemType.Ore 이면 "Items/Ore"를 찾음
        string path = prefabPath + itemType.ToString();
        GameObject loadedPrefab = Resources.Load<GameObject>(path);

        if (loadedPrefab != null)
        {
            itemPrefab = loadedPrefab;

            // 2. 프리팹의 Mesh 크기를 가져와서 Offset 자동 설정
            MeshRenderer meshRenderer = itemPrefab.GetComponentInChildren<MeshRenderer>();
            if (meshRenderer != null)
            {
                float itemHeight = meshRenderer.bounds.size.y;
                stackOffset = new Vector3(0, itemHeight, 0);
            }
        }
        else
        {
            // 폴더에 이름이 맞는 프리팹이 없을 경우 경고
            Debug.LogWarning($"[ItemStacker] '{path}' 경로에서 프리팹을 찾을 수 없습니다. 이름과 경로를 확인하세요.");
        }
    }

    public void AddItem()
    {
        if (!CanStack || itemPrefab == null) return;

        GameObject newItem = Instantiate(itemPrefab, stackPivot);
        Vector3 targetLocalPos = Vector3.zero + (stackOffset * stackedItems.Count);

        newItem.transform.localPosition = targetLocalPos;
        newItem.transform.localRotation = Quaternion.identity;
        stackedItems.Add(newItem);
    }

    public GameObject PopItem()
    {
        if (stackedItems.Count == 0) return null;
        int lastIndex = stackedItems.Count - 1;
        GameObject item = stackedItems[lastIndex];
        stackedItems.RemoveAt(lastIndex);
        return item;
    }
}