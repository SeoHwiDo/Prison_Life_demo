using UnityEngine;

public enum ActivationCondition
{
    MoneyHigherThan,    // 돈이 일정액 이상일 때
    ToolLevelExactly,   // 특정 장비 단계일 때
    MoneyAndToolLevel   // 돈과 장비 단계 모두 충족할 때
}

public class RequirementActivator : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private GameObject targetObject;

    [Header("Conditions")]
    [SerializeField] private ActivationCondition condition;
    [SerializeField] private int moneyThreshold;
    [SerializeField] private ToolType requiredTool;

    private ItemStacker playerMoney;
    private EquipmentManager equipmentManager;

    // [추가] 한 번이라도 활성화되었는지 체크하는 플래그
    private bool hasBeenActivated = false;

    private void Start()
    {
        playerMoney = GameManager.Instance.PlayerMoney;
        equipmentManager = GameManager.Instance.PlayerEquip;

        // 시작 시점에 이미 켜져 있다면 활성화된 것으로 간주할지 선택 (기본은 false)
        if (targetObject != null && targetObject.activeSelf)
        {
            hasBeenActivated = true;
        }
    }

    private void Update()
    {
        if (targetObject == null) return;

        // 1. 이미 한 번 활성화된 적이 있는데, 지금 오브젝트가 꺼져 있다면?
        if (hasBeenActivated && !targetObject.activeSelf)
        {
            // 이 스크립트를 비활성화하여 다시는 Update가 실행되지 않게 함
            this.enabled = false;
            return;
        }

        // 2. 아직 활성화되지 않은 상태에서 조건을 체크
        if (!hasBeenActivated && CheckCondition())
        {
            targetObject.SetActive(true);
            hasBeenActivated = true; // 활성화됨을 기록
        }
    }

    private bool CheckCondition()
    {
        switch (condition)
        {
            case ActivationCondition.MoneyHigherThan:
                return playerMoney.CurrentCount * 10 >= moneyThreshold;

            case ActivationCondition.ToolLevelExactly:
                return equipmentManager.CurrentTool == requiredTool;

            case ActivationCondition.MoneyAndToolLevel:
                return playerMoney.CurrentCount * 10 >= moneyThreshold &&
                       equipmentManager.CurrentTool == requiredTool;

            default:
                return false;
        }
    }
}