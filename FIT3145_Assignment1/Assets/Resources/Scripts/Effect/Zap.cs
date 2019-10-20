using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zap : Effect
{
    [HideInInspector] public float m_zapDamage = 1.0f;

    public override void UpdateEffect()
    {
        m_parentCharacter.ReceiveHit(m_zapDamage);
        FX_Manager.Instance.SpawnParticleEffect(EParticleEffect.LIGHTNING_IMPACT, m_parentCharacter.transform.position);
        SetLifeTime(0);
    }
}
