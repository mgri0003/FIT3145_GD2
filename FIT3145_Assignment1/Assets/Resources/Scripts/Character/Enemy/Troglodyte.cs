using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Troglodyte : Enemy_Core
{
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
            else
            {
                Vector3 vecToTarget = (m_targetCharacter.transform.position - transform.position);
                vecToTarget.y = 0;
                MoveCharacter(vecToTarget, Space.World);
                retval = true;
            }
        }

        return retval;
    }
}
