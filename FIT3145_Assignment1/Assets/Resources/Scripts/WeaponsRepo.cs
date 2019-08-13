using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsRepo : MonoBehaviour
{
    private static List<GameObject> m_weapons = new List<GameObject>();

    private void Start()
    {
        //this allows this object to exist even when the level transitions
        DontDestroyOnLoad(gameObject);
    }

    private void Awake()
    {
        //load all weapons in folder
        Object[] loadedObjects = Resources.LoadAll("GameObjects/Weapons", typeof(Object)) as Object[];
        Debug.Assert(loadedObjects != null, "WeaponsRepo: Failed To Load Objects");

        foreach (Object o in loadedObjects)
        {
            m_weapons.Add(o as GameObject);
        }
        Debug.Assert(m_weapons != null, "WeaponsRepo: Failed To Add Weapons");

        if (m_weapons != null)
        {
            Debug.Log("WeaponsRepo: Loaded " + m_weapons.Count + " Weapons");
        }
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
            return Instantiate(weaponToSpawn);
        }

        return null;
    }

    public static GameObject SpawnRandomWeapon()
    {
        return SpawnWeapon((uint)Random.Range(0, m_weapons.Count));
    }
}
