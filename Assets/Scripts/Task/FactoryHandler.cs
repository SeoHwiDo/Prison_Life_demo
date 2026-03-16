public class FactoryHandler : InOutHandler
{
    void Awake()
    {
        // 부모의 변수에 에이전트의 스태커를 할당
        agentInputSource = gameObject.GetStacker(ItemType.StackOre);
        agentOutputSink = gameObject.GetStacker(ItemType.StackHandCuffs);
    }
}