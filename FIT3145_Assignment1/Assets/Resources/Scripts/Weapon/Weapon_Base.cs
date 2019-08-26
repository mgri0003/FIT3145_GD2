using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EWeaponType
{
    MELEE,
    RANGED,
    NONE,
    MAX
}

public enum EWeaponStat
{
    DAMAGE,
    MAX
}

public abstract class Weapon_Base : MonoBehaviour
{
    //--Variables--
    [SerializeField] private string m_name = "Default_Weapon";
    [SerializeField] protected Stat[] m_weaponBaseStats = new Stat[(int)EWeaponStat.MAX];
    [SerializeField] private EWeaponType m_weaponType = EWeaponType.NONE;

    [SerializeField] private Rigidbody m_physicsRigidbody = null;
    [SerializeField] private BoxCollider m_physicsCollider = null;

    //--methods--
    public abstract bool Use();

    //Getters
    public string GetWeaponName() { return m_name; }
    public EWeaponType GetWeaponType() { return m_weaponType; }
    public ref Stat AccessWeaponStat(int statIndex)
    {
        Debug.Assert(statIndex >= 0 && statIndex < m_weaponBaseStats.Length, "Incorrect Stat Index");
        return ref m_weaponBaseStats[statIndex];
    }

    protected virtual void InitWeaponStats()
    {
        m_weaponBaseStats[(int)EWeaponStat.DAMAGE].SetNameAndDefaultParams("Damage");
    }

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
