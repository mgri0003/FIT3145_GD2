using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Stat
{
    //Getters
    public string GetName() { return m_name; }
    public float GetCurrent() { return m_current; }
    public float GetMax() { return m_max; }
    public float GetMin() { return m_min; }

    //Setters
    public void SetName(string newName)
    {
        m_name = newName;
    }
    public void SetCurrent(float value)
    {
        value = Mathf.Clamp(value, GetMin(), GetMax() != -1 ? GetMax() : value);
        m_current = value;
    }
    public void SetMax(float value)
    {
        m_max = value;
    }
    public void SetMin(float value)
    {
        m_min = value;
    }


    //Adders
    public void AddCurrent(float value) { SetCurrent(value + GetCurrent()); }
    public void AddMax(float value) { SetMax(value + GetMax()); }
    public void AddMin(float value) { SetMin(value + GetMin()); }

    //Other
    public void SetAll(string name, float current, float max, float min)
    {
        SetName(name);
        SetMax(max);
        SetMin(min);
        SetCurrent(current);
    }
    public void SetNameAndDefaultParams(string name)
    {
        SetAll(name, 0, -1, 0);
    }
    public void ResetCurrent()
    {
        if(GetMax() != -1)
        {
            SetCurrent(GetMax());
        }
        else
        {
            SetCurrent(GetMin());
        }
    }

    //--Variables--//
    [SerializeField] private float m_current;
    [SerializeField] private float m_max;
    [SerializeField] private float m_min;
    [SerializeField] private string m_name;
}
