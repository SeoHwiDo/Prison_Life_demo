using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handle;
    [SerializeField] private float range = 100f;

    public Vector2 InputDirection { get; private set; } = Vector2.zero;

    private Vector2 startPos;

    void Start()
    {
        // 시작 시 조이스틱 숨기기
        background.gameObject.SetActive(false);
    }

    // 터치 시작 시 호출
    public void OnPointerDown(PointerEventData eventData)
    {
        background.gameObject.SetActive(true);

        // 터치한 위치로 조이스틱 배경 이동
        background.position = eventData.position;
        handle.anchoredPosition = Vector2.zero;
        startPos = eventData.position;
    }

    // 드래그 중 호출
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 direction = eventData.position - startPos;

        // 조이스틱 범위 내로 핸들 위치 제한
        float distance = Mathf.Min(direction.magnitude, range);
        InputDirection = direction.normalized * (distance / range);

        handle.anchoredPosition = InputDirection * range;
    }

    // 터치 종료 시 호출
    public void OnPointerUp(PointerEventData eventData)
    {
        InputDirection = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;

        // 조이스틱 숨기기
        background.gameObject.SetActive(false);
    }
}