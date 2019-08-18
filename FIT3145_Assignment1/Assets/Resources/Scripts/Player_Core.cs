using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Core : MonoBehaviour
{
    //-Variables-
    [SerializeField] private float m_movementSpeed = 0.05f;

    //Components
    [HideInInspector] public Animator m_animator;
    [HideInInspector] public Player_Rotator m_playerRotator;
    [HideInInspector] public Player_WeaponHolder m_playerWeaponHolder;

    //Melee
    [SerializeField] private Hitbox m_MeleeHitbox = null; 
    
    //-Methods-

    // Start is called before the first frame update
    void Start()
    {
        InitPlayer();

        //Debug

    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement();
        UpdatePlayerRotator();

        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            PrimaryAction();
        }

        //Debug
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (m_playerWeaponHolder.IsHoldingAnyWeapon())
            {
                m_playerWeaponHolder.RemoveWeaponFromHand(EPlayerHand.HAND_RIGHT);
                m_playerWeaponHolder.RemoveWeaponFromHand(EPlayerHand.HAND_LEFT);
            }
            else
            {
                m_playerWeaponHolder.AttachWeaponToHand(EPlayerHand.HAND_RIGHT, WeaponsRepo.SpawnRandomWeapon().GetComponent<Weapon_Base>());
            }
        }

    }

    void InitPlayer()
    {
        SetupComponents();
        GetComponent<Character_Aimer>().InitBones();
        m_playerWeaponHolder.Init();
    }

    void SetupComponents()
    {
        m_animator = GetComponent<Animator>();
        Debug.Assert(m_animator != null, "Animator Is Null");

        m_playerRotator = GetComponent<Player_Rotator>();
        Debug.Assert(m_playerRotator != null, "Player Rotator Is Null");

        m_playerWeaponHolder = GetComponent<Player_WeaponHolder>();
        Debug.Assert(m_playerWeaponHolder != null, "Player Weapon Holder Is Null");
    }

    private void UpdateMovement()
    {
        float hVal = Input.GetAxis("Horizontal");
        float vVal = Input.GetAxis("Vertical");
        bool isMoving = (hVal != 0 || vVal != 0);

        if (isMoving)
        {
            transform.Translate(new Vector3(hVal, 0, vVal) * m_movementSpeed, Space.Self);
            m_playerRotator.RefreshCurrentPlayerRotation();
        }

        m_animator.SetBool("AP_isMoving", isMoving);
    }

    private void UpdatePlayerRotator()
    {
        m_playerRotator.UpdateDesiredRotation(Input.GetAxis("Mouse X"));
    }

    private void PrimaryAction()
    {
        Player_WeaponHolder m_playerWeaponHolder = GetComponent<Player_WeaponHolder>();
        if(m_playerWeaponHolder)
        {
            if (m_playerWeaponHolder.IsHoldingAnyWeapon())
            {
                Weapon_Base weapon = m_playerWeaponHolder.GetWeaponInHand(EPlayerHand.HAND_RIGHT);
                switch (weapon.GetWeaponType())
                {
                    case EWeapon_Type.MELEE:
                    {
                        UseMeleeWeapon(weapon);
                    }
                    break;
                    case EWeapon_Type.RANGED:
                    {
                        UseRangedWeapon(weapon);
                    }
                    break;
                }


            }
        }

    }

    private void SendMeleeAttack()
    {
        //Debug.Log("SendMeleeAttack()");
        Debug.Assert(m_MeleeHitbox, "Hitbox unassigned!?!?/");
        GameObject goHit = m_MeleeHitbox.GetFirstGameObjectCollided();
        if (goHit)
        {
            //boop em forward
            goHit.GetComponent<Rigidbody>().AddForce(transform.forward * 200.0f);
        }
        else
        {
            //ya missed
        }
    }

    private void UseMeleeWeapon(in Weapon_Base weaponToUse)
    {
        m_animator.Play("Attack_MeleeWeapon");
    }
    private void UseRangedWeapon(in Weapon_Base weaponToUse)
    {
        weaponToUse.Use();
    }
}
