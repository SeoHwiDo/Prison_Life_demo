using UnityEngine;
using System.Collections;

public class SellTask : MonoBehaviour, ITaskHandler
{
    [SerializeField] private float interval = 2.0f;
    private Coroutine taskRoutine;

    public void StartTask(GameObject agent)
    {
        taskRoutine = StartCoroutine(Process(agent));
    }

    public void StopTask() { if (taskRoutine != null) StopCoroutine(taskRoutine); taskRoutine = null; }

    private IEnumerator Process(GameObject agent)
    {
        var inputStacker = agent.GetStacker(ItemType.HandCuffs);
        var outputStacker = agent.GetStacker(ItemType.Money);
        while (true)
        {
            // 예: 원석(Ore) 1개를 소모하여 수갑(Handcuffs) 1개 생성
            if (inputStacker != null && outputStacker != null)
            {
                Destroy(inputStacker.PopItem()); // 원석 제거
                outputStacker.AddItem(); // 결과물 추가
            }
            yield return new WaitForSeconds(interval);
        }
    }
}