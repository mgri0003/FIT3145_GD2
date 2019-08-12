using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Core : MonoBehaviour
{
    //-Variables-
    [SerializeField] private float m_movementSpeed = 0.05f;

    //Components
    [HideInInspector] public Animator m_animator;
    [HideInInspector] public Player_Rotator m_playerRotator;

    //-Methods-


    // Start is called before the first frame update
    void Start()
    {
        SetupComponents();
        GetComponent<Character_Aimer>().InitBones();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement();
        m_playerRotator.UpdateDesiredRotation(Input.GetAxis("Mouse X"));
    }

    void SetupComponents()
    {
        m_animator = GetComponent<Animator>();
        Debug.Assert(m_animator != null, "Animator Is Null");

        m_playerRotator = GetComponent<Player_Rotator>();
        Debug.Assert(m_playerRotator != null, "Player Rotator Is Null");
    }

    private void UpdateMovement()
    {
        float hVal = Input.GetAxis("Horizontal");
        float vVal = Input.GetAxis("Vertical");
        bool isMoving = (hVal != 0 || vVal != 0);

        if (isMoving)
        {
            transform.Translate(new Vector3(hVal, 0, vVal) * m_movementSpeed, Space.Self);
            m_playerRotator.RefreshPlayerRotation();
        }

        m_animator.SetBool("AP_isMoving", isMoving);
    }
}
