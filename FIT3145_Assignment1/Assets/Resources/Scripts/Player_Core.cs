using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Core : MonoBehaviour
{
    //Variables
    [SerializeField] private float m_movementSpeed = 0.05f;
    float m_currentRotation = 0;

    //Methods

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float rotationX_Delta = Input.GetAxis("Mouse X");
        float hVal = Input.GetAxis("Horizontal");
        float vVal = Input.GetAxis("Vertical");

        if(hVal != 0 || vVal != 0)
        {
            transform.Translate(new Vector3(hVal, 0, vVal) * m_movementSpeed, Space.Self);
        }

        m_currentRotation += rotationX_Delta;
        transform.rotation = Quaternion.Euler(0, m_currentRotation, 0);

    }
}
