using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EPlayerHand
{
    HAND_RIGHT,
    HAND_LEFT,
    MAX
}

public class Player_WeaponHolder : MonoBehaviour
{
    //--Variables--
    private Transform[] m_handTransforms = new Transform[(int)EPlayerHand.MAX];
    private Weapon_Base[] m_currentWeapons = new Weapon_Base[(int)EPlayerHand.MAX];

    //--Methods--

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Init()
    {
        //Setup Hand Bones
        m_handTransforms[(int)EPlayerHand.HAND_RIGHT] = GetComponent<Player_Core>().m_animator.GetBoneTransform(HumanBodyBones.RightHand);
        m_handTransforms[(int)EPlayerHand.HAND_LEFT] = GetComponent<Player_Core>().m_animator.GetBoneTransform(HumanBodyBones.LeftHand);
    }

    public bool IsHoldingWeaponInHand(in EPlayerHand hand)
    {
        Debug.Assert(hand == EPlayerHand.HAND_RIGHT || hand == EPlayerHand.HAND_LEFT, "Invalid Hand Type Passed In");
        if (hand == EPlayerHand.HAND_RIGHT)
        {
            return m_currentWeapons[(int)EPlayerHand.HAND_RIGHT] != null;
        }
        else if (hand == EPlayerHand.HAND_LEFT)
        {
            return m_currentWeapons[(int)EPlayerHand.HAND_LEFT] != null;
        }

        return false;
    }

    public bool IsHoldingAnyWeapon()
    {
        return IsHoldingWeaponInHand(EPlayerHand.HAND_RIGHT) || IsHoldingWeaponInHand(EPlayerHand.HAND_LEFT);
    }

    public void AttachWeaponToHand(in EPlayerHand hand, in Weapon_Base weapon)
    {
        Debug.Assert(hand == EPlayerHand.HAND_RIGHT || hand == EPlayerHand.HAND_LEFT, "Invalid Hand Type Passed In");
        Debug.Assert(weapon, "Weapon Can't be Null!");
        if(weapon)
        {
            //parent the weapon to the hand
            weapon.transform.SetParent(m_handTransforms[(int)hand]);
            
            //set the current weapon on the hand
            m_currentWeapons[(int)hand] = weapon;
            m_currentWeapons[(int)hand].transform.localPosition = new Vector3(0, 0, 0);
        }
    }

    public void RemoveWeaponFromHand(in EPlayerHand hand)
    {
        Debug.Assert(hand == EPlayerHand.HAND_RIGHT || hand == EPlayerHand.HAND_LEFT, "Invalid Hand Type Passed In");

        //if a weapon is assigned to this hand
        if(m_currentWeapons[(int)hand])
        {
            //remove parent
            m_currentWeapons[(int)hand].transform.SetParent(null);

            //remove weapon from current weapons
            m_currentWeapons[(int)hand] = null;
        }

    }
}
