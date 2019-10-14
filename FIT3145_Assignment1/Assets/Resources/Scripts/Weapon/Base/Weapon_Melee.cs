using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Melee : Weapon_Base
{
    private AudioSource m_audioSource = null;

    //--Methods--
    void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
    }

    public void PlaySwingSound()
    {
        if (m_audioSource)
        {
            m_audioSource.Play();
        }
    }

    public override bool Use()
    {
        return true;
    }

    public override bool AutoUse()
    {
        if(IsAutoUseAllowed())
        {
            return true;
        }

        return false;
    }

    public void SendAttack(Character_Core characterToHit)
    {
        characterToHit.ReceiveHit(AccessWeaponStat((int)EWeaponStat.ALL_DAMAGE).GetCurrent(), GetOnHitEffectsFromUpgrades());
    }
}
