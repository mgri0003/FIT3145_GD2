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
    [SerializeField] private UpgradePath m_upgradePath;
    protected List<Upgrade> m_currentUpgrades = new List<Upgrade>();
    private uint m_upgradeLimit = 1;
    private const uint m_totalUpgradeLimit = 3;
    [SerializeField] private Transform[] m_upgradeVisualLocations = new Transform[m_totalUpgradeLimit];

    //--methods--
    protected Weapon_Base()
    {
        Constructor_InitWeaponStats();
        m_upgradePath.SetParentWeapon(this);
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
        Debug.Assert((int)stat >= 0 && (int)stat < m_weaponStats.Length, "Incorrect Stat Index");
        return ref m_weaponStats[(int)stat];
    }

    public UpgradePath GetUpgradePath() { return m_upgradePath; }
    public void UpgradeWeapon()
    {
        m_upgradePath.Upgrade();
    }

    public void IncreaseUpgradeLimit()
    {
        ++m_upgradeLimit;
    }

    public bool CanAddUpgrade()
    {
        return m_currentUpgrades.Count < m_upgradeLimit;
    }

    public void AddUpgrade(Upgrade newUpgrade)
    {
        if(CanAddUpgrade())
        {
            m_currentUpgrades.Add(newUpgrade);
            newUpgrade.SetParentWeapon(this);
            newUpgrade.OnUpgradeAttached();

            newUpgrade.transform.parent = transform;
            newUpgrade.transform.localPosition = m_upgradeVisualLocations[m_currentUpgrades.Count - 1].localPosition;
            newUpgrade.transform.rotation = transform.rotation;
        }
    }
    public void RemoveUpgrade(Upgrade upgradeToRemove)
    {
        m_currentUpgrades.Remove(upgradeToRemove);
        upgradeToRemove.transform.parent = null;
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

}
