using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon_Base : MonoBehaviour
{
    //--Variables--
    public float m_damage = 1;


    //--methods--
    public abstract void Use();
}
