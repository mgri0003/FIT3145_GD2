using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    private List<GameObject> m_collidingObjects = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Character"))
        {
            m_collidingObjects.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Character"))
        {
            m_collidingObjects.Remove(other.gameObject);
        }
    }

    public GameObject GetFirstGameObjectCollided()
    {
        if(m_collidingObjects.Count > 0)
        {
           return m_collidingObjects[0];
        }

        return null;
    }

    public List<GameObject> GetAllGameObjectsCollided()
    {
        return m_collidingObjects;
    }
}
