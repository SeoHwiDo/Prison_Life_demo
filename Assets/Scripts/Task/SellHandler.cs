public class SellHandler : InOutHandler
{
    void Awake()
    {
        // 부모의 변수에 에이전트의 스태커를 할당
        if (agentInputSource == null)
        {
            agentInputSource = gameObject.GetStacker(ItemType.StackHandCuffs);

        }
        if(agentOutputSink == null)
        {
            agentOutputSink = gameObject.GetStacker(ItemType.StackMoney);
        }
        
    }
}