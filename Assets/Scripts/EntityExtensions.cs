using UnityEngine;
using System.Linq;

public static class EntityExtensions
{
    public static ItemStacker GetStacker(this GameObject entity, ItemType type)
    {
        // 오브젝트에 붙은 여러 개의 ItemStacker 중 타입이 일치하는 것 반환
        return entity.GetComponents<ItemStacker>().FirstOrDefault(s => s.MyItemType == type);
    }
}