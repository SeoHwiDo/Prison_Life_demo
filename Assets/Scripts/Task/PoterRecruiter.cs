using UnityEngine;
using System.Collections;

public class PoterRecruiter : MonoBehaviour
{
    [Header("Recruit Settings")]
    [SerializeField] private GameObject prisonerPrefab; // 소환할 일꾼 프리팹
    [SerializeField] private int price = 100;           // 전체 고용 가격
    [SerializeField] private int recruitCount = 1;      // 한 번에 소환할 일꾼 수
    [SerializeField] private Transform spawnPoint;      // 소환 위치
    [SerializeField] private Transform oreContainer;    // 소환된 일꾼에게 할당할 광석 부모

    [Header("UI & Visuals")]
    [SerializeField] private ItemStacker progressStacker; // 돈이 쌓이는 시각적 스태커
    [SerializeField] private float processTime = 0.2f;    // 돈 한 개당 소모 속도

    private bool isProcessing = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isProcessing) return;

        // 1. 플레이어 태그 확인 (필요 시) 및 돈 확인
        // 기존 코드의 10배 배율 유지 (money 1개 = 가치 10)
        if (other.CompareTag("Player") && GameManager.Instance.PlayerMoney.CurrentCount * 10 >= price)
        {
            StartCoroutine(RecruitRoutine());
        }
    }

    private IEnumerator RecruitRoutine()
    {
        isProcessing = true;
        int targetStackCount = price / 10;

        // 2. 돈 지불 과정 (시각적 연출)
        while (progressStacker.CurrentCount < targetStackCount)
        {
            GameObject money = GameManager.Instance.PlayerMoney.PopItem();
            if (money != null)
            {
                Destroy(money);
                progressStacker.AddItem(); // 진행도 스택 추가
            }
            else
            {
                // 돈이 모자라면 중단 (예외 처리)
                Debug.Log("고용 중 돈이 부족합니다.");
                isProcessing = false;
                yield break;
            }

            yield return new WaitForSeconds(processTime);
        }

        // 3. 고용 완료 및 일꾼 소환
        SpawnPrisoners();

        // 4. 후처리 (진행도 초기화 및 발판 비활성화 또는 재사용 설정)
        progressStacker.PopItems(targetStackCount);

        // 일회성 구매라면 아래 주석 해제
        // gameObject.SetActive(false); 

        isProcessing = false;
    }

    private void SpawnPrisoners()
    {
        for (int i = 0; i < recruitCount; i++)
        {
            GameObject prisoner = Instantiate(prisonerPrefab, spawnPoint.position, spawnPoint.rotation);

            // 소환된 AI에게 광석 정보 할당
            if (prisoner.TryGetComponent<PrisonerMiningAI>(out var ai))
            {
                // Reflection이나 Public 필드를 통해 oreContainer 주입
                // (이전에 만든 PrisonerMiningAI에 oreContainer 필드가 있어야 함)
                SetOreContainer(ai);
            }

            Debug.Log($"일꾼 소환 완료! ({i + 1}/{recruitCount})");
        }
    }

    private void SetOreContainer(PrisonerMiningAI ai)
    {
        // 리플렉션을 사용하거나, PrisonerMiningAI의 oreContainer를 public으로 바꿔서 할당
        var field = typeof(PrisonerMiningAI).GetField("oreContainer",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (field != null)
        {
            field.SetValue(ai, oreContainer);
        }
    }
}