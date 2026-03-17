using UnityEngine;
using System.Collections;

public class OreToHandCuffs : MonoBehaviour
{
    [Header("Stacker References")]
    [SerializeField] private ItemStacker inputStacker;  // Ore가 쌓이는 곳 (FactoryInput 구역)
    [SerializeField] private ItemStacker outputStacker; // Handcuffs가 쌓이는 곳 (FactoryOutput 구역)

    [Header("Process Settings")]
    [SerializeField] private float processTime = 1.0f;   // 아이템 1개 가공에 걸리는 시간

    private bool isProcessing = false;

    private void Start()
    {
        // 공장 가동 시작
        StartCoroutine(ProcessRoutine());
    }

    private IEnumerator ProcessRoutine()
    {
        while (true)
        {
            // 1. 입력 칸에 재료가 있고, 출력 칸에 공간이 있는지 확인
            if (inputStacker.CurrentCount > 0 && outputStacker.CanStack)
            {
                isProcessing = true;

                // 2. 재료 소모 (입력 스태커에서 제거)
                GameObject ore = inputStacker.PopItem();
                if (ore != null)
                {
                    Destroy(ore); // 혹은 가공 중인 시각적 연출을 위해 기계 안으로 이동시킬 수 있음
                }


                // 3. 가공 시간 대기
                yield return new WaitForSeconds(processTime);

                // 4. 결과물 생성 (출력 스태커에 추가)
                outputStacker.AddItems(2);

            }
            else
            {
                isProcessing = false;
                // 조건이 맞지 않으면 잠시 대기 후 다시 체크 (CPU 부하 감소)
                yield return new WaitForSeconds(0.5f);
                //break;
            }
        }
    }

    // 외부(UI 등)에서 현재 기계가 작동 중인지 확인할 수 있는 프로퍼티
    public bool IsProcessing => isProcessing;
}