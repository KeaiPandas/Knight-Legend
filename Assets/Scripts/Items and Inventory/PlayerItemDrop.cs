using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using static UnityEditor.Progress;

public class PlayerItemDrop : ItemDrop
{
    [Header("Player's drop")]
    [SerializeField] private float chanceToLooseItems;
    [SerializeField] private float chanceToLooseMaterials;

    public override void GenerateDrop()
    {
        Inventory inventory = Inventory.instance;

        List<InventoryItem> itemToUnequips = new List<InventoryItem>();
        List<InventoryItem> materialsToLoose = new List<InventoryItem>();

        foreach (InventoryItem item in inventory.GetEquipmentList())
        {
            if (Random.Range(0, 100) <= chanceToLooseItems)
            {
                DropItem(item.data);
                itemToUnequips.Add(item);
            }
        }

        for (int i = 0; i < itemToUnequips.Count; i++)
        {
            inventory.UnequipItem(itemToUnequips[i].data as ItemDataEquipment);
        }

        foreach (InventoryItem item in inventory.GetStashList())
        {
            if (Random.Range(0, 100) <= chanceToLooseMaterials)
            {
                DropItem(item.data);
                materialsToLoose.Add(item);
            }
        }

        for (int i = 0; i < materialsToLoose.Count; i++)
        {
            inventory.RemoveItem(materialsToLoose[i].data);
        }

    }
}
