using UnityEngine;
using TMPro; // TextMeshPro 사용 권장
using System.Collections;

public class StackerFeedback : MonoBehaviour
{
    [SerializeField] private GameObject maxIndicator; // "MAX" 텍스트 오브젝트 (Canvas)
    [SerializeField] private float displayDuration = 1.0f; // 표시될 시간
    [SerializeField] private float moveUpSpeed = 0.5f; // 위로 떠오르는 속도

    private Coroutine feedbackRoutine;
    private Vector3 initialPosition;

    void Awake()
    {
        if (maxIndicator != null)
        {
            initialPosition = maxIndicator.transform.localPosition;
            maxIndicator.SetActive(false);
        }
    }

    public void ShowMax()
    {
        if (maxIndicator == null) return;

        // 이미 실행 중이라면 중지하고 새로 시작 (갱신)
        if (feedbackRoutine != null) StopCoroutine(feedbackRoutine);
        feedbackRoutine = StartCoroutine(DisplayRoutine());
    }

    private IEnumerator DisplayRoutine()
    {
        maxIndicator.SetActive(true);
        maxIndicator.transform.localPosition = initialPosition;

        float elapsed = 0f;
        while (elapsed < displayDuration)
        {
            // 위로 살짝 떠오르는 연출
            maxIndicator.transform.localPosition += Vector3.up * moveUpSpeed * Time.deltaTime;

            // (선택 사항) 투명도 조절 로직을 여기에 추가할 수 있습니다.

            elapsed += Time.deltaTime;
            yield return null;
        }

        maxIndicator.SetActive(false);
        maxIndicator.transform.localPosition = initialPosition;
        feedbackRoutine = null;
    }
}