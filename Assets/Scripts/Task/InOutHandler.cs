using UnityEngine;
using System.Collections;
using ReadOnlyDrawer;

public abstract class InOutHandler : MonoBehaviour
{
    private Coroutine inputRoutine;
    private Coroutine outputRoutine;

    // 자식 클래스에서 접근할 수 있도록 protected로 설정
    [SerializeField] protected ItemStacker agentInputSource; // 에이전트가 줄 아이템 스택
    [SerializeField] protected ItemStacker agentOutputSink;  // 에이전트가 받을 아이템 스택

    [Header("Settings")]
    [SerializeField] protected float inputInterval = 1.0f;
    [SerializeField] protected float outputInterval = 0.5f;

    // 공통 입력 시작 로직
    public void StartInput(ItemStacker targetStacker)
    {
        if (inputRoutine != null) return;
        inputRoutine = StartCoroutine(InputProcess(targetStacker));
    }

    // 공통 출력 시작 로직
    public void StartOutput(ItemStacker targetStacker)
    {
        if (outputRoutine != null) return;
        outputRoutine = StartCoroutine(OutputProcess(targetStacker));
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

    protected virtual IEnumerator InputProcess(ItemStacker targetStacker)
    {
        while (true)
        {
            if (agentInputSource != null && agentInputSource.CurrentCount > 0 && targetStacker.CanStack)
            {
                var items = agentInputSource.PopItems(agentInputSource.CurrentCount);
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        Destroy(item);
                        targetStacker.AddItem();
                    }
                        
                }
            }
            yield return new WaitForSeconds(inputInterval);
        }
    }

    protected virtual IEnumerator OutputProcess(ItemStacker targetStacker)
    {
        while (true)
        {
            if (agentOutputSink != null && targetStacker.CurrentCount > 0 && agentOutputSink.CanStack)
            {
                var items = targetStacker.PopItems(targetStacker.CurrentCount);
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        Destroy(item);
                    agentOutputSink.AddItem();
                    }

                }
            }
            yield return new WaitForSeconds(outputInterval);
        }
    }
}