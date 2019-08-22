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
    [SerializeField] private float m_projectileSpeed = 0;
    [SerializeField] private uint m_clipSize = 1;
    [SerializeField] private float m_maxReloadTime = 1;


    //current stats
    private uint m_currentAmmo = 0;
    private float m_currentReloadTime = -1;


    //--Methods--
    public override bool Use()
    {
        if(!IsReloading())
        {
            if (m_currentAmmo > 0)
            {
                FireSingleProjectile();
                return true;
            }
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
        UpdateCurrentReload();
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

    public void SetWeaponLookAt(Vector3 newAim)
    {
        m_lookAtPos = newAim;
    }
}
