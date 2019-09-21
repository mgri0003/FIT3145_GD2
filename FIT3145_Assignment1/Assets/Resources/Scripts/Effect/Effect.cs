using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 649

public abstract class Effect : MonoBehaviour
{
    //--Variables--//
    [SerializeField] private string m_effectName;
    protected Character_Core m_parentCharacter;
    [SerializeField] private float m_lifetime = 0.0f;

    //--Methods--//
    private void Awake()
    {
        Debug.Assert(m_parentCharacter, "Character Not Assigned!?!?!?");
    }

    public abstract void UpdateEffect();
    public void SetCharacter(in Character_Core character) { m_parentCharacter = character; }

    public float GetLifeTime() { return m_lifetime; }
    public void SetLifeTime(float newValue)
    {
        m_lifetime = newValue;
        m_lifetime = Mathf.Clamp(m_lifetime, 0, float.MaxValue);
    }
    public void AddLifeTime(float newValue) { SetLifeTime(GetLifeTime() + newValue); }

    public string GetEffectName() { return m_effectName; }
}
