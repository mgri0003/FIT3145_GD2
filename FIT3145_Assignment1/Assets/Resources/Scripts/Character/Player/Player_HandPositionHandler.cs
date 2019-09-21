using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class Player_HandPositionHandler : MonoBehaviour
{
    private Player_Core m_player = null;
    [SerializeField] private Transform[] m_handTransforms = new Transform[(int)EPlayerHand.MAX];

    // Start is called before the first frame update
    void Start()
    {
        m_player = GetComponent<Player_Core>();
        Debug.Assert(m_player, "Player_HandPositionHandler : missing player_core component!?!?");
    }

    private void OnAnimatorIK(int layerIndex)
    {
#if false
        bool disableRightHandIK = false;
        bool disableLeftHandIK = false;

        if(m_player.IsDead())
        {
            disableRightHandIK = true;
            disableLeftHandIK = true;
        }

        //if the player is holding a weapon in their right hand
        if (!HandleHandIK(EPlayerHand.HAND_RIGHT))
        {
            disableRightHandIK = true;
        }
        if (!HandleHandIK(EPlayerHand.HAND_LEFT))
        {
            disableLeftHandIK = true;
        }

        if(disableRightHandIK)
        {
            m_player.m_animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0.0f);
            m_player.m_animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0.0f);
        }
        if(disableLeftHandIK)
        {
            m_player.m_animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0.0f);
            m_player.m_animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0.0f);
        }
#endif
    }

    private bool HandleHandIK(in EPlayerHand handToMove)
    {
        bool success = false;

        EPlayerHand handToMoveTowards = (handToMove == EPlayerHand.HAND_RIGHT ? EPlayerHand.HAND_LEFT : EPlayerHand.HAND_RIGHT);
        AvatarIKGoal ikHandToMove = (handToMove == EPlayerHand.HAND_RIGHT? AvatarIKGoal.RightHand : AvatarIKGoal.LeftHand);

        if (m_player.m_playerWeaponHolder.IsHoldingWeaponInHand(handToMoveTowards))
        {
            //but not holding a weapon in their left hand
            if (!m_player.m_playerWeaponHolder.IsHoldingWeaponInHand(handToMove))
            {
                m_player.m_animator.SetIKPositionWeight(ikHandToMove, 1.0f);
                m_player.m_animator.SetIKRotationWeight(ikHandToMove, 1.0f);
                m_player.m_animator.SetIKPosition(ikHandToMove, m_handTransforms[(int)handToMoveTowards].position);
                m_player.m_animator.SetIKRotation(ikHandToMove, Quaternion.Euler(m_handTransforms[(int)handToMoveTowards].rotation.eulerAngles + Quaternion.Euler(-20, 0, -75).eulerAngles));

                success = true;
            }
        }

        return success;
    }
}
