using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Weapon_Ranged
{
    const float NUMBER_OF_BULLETS = 6;

    protected override void FireRangedWeapon()
    {
        for(int i = 0; i < NUMBER_OF_BULLETS; ++i)
        {
            float randX = Random.Range(0, 1) == 1 ? -0.25f : 0.25f;
            float randY = Random.Range(0, 1) == 1 ? -0.25f : 0.25f;
            GameObject projGO = FireSingleProjectile(false, new Vector3(randX + Random.Range(-0.5f , 0.5f), randY + Random.Range(-0.5f, 0.5f), 0));
        }

        AddCurrentAmmo(-1);

        PlayShootSound();
    }
}
