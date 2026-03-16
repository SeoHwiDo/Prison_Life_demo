using UnityEngine;

public class TaskManager : MonoBehaviour
{
    private MineHandler mineHandler;
    private SellHandler sellHandler;
    private FactoryHandler factoryHandler;

    private bool isInMineZone = false;

    void Awake()
    {
        mineHandler = GetComponent<MineHandler>();
        sellHandler = GetComponent<SellHandler>();
        factoryHandler = GetComponent<FactoryHandler>();
    }

    private void OnTriggerEnter(Collider other)
    {
        string tag = other.tag;
        Debug.Log(tag);
        switch (tag)
        {
            case "Mine":
                isInMineZone = true;
                break;

            case "Ore":
                if (isInMineZone && other.TryGetComponent<OreReSpawn>(out var ore))
                    mineHandler?.StartMining(ore);
                break;

            case "SellInput":
                if (other.TryGetComponent<ItemStacker>(out var sInputStack))
                    sellHandler.StartInput(sInputStack);
                break;
            case "SellOutput":
                if (other.TryGetComponent<ItemStacker>(out var sOutputStack))
                    sellHandler.StartOutput(sOutputStack);
                break;

            case "FactoryInput":
                // 구역 오브젝트(공장)에 붙은 Output Stacker를 가져옴 
                if (other.TryGetComponent<ItemStacker>(out var fInputStack))
                    factoryHandler?.StartInput(fInputStack);
                break;

            case "FactoryOutput":
                if (other.TryGetComponent<ItemStacker>(out var fOutputStack))
                    factoryHandler?.StartOutput(fOutputStack);
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        string tag = other.tag;

        switch (tag)
        {
            case "Mine":
                isInMineZone = false;
                mineHandler?.StopMining();
                break;

            case "Ore":
                mineHandler?.StopMining();
                break;

            case "SellInput":
                sellHandler?.StopInput();
                break;
            case "SellOutput":
                sellHandler?.StopOutput();
                break;

            case "FactoryInput":
                factoryHandler?.StopInput();
                break;
            case "FactoryOutput":
                factoryHandler?.StopOutput();
                break;
        }
    }
}