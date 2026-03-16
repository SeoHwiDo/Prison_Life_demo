using UnityEngine;
using System.Collections;

public class SellHandler : MonoBehaviour
{
    private Coroutine taskRoutine;
    private ItemStacker handcuffStacker;
    private ItemStacker moneyStacker;

    void Awake()
    {
        handcuffStacker = gameObject.GetStacker(ItemType.StackHandCuffs);
        moneyStacker = gameObject.GetStacker(ItemType.StackMoney);
    }

    public void StartSell()
    {
        if (taskRoutine != null) return;
        taskRoutine = StartCoroutine(SellProcess());
    }

    public void StopSell()
    {
        if (taskRoutine != null) StopCoroutine(taskRoutine);
        taskRoutine = null;
    }

    private IEnumerator SellProcess()
    {
        while (true)
        {
            // 수갑이 있고, 돈 스택이 가득 차지 않았을 때 실행
            if (handcuffStacker.CurrentCount > 0 && moneyStacker.CanStack)
            {
                GameObject cuff = handcuffStacker.PopItem();
                if (cuff != null)
                {
                    Destroy(cuff); // 수갑 소모
                    moneyStacker.AddItem(); // 돈 획득
                }
            }
            else if (!moneyStacker.CanStack)
            {
                Debug.Log("돈 스택이 가득 차서 판매를 중단합니다.");
                break;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}