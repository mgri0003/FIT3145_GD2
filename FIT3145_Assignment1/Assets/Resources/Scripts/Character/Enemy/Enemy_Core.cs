using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 649

public abstract class Enemy_Core : Character_Core
{
    //--Variables--//
    protected Character_Core m_targetCharacter = null;
    [SerializeField] private float m_meleeDamage = 1.0f;
    [SerializeField] private float m_minDistanceToAttack = 0.8f;
    [SerializeField] private float m_minDistanceToSpecial = 0.8f;
    [SerializeField] private float m_minDistanceToMove = 5.0f;
    [SerializeField] private GameObject[] m_itemDrops;
    private bool m_aggro = false;
    [SerializeField] private uint m_scrapDrop = 0;

    //--Methods--//
    protected override void Start()
    {
        base.Start();

        //Debug.Assert(m_targetCharacter, "Missing Target Character, should be set when enemy is spawned!");
        m_characterAimer.Init(m_targetCharacter.transform);
    }

    public void DisableAggro() { m_aggro = false; }
    public void TriggerAggro() { m_aggro = true; }
    public bool IsAggro() { return m_aggro; }

    public Vector3 GetDirectionToTarget()
    {
        Vector3 retVal = Vector3.zero;

        if (m_targetCharacter)
        {
            retVal = (m_targetCharacter.transform.position - transform.position);
            retVal.Normalize();
        }

        return retVal;
    }

    public void FindPlayerToTarget()
    {
        SetTargetCharacter(GameObject.Find("MainPlayer").GetComponent<Player_Core>());
    }

    override protected void Update()
    {
        base.Update();
        m_characterAimer.SetEnabled(false);

        bool isMoving = false;

        if (!IsDead())
        {
            if(m_targetCharacter)
            {
                if(!m_targetCharacter.IsDead())
                {
                    isMoving = UpdateEnemyAct();
                }
            }
        }

        //Update anim vars
        m_animator.SetBool("AP_isMoving", isMoving);
    }

    protected abstract bool UpdateEnemyAct();

    public void SetTargetCharacter(in Character_Core targetCharacter)
    {
        m_targetCharacter = targetCharacter;
    }

    protected override void SendMeleeAttack(in int AE_handIndex)
    {
        foreach(GameObject goHit in m_MeleeHitbox.GetAllGameObjectsCollided())
        {
            if (goHit && goHit.CompareTag("Character"))
            {
                if (goHit.GetComponent<Character_Core>().IsPlayer())
                {
                    goHit.GetComponent<Character_Core>().ReceiveHit(m_meleeDamage);
                }
            }
        }
    }

    protected bool IsCloseEnoughToAct()
    {
        return Vector3.Distance(transform.position, m_targetCharacter.transform.position) < m_minDistanceToMove;
    }

    protected bool IsCloseEnoughToAttack()
    {
        return Vector3.Distance(transform.position, m_targetCharacter.transform.position) < m_minDistanceToAttack;
    }

    protected bool IsCloseEnoughToSpecial()
    {
        return Vector3.Distance(transform.position, m_targetCharacter.transform.position) < m_minDistanceToSpecial;
    }

    protected override void Die()
    {
        Invoke("OnDeathComplete", 2.0f);

        DropItems();

        GetComponent<Collider>().enabled = false;
        m_rigidbody.constraints = RigidbodyConstraints.FreezeAll;

        GamePlayManager.Instance.AddScrap((int)m_scrapDrop);

        base.Die();
    }

    public override void ReceiveHit(float damage, List<Effect> effectsToApply = null)
    {
        base.ReceiveHit(damage, effectsToApply);

        TriggerAggro();
    }

    private void OnDeathComplete()
    {
        FX_Manager.Instance.SpawnParticleEffect(EParticleEffect.HIT, transform.position - new Vector3(0,0.5f,0));
        gameObject.SetActive(false);
    }

    private void DropItems()
    {
        if (m_itemDrops.Length > 0)
        {
            int i = 0;
            foreach (GameObject go in m_itemDrops)
            {
                GameObject droppedGO = Instantiate(go, transform.position + new Vector3(0 + (i * 0.5f), 0.5f, 0), Quaternion.Euler(0, 0, 0));
                ++i;
            }
        }
    }
}
