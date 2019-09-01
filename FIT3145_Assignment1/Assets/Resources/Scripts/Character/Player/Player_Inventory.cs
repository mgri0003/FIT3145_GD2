using System.Collections;
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
        newItem.transform.position = GamePlayManager.Instance.GetInventoryZonePosition() + new Vector3(0, 5, 0);

        //disable item physics
        if (newItem.GetComponent<Item>())
        {
            newItem.GetComponent<Item>().SetPhysicsActive(false);
        }
    }

    public void AddItemsToInventory(List<Item> newItems)
    {
        //move newly picked up items to inventory zone
        for(int i = 0; i < newItems.Count; ++i)
        {
            AddItemToInventory(newItems[i]);
        }
    }

    public void RemoveItemFromInventory(Item go)
    {
        m_inventory.Remove(go);
    }
}
