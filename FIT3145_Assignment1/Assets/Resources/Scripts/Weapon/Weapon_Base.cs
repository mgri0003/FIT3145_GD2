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
    ALL_DAMAGE,
    RANGED_PROJECTILE_SPEED,
    RANGED_CLIP_SIZE,
    RANGED_RELOAD_TIME,
    RANGED_FIRE_RATE_COOLDOWN,

    MAX
}

public abstract class Weapon_Base : Item
{
    //--Variables--
    [SerializeField] protected Stat[] m_weaponStats = new Stat[(int)EWeaponStat.MAX];
    [SerializeField] private EWeaponType m_weaponType = EWeaponType.NONE;
    [SerializeField] private ImprovementPath m_improvementPath;
    protected List<Upgrade> m_currentUpgrades = new List<Upgrade>();
    private uint m_upgradeLimit = 1;
    private const uint m_totalUpgradeLimit = 3;
    [SerializeField] private Transform[] m_upgradeVisualLocations = new Transform[m_totalUpgradeLimit];

    //--methods--
    protected Weapon_Base()
    {
        Constructor_InitWeaponStats();
        m_improvementPath.SetParentWeapon(this);
    }

    protected void Constructor_InitWeaponStats()
    {
        m_weaponStats[(int)EWeaponStat.ALL_DAMAGE].SetNameAndDefaultParams("Damage");
        m_weaponStats[(int)EWeaponStat.RANGED_PROJECTILE_SPEED].SetNameAndDefaultParams("Projectile Speed");
        m_weaponStats[(int)EWeaponStat.RANGED_CLIP_SIZE].SetNameAndDefaultParams("Clip Size");
        m_weaponStats[(int)EWeaponStat.RANGED_RELOAD_TIME].SetNameAndDefaultParams("Reload Time");
        m_weaponStats[(int)EWeaponStat.RANGED_FIRE_RATE_COOLDOWN].SetNameAndDefaultParams("Fire Rate");
    }

    public abstract bool Use();

    public EWeaponType GetWeaponType() { return m_weaponType; }
    public ref Stat AccessWeaponStat(EWeaponStat stat)
    {
        Debug.Assert((int)stat >= 0 && (int)stat < m_weaponStats.Length, "Incorrect Stat Index : Value Passed In: " + stat.ToString());
        return ref m_weaponStats[(int)stat];
    }

    public ImprovementPath GetImprovementPath() { return m_improvementPath; }
    public void ImproveWeapon()
    {
        m_improvementPath.Improve();
    }

    public void IncreaseUpgradeLimit()
    {
        ++m_upgradeLimit;
    }

    public bool CanAddUpgrade()
    {
        return m_currentUpgrades.Count < m_upgradeLimit;
    }

    public bool AddUpgrade(Upgrade newUpgrade)
    {
        if(CanAddUpgrade())
        {
            m_currentUpgrades.Add(newUpgrade);
            newUpgrade.SetParentWeapon(this);
            //newUpgrade.OnUpgradeAttached();

            newUpgrade.transform.parent = transform;
            newUpgrade.transform.localPosition = m_upgradeVisualLocations[m_currentUpgrades.Count - 1].localPosition;
            newUpgrade.transform.rotation = transform.rotation;

            return true;
        }

        return false;
    }
    public void RemoveUpgrade(int index)
    {
        m_currentUpgrades[index].transform.SetParent(null);
        m_currentUpgrades.RemoveAt(index);
    }
    public void RemoveAllUpgrades()
    {
        while(m_currentUpgrades.Count > 0)
        {
            RemoveUpgrade(0);
        }
    }

    public List<Effect> GetOnHitEffectsFromUpgrades()
    {
        List<Effect> effectsToApply = new List<Effect>();
        foreach (Upgrade up in m_currentUpgrades)
        {
            if (up.GetOnHitEffect() != null)
            {
                effectsToApply.Add(up.GetOnHitEffect());
            }
        }

        return effectsToApply;
    }

    public ref List<Upgrade> AccessCurrentUpgrades()
    {
        return ref m_currentUpgrades;
    }

    public uint GetUpgradeLimit()
    {
        return m_upgradeLimit;
    }

    public int GetUpgradesAvailableCount()
    {
        return (int)m_upgradeLimit - m_currentUpgrades.Count;
    }

    public void SetAllAttachedUpgradeParticleEffectsScale(float value)
    {
        foreach (Upgrade up in m_currentUpgrades)
        {
            up.SetParticleEffectScale(value);
        }
    }

    public string GetWeaponStatAsString(in EWeaponStat stat)
    {
        string[] WeaponStatNames = 
        {
            "DAMAGE",
            "PROJECTILE SPEED",
            "CLIP SIZE",
            "RELOAD TIME",
            "FIRE RATE"
        };

        return WeaponStatNames[(int)stat];
    }

    public string GetWeaponStatDescription()
    {
        string retVal = "";

        for (int i = 0; i < (int)EWeaponStat.MAX; ++i)
        {
            bool isMeleeStat = ((EWeaponStat)i).ToString().Contains("MELEE_");
            bool isRangedStat = ((EWeaponStat)i).ToString().Contains("RANGED_");

            if (((EWeaponStat)i).ToString().Contains("ALL_")
                || (GetWeaponType() == EWeaponType.MELEE && isMeleeStat)
                || (GetWeaponType() == EWeaponType.RANGED && isRangedStat))
            {
                Stat stat = AccessWeaponStat((EWeaponStat)i);
                retVal += stat.GetName() + ": " + stat.GetCurrent().ToString() + "\n";
            }
        }

        return retVal;
    }

    public override string GetItemTypeDescription()
    {
        return GetWeaponStatDescription();
    }
}
