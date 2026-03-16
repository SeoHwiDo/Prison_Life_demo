using UnityEngine;
using System.Collections;

public class NPCSellHandler : MonoBehaviour
{
    private Coroutine sellRoutine;

  
    public void StartSellTask(NPCController targetNpc, ItemStacker handcuffSource, ItemStacker moneySink)
    {
        if (sellRoutine != null) return;
        if (targetNpc.IsProcessed) return;
        sellRoutine = StartCoroutine(SellProcess(targetNpc,handcuffSource, moneySink));
    }

    public void StopSellTask()
    {
        if (sellRoutine != null) StopCoroutine(sellRoutine);
        sellRoutine = null;
    }

    private IEnumerator SellProcess(NPCController targetNpc, ItemStacker handcuffSource, ItemStacker moneySink)
    {
        // 수갑이 생길 때까지 대기 혹은 반복 체크
        while (!targetNpc.IsProcessed)
        {
            if (handcuffSource.CurrentCount > 0 && moneySink.CanStack)
            {
                // 1. 작업 수행
                GameObject cuff = handcuffSource.PopItem();
                if (cuff != null) Destroy(cuff);

                moneySink.AddItem();

                // 2. NPC 상태 변경
                targetNpc.MarkAsProcessed();
                Debug.Log($"{gameObject.name}: 판매 및 죄수 전환 완료");

                sellRoutine = null;
                yield break; // 작업 완료 후 코루틴 종료
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}