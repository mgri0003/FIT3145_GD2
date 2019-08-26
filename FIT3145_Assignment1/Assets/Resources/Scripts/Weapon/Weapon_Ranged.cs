using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EWeaponStat_Ranged
{
    PROJECTILE_SPEED = EWeaponStat.MAX,
    AMMO,
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

    //--Methods--
    Weapon_Ranged()
    {
        m_weaponBaseStats = new Stat[(int)EWeaponStat_Ranged.MAX];
        InitWeaponStats();
    }

    protected override void InitWeaponStats()
    {
        base.InitWeaponStats();
        m_weaponBaseStats[(int)EWeaponStat_Ranged.PROJECTILE_SPEED].SetNameAndDefaultParams("Projectile Speed");
        m_weaponBaseStats[(int)EWeaponStat_Ranged.AMMO].SetNameAndDefaultParams("Ammo");
        m_weaponBaseStats[(int)EWeaponStat_Ranged.RELOAD_TIME].SetAll("Reload Time", 0, 0, -1);
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
        AccessWeaponStat((int)EWeaponStat_Ranged.AMMO).AddCurrent(-1);
    }

    void Start()
    {
        AccessWeaponStat((int)EWeaponStat_Ranged.AMMO).ResetCurrent();
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
        return AccessWeaponStat((int)EWeaponStat_Ranged.RELOAD_TIME).GetCurrent() != -1;
    }

    public void Reload()
    {
        if(!IsReloading())
        {
            //Start reloading!
            AccessWeaponStat((int)EWeaponStat_Ranged.RELOAD_TIME).ResetCurrent();
        }
    }

    private void UpdateCurrentReload()
    {
        if (IsReloading())
        {
            AccessWeaponStat((int)EWeaponStat_Ranged.RELOAD_TIME).AddCurrent(-Time.deltaTime);
            if(AccessWeaponStat((int)EWeaponStat_Ranged.RELOAD_TIME).GetCurrent() < 0)
            {
                AccessWeaponStat((int)EWeaponStat_Ranged.RELOAD_TIME).SetCurrent(0);
            }

            if (AccessWeaponStat((int)EWeaponStat_Ranged.RELOAD_TIME).GetCurrent() == 0)
            {
                //reload complete
                //reset ammo back to full
                AccessWeaponStat((int)EWeaponStat_Ranged.AMMO).ResetCurrent();

                //set reload time to complete (-1)
                AccessWeaponStat((int)EWeaponStat_Ranged.RELOAD_TIME).SetCurrent(-1);
            }
        }
    }

    private bool CanFire()
    {
        return !IsReloading() && IsFireRateCooldownComplete() && AccessWeaponStat((int)EWeaponStat_Ranged.AMMO).GetCurrent() > 0;
    }

    private void UpdateFireRateCooldown()
    {
        if(!IsFireRateCooldownComplete())
        {
            AccessWeaponStat((int)EWeaponStat_Ranged.FIRE_RATE_COOLDOWN).AddCurrent(-Time.deltaTime);
        }
    }

    private bool IsFireRateCooldownComplete()
    {
        return AccessWeaponStat((int)EWeaponStat_Ranged.FIRE_RATE_COOLDOWN).GetCurrent() == 0;
    }

    protected void ResetFireRateCooldown()
    {
        AccessWeaponStat((int)EWeaponStat_Ranged.FIRE_RATE_COOLDOWN).ResetCurrent();
    }
}
