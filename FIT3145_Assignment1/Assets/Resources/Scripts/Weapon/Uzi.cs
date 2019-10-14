using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Uzi : Weapon_Ranged
{
    protected override void FireRangedWeapon()
    {
        FireSingleProjectile();
        PlayShootSound();
    }
}
