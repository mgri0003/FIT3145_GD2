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
    [SerializeField] private LayerMask m_aimMask;
    private const float m_MINIMUM_AIM_RANGE = 2.0f;

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

    public void UpdateRangedWeaponAim()
    {
        RaycastHit rayHit;
        Physics.Raycast(Camera_Main.GetMainCamera().transform.position, Camera_Main.GetMainCamera().transform.forward, out rayHit, 100.0f, m_aimMask);
        for (uint i = 0; i < (uint)EPlayerHand.MAX; ++i)
        {
            if (IsHoldingWeaponInHandOfType((EPlayerHand)i, EWeapon_Type.RANGED))
            {
                Vector3 lookAtPosition = Vector3.zero;
                //if a target is hit by the raycast
                if (rayHit.transform && rayHit.transform.root != transform.root && rayHit.distance > m_MINIMUM_AIM_RANGE)
                {
                    //set the lookAt position to the position of the raycast hit.
                    lookAtPosition = rayHit.point;
                }
                else
                {
                    //default the look at pos to the normal aim target position;
                    lookAtPosition = GetComponent<Character_Aimer>().GetAimTarget().position;
                }

                //set the ranged weapon's look at pos
                ((Weapon_Ranged)GetWeaponInHand((EPlayerHand)i)).SetWeaponLookAt(lookAtPosition);
            }
        }
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

    public bool IsHoldingWeaponInHandOfType(in EPlayerHand hand, in EWeapon_Type weaponType)
    {
        bool retVal = false;

        if (IsHoldingWeaponInHand(hand))
        {
            if(GetWeaponInHand(hand).GetWeaponType() == weaponType)
            {
                retVal = true;
            }
        }

        return retVal;
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

            //set the current weapons rotation to point towards the aim target
            m_currentWeapons[(int)hand].transform.LookAt(GetComponent<Character_Aimer>().GetAimTarget());

            //disable physics for weapon when in hand
            m_currentWeapons[(int)hand].SetPhysicsActive(false);
        }
    }

    public void DetachWeaponFromHand(in EPlayerHand hand)
    {
        Debug.Assert(hand == EPlayerHand.HAND_RIGHT || hand == EPlayerHand.HAND_LEFT, "Invalid Hand Type Passed In");

        //if a weapon is assigned to this hand
        if(m_currentWeapons[(int)hand])
        {
            //remove parent
            m_currentWeapons[(int)hand].transform.SetParent(null);

            //enable physics for weapon when in hand
            m_currentWeapons[(int)hand].SetPhysicsActive(true);

            //remove weapon from current weapons
            m_currentWeapons[(int)hand] = null;
        }

    }

    public Weapon_Base GetWeaponInHand(in EPlayerHand hand)
    {
        return m_currentWeapons[(int)hand];
    }

    public void ReloadRangedWeapons()
    {
        for (uint i = 0; i < (uint)EPlayerHand.MAX; ++i)
        {
            if (IsHoldingWeaponInHandOfType((EPlayerHand)i, EWeapon_Type.RANGED))
            {
                ((Weapon_Ranged)GetWeaponInHand((EPlayerHand)i)).Reload();
            }
        }
    }
}
