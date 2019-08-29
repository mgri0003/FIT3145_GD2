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

    public void AddItemToInventory(GameObject newItem, bool forceClearColliding = true)
    {
        m_inventory.Add(newItem);

        //move items to inventory zone
        newItem.transform.position = GamePlayManager.Instance.GetInventoryZonePosition() + new Vector3(0, 5, 0);

        //disable item physics
        if (newItem.GetComponent<Weapon_Base>())
        {
            newItem.GetComponent<Weapon_Base>().SetPhysicsActive(false);
        }

        if(forceClearColliding)
        {
            GetComponent<Player_Core>().m_playerItemPickupArea.ClearCollidingObjects();
        }
    }

    public void AddItemsToInventory(List<GameObject> newItems, bool forceClearColliding = true)
    {
        //move newly picked up items to inventory zone
        foreach (GameObject go in m_inventory)
        {
            AddItemToInventory(go, forceClearColliding);
        }
    }

    public void RemoveItemFromInventory(GameObject go)
    {
        m_inventory.Remove(go);
    }
}
