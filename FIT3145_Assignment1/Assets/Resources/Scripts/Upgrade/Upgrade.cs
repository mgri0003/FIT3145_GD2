using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Upgrade : Item
{
    Weapon_Base m_parentWeapon;
    public void SetParentWeapon(Weapon_Base newWeapon)
    {
        m_parentWeapon = newWeapon;
    }

    [SerializeField] Effect m_onHitEffect = null;
    public Effect GetOnHitEffect() { return m_onHitEffect; }

    //public abstract void OnUpgradeWeaponUsed();
    //public abstract void OnUpgradeAttached();


}
