using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject npcPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnInterval = 3f;

    void Start() => StartCoroutine(SpawnRoutine());

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // 규칙 2: toSellQueue에 빈자리가 있을 때만 생성
            if (NPCQueueManager.Instance.CanSpawnInSellQueue())
            {
                GameObject go = Instantiate(npcPrefab, spawnPoint.position, Quaternion.identity);
                NPCController npc = go.GetComponent<NPCController>();
                NPCQueueManager.Instance.AddToSellQueue(npc);
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}