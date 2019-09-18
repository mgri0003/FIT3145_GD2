using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EItemType
{
    WEAPON,
    AUGMENT,
    UPGRADE,
    MAX
}

public abstract class Item : MonoBehaviour
{
    //--Variables--//
    [SerializeField] private Rigidbody m_physicsRigidbody = null;
    [SerializeField] private BoxCollider m_physicsCollider = null;

    [SerializeField] private string m_name = "Default_Item";
    [SerializeField] private EItemType m_itemType = EItemType.WEAPON;
    [SerializeField] private Sprite m_itemSprite = null;
    [SerializeField] private string m_itemDescription = "";

    //--Methods--//
    public string GetItemName() { return m_name; }
    public EItemType GetItemType() { return m_itemType; }
    public Sprite GetItemSprite() { return m_itemSprite; }
    public string GetItemDescription() { return m_itemDescription; }
    public abstract string GetItemTypeDescription();

    public void MoveItemToInventoryZone()
    {
        transform.localPosition = Vector3.zero;
        transform.position = GamePlayManager.Instance.GetInventoryZonePosition() + new Vector3(0, 5, 0);
        SetPhysicsActive(false);
    }

    public void MoveItemToLocation(in Vector3 worldPos, bool enablePhysics = true)
    {
        transform.localPosition = Vector3.zero;
        transform.position = worldPos;

        if(enablePhysics)
        {
            SetPhysicsActive(true);
        }
    }

    public void SetPhysicsActive(bool enablePhysics)
    {
        Debug.Assert(m_physicsRigidbody, "Missing Rigidbody!?!?");
        Debug.Assert(m_physicsCollider, "Missing Collider!?!?");

        if (enablePhysics)
        {
            if (m_physicsRigidbody)
            {
                m_physicsRigidbody.useGravity = true;
                m_physicsRigidbody.constraints = RigidbodyConstraints.None;
            }
            if (m_physicsCollider)
            {
                m_physicsCollider.enabled = true;
            }
        }
        else
        {
            if (m_physicsRigidbody)
            {
                m_physicsRigidbody.useGravity = false;
                m_physicsRigidbody.constraints = RigidbodyConstraints.FreezeAll;
            }
            if (m_physicsCollider)
            {
                m_physicsCollider.enabled = false;
            }
        }
    }
}
