using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Inventory : MonoBehaviour
{
    //--Variables--//
    private List<GameObject> m_inventory = new List<GameObject>();

    //--Methods--//
    public ref List<GameObject> AccessInventoryList()
    {
        return ref m_inventory;
    }

    public void AddItemToInventory(GameObject newItem)
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

    public void AddItemsToInventory(List<GameObject> newItems)
    {
        //move newly picked up items to inventory zone
        for(int i = 0; i < newItems.Count; ++i)
        {
            AddItemToInventory(newItems[i]);
        }
    }

    public void RemoveItemFromInventory(GameObject go)
    {
        m_inventory.Remove(go);
    }
}
