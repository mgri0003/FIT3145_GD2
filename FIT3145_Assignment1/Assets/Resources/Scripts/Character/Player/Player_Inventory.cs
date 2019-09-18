﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Inventory : MonoBehaviour
{
    //--Variables--//
    private List<Item> m_inventory = new List<Item>();

    //--Methods--//
    public ref List<Item> AccessInventoryList()
    {
        return ref m_inventory;
    }

    public void AddItemToInventory(Item newItem)
    {
        m_inventory.Add(newItem);

        //move items to inventory zone
        newItem.MoveItemToInventoryZone();
    }

    public void AddItemsToInventory(List<Item> newItems)
    {
        //move newly picked up items to inventory zone
        for(int i = 0; i < newItems.Count; ++i)
        {
            AddItemToInventory(newItems[i]);
        }
    }

    public void RemoveItemFromInventory(in Item go)
    {
        m_inventory.Remove(go);
    }

    public void RemoveAllItemsFromInventory()
    {
        m_inventory.Clear();
    }

    public void DropAllItemsInInventory()
    {
        foreach(Item item in AccessInventoryList())
        {
            item.MoveItemToLocation(transform.position + new Vector3(Random.Range(-2,2),0, Random.Range(-2, 2)));
        }

        RemoveAllItemsFromInventory();
    }
}
