using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegeneratingMagazine : Upgrade
{
    private const int BULLET_REGEN = 1;
    private const int REGEN_TIME_INTERVAL = 2;
    private float m_regenTime = REGEN_TIME_INTERVAL;

    public override string GetItemTypeDescription()
    {
        return "Regenerates " + BULLET_REGEN + " ammo every " + REGEN_TIME_INTERVAL + " seconds";
    }

    protected override void ApplyUpgradeSettings()
    {
        
    }

    protected override void OnUpgradeUpdate()
    {
        if(m_regenTime > 0.0f)
        {
            m_regenTime -= Time.deltaTime;
        }

        if(m_regenTime <= 0.0f)
        {
            m_regenTime = REGEN_TIME_INTERVAL;

            RegenAmmo();
        }
    }

    protected override void ResetUpgradeSettings()
    {
        
    }

    private void RegenAmmo()
    {
        (GetParentWeapon() as Weapon_Ranged).AddCurrentAmmo(1);
    }

}
