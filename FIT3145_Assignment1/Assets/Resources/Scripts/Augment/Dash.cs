using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : Augment
{
    private Vector3 m_dashValue = new Vector3(0, 0, 0);
    private const float m_dashStrength = 10;
    private const float m_maxDashTime = 0.25f;
    private float m_dashTimer = 0.0f;

    protected override bool AugmentAbility()
    {
        if (m_player.GetDirectionInput().magnitude != 0)
        {
            m_dashValue = m_player.GetDirectionInput().normalized;
            m_dashTimer = m_maxDashTime;

            //Play Animation based on Direction

            //Forward
            if (m_player.GetDirectionInput().z > 0)
            {
                m_player.m_animator.Play("Dash_Forward", 0, 0.0f);
            }
            //back
            else if (m_player.GetDirectionInput().z < 0)
            {
                m_player.m_animator.Play("Dash_Back", 0, 0.0f);
            }
            //Right
            else if (m_player.GetDirectionInput().x > 0)
            {
                m_player.m_animator.Play("Dash_Right", 0, 0.0f);
            }
            //Left
            else if (m_player.GetDirectionInput().x < 0)
            {
                m_player.m_animator.Play("Dash_Left", 0, 0.0f);
            }



            return true;
        }

        return false;
    }

    protected override void AugmentUpdate()
    {
        if(m_dashTimer > 0)
        {
            m_player.m_rigidbody.velocity = m_player.transform.TransformDirection(m_dashValue * m_dashStrength);

            m_dashTimer -= Time.deltaTime;
            m_dashTimer = Mathf.Clamp(m_dashTimer, 0.0f, m_maxDashTime);

            if(m_dashTimer == 0.0f)
            {
                m_dashTimer = -1;
                m_player.m_rigidbody.velocity = Vector3.zero;
            }
        }
    }

    public override void AugmentOnDeath()
    {
        
    }
}
