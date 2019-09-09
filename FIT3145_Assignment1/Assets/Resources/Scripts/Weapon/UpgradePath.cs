﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 649

[System.Serializable]
public struct UpgradeSegment
{
    [SerializeField] public EWeaponStat StatToImprove;
    [SerializeField] public float Value;
    [SerializeField] public float ScrapCost;
    [SerializeField] public bool SlotUnlock;
}

[System.Serializable]
public struct UpgradePath
{
    //--Variables--//
    Weapon_Base m_parentWeapon;
    private uint m_currentUpgradeIndex;
    [SerializeField] private UpgradeSegment[] m_upgradesSegments;

    //--Methods--//
    public uint GetCurrentUpgradeIndex() { return m_currentUpgradeIndex; }

    public void SetParentWeapon(Weapon_Base newWeapon)
    {
        m_parentWeapon = newWeapon;
    }

    public void Upgrade()
    {
        if (CanUpgrade())
        {
            UpgradeSegment miniUpgrade = m_upgradesSegments[m_currentUpgradeIndex];

            if(miniUpgrade.StatToImprove != EWeaponStat.MAX)
            {
                m_parentWeapon.AccessWeaponStat(miniUpgrade.StatToImprove).AddCurrent(miniUpgrade.Value);

                //hacky way of fixing up ammo if clip size is modified
                if(m_parentWeapon.GetWeaponType() == EWeaponType.RANGED && miniUpgrade.StatToImprove == EWeaponStat.RANGED_CLIP_SIZE)
                {
                    ((Weapon_Ranged)(m_parentWeapon)).ResetAmmo();
                }
            }

            if(miniUpgrade.SlotUnlock)
            {
                m_parentWeapon.IncreaseUpgradeLimit();
            }

            ++m_currentUpgradeIndex;
        }
    }

    public bool CanUpgrade()
    {
        return m_currentUpgradeIndex < m_upgradesSegments.Length;
    }

    public uint GetUpgradeSegmentCount()
    {
        return (uint)m_upgradesSegments.Length;
    }

    public UpgradeSegment[] GetUpgradeSegments()
    {
        return m_upgradesSegments;
    }
}
