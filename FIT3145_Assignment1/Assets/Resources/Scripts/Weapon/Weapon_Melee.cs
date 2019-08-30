using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Melee : Weapon_Base
{
    //--Methods--
    Weapon_Melee()
    {
        Constructor_InitWeaponStats();
    }

    public override bool Use()
    {
        throw new System.NotImplementedException();
    }

    public void SendAttack(Character_Core characterToHit)
    {
        characterToHit.ReceiveHit(AccessWeaponStat((int)EWeaponStat.DAMAGE).GetCurrent());
    }
}
