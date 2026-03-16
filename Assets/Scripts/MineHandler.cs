using UnityEngine;
using System.Collections;

public class MineHandler : MonoBehaviour
{
    private Coroutine taskRoutine;
    private EquipmentManager equipment;
    private ItemStacker oreStacker;

    void Awake()
    {
        equipment = GetComponent<EquipmentManager>();
        // 확장 메서드를 사용하여 Ore 타입의 스태커 참조
        oreStacker = gameObject.GetStacker(ItemType.StackOre);
    }

    public void StartMining(OreReSpawn targetOre)
    {
        if (taskRoutine != null) return;
        taskRoutine = StartCoroutine(MiningProcess(targetOre));
    }

    public void StopMining()
    {
        if (taskRoutine != null) StopCoroutine(taskRoutine);
        taskRoutine = null;
    }

    private IEnumerator MiningProcess(OreReSpawn targetOre)
    {
        while (true)
        {
            // 1. 채굴 동작 실행 (스택 가득 참 여부와 상관없이 무조건 실행)
            // 이를 통해 광석 오브젝트의 메시 제거/애니메이션이 작동합니다.
            targetOre.Mine();

            // 2. 아이템 획득 판단 (스택에 자리가 있을 때만 아이템 추가)
            if (oreStacker != null && oreStacker.CanStack)
            {
                oreStacker.AddItem();
            }
            else
            {
                // 디버그용 (필요 시 제거 가능)
                Debug.Log($"{gameObject.name}의 인벤토리가 가득 찼지만 채굴은 계속합니다.");
            }

            // 3. 장비 매니저로부터 실시간 속도를 가져와 대기
            // 작업 리듬(Interval)을 유지하기 위해 조건문 밖에서 실행합니다.
            float currentInterval = equipment ? equipment.GetMineInterval() : 1.0f;
            yield return new WaitForSeconds(currentInterval);
        }
    }
}