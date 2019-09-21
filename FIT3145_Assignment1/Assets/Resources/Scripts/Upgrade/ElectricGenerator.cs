using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricGenerator : Upgrade
{
    private const float m_DefaultOnHitDamage = 20;
    public float m_OnHitDamage = m_DefaultOnHitDamage;

    public override string GetItemTypeDescription()
    {
        return "Deal " + (m_OnHitDamage * GetBalanceScale()).ToString() + " Damage On-Hit";
    }

    protected override void ApplyUpgradeSettings()
    {
        RecalculateDamageBasedOffBalanceScale();
    }

    protected override void ResetUpgradeSettings()
    {
        m_OnHitDamage = m_DefaultOnHitDamage;
    }

    private void RecalculateDamageBasedOffBalanceScale()
    {
        m_OnHitDamage = (m_DefaultOnHitDamage * GetBalanceScale());
        (GetOnHitEffect() as Zap).m_zapDamage = m_OnHitDamage;
    }
}
