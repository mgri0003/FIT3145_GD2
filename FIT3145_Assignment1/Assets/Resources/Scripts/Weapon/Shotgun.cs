using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Weapon_Ranged
{
    const float NUMBER_OF_BULLETS = 6;
    const float BULLET_LIFETIME = 0.4f;

    protected override void FireRangedWeapon()
    {
        for(int i = 0; i < NUMBER_OF_BULLETS; ++i)
        {
            float randX = Random.Range(0, 1) == 1 ? -0.15f : 0.15f;
            float randY = Random.Range(0, 1) == 1 ? -0.15f : 0.15f;
            GameObject projGO = FireSingleProjectile(false, new Vector3(randX + Random.Range(-1.0f , 1.0f), randY + Random.Range(-1.0f, 1.0f), 0));
            projGO.GetComponent<Weapon_Projectile>().SetLifeTime(BULLET_LIFETIME);
        }

        ConsumeAmmo(1);
    }
}
