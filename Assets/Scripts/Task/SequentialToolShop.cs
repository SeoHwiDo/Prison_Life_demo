
using UnityEngine;
using System.Collections;

public class SequentialToolShop : MonoBehaviour
{
    [Header("Purchase Settings")]
    [SerializeField] private ToolType toolToGrant; // 이 발판이 부여할 장비
    [SerializeField] private int price;            // 가격
    [SerializeField] private ToolType requiredTool;    // 구매를 위해 필요한 '이전' 단계 (0, 1, 2...)
    [SerializeField] private ItemStacker outputStacker; // 가공 결과물을 생성할 스태커
    private float processTime = 0.2f;
    private bool isProcessing = false;
    public bool IsProcessing => isProcessing;
    private void OnTriggerEnter(Collider other)
    {
        // 1. 플레이어 돈 확인
        if (GameManager.Instance.PlayerMoney.CurrentCount*10 >= price)
        {

            // 2. 장비 관리자 확인
            Debug.Log(GameManager.Instance.PlayerMoney.CurrentCount * 10);
            if (GameManager.Instance.PlayerEquip.CurrentTool== requiredTool)
            {
               StartCoroutine(ProcessRoutine());
            }
        }
    }
    private IEnumerator ProcessRoutine()
    {
        isProcessing=true;
        while (outputStacker.CurrentCount<=price/10)
        {
            GameObject money = GameManager.Instance.PlayerMoney.PopItem();
            if (money != null)
            {
                Destroy(money); // 혹은 가공 중인 시각적 연출을 위해 기계 안으로 이동시킬 수 있음
            }

            // 3. 움직임 애니메이션 (선택 사항 - 예시에서는 단순 대기)
            yield return new WaitForSeconds(processTime);

            // 4. 결과물 생성 (출력 스태커에 추가)
            outputStacker.AddItem();

        }
        if(outputStacker.CurrentCount<price/10)
        {
            Debug.Log("돈이 부족합니다.");
            int remain = outputStacker.CurrentCount;
            GameManager.Instance.PlayerMoney.AddItems(remain);
            outputStacker.PopItems(remain);
            yield break;
        }
        else
        {
            GameManager.Instance.PlayerEquip.SetToolMultiplier(toolToGrant);
            Debug.Log($"{toolToGrant} 구매 완료! 다음 단계 발판 활성화 대기.");
            // 구매 후 이 발판은 더 이상 필요 없으므로 비활성화
            gameObject.SetActive(false);
        }
        isProcessing=false;
    }

    public int GetPrice() => price;
    public ToolType GetRequiredTool() => requiredTool;
}