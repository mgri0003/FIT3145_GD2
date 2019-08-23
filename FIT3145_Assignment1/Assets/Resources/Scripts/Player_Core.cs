using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Core : Character_Core
{
    //-Variables-
    [SerializeField] private float m_movementSpeed = 0.05f;

    //Components
    [HideInInspector] public Player_Rotator m_playerRotator;
    [HideInInspector] public Player_WeaponHolder m_playerWeaponHolder;

    //Melee
    [SerializeField] private Hitbox m_MeleeHitbox = null;

    //Camera

    //-Methods-

    private void OnGUI()
    {
        GUI.Box(new Rect(0, 0, 100, 30), "Health: " + m_characterStats.GetHealth());
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        InitPlayer();
    }

    protected override void SetupComponents()
    {
        base.SetupComponents();

        m_playerRotator = GetComponent<Player_Rotator>();
        Debug.Assert(m_playerRotator != null, "Player Rotator Is Null");

        m_playerWeaponHolder = GetComponent<Player_WeaponHolder>();
        Debug.Assert(m_playerWeaponHolder != null, "Player Weapon Holder Is Null");
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

        if (Input.GetKeyDown(KeyCode.R))
        {
            //Reload
            m_playerWeaponHolder.ReloadRangedWeapons();
        }

        //Debug
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (m_playerWeaponHolder.IsHoldingAnyWeapon())
            {
                m_playerWeaponHolder.RemoveWeaponFromHand(EPlayerHand.HAND_RIGHT);
                m_playerWeaponHolder.RemoveWeaponFromHand(EPlayerHand.HAND_LEFT);
            }
            else
            {
                m_playerWeaponHolder.AttachWeaponToHand(EPlayerHand.HAND_RIGHT, WeaponsRepo.SpawnWeapon(0).GetComponent<Weapon_Base>());
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (m_playerWeaponHolder.IsHoldingAnyWeapon())
            {
                m_playerWeaponHolder.RemoveWeaponFromHand(EPlayerHand.HAND_RIGHT);
                m_playerWeaponHolder.RemoveWeaponFromHand(EPlayerHand.HAND_LEFT);
            }
            else
            {
                m_playerWeaponHolder.AttachWeaponToHand(EPlayerHand.HAND_RIGHT, WeaponsRepo.SpawnWeapon(1).GetComponent<Weapon_Base>());
            }
        }

    }

    void InitPlayer()
    {
        m_playerWeaponHolder.Init();
        GetComponent<Character_Aimer>().Init(Camera_Main.GetMainCamera().GetAimTarget());
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

    public void SubtleMove()
    {
        //If your not in the middle of a transition
        //If your not already in the Move State
        if (!m_animator.IsInTransition(0) && !m_animator.GetCurrentAnimatorStateInfo(0).IsName("Move"))
        {
            //play the move state sublty :P
            m_animator.CrossFade("Move", 0.05f);
        }
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
        if (goHit && goHit.CompareTag("Character"))
        {
            Weapon_Melee meleeWeaponR = null;
            Weapon_Melee meleeWeaponL = null;

            if (m_playerWeaponHolder.IsHoldingWeaponInHandOfType(EPlayerHand.HAND_RIGHT, EWeapon_Type.MELEE))
            {
                meleeWeaponR = (Weapon_Melee)m_playerWeaponHolder.GetWeaponInHand(EPlayerHand.HAND_RIGHT);
            }
            if (m_playerWeaponHolder.IsHoldingWeaponInHandOfType(EPlayerHand.HAND_LEFT, EWeapon_Type.MELEE))
            {
                meleeWeaponL = (Weapon_Melee)m_playerWeaponHolder.GetWeaponInHand(EPlayerHand.HAND_LEFT);
            }
            if (meleeWeaponR)
            {
                meleeWeaponR.SendAttack(goHit.GetComponent<Character_Core>());
            }
            if (meleeWeaponL)
            {
                meleeWeaponL.SendAttack(goHit.GetComponent<Character_Core>());
            }
        }
    }

    private void UseMeleeWeapon(in Weapon_Base weaponToUse)
    {
        m_animator.Play("Attack_MeleeWeapon", 1);
    }
    private void UseRangedWeapon(in Weapon_Base weaponToUse)
    {
        m_playerWeaponHolder.UpdateRangedWeaponAim();
        if(weaponToUse.Use())
        {
            m_animator.Play("Attack_RangedWeapon", 1, 0.0f);
        }
    }
}
