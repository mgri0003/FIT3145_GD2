using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Melee : Weapon_Base
{
    //--Methods--
    public override bool Use()
    {
        foreach (Upgrade up in m_currentUpgrades)
        {
            up.OnUpgradeWeaponUsed();
        }

        return true;
    }

    public void SendAttack(Character_Core characterToHit)
    {
        characterToHit.ReceiveHit(AccessWeaponStat((int)EWeaponStat.ALL_DAMAGE).GetCurrent(), GetOnHitEffectsFromUpgrades());
    }
}
