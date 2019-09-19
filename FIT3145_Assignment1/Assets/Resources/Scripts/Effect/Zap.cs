using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zap : Effect
{
    public override void UpdateEffect()
    {
        m_parentCharacter.ReceiveHit(20);
        FX_Manager.Instance.SpawnParticleEffect(EParticleEffect.LIGHTNING_IMPACT, m_parentCharacter.transform.position);
        SetLifeTime(0);
    }
}
