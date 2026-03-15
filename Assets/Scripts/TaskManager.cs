using UnityEngine;
using System.Collections;

public class TaskManager : MonoBehaviour
{
    private Coroutine currentTaskRoutine;
    private string currentActiveTag = "";

    // 1. 영역 진입 시 작업 시작
    private void OnTriggerEnter(Collider other)
    {
        // 이미 작업 중이라면 중복 실행 방지
        if (currentTaskRoutine != null) return;

        string targetTag = other.tag;

        // 태그별 작업 분기
        switch (targetTag)
        {
            case "Ore": // 자원 채집 영역
                currentActiveTag = targetTag;
                currentTaskRoutine = StartCoroutine(MiningTask(other.gameObject));
                break;

            case "Factory":  // 가공 기계 영역
                currentActiveTag = targetTag;
                currentTaskRoutine = StartCoroutine(ProductionTask(other.gameObject));
                break;

            case "Sell":     // 판매 영역
                currentActiveTag = targetTag;
                currentTaskRoutine = StartCoroutine(SellingTask());
                break;
        }
    }

    // 2. 영역 이탈 시 즉시 작업 중지
    private void OnTriggerExit(Collider other)
    {
        // 현재 작업 중인 오브젝트에서 나가는 것인지 확인
        if (other.CompareTag(currentActiveTag))
        {
            StopCurrentTask();
        }
    }

    private void StopCurrentTask()
    {
        if (currentTaskRoutine != null)
        {
            StopCoroutine(currentTaskRoutine);
            currentTaskRoutine = null;
            currentActiveTag = "";
            Debug.Log("작업이 중단되었습니다.");
        }
    }

    // --- 각 태그별 작업 로직 (예시) ---

    private IEnumerator MiningTask(GameObject target)
    {
        Debug.Log("채집 시작...");
        while (true)
        {
            // 앞서 만든 InteractableObject의 Interact 호출 예시
            if (target.TryGetComponent<OreReSpawn>(out var oreReSpawnComp))
            {
                oreReSpawnComp.Mine();
                // 채집 애니메이션 트리거 등 추가 가능
            }

            yield return new WaitForSeconds(1.0f); // 1초마다 반복 작업
        }
    }

    private IEnumerator ProductionTask(GameObject machine)
    {
        Debug.Log("아이템 가공 중...");
        while (true)
        {
            // 가공 로직 구현 (예: 인벤토리의 자원 감소 -> 결과물 생성)
            yield return new WaitForSeconds(2.0f);
        }
    }

    private IEnumerator SellingTask()
    {
        Debug.Log("아이템 판매 중...");
        while (true)
        {
            // 판매 로직 구현 (예: 결과물 제거 -> 돈 증가)
            yield return new WaitForSeconds(0.5f);
        }
    }
}