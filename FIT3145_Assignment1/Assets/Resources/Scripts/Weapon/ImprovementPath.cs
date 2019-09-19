using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 649

[System.Serializable]
public struct ImprovementSegment
{
    [SerializeField] public EWeaponStat StatToImprove;
    [SerializeField] public float Value;
    [SerializeField] public uint ScrapCost;
    [SerializeField] public bool UpgradeSlotIncrease;
}

[System.Serializable]
public struct ImprovementPath
{
    //--Variables--//
    Weapon_Base m_parentWeapon;
    private uint m_currentImprovementIndex;
    [SerializeField] private ImprovementSegment[] m_improvementSegments;

    //--Methods--//
    public uint GetCurrentImprovementIndex() { return m_currentImprovementIndex; }

    public void SetParentWeapon(Weapon_Base newWeapon)
    {
        m_parentWeapon = newWeapon;
    }

    public void Improve()
    {
        if (CanImprove())
        {
            ImprovementSegment improvement = m_improvementSegments[m_currentImprovementIndex];

            if(improvement.StatToImprove != EWeaponStat.MAX)
            {
                m_parentWeapon.AccessWeaponStat(improvement.StatToImprove).AddCurrent(improvement.Value);

                //hacky way of fixing up ammo if clip size is modified
                if(m_parentWeapon.GetWeaponType() == EWeaponType.RANGED && improvement.StatToImprove == EWeaponStat.RANGED_CLIP_SIZE)
                {
                    ((Weapon_Ranged)(m_parentWeapon)).ResetAmmo();
                }
            }

            if(improvement.UpgradeSlotIncrease)
            {
                m_parentWeapon.IncreaseUpgradeLimit();
            }

            ++m_currentImprovementIndex;
        }
    }

    public bool CanImprove()
    {
        return m_currentImprovementIndex < m_improvementSegments.Length;
    }

    public uint GetImprovementSegmentCount()
    {
        return (uint)m_improvementSegments.Length;
    }

    public ImprovementSegment[] GetImprovementSegments()
    {
        return m_improvementSegments;
    }

    public ImprovementSegment GetNextAvailableImprovementSegment()
    {
        Debug.Assert(!IsFullyImproved(), "Cannot get the latest Improvement Segment, no Improvements left");

        if (!IsFullyImproved())
        {
            return m_improvementSegments[m_currentImprovementIndex];
        }

        return new ImprovementSegment();
    }

    public bool IsFullyImproved()
    {
        return m_currentImprovementIndex == m_improvementSegments.Length;
    }
}
