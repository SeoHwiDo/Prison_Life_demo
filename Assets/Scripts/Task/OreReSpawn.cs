using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreReSpawn : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Settings")]
    [SerializeField] private float respawnTime = 5f; // 재생성 대기 시간
    [SerializeField] private GameObject meshChild;    // 숨길 하위 메시 오브젝트

    private Collider objectCollider;
    private bool bIsCanMine = true;
    public bool IsCanMine => bIsCanMine;
    void Awake()
    {
        objectCollider = GetComponent<Collider>();

        // 만약 Inspector에서 할당 안 했다면 첫 번째 자식을 자동으로 할당
        if (meshChild == null && transform.childCount > 0)
            meshChild = transform.GetChild(0).gameObject;
    }
    public void Mine()
    {
        if (!bIsCanMine) return;

        StartCoroutine(Mineing());
    }
    private IEnumerator Mineing()
    {
        bIsCanMine = false;

        // 1. 시각적/물리적 비활성화
        meshChild.SetActive(false);
        if (objectCollider != null) objectCollider.enabled = false;

        // 2. 특정 시간 대기
        yield return new WaitForSeconds(respawnTime);

        // 3. 다시 활성화
        meshChild.SetActive(true);
        if (objectCollider != null) objectCollider.enabled = true;
        bIsCanMine = true;

    }
}
