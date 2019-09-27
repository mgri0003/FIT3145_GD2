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
        transform.Rotate(m_rotationSpeed.x * Time.deltaTime, m_rotationSpeed.y * Time.deltaTime, m_rotationSpeed.z * Time.deltaTime, m_space);
    }

    public void SetSpace(Space newSpace) { m_space = newSpace; }
    public void SetRotationVector(Vector3 newRotVec) { m_rotationSpeed = newRotVec; }
}
