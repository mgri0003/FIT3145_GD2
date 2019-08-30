using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EWeaponStat_Ranged
{
    PROJECTILE_SPEED = EWeaponStat.MAX,
    CLIP_SIZE,
    RELOAD_TIME,
    FIRE_RATE_COOLDOWN,

    MAX
}

public class Weapon_Ranged : Weapon_Base
{
    //--Variables--
    [SerializeField] private GameObject m_projectile = null;
    [SerializeField] private Transform m_firingTransform = null;
    [SerializeField] private Vector3 m_lookAtPos = Vector3.zero;

    //Current Variables
    uint m_currentAmmo = 0;
    float m_currentReloadTime = 0;
    float m_currentFireRateCooldown = 0;

    //--Methods--
    Weapon_Ranged()
    {
        m_weaponBaseStats = new Stat[(int)EWeaponStat_Ranged.MAX];
        Constructor_InitWeaponStats();
    }

    protected override void Constructor_InitWeaponStats()
    {
        base.Constructor_InitWeaponStats();
        m_weaponBaseStats[(int)EWeaponStat_Ranged.PROJECTILE_SPEED].SetNameAndDefaultParams("Projectile Speed");
        m_weaponBaseStats[(int)EWeaponStat_Ranged.CLIP_SIZE].SetNameAndDefaultParams("Clip Size");
        m_weaponBaseStats[(int)EWeaponStat_Ranged.RELOAD_TIME].SetNameAndDefaultParams("Reload Time");
        m_weaponBaseStats[(int)EWeaponStat_Ranged.FIRE_RATE_COOLDOWN].SetNameAndDefaultParams("Fire Rate");
    }

    public override bool Use()
    {
        if(CanFire())
        {
            FireSingleProjectile();
            ResetFireRateCooldown();
            return true;
        }

        return false;
    }

    private void FireSingleProjectile()
    {
        Debug.Assert(m_projectile, "Ranged Weapon Does Not Have A Projectile!?!?");
        if (m_projectile)
        {
            //create the projectile GameObject
            GameObject newProjectileGO = Instantiate(m_projectile, m_firingTransform.position, m_firingTransform.rotation);
            Debug.Assert(newProjectileGO, "Failed to instantiate Projectile???");
            if (newProjectileGO)
            {
                //get the projectile class and initialise it!
                Weapon_Projectile projectile = newProjectileGO.GetComponent<Weapon_Projectile>();
                Debug.Assert(projectile, "Projectile Objects Does not have projectile Component???");
                if (projectile)
                {
                    //initialse the projectile (and off it goes!)
                    Vector3 firingDir = m_lookAtPos - m_firingTransform.position;
                    projectile.Init(AccessWeaponStat((int)EWeaponStat.DAMAGE).GetCurrent(), AccessWeaponStat((int)EWeaponStat_Ranged.PROJECTILE_SPEED).GetCurrent(), firingDir.normalized);
                }
            }
        }

        //reduce ammo by 1
        m_currentAmmo--;
    }

    void Start()
    {
        m_currentAmmo = (uint)AccessWeaponStat((int)EWeaponStat_Ranged.CLIP_SIZE).GetCurrent();
    }

    void Update()
    {
        UpdateFireRateCooldown();
        UpdateCurrentReload();
    }

    public void SetWeaponLookAt(Vector3 newAim)
    {
        m_lookAtPos = newAim;
    }


    public bool IsReloading()
    {
        return m_currentReloadTime != -1;
    }

    public void Reload()
    {
        if(!IsReloading())
        {
            //Start reloading!
            m_currentReloadTime = AccessWeaponStat((int)EWeaponStat_Ranged.RELOAD_TIME).GetCurrent();
        }
    }

    private void UpdateCurrentReload()
    {
        if (IsReloading())
        {
            m_currentReloadTime -= Time.deltaTime;
            m_currentReloadTime = Mathf.Clamp(m_currentReloadTime, 0, AccessWeaponStat((int)EWeaponStat_Ranged.RELOAD_TIME).GetCurrent());

            if (m_currentReloadTime == 0)
            {
                //reload complete
                //reset ammo back to full
                m_currentAmmo = (uint)AccessWeaponStat((int)EWeaponStat_Ranged.CLIP_SIZE).GetCurrent();

                //set reload time to complete (-1)
                m_currentReloadTime = -1;
            }
        }
    }

    private bool CanFire()
    {
        return !IsReloading() && IsFireRateCooldownComplete() && m_currentAmmo > 0;
    }

    private void UpdateFireRateCooldown()
    {
        if(!IsFireRateCooldownComplete())
        {
            m_currentFireRateCooldown -= Time.deltaTime;
            m_currentFireRateCooldown = Mathf.Clamp(m_currentFireRateCooldown, 0, AccessWeaponStat((int)EWeaponStat_Ranged.FIRE_RATE_COOLDOWN).GetCurrent());
        }
    }

    private bool IsFireRateCooldownComplete()
    {
        return m_currentFireRateCooldown == 0;
    }

    protected void ResetFireRateCooldown()
    {
        m_currentFireRateCooldown = AccessWeaponStat((int)EWeaponStat_Ranged.FIRE_RATE_COOLDOWN).GetCurrent();
    }

    public uint GetCurrentAmmo()
    {
        return m_currentAmmo;
    }
}
