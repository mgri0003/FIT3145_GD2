using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EWeapon_Type
{
    MELEE,
    RANGED,
    NONE,
    MAX
}
public abstract class Weapon_Base : MonoBehaviour
{
    //--Variables--
    [SerializeField] private string m_name = "Default_Weapon";
    [SerializeField] private float m_damage = 1;
    [SerializeField] private EWeapon_Type m_weaponType = EWeapon_Type.NONE;

    [SerializeField] private Rigidbody m_physicsRigidbody = null;
    [SerializeField] private BoxCollider m_physicsCollider = null;

    //--methods--
    public abstract bool Use();


    //Getters
    public string GetWeaponName() { return m_name; }
    public float GetWeaponDamage() { return m_damage; }
    public EWeapon_Type GetWeaponType() { return m_weaponType; }

    public void SetPhysicsActive(bool enablePhysics)
    {
        Debug.Assert(m_physicsRigidbody, "Missing Rigidbody!?!?");
        Debug.Assert(m_physicsCollider, "Missing Collider!?!?");

        if(enablePhysics)
        {
            if(m_physicsRigidbody)
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
