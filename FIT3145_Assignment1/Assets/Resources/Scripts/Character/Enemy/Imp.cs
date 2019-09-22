using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Imp : Enemy_Core
{
    [SerializeField] GameObject m_fireballProjectile = null;
    [SerializeField] Transform m_firingTransform = null;
    [SerializeField] private float m_FIREBALL_MAX_COOLDOWN = 4.0f;
    [SerializeField] private float m_FIREBALL_DAMAGE = 20.0f;
    [SerializeField] private float m_FIREBALL_SPEED = 5.0f;
    [SerializeField] private float m_FIREBALL_LIFETIME = 2.0f;
    private float m_fireballCooldown = 0.0f;

    protected override bool UpdateEnemyAct()
    {
        bool retval = false;

        if (IsCloseEnoughToAct() || IsAggro())
        {
            //look at player
            m_characterAimer.SetEnabled(true);
            transform.LookAt(m_targetCharacter.transform);

            if (IsCloseEnoughToAttack())
            {
                m_animator.Play("Attack_MeleeWeapon", 1);
            }
            else if (IsCloseEnoughToSpecial())
            {
                if(m_fireballCooldown <= 0.0f)
                {
                    m_fireballCooldown = m_FIREBALL_MAX_COOLDOWN;

                    //SHOOT FIREBALL!
                    Debug.Assert(m_fireballProjectile, "missing fireball projectile for IMP");
                    if (m_fireballProjectile)
                    {
                        //create the projectile GameObject
                        GameObject newProjectileGO = Instantiate(m_fireballProjectile, m_firingTransform.position, m_firingTransform.rotation);
                        Debug.Assert(newProjectileGO, "Failed to instantiate Projectile???");
                        if (newProjectileGO)
                        {
                            //get the projectile class and initialise it!
                            Weapon_Projectile projectile = newProjectileGO.GetComponent<Weapon_Projectile>();
                            Debug.Assert(projectile, "Projectile Objects Does not have projectile Component???");
                            if (projectile)
                            {
                                //initialise the projectile (and off it goes!)
                                Vector3 firingDir = GetDirectionToTarget();
                                
                                projectile.Init(
                                    m_FIREBALL_DAMAGE,
                                    m_FIREBALL_SPEED,
                                    m_FIREBALL_LIFETIME,
                                    firingDir.normalized);
                            }
                        }
                    }
                }
            }
            else
            {
                Vector3 vecToTarget = GetDirectionToTarget();
                vecToTarget.y = 0;
                MoveCharacter(vecToTarget, Space.World);
                retval = true;
            }
        }

        m_fireballCooldown -= Time.deltaTime;
        m_fireballCooldown = Mathf.Clamp(m_fireballCooldown, 0, m_FIREBALL_MAX_COOLDOWN);

        return retval;
    }
}
