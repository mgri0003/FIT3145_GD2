using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Stats : MonoBehaviour
{
    private const float m_MAXHEALTH = 100;
    [SerializeField] private float m_health = m_MAXHEALTH;

    public float GetHealth() { return m_health; }
    public void SetHealth(float newVal)
    {
        m_health = newVal;
        m_health = Mathf.Clamp(m_health, 0, m_MAXHEALTH);
    }
    public void AddHealth(float newVal) { SetHealth(GetHealth() + newVal); }

}
