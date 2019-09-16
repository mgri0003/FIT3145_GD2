using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [SerializeField] public float m_lifetime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("DestroySelf", m_lifetime);
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }
}
