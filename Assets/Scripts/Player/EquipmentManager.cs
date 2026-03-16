using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    [SerializeField] private float baseMineInterval = 2.0f;
    [SerializeField] private float toolSpeedMultiplier = 1.0f; // 장비에 따라 조절

    // 최종 채굴 속도 반환: $Interval = \frac{Base}{Multiplier}$
    public float GetMineInterval()
    {
        return baseMineInterval / toolSpeedMultiplier;
    }

    // 장비 교체 시 호출할 함수 (예: 인턴 과제 영상의 드릴 차량 탑승 등)
    public void SetToolMultiplier(float multiplier)
    {
        toolSpeedMultiplier = multiplier;
    }
}