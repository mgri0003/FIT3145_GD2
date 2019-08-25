using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used from "https://wiki.unity3d.com/index.php/Singleton"
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T m_instance = null;

    public static T Instance
    {
        get
        {
            if (m_instance == null)
            {
                // Search for existing instance.
                m_instance = (T)FindObjectOfType(typeof(T));
            }

            return m_instance;
        }
    }
}
