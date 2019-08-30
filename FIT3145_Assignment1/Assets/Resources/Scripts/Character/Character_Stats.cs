using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Stats : MonoBehaviour
{
    private const float m_MAXHEALTH = 100;
    private const float m_BASEMOVEMENTSPEED = 0.05f;
    [SerializeField] private Stat m_health;
    [SerializeField] private Stat m_movementSpeed;

    Character_Stats()
    {
        InitStats();
    }

    private void InitStats()
    {
        m_health.SetAll("Health", m_MAXHEALTH, m_MAXHEALTH);
        m_movementSpeed.SetAll("Movement Speed", m_BASEMOVEMENTSPEED, -1);
    }

    //Health
    public ref Stat AccessHealthStat() { return ref m_health; }
    public ref Stat AccessMovementSpeedStat() { return ref m_movementSpeed; }

}
