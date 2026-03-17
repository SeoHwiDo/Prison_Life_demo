using UnityEngine;
using UnityEngine.UI; // Image 컴포넌트 사용을 위해 추가
using System.Collections;

public class PoterRecruiter : MonoBehaviour
{
    [Header("Recruit Settings")]
    [SerializeField] private GameObject[] prisoners;
    [SerializeField] private int price = 100;

    [Header("UI & Visuals")]
    [SerializeField] private ItemStacker progressStacker;
    [SerializeField] private Image progressImage;         // 진행도를 표시할 UI 이미지
    [SerializeField] private float processTime = 0.2f;

    private bool isPlayerInside = false;
    private bool isSpawned = false;

    void Start()
    {
        // 시작 시 기존에 저장된 진행도가 있다면 UI에 반영
        UpdateUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isSpawned) return;

        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            StopAllCoroutines();
            StartCoroutine(PaymentRoutine());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
        }
    }

    private IEnumerator PaymentRoutine()
    {
        int targetStackCount = price / 10;

        while (isPlayerInside && progressStacker.CurrentCount < targetStackCount)
        {
            if (GameManager.Instance.PlayerMoney.CurrentCount > 0)
            {
                GameObject money = GameManager.Instance.PlayerMoney.PopItem();
                if (money != null)
                {
                    Destroy(money);
                    progressStacker.AddItem();

                    // 돈이 추가될 때마다 UI 업데이트
                    UpdateUI();
                }
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            yield return new WaitForSeconds(processTime);
        }

        if (progressStacker.CurrentCount >= targetStackCount)
        {
            CompleteRecruit();
        }
    }

    // UI의 fillAmount를 계산하여 업데이트하는 메서드
    private void UpdateUI()
    {
        if (progressImage != null)
        {
            float targetStackCount = price / 10f;
            // 수식: 현재 쌓인 개수 / 목표 개수
            progressImage.fillAmount = (float)progressStacker.CurrentCount / targetStackCount;
        }
    }

    private void CompleteRecruit()
    {
        isSpawned = true;
        // 완료 시 게이지를 꽉 채움
        if (progressImage != null) progressImage.fillAmount = 1f;

        SpawnPrisoners();
        gameObject.SetActive(false);
    }

    private void SpawnPrisoners()
    {
        foreach (var prisoner in prisoners)
        {
            if (prisoner != null) prisoner.SetActive(true);
        }

    }
}