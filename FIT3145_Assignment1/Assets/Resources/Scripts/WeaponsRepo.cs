using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsRepo : MonoBehaviour
{
    private List<GameObject> m_weapons = new List<GameObject>();

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
}
