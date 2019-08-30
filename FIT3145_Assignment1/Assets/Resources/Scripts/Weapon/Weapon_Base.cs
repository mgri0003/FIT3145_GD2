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

public abstract class Weapon_Base : Item
{
    //--Variables--
    [SerializeField] protected Stat[] m_weaponBaseStats = new Stat[(int)EWeaponStat.MAX];
    [SerializeField] private EWeaponType m_weaponType = EWeaponType.NONE;

    //--methods--
    public abstract bool Use();

    //Getters
    public EWeaponType GetWeaponType() { return m_weaponType; }
    public ref Stat AccessWeaponStat(int statIndex)
    {
        Debug.Assert(statIndex >= 0 && statIndex < m_weaponBaseStats.Length, "Incorrect Stat Index");
        return ref m_weaponBaseStats[statIndex];
    }

    protected virtual void Constructor_InitWeaponStats()
    {
        m_weaponBaseStats[(int)EWeaponStat.DAMAGE].SetNameAndDefaultParams("Damage");
    }
}
