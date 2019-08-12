using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Rotator : MonoBehaviour
{
    //--variables--
    private float m_desiredRotation = 0;
    private float m_currentRotation = 0;

    //constants
    public const float PLAYER_ROTATION_MAX = 50;

    // Update is called once per frame
    void Update()
    {
        PlayerRotationFixer();
    }

    public void UpdateDesiredRotation(float rotationVal)
    {
        m_desiredRotation += rotationVal;
        if (m_desiredRotation > 360)
        {
            m_desiredRotation = 0;
        }
        else if (m_desiredRotation < 0)
        {
            m_desiredRotation = 360;
        }
    }

    public void RefreshPlayerRotation()
    {
        transform.rotation = Quaternion.Euler(0, m_desiredRotation, 0);
        m_currentRotation = m_desiredRotation;
    }

    private void PlayerRotationFixer()
    {
        if (Mathf.Abs(m_desiredRotation - m_currentRotation) > PLAYER_ROTATION_MAX)
        {
            RefreshPlayerRotation();
        }
    }


    //Getters
    public float GetDesiredRotation() { return m_desiredRotation; }
}
