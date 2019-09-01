using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField] private List<string> m_collidingTags = new List<string>();
    private List<GameObject> m_collidingObjects = new List<GameObject>();

    private void Update()
    {
        //hack to fix garb exit
        for(int i = 0; i < m_collidingObjects.Count; ++i)
        {
            if(!m_collidingObjects[i].activeInHierarchy)
            {
                OnTriggerExit(m_collidingObjects[i].GetComponent<Collider>());
            }
        }
    }

    private bool DoesColliderHasTags(in Collider col)
    {
        bool hasTag = false;
        foreach(string tg in m_collidingTags)
        {
            if(col.CompareTag(tg))
            {
                hasTag = true;
            }
        }

        return hasTag;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(DoesColliderHasTags(other) || m_collidingTags.Count == 0)
        {
            m_collidingObjects.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (DoesColliderHasTags(other) || m_collidingTags.Count == 0)
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

    public bool IsColliding()
    {
        return m_collidingObjects.Count > 0;
    }

    public void ClearCollidingObjects()
    {
        m_collidingObjects.Clear();
    }
}
