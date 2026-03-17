using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class NPCQueueManager : MonoBehaviour
{
    public static NPCQueueManager Instance { get; private set; }

    [Header("Queue Points")]
    [SerializeField] private Transform toSellPointsParent;
    [SerializeField] private Transform toPrisonPointsParent;

    [Header("Global Stackers")]
    [SerializeField] private ItemStacker sellZoneHandcuffStacker;
    [SerializeField] private ItemStacker sellZoneMoneyStacker;
    [SerializeField] private PrisonerManager prisonerManager;
    [SerializeField] private SellToNPC sellToNPC;


    private List<Transform> toSellPoints;
    private List<Transform> toPrisonPoints;

    private List<NPCController> toSellQueue = new List<NPCController>();
    private List<NPCController> toPrisonQueue = new List<NPCController>();

    private NPCSellHandler currentWorker;


   
    void Awake()
    {
        Instance = this;
        toSellPoints = toSellPointsParent.Cast<Transform>().OrderBy(t => t.GetSiblingIndex()).ToList();
        toPrisonPoints = toPrisonPointsParent.Cast<Transform>().OrderBy(t => t.GetSiblingIndex()).ToList();
    }

    public bool CanSpawnInSellQueue() => toSellQueue.Count < toSellPoints.Count;

    public void AddToSellQueue(NPCController npc)
    {
        toSellQueue.Add(npc);
        UpdatePos(toSellQueue, toSellPoints);
    }

    void Update()
    {
        // 1. Sell 지점(Top)에 있는 NPC에게 작업 명령
        if (toSellQueue.Count > 0)
        {
            var topNpc = toSellQueue[0];
         
            if (sellToNPC.CurrentWorker != null&&topNpc.HasReachedDestination() && !topNpc.IsProcessed)
            {
                sellToNPC.CurrentWorker.StartSellTask(topNpc,sellZoneHandcuffStacker, sellZoneMoneyStacker);
            }
            else
                sellToNPC.CurrentWorker?.StopSellTask();
            if (toSellQueue.Count > 0 && toSellQueue[0].IsProcessed && toPrisonQueue.Count < toPrisonPoints.Count)
            {
                var npc = toSellQueue[0]; toSellQueue.RemoveAt(0); toPrisonQueue.Add(npc);
                UpdatePos(toSellQueue, toSellPoints); UpdatePos(toPrisonQueue, toPrisonPoints);
            }

            if (toPrisonQueue.Count > 0 && toPrisonQueue[0].HasReachedDestination() && prisonerManager.CanAddPrisoner())
            {
                var npc = toPrisonQueue[0]; toPrisonQueue.RemoveAt(0);
                prisonerManager.AddPrisonerToArea(npc); UpdatePos(toPrisonQueue, toPrisonPoints);
            }
            
        }
    }
    private void UpdatePos(List<NPCController> q, List<Transform> p)
    {
        for (int i = 0; i < q.Count; i++) q[i].MoveTo(p[i].position);
    }
   
}