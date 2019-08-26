using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Stat
{
    Stat(string name, float current, float max)
    {
        m_name = name;
        m_current = current;
        m_max = max;
    }

    //Getters
    public string GetName() { return m_name; }
    public float GetCurrent() { return m_current; }
    public float GetMax() { return m_max; }

    //Setters
    public void SetName(string newName)
    {
        m_name = newName;
    }
    public void SetCurrent(float value)
    {
        if (m_max != -1)
        {
            value = Mathf.Clamp(m_current, 0, m_max);
        }
        m_current = value;
    }
    public void SetMax(float value)
    {
        m_current = value;
    }

    //Adders
    public void AddCurrent(float value) { SetCurrent(value + GetCurrent()); }
    public void AddMax(float value) { SetMax(value + GetMax()); }

    private float m_current;
    private float m_max;
    private string m_name;
}
