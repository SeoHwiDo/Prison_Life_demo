using UnityEngine;
using System.Collections.Generic;

// 아이템 종류 정의
public enum ItemType { Ore, HandCuffs, Money }

public class ItemStacker : MonoBehaviour
{
    [Header("Item Settings")]
    [SerializeField] private ItemType itemType;       // 이 스태커가 취급하는 타입
    [SerializeField] private GameObject itemPrefab;   // 쌓을 아이템 모델

    [Header("Stack Settings")]
    [SerializeField] private Transform stackPivot;
    [SerializeField] private Vector3 stackOffset;
    [SerializeField] private int maxStack = 10;

    private List<GameObject> stackedItems = new List<GameObject>();

    public ItemType MyItemType => itemType;
    public bool CanStack => stackedItems.Count < maxStack;
    public int CurrentCount => stackedItems.Count;

    // 이제 프리팹을 외부에서 넘겨줄 필요 없이 내부 설정값을 사용합니다.
    public void AddItem()
    {
        if (!CanStack) return;

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