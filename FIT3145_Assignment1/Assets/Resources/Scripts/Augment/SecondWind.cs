using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondWind : Augment
{
    protected override bool AugmentAbility()
    {
        if(m_player.IsDead())
        {
            m_player.Revive();
            m_player.m_characterStats.AccessHealthStat().SetCurrent(m_player.m_characterStats.AccessHealthStat().GetMax() / 2);
            FX_Manager.Instance.SpawnParticleEffect(EParticleEffect.SECOND_WIND, m_player.transform.position);
            return true;
        }

        return false;
    }

    public override void AugmentOnDeath()
    {
        ActivateAugmentAbility();
    }

    protected override void AugmentUpdate()
    {

    }
}
