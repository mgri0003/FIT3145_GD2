using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Aimer : MonoBehaviour
{
    bool m_hasInit = false;
    private Transform m_chestBone;
    private Transform m_aimTarget;

    public void Init(in Transform newAimTarget)
    {
        m_chestBone = GetComponent<Player_Core>().m_animator.GetBoneTransform(HumanBodyBones.Chest);

        if(m_chestBone == null)
        {
            Debug.Assert(false, "Missing Chest Bone!?!?!?");
            return;
        }

        SetAimTarget(newAimTarget);

        m_hasInit = true;
    }

    private void LateUpdate()
    {
        if(m_hasInit)
        {
            Vector3 posToLook = m_aimTarget.transform.position;

            m_chestBone.LookAt(posToLook);
            m_chestBone.rotation = m_chestBone.rotation;
        }
    }

    public void SetAimTarget(Transform newAimTarget)
    {
        m_aimTarget = newAimTarget;
    }

    public Transform GetAimTarget()
    {
        return m_aimTarget;
    }
}
