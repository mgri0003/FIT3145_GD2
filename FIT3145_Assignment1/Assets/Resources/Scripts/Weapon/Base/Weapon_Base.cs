using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EWeaponType
{
    MELEE,
    RANGED,
    INVALID,
    MAX
}

public enum EWeaponStat
{
    ALL_DAMAGE,
    RANGED_PROJECTILE_SPEED,
    RANGED_CLIP_SIZE,
    RANGED_RELOAD_TIME,
    RANGED_FIRE_RATE_COOLDOWN,
    RANGED_PROJECTILE_LIFETIME,

    MAX
}

public abstract class Weapon_Base : Item
{
    //--Variables--
    [SerializeField] private EWeaponType m_weaponType = EWeaponType.INVALID;
    [SerializeField] protected Stat[] m_weaponStats = new Stat[(int)EWeaponStat.MAX];
    [SerializeField] private ImprovementPath m_improvementPath;
    protected List<Upgrade> m_currentUpgrades = new List<Upgrade>();
    private uint m_upgradeLimit = 1;
    private const uint m_totalUpgradeLimit = 3;
    [SerializeField] private Transform[] m_upgradeVisualLocations = new Transform[m_totalUpgradeLimit];
    [SerializeField] private Vector3 m_weaponHoldOffset = Vector3.zero;
    [SerializeField] protected float m_upgradeBalanceScale = 1.0f;
    [SerializeField] protected bool m_autoUseAllowed = false;

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
        m_weaponStats[(int)EWeaponStat.RANGED_PROJECTILE_LIFETIME].SetNameAndDefaultParams("Projectile Lifetime");
    }

    public Vector3 GetWeaponHoldOffset() { return m_weaponHoldOffset; }

    public abstract bool Use();
    public abstract bool AutoUse();

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

    public bool CanAddUpgrade(Upgrade newUpgrade)
    {
        bool isValidWeaponToUpgrade = false;

        //a melee upgrade on melee weapon?
        if((newUpgrade.GetWeaponTypeRestriction() == EUpgradeWeaponTypeRestriction.MELEE_ONLY) && (GetWeaponType() == EWeaponType.MELEE))
        {
            isValidWeaponToUpgrade = true;
        }
        //a ranged upgrade on ranged weapon?
        else if ((newUpgrade.GetWeaponTypeRestriction() == EUpgradeWeaponTypeRestriction.RANGED_ONLY) && (GetWeaponType() == EWeaponType.RANGED))
        {
            isValidWeaponToUpgrade = true;
        }
        //if the upgrade can be applied to either weapon
        else if ((newUpgrade.GetWeaponTypeRestriction() == EUpgradeWeaponTypeRestriction.NONE))
        {
            isValidWeaponToUpgrade = true;
        }

        return (m_currentUpgrades.Count < m_upgradeLimit) && isValidWeaponToUpgrade;
    }

    public bool AddUpgrade(Upgrade newUpgrade)
    {
        if(CanAddUpgrade(newUpgrade))
        {
            m_currentUpgrades.Add(newUpgrade);
            newUpgrade.SetParentWeapon(this);
            newUpgrade.SetBalanceScale(m_upgradeBalanceScale);
            newUpgrade.OnUpgradeAttached();

            newUpgrade.transform.parent = transform;
            newUpgrade.transform.localPosition = m_upgradeVisualLocations[m_currentUpgrades.Count - 1].localPosition;
            newUpgrade.transform.rotation = transform.rotation;

            return true;
        }

        return false;
    }
    public void RemoveUpgrade(int index)
    {
        m_currentUpgrades[index].OnUpgradeDettached();
        m_currentUpgrades[index].ResetBalanceScale();
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
            "FIRE RATE",
            "PROJECTILE LIFETIME"
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

    public bool IsAutoUseAllowed() { return m_autoUseAllowed; }
}
