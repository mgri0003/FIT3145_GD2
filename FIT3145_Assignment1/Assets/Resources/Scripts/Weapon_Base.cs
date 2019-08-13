using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon_Base : MonoBehaviour
{
    //--Variables--
    public string m_name = "Default_Weapon";
    public float m_damage = 1;


    //--methods--
    public abstract void Use();
}
