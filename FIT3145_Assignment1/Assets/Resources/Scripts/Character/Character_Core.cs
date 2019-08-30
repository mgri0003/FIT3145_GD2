using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character_Core : MonoBehaviour
{
    [HideInInspector] public Animator m_animator;
    [HideInInspector] public Character_Stats m_characterStats;
    [HideInInspector] public Rigidbody m_rigidbody;
    [HideInInspector] public Character_Aimer m_characterAimer;
    [SerializeField] protected Hitbox m_MeleeHitbox = null;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        SetupComponents();
    }

    protected virtual void SetupComponents()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        Debug.Assert(m_rigidbody != null, "Rigidbody Is Null");

        m_animator = GetComponent<Animator>();
        Debug.Assert(m_animator != null, "Animator Is Null");

        m_characterStats = GetComponent<Character_Stats>();
        Debug.Assert(m_characterStats != null, "Character Stats Holder Is Null");

        m_characterAimer = GetComponent<Character_Aimer>();
        Debug.Assert(m_characterAimer != null, "Character Aimer Is Null");

        //Error Check Additional Stuff
        Debug.Assert(m_MeleeHitbox, "Hitbox unassigned!?!?/");
    }

    public void MoveCharacter(Vector3 dir, Space spaceType)
    {
        dir.Normalize();
        transform.Translate(dir * m_characterStats.AccessMovementSpeedStat().GetCurrent(), spaceType);
    }

    public void ReceiveHit(float damage)
    {
        //only receive hits if your alive :P
        if(!IsDead())
        {
            //deal damage to health
            m_characterStats.AccessHealthStat().AddCurrent(-damage);

            if (m_characterStats.AccessHealthStat().GetCurrent() == 0)
            {
                Die();
            }
        }
    }

    protected virtual void Revive()
    {
        m_characterAimer.SetEnabled(true);
    }

    protected virtual void Die()
    {
        Debug.Log(transform.name + " has died");
        m_characterAimer.SetEnabled(false);
    }

    public bool IsDead()
    {
        return (m_characterStats.AccessHealthStat().GetCurrent() == 0);
    }

    protected abstract void SendMeleeAttack(in int AE_handIndex);
}
