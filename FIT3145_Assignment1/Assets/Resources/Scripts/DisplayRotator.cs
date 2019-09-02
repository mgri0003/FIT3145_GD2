using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayRotator : MonoBehaviour
{
    [SerializeField] Vector3 m_rotationSpeed;
    [SerializeField] Space m_space;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(m_rotationSpeed.x, m_rotationSpeed.y, m_rotationSpeed.z, m_space);
    }
}
