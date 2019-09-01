using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Core : Character_Core
{
    //--Variables--//
    Character_Core m_targetCharacter = null;
    [SerializeField] private float m_meleeDamage = 1.0f;
    private float m_minDistanceToAttack = 0.8f;
    private float m_minDistanceToMove = 5.0f;
    [SerializeField] private GameObject[] m_itemDrops;

    //--Methods--//
    protected override void Start()
    {
        base.Start();

        //Debug.Assert(m_targetCharacter, "Missing Target Character, should be set when enemy is spawned!");
        m_characterAimer.Init(m_targetCharacter.transform);
    }

    public void FindPlayerToTarget()
    {
        SetTargetCharacter(GameObject.Find("MainPlayer").GetComponent<Player_Core>());
    }

    private void Update()
    {
        bool isMoving = false;

        if (!IsDead())
        {
            if(m_targetCharacter)
            {
                if(IsCloseEnoughToAct())
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
                        MoveCharacter(vecToTarget, Space.World);
                        isMoving = true;
                    }
                }
                else
                {
                    m_characterAimer.SetEnabled(false);
                }
            }
        }

        //Update anim vars
        m_animator.SetBool("AP_isMoving", isMoving);
    }

    public void SetTargetCharacter(in Character_Core targetCharacter)
    {
        m_targetCharacter = targetCharacter;
    }

    protected override void SendMeleeAttack(in int AE_handIndex)
    {
        GameObject goHit = m_MeleeHitbox.GetFirstGameObjectCollided();
        if (goHit && goHit.CompareTag("Character"))
        {
            goHit.GetComponent<Character_Core>().ReceiveHit(m_meleeDamage);
        }
    }

    public bool IsCloseEnoughToAct()
    {
        return Vector3.Distance(transform.position, m_targetCharacter.transform.position) < m_minDistanceToMove;
    }

    public bool IsCloseEnoughToAttack()
    {
        return Vector3.Distance(transform.position, m_targetCharacter.transform.position) < m_minDistanceToAttack;
    }

    protected override void Die()
    {
        if(m_itemDrops.Length > 0)
        {
            int i = 0;
            foreach (GameObject go in m_itemDrops)
            {
                GameObject droppedGO = Instantiate(go, transform.position + new Vector3(0 + (i * 0.5f), 0.5f, 0), Quaternion.Euler(0, 0, 0));
                ++i;
            }
        }

        gameObject.SetActive(false);
        base.Die();
    }
}
