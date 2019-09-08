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

    [SerializeField] GameObject m_spawnable_particleEffect = null;
    private GameObject m_particleEffect = null;

    public void SetParticleEffectScale(float scaleValue)
    {
        if(m_particleEffect)
        {
            m_particleEffect.transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
        }
    }


    private void Awake()
    {
        if(m_spawnable_particleEffect)
        {
            m_particleEffect = Instantiate(m_spawnable_particleEffect);
            m_particleEffect.GetComponent<PositionFollower>().SetTransformToFollow(transform);
        }
    }

    //public abstract void OnUpgradeWeaponUsed();
    //public abstract void OnUpgradeAttached();


}
