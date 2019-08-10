using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Core : MonoBehaviour
{
    //-Variables-
    private Animator m_animator;
    [SerializeField] private float m_movementSpeed = 0.05f;
    private float m_desiredRotation = 0;

    //-Methods-


    // Start is called before the first frame update
    void Start()
    {
        m_animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement();
        UpdateDesiredRotation();
    }

    private void UpdateMovement()
    {
        float hVal = Input.GetAxis("Horizontal");
        float vVal = Input.GetAxis("Vertical");
        bool isMoving = (hVal != 0 || vVal != 0);

        if (isMoving)
        {
            transform.Translate(new Vector3(hVal, 0, vVal) * m_movementSpeed, Space.Self);
            transform.rotation = Quaternion.Euler(0, m_desiredRotation, 0);
        }

        m_animator.SetBool("AP_isMoving", isMoving);
    }

    private void UpdateDesiredRotation()
    {
        m_desiredRotation += Input.GetAxis("Mouse X");
    }


    //Getters
    public float GetDesiredRotation() { return m_desiredRotation; }
    
}
