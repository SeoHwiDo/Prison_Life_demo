using UnityEngine;
using System.Collections;

public class MineTask : MonoBehaviour, ITaskHandler  
{
    private Coroutine taskRoutine;

    public void StartTask(GameObject agent)
    {
        if (taskRoutine != null) return;
        taskRoutine = StartCoroutine(Process(agent));
    }

    public void StopTask()
    {
        if (taskRoutine != null) { 
            StopCoroutine(taskRoutine); 
        }
        taskRoutine = null;
    }

    private IEnumerator Process(GameObject agent)
    {
        var stacker = agent.GetStacker(ItemType.Ore);
        var oreReSpawn = GetComponent<OreReSpawn>();
        var equipmentManager = agent.GetComponent<EquipmentManager>();

        while (true)
        {
            if (stacker != null && stacker.CanStack)
            {
                oreReSpawn?.Mine();
                stacker.AddItem();
            }
            yield return new WaitForSeconds(equipmentManager.GetMineInterval());
        }
    }
}