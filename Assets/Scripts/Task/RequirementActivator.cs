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
    [SerializeField] private GameObject targetObject; // 활성화/비활성화할 대상

    [Header("Conditions")]
    [SerializeField] private ActivationCondition condition;
    [SerializeField] private int moneyThreshold;
    [SerializeField] private ToolType requiredTool;

    private ItemStacker playerMoney; // 플레이어 돈 (UI 연동용)
    private EquipmentManager equipmentManager;
    private void Start()
    {
        playerMoney=GameManager.Instance.PlayerMoney;
        equipmentManager=GameManager.Instance.PlayerEquip;
    }
    private void Update()
    {
        if (targetObject == null) return;

        bool isConditionMet = CheckCondition();

        // 조건 충족 시 오브젝트 활성화, 미충족 시 비활성화
        if (isConditionMet)
        {
            targetObject.SetActive(isConditionMet);
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