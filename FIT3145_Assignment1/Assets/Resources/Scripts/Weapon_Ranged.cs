using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Ranged : Weapon_Base
{
    //--Variables--
    [SerializeField] private GameObject m_projectile = null;
    [SerializeField] private Transform m_firingTransform = null;
    [SerializeField] private Vector3 m_lookAtPos = Vector3.zero;

    //Base stats
    [SerializeField] private float m_projectileSpeed = 1.0f;
    [SerializeField] private uint m_clipSize = 1;
    [SerializeField] private float m_maxReloadTime = 1.0f;
    [SerializeField] private float m_maxFireRateCooldown = 1.0f;


    //current stats
    private uint m_currentAmmo = 0;
    private float m_currentReloadTime = -1.0f;
    private float m_currentFireRateCooldown = 0.0f;


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
                    //initialse the projectile (and off it goes!)
                    Vector3 firingDir = m_lookAtPos - m_firingTransform.position;
                    projectile.Init(GetWeaponDamage(), m_projectileSpeed, firingDir.normalized);
                }
            }
        }

        //reduce ammo by 1
        --m_currentAmmo;
    }

    void Start()
    {
        m_currentAmmo = m_clipSize;
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
            m_currentReloadTime = m_maxReloadTime;
        }
    }

    private void UpdateCurrentReload()
    {
        if (IsReloading())
        {
            m_currentReloadTime -= Time.deltaTime;
            m_currentReloadTime = Mathf.Clamp(m_currentReloadTime, 0, m_maxReloadTime);

            if (m_currentReloadTime == 0)
            {
                //reload complete
                m_currentAmmo = m_clipSize;
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
            m_currentFireRateCooldown = Mathf.Clamp(m_currentFireRateCooldown, 0, m_maxFireRateCooldown);
        }
    }

    private bool IsFireRateCooldownComplete()
    {
        return m_currentFireRateCooldown == 0;
    }

    protected void ResetFireRateCooldown()
    {
        m_currentFireRateCooldown = m_maxFireRateCooldown;
    }

    //Getters
    public float GetClipSize() { return m_clipSize; }
    public float GetCurrentAmmo() { return m_currentAmmo; }
    public float GetCurrentReloadTime() { return m_currentReloadTime; }
    public float GetCurrentFireRateCooldown() { return m_currentFireRateCooldown; }
    public float GetMaxFireRateCooldown() { return m_maxFireRateCooldown; }
}
