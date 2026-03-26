public class FactoryHandler : InOutHandler
{
    void Awake()
    {
        if (agentInputSource == null)
        {
            agentInputSource = gameObject.GetStacker(ItemType.StackOre);
        }
        if (agentOutputSink == null)
        {
            agentOutputSink = gameObject.GetStacker(ItemType.StackHandCuffs);
        }
    }
}