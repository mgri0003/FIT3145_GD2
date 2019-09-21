using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Melee : Weapon_Base
{
    //--Methods--
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
