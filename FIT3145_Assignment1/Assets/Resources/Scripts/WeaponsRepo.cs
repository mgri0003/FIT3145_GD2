using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsRepo : Singleton<WeaponsRepo>
{
    private static List<GameObject> m_weapons = new List<GameObject>();
    private static List<GameObject> m_projectiles = new List<GameObject>();
    bool m_loadedContent = false;

    public void LoadAllContent()
    {
        if(!IsContentLoaded())
        {
            m_loadedContent = true;
            LoadAllWeapons();
            LoadAllProjectiles();
        }
    }

    public bool IsContentLoaded()
    {
        return m_loadedContent;
    }

    //==============WEAPON==============//

    private void LoadAllWeapons()
    {
        //load all weapons in folder
        Object[] loadedObjects = Resources.LoadAll("GameObjects/Weapons", typeof(Object)) as Object[];
        Debug.Assert(loadedObjects != null, "WeaponsRepo: Failed To Load Objects (Weapons)");

        //add these loaded objects into weapons list as GameObjects
        foreach (Object o in loadedObjects)
        {
            m_weapons.Add(o as GameObject);
        }

        //error check GameObject adding and display number of weapons loaded
        Debug.Assert(m_weapons.Count != 0, "WeaponsRepo: Failed To Add Weapons");
        Debug.Log("WeaponsRepo: Loaded " + m_weapons.Count + " Weapons");
    }

    public static GameObject GetWeapon(uint index)
    {
        Debug.Assert(index < m_weapons.Count, "Invalid Index");
        if(index < m_weapons.Count)
        {
            return m_weapons[(int)index];
        }

        return null;
    }

    public static GameObject GetRandomWeapon()
    {
        return GetWeapon((uint)Random.Range(0, m_weapons.Count));
    }

    public static GameObject SpawnWeapon(uint index)
    {
        GameObject weaponToSpawn = GetWeapon(index);
        Debug.Assert(weaponToSpawn, "Can't Spawn Weapon, Weapon Is Null");
        if (weaponToSpawn)
        {
            GameObject newGO = Instantiate(weaponToSpawn, Vector3.zero, Quaternion.Euler(0, 0, 0));
            newGO.transform.localRotation = Quaternion.Euler(0, 0, 0);
            return newGO;
        }

        return null;
    }

    public static GameObject SpawnRandomWeapon()
    {
        return SpawnWeapon((uint)Random.Range(0, m_weapons.Count));
    }

    //============PROJECTILES==============//
    private void LoadAllProjectiles()
    {
        //load all weapons in folder
        Object[] loadedObjects = Resources.LoadAll("GameObjects/Projectiles", typeof(Object)) as Object[];
        Debug.Assert(loadedObjects != null, "WeaponsRepo: Failed To Load Objects (Projectiles)");

        //add these loaded objects into Projectiles list as GameObjects
        foreach (Object o in loadedObjects)
        {
            m_projectiles.Add(o as GameObject);
        }

        //error check GameObject adding and display number of projectiles loaded
        Debug.Assert(m_projectiles.Count != 0, "WeaponsRepo: Failed To Add Projectiles");
        Debug.Log("WeaponsRepo: Loaded " + m_projectiles.Count + " Projectiles");
    }

    public static GameObject GetProjectile(uint index)
    {
        Debug.Assert(index < m_projectiles.Count, "Invalid Index");
        if (index < m_projectiles.Count)
        {
            return m_projectiles[(int)index];
        }

        return null;
    }

    public static GameObject GetRandomProjectile()
    {
        return GetProjectile((uint)Random.Range(0, m_projectiles.Count));
    }
}
