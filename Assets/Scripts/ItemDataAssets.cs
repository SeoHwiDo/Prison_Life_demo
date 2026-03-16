using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ItemDataAsset", menuName = "ScriptableObjects/ItemDataAsset")]
public class ItemDataAsset : ScriptableObject
{
    [System.Serializable]
    public struct ItemMapping
    {
        public ItemType type;
        public GameObject prefab;
    }

    public List<ItemMapping> itemMappings;

    public GameObject GetPrefab(ItemType type)
    {
        var mapping = itemMappings.Find(m => m.type == type);
        return mapping.prefab;
    }
}