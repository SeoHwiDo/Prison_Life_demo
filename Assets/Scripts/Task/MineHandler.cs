using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ReadOnlyDrawer;

public class MineHandler : MonoBehaviour
{
    private Coroutine taskRoutine;
    [SerializeField] private GameObject Agent;
    [SerializeField] private ItemStacker oreStacker;
    private Animator animator;
    private EquipmentManager equipment;
    private readonly int hashIsOnOre = Animator.StringToHash("IsOnOre");

    // 곡괭이 전용: 범위 내 광석 리스트
    [SerializeField, ReadOnly] private List<OreReSpawn> detectedOres = new List<OreReSpawn>();

    void Awake()
    {
        // Agent가 설정되어 있지 않다면 자신을 할당
        if (Agent == null) Agent = this.gameObject;

        equipment = Agent.GetComponent<EquipmentManager>();
        animator = Agent.GetComponent<Animator>();
    }

    // TaskManager에서 호출
    public void AddOre(OreReSpawn ore)
    {
        // 1. 곡괭이일 때만 리스트에 추가하고 루프 시작
        if (equipment != null && equipment.CurrentTool == ToolType.Pickaxe)
        {
            if (!detectedOres.Contains(ore))
            {
                detectedOres.Add(ore);
            }

            if (taskRoutine == null)
            {
                taskRoutine = StartCoroutine(MiningProcess());
            }
        }
        else
        {
            // 2. 곡괭이가 아닐 때는 리스트에 담지 않고 즉시 실행
            ExecuteImmediateMine(ore);
        }
    }

    public void RemoveOre(OreReSpawn ore)
    {
        if (detectedOres.Contains(ore))
        {
            detectedOres.Remove(ore);
        }
    }

    // 드릴/차량용 즉시 채굴 로직
    public void ExecuteImmediateMine(OreReSpawn ore)
    {
        if (ore != null && ore.IsCanMine)
        {
            ore.Mine();
            if (oreStacker != null)
            {
                oreStacker.AddItem();
            }
        }
    }

    public void StopMining()
    {
        if (taskRoutine != null)
        {
            StopCoroutine(taskRoutine);
            taskRoutine = null;
        }

        if (animator != null)
        {
            animator.SetBool(hashIsOnOre, false);
        }
    }

    private IEnumerator MiningProcess()
    {
        while (true)
        {
            // 리스트 정제
            detectedOres.RemoveAll(ore => ore == null || !ore.IsCanMine);

            if (detectedOres.Count == 0)
            {
                StopMining();
                yield break;
            }

            float interval = equipment ? equipment.GetMineInterval() : 1.0f;

            // 곡괭이 애니메이션 실행
            if (animator != null)
            {
                animator.SetBool(hashIsOnOre, true);
            }

            // 애니메이션 사이클 대기
            yield return new WaitForSeconds(interval);

            // 대기 후 다시 리스트 정제

            if (detectedOres.Count > 0)
            {
                // 애니메이션이 끝나는 시점에 감지된 마지막 광석 채굴
                OreReSpawn target = detectedOres[detectedOres.Count - 1];

                target.Mine();
                if (oreStacker != null)
                {
                    oreStacker.AddItem();
                }

                // 기존 로직 유지: 한 번 휘두른 후 리스트 비움 (혹은 필요에 따라 유지)
                detectedOres.Clear();
            }
            else
            {
                StopMining();
                yield break;
            }
        }
    }
}