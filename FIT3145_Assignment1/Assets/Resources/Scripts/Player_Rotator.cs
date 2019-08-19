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
    public const float PLAYER_ROTATION_SPEED = 8;

    // Update is called once per frame
    void LateUpdate()
    {
        SmoothRotatePlayer();
    }

    public void UpdateDesiredRotation(in float rotationVal)
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

    public void RefreshCurrentPlayerRotation()
    {
        m_currentRotation = m_desiredRotation;
        GetComponent<Player_Core>().SubtleMove();
    }

    private void SmoothRotatePlayer()
    {
        //smoothly rotate player
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, m_currentRotation, 0), PLAYER_ROTATION_SPEED * Time.deltaTime);

        //if player looks too much without rotating (force a rotation)
        if (Mathf.Abs(m_desiredRotation - m_currentRotation) > PLAYER_ROTATION_MAX)
        {
            RefreshCurrentPlayerRotation();
        }
    }

    //Getters
    public float GetDesiredRotation() { return m_desiredRotation; }
}
