using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Aimer : MonoBehaviour
{
    bool m_bonesHaveInit = false;
    Transform m_chestBone;
    [SerializeField] private Transform m_aimTarget;

    public void InitBones()
    {
        m_chestBone = GetComponent<Player_Core>().m_animator.GetBoneTransform(HumanBodyBones.Chest);

        if(m_chestBone == null)
        {
            Debug.Assert(false, "Missing Chest Bone!?!?!?");
            return;
        }

        m_bonesHaveInit = true;
    }

    private void LateUpdate()
    {
        if(m_bonesHaveInit)
        {
            Vector3 posToLook = m_aimTarget.transform.position;

            m_chestBone.LookAt(posToLook);
            m_chestBone.rotation = m_chestBone.rotation * Quaternion.Euler(new Vector3());
        }
    }

    void SetAimTarget(Transform newAimTarget)
    {
        m_aimTarget = newAimTarget;
    }
}
