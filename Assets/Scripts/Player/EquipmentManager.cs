using System.Collections.Generic;
using UnityEngine;
public enum ToolType
{
    Pickaxe=1,
    Drill,
    DrillVehicle,
    // 향후 추가 장비 유형
}

public class EquipmentManager : MonoBehaviour
{
    [SerializeField] private GameObject toolObj;
    [SerializeField] private float baseMineInterval = 2.0f;
    [SerializeField] private ToolType tool;// 장비에 따라 조절
    public ToolType CurrentTool => tool;
    private bool isInMineArea = false;
    private readonly int hashHasObj= Animator.StringToHash("HasObj");
    private Animator myAnimator;

    void Awake()
    {
        myAnimator = GetComponentInParent<Animator>(); // 부모나 자신에게서 애니메이터 확보
    }
    public float GetMineInterval()
    {
        return baseMineInterval / (float)tool;
    }

    public void SetToolMultiplier(ToolType SwapTool)
    {
        tool = SwapTool;
        GameManager.Instance.SetMaxCapacity((int)tool * 10); // 예시: 각 도구마다 최대 수용량 증가

    }
    public void SetMineAreaPresence(bool isInside)
    {
        isInMineArea = isInside;
        UpdateToolActivation();
    }
    private void UpdateToolActivation()
    {
        if (toolObj != null)
        {
            toolObj.SetActive(isInMineArea);
        }

        if (myAnimator != null)
        {
            myAnimator.SetBool(hashHasObj, isInMineArea);
        }
    }
}
