using UnityEngine;
using System.Collections.Generic;

public class PlayerStackManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform stackPivot; // 등의 시작점
    [SerializeField] private List<ItemStacker> managedStackers;
    [SerializeField] private float stackPadding = 0.1f;

    public void RefreshStackLayout()
    {
        if (managedStackers == null || managedStackers.Count == 0) return;

        // 등에 가장 가까운 Z=0 지점부터 시작
        Vector3 currentOffset = Vector3.zero;

        foreach (var stacker in managedStackers)
        {
            if (stacker.CurrentCount > 0)
            {
                // 1. 해당 스태커 뭉치를 현재 Z 오프셋 위치에 배치
                stacker.transform.localPosition = currentOffset;
                

                // 실제 모델의 Z 크기(StackOffsetZ)만큼 뒤로 밀어냄
                float totalDepth = stacker.StackOffset + stackPadding;

                // Z축 방향으로 오프셋 누적
                currentOffset -= new Vector3(0, 0, totalDepth);

                stacker.gameObject.SetActive(true);
            }
            else
            {
                stacker.transform.localPosition = Vector3.zero;
                stacker.gameObject.SetActive(false);
            }
        }
    }
}