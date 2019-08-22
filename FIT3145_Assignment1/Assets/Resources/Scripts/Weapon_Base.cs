using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EWeapon_Type
{
    MELEE,
    RANGED,
    NONE,
    MAX
}

public abstract class Weapon_Base : MonoBehaviour
{
    //--Variables--
    [SerializeField] private string m_name = "Default_Weapon";
    [SerializeField] private float m_damage = 1;
    [SerializeField] private EWeapon_Type m_weaponType = EWeapon_Type.NONE;

    //--methods--
    public abstract bool Use();


    //Getters
    public string GetWeaponName() { return m_name; }
    public float GetWeaponDamage() { return m_damage; }
    public EWeapon_Type GetWeaponType() { return m_weaponType; }

}
