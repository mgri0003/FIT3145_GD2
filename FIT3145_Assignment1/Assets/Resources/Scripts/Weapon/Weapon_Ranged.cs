using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Ranged : Weapon_Base
{
    //--Variables--
    [SerializeField] private GameObject m_projectile = null;
    [SerializeField] private Transform m_firingTransform = null;
    [SerializeField] private Vector3 m_lookAtPos = Vector3.zero;

    //Current Variables
    private uint m_currentAmmo = 0;
    private float m_currentReloadTime = 0;
    private float m_currentFireRateCooldown = 0;

    //--Methods--

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
                    //initialise the projectile (and off it goes!)
                    Vector3 firingDir = m_lookAtPos - m_firingTransform.position;
                    projectile.Init(AccessWeaponStat((int)EWeaponStat.ALL_DAMAGE).GetCurrent(), AccessWeaponStat(EWeaponStat.RANGED_PROJECTILE_SPEED).GetCurrent(), firingDir.normalized);
                    projectile.AddProjectileEffects(GetOnHitEffectsFromUpgrades());
                }
            }
        }

        //reduce ammo by 1
        m_currentAmmo--;
    }

    void Start()
    {
        m_currentAmmo = (uint)AccessWeaponStat(EWeaponStat.RANGED_CLIP_SIZE).GetCurrent();
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

    public uint GetCurrentAmmo()
    {
        return m_currentAmmo;
    }

    public float GetCurrentReloadTime()
    {
        return m_currentReloadTime;
    }

    public void ResetAmmo()
    {
        m_currentAmmo = (uint)AccessWeaponStat(EWeaponStat.RANGED_CLIP_SIZE).GetCurrent();
    }
}
