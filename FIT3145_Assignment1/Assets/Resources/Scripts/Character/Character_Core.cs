using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character_Core : MonoBehaviour
{
    //--Variables--//
    [HideInInspector] public Animator m_animator;
    [HideInInspector] public Character_Stats m_characterStats;
    [HideInInspector] public Rigidbody m_rigidbody;
    [HideInInspector] public Character_Aimer m_characterAimer;
    [SerializeField] protected Hitbox m_MeleeHitbox = null;
    [SerializeField] private bool m_ignoreHitAnimation = false;

    private List<Effect> m_currentEffects = new List<Effect>();

    //Audio
    private AudioSource m_audioSource = null;
    [SerializeField] private AudioClip m_audio_hurt;
    [SerializeField] private AudioClip m_audio_death;


    //--Methods--//

    protected virtual void Update()
    {
        ProcessEffects();
    }

    protected virtual void LateUpdate()
    {
        if (!IsDead())
        {
            m_characterAimer.UpdateCharacterAimer();
        }
    }

    public bool IsPlayer()
    {
        Player_Core player = null;

        if (this != null)
        {
            player = (this as Player_Core);
        }

        return (player != null);
    }

    public void AddEffect(Effect newEffect)
    {
        m_currentEffects.Add(newEffect);
        newEffect.SetCharacter(this);
    }

    public void RemoveEffect(Effect effectToRemove)
    {
        m_currentEffects.Remove(effectToRemove);
    }

    public void RemoveAllEffects()
    {
        m_currentEffects.Clear();
    }
    public void ProcessEffects()
    {
        for(int i = 0; i < m_currentEffects.Count; ++i)
        {
            m_currentEffects[i].UpdateEffect();
            m_currentEffects[i].AddLifeTime(-Time.deltaTime);
        }

        for (int i = 0; i < m_currentEffects.Count; ++i)
        {
            if(m_currentEffects[i].GetLifeTime() == 0)
            {
                RemoveEffect(m_currentEffects[i]);
                i = 0; //reset the loop
            }
        }
    }
    

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


        m_audioSource = GetComponent<AudioSource>();
        Debug.Assert(m_audioSource != null, "AudioSource Is Null");

        //Error Check Additional Stuff
        Debug.Assert(m_MeleeHitbox, "Hitbox unassigned!?!?/");
    }

    public void MoveCharacter(Vector3 dir, Space spaceType)
    {
        if (dir != Vector3.zero)
        {
            dir.Normalize();

            Vector3 newMovementVal = (spaceType == Space.Self ? transform.TransformDirection(dir) : dir) * m_characterStats.AccessMovementSpeedStat().GetCurrent() * Time.deltaTime;
            m_rigidbody.MovePosition(transform.position + newMovementVal);
        }
    }

    public virtual void ReceiveHit(float damage, List<Effect> effectsToApply = null)
    {
        //only receive hits if your alive :P
        if(!IsDead())
        {
            //Debug.Log(transform.name +  " | Hit Received | DMG: " + damage);

            //add effects
            if(effectsToApply != null)
            {
                foreach (Effect effect in effectsToApply)
                {
                    AddEffect(effect);
                }
            }

            //deal damage to health
            m_characterStats.AccessHealthStat().AddCurrent(-damage);
            FX_Manager.Instance.SpawnParticleEffect(EParticleEffect.HIT, transform.position);

            if(!m_ignoreHitAnimation)
            {
                m_animator.Play("HitStun", 0, 0.0f);
            }

            //play hurt sound
            if(m_audioSource)
            {
                m_audioSource.clip = m_audio_hurt;
                if(m_audioSource.clip)
                {
                    m_audioSource.Play();
                }
            }

            if (m_characterStats.AccessHealthStat().GetCurrent() == 0)
            {
                Die();
            }
        }
    }

    public virtual void Revive()
    {
        m_characterStats.AccessHealthStat().ResetCurrent();
        m_animator.Play("Idle", 0, 0.0f);
    }

    protected virtual void Die()
    {
        //Debug.Log(transform.name + " has died");
        m_animator.Play("Death", 0, 0.0f);
        ResetHandAnimations();

        if (m_audioSource)
        {
            m_audioSource.clip = m_audio_death;
            if (m_audioSource.clip)
            {
                m_audioSource.Play();
            }
        }
    }

    public bool IsDead()
    {
        return (m_characterStats.AccessHealthStat().GetCurrent() == 0);
    }

    protected abstract void SendMeleeAttack(in int AE_handIndex);
    protected abstract void PlayMeleeAttackSound(in int AE_handIndex);

    public void ResetHandAnimations()
    {
        m_animator.Play("EMPTY", 1, 0.0f);
        m_animator.Play("EMPTY", 2, 0.0f);
    }
}
