using UnityEngine;
using System.Collections;
using ReadOnlyDrawer;

public class FactoryHandler : MonoBehaviour
{
    private Coroutine inputRoutine;
    private Coroutine outputRoutine;

    [SerializeField, ReadOnly] private ItemStacker oreStacker;
    [SerializeField, ReadOnly] private ItemStacker agentHandcuffStacker;

    void Awake()
    {
        oreStacker = gameObject.GetStacker(ItemType.StackOre);
        agentHandcuffStacker = gameObject.GetStacker(ItemType.StackHandCuffs);
    }

    // 공장 입력 (Ore -> Factory Output Stacker)
    public void StartInput(ItemStacker factoryOutput)
    {
        if (inputRoutine != null) return;
        inputRoutine = StartCoroutine(InputProcess(factoryOutput));
    }

    // 공장 출력 (Factory Output Stacker -> Agent)
    public void StartOutput(ItemStacker factoryOutput)
    {
        if (outputRoutine != null) return;
        outputRoutine = StartCoroutine(OutputProcess(factoryOutput));
    }

    public void StopInput()
    {
        if (inputRoutine != null) StopCoroutine(inputRoutine);
        inputRoutine = null;
    }
    public void StopOutput()
    {
        if (outputRoutine != null) StopCoroutine(outputRoutine);
        outputRoutine = null;
    }

    private IEnumerator InputProcess(ItemStacker factoryInput)
    {   
        while (true)
        {
            Debug.Log($"[Factory Input] Ore Count: {oreStacker.CurrentCount}, Factory Output Can Stack: {factoryInput.CanStack}");
            if (oreStacker.CurrentCount > 0 && factoryInput.CanStack)
            {
                GameObject ore = oreStacker.PopItem();
                if (ore != null)
                {
                    Destroy(ore);
                    factoryInput.AddItem(); // 공장Stacker로 이동
                }
            }
            yield return new WaitForSeconds(1.0f);
        }
    }

    private IEnumerator OutputProcess(ItemStacker factoryOutput)
    {
        while (true)
        {
            if (factoryOutput.CurrentCount > 0 && agentHandcuffStacker.CanStack)
            {
                GameObject cuff = factoryOutput.PopItem();
                if (cuff != null)
                {
                    Destroy(cuff); // 공장 쪽 오브젝트 제거
                    agentHandcuffStacker.AddItem(); // 에이전트 쪽으로 이동
                }
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}