﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon_Ranged : Weapon_Base
{
    //--Variables--
    [SerializeField] private GameObject m_projectile = null;
    [SerializeField] private Transform m_firingTransform = null;
    private Vector3 m_lookAtPos = Vector3.zero;

    //Current Variables
    private int m_currentAmmo = 0;
    private float m_currentReloadTime = 0;
    private float m_currentFireRateCooldown = 0;

    //--Methods--
    protected abstract void FireRangedWeapon();

    public override bool Use()
    {
        if(CanFire())
        {
            FireRangedWeapon();
            ResetFireRateCooldown();
            return true;
        }
        else
        {
            //if firing failed, auto reload
            if(OutOfAmmo())
            {
                Reload();
            }
        }

        return false;
    }

    public override bool AutoUse()
    {
        if(IsAutoUseAllowed())
        {
            return Use();
        }

        return false;
    }

    protected GameObject FireSingleProjectile(in bool consumeAmmo = true, Vector3 firingDirectionOffset = default)
    {
        GameObject retVal = null;

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
                    //initialise the projectile (and off it goes!)
                    Vector3 firingDir = (m_lookAtPos + firingDirectionOffset) - m_firingTransform.position;
                    projectile.Init(
                        AccessWeaponStat((int)EWeaponStat.ALL_DAMAGE).GetCurrent(), 
                        AccessWeaponStat(EWeaponStat.RANGED_PROJECTILE_SPEED).GetCurrent(), 
                        AccessWeaponStat(EWeaponStat.RANGED_PROJECTILE_LIFETIME).GetCurrent(), 
                        firingDir.normalized);

                    projectile.SetOwner(GamePlayManager.Instance.GetCurrentPlayer().gameObject);

                    projectile.AddProjectileEffects(GetOnHitEffectsFromUpgrades());

                    retVal = newProjectileGO;
                }
            }
        }

        //reduce ammo by 1
        if(consumeAmmo)
        {
            AddCurrentAmmo(-1);
        }

        return retVal;
    }

    void Start()
    {
        m_currentAmmo = (int)AccessWeaponStat(EWeaponStat.RANGED_CLIP_SIZE).GetCurrent();
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
            m_currentReloadTime = AccessWeaponStat(EWeaponStat.RANGED_RELOAD_TIME).GetCurrent();
        }
    }

    private void UpdateCurrentReload()
    {
        if (IsReloading())
        {
            m_currentReloadTime -= Time.deltaTime;
            m_currentReloadTime = Mathf.Clamp(m_currentReloadTime, 0, AccessWeaponStat(EWeaponStat.RANGED_RELOAD_TIME).GetCurrent());

            if (m_currentReloadTime == 0)
            {
                //reload complete
                //reset ammo back to full
                ResetAmmo();

                //set reload time to complete (-1)
                m_currentReloadTime = -1;
            }
        }
    }

    private bool OutOfAmmo()
    {
        return m_currentAmmo == 0;
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
            m_currentFireRateCooldown = Mathf.Clamp(m_currentFireRateCooldown, 0, AccessWeaponStat(EWeaponStat.RANGED_FIRE_RATE_COOLDOWN).GetCurrent());
        }
    }

    private bool IsFireRateCooldownComplete()
    {
        return m_currentFireRateCooldown == 0;
    }

    protected void ResetFireRateCooldown()
    {
        m_currentFireRateCooldown = AccessWeaponStat(EWeaponStat.RANGED_FIRE_RATE_COOLDOWN).GetCurrent();
    }

    public void SetCurrentAmmo(int ammoAmonut)
    {
        m_currentAmmo = ammoAmonut;
        ClampAmmo();
    }

    public void AddCurrentAmmo(int ammoAmonut)
    {
        SetCurrentAmmo(GetCurrentAmmo() + ammoAmonut);
    }

    public int GetCurrentAmmo()
    {
        return m_currentAmmo;
    }

    public float GetCurrentReloadTime()
    {
        return m_currentReloadTime;
    }

    public void ResetAmmo()
    {
        m_currentAmmo = (int)AccessWeaponStat(EWeaponStat.RANGED_CLIP_SIZE).GetCurrent();
    }

    private void ClampAmmo()
    {
        m_currentAmmo = Mathf.Clamp(m_currentAmmo, 0, (int)AccessWeaponStat(EWeaponStat.RANGED_CLIP_SIZE).GetCurrent());
    }
}
