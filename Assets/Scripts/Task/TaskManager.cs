using ReadOnlyDrawer;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    [SerializeField,ReadOnly]private MineHandler mineHandler;
    private SellHandler sellHandler;
    private FactoryHandler factoryHandler;
    private EquipmentManager equipment; // 로컬 장비 관리자 참조 추가
    private bool isInMineZone = false;

    void Awake()
    {
        mineHandler = GetComponent<MineHandler>();
        sellHandler = GetComponent<SellHandler>();
        factoryHandler = GetComponent<FactoryHandler>();
        equipment = GetComponent<EquipmentManager>();
    }
    public void SetInMineZone(bool inZone)
    {
        isInMineZone = inZone;
        if (equipment != null) equipment.SetMineAreaPresence(inZone);

        if (!inZone) mineHandler?.StopMining();
    }
    public void RequestAddOre(OreReSpawn ore)
    {
        if (isInMineZone && ore != null && ore.IsCanMine)
        {
            mineHandler?.AddOre(ore);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        string tag = other.tag;
        Debug.Log(tag);
        switch (tag)
        {
            case "Mine":
                SetInMineZone(true);
                break;

            case "Ore":
                // 특정 광석을 시작하는게 아니라 리스트에 추가함
                if (other.TryGetComponent<OreReSpawn>(out var ore))
                    RequestAddOre(ore);
                break;

            case "SellInput":
                if (other.TryGetComponent<ItemStacker>(out var sInputStack))
                    sellHandler?.StartInput(sInputStack);
                break;
            case "SellOutput":
                if (other.TryGetComponent<ItemStacker>(out var sOutputStack))
                    sellHandler?.StartOutput(sOutputStack);
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
                SetInMineZone(false);
                break;
            case "Ore":
                if (other.TryGetComponent<OreReSpawn>(out var ore))
                    mineHandler?.RemoveOre(ore);
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
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ore") && isInMineZone)
        {
            // 곡괭이가 아닐 때만 작동하게 설계됨
            if (GameManager.Instance.PlayerEquip.CurrentTool != ToolType.Pickaxe)
            {
                if (other.TryGetComponent<OreReSpawn>(out var ore))
                {
                    mineHandler?.AddOre(ore);
                }
            }
        }
    }
}