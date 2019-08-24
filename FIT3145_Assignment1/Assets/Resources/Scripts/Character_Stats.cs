using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Stats : MonoBehaviour
{
    private const float m_MAXHEALTH = 100;
    [SerializeField] private float m_health = m_MAXHEALTH;

    [SerializeField] private float m_movementSpeed = 0.05f;

    //Health
    public float GetHealth() { return m_health; }
    public void SetHealth(float newVal)
    {
        m_health = newVal;
        m_health = Mathf.Clamp(m_health, 0, m_MAXHEALTH);
    }
    public void AddHealth(float newVal) { SetHealth(GetHealth() + newVal); }

    //Movement Speed
    public float GetMovementSpeed() { return m_movementSpeed; }
    public void SetMovementSpeed(float newVal)
    {
        m_movementSpeed = newVal;
    }
    public void AddMovementSpeed(float newVal) { SetMovementSpeed(GetMovementSpeed() + newVal); }

}
