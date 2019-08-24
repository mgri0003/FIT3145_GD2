using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Core : MonoBehaviour
{
    [HideInInspector] public Animator m_animator;
    [HideInInspector] public Character_Stats m_characterStats;
    [HideInInspector] public Rigidbody m_rigidbody;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        SetupComponents();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected virtual void SetupComponents()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        Debug.Assert(m_rigidbody != null, "Rigidbody Is Null");

        m_animator = GetComponent<Animator>();
        Debug.Assert(m_animator != null, "Animator Is Null");

        m_characterStats = GetComponent<Character_Stats>();
        Debug.Assert(m_characterStats != null, "Character Stats Holder Is Null");
    }

    public void ReceiveHit(float damage)
    {
        //only recieve hits if your alive :P
        if(!IsDead())
        {
            //deal damage to health
            m_characterStats.AddHealth(-damage);

            if (m_characterStats.GetHealth() == 0)
            {
                Die();
            }
        }
    }

    protected virtual void Die()
    {
        Debug.Log(transform.name + " has died");
    }

    public bool IsDead()
    {
        return (m_characterStats.GetHealth() == 0);
    }
}
