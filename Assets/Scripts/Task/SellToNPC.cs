using UnityEngine;

public class SellToNPC : MonoBehaviour
{
    // 작업자 존재 여부
    public NPCSellHandler CurrentWorker { get; private set; }
    private void OnTriggerEnter(Collider other)
    {
        // 1. 태그 대신 컴포넌트로 작업자(Player 또는 Agent) 감지
        // 작업자가 작업을 수행할 수 있는 핸들러를 가지고 있는지 확인합니다.
        if (other.TryGetComponent<NPCSellHandler>(out var handler))
        {
            // 2. 감지된 핸들러를 매니저에게 전달
            CurrentWorker= handler;
            Debug.Log($"[SellZone] 작업자 감지됨: {other.name}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<NPCSellHandler>(out var handler))
        {
            // 3. 작업자가 영역을 나가면 매니저의 작업자 참조를 제거
            CurrentWorker= null;
            Debug.Log($"[SellZone] 작업자 이탈: {other.name}");
        }
    }
}