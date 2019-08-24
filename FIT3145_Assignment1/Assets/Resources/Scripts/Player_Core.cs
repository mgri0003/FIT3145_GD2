using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Core : Character_Core
{
    //-Variables-

    //Components
    [HideInInspector] public Player_Rotator m_playerRotator;
    [HideInInspector] public Player_WeaponHolder m_playerWeaponHolder;

    //Camera

    //-Methods-

    private void OnGUI()
    {
        GUI.Box(new Rect(0, 0, 100, 30), "Health: " + m_characterStats.GetHealth());

        UI_DisplayWeaponForHand(EPlayerHand.HAND_RIGHT);
        UI_DisplayWeaponForHand(EPlayerHand.HAND_LEFT);
    }
    private void UI_DisplayWeaponForHand(in EPlayerHand hand)
    {
        bool isRightLeftHand = (hand == EPlayerHand.HAND_RIGHT);

        string handDisplay = isRightLeftHand ? "Right Hand: " : "Left Hand: ";
        string weaponDisplay = "Empty";
        Weapon_Base weapon = m_playerWeaponHolder.GetWeaponInHand(hand);
        if (weapon)
        {
            weaponDisplay = weapon.GetWeaponName();
            if (weapon.GetWeaponType() == EWeapon_Type.RANGED)
            {
                if (((Weapon_Ranged)weapon).IsReloading())
                {
                    weaponDisplay += "| Reloading!";
                }
                else
                {
                    weaponDisplay += "| Ammo(" + ((Weapon_Ranged)weapon).GetCurrentAmmo() + ")";
                }
            }
        }
        GUI.Box(new Rect(0, isRightLeftHand ? 30 : 60, 300, 30), handDisplay + weaponDisplay);
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
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            SecondaryAction();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            //Reload
            m_playerWeaponHolder.ReloadRangedWeapons();
        }

        //Debug
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            DEBUG_SpawnWeaponInHand(EPlayerHand.HAND_RIGHT, 0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            DEBUG_SpawnWeaponInHand(EPlayerHand.HAND_LEFT, 0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            DEBUG_SpawnWeaponInHand(EPlayerHand.HAND_RIGHT, 1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            DEBUG_SpawnWeaponInHand(EPlayerHand.HAND_LEFT, 1);
        }

    }

    public void DEBUG_SpawnWeaponInHand(in EPlayerHand hand, in uint weaponID)
    {
        if (m_playerWeaponHolder.IsHoldingWeaponInHand(hand))
        {
            m_playerWeaponHolder.DetachWeaponFromHand(hand);
        }
        else
        {
            m_playerWeaponHolder.AttachWeaponToHand(hand, WeaponsRepo.SpawnWeapon(weaponID).GetComponent<Weapon_Base>());
        }
    }

    void InitPlayer()
    {
        m_playerWeaponHolder.Init();
        m_characterAimer.Init(Camera_Main.GetMainCamera().GetAimTarget());
    }
    
    private void UpdateMovement()
    {
        float hVal = Input.GetAxis("Horizontal");
        float vVal = Input.GetAxis("Vertical");
        bool isMoving = (hVal != 0 || vVal != 0);

        if (isMoving)
        {
            MoveCharacter(new Vector3(hVal, 0, vVal), Space.Self);
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
        ExecuteHandAction(EPlayerHand.HAND_RIGHT);
    }
    private void SecondaryAction()
    {
        ExecuteHandAction(EPlayerHand.HAND_LEFT);
    }

    private void ExecuteHandAction(in EPlayerHand hand)
    {
        if (m_playerWeaponHolder.IsHoldingWeaponInHand(hand))
        {
            Weapon_Base weapon = m_playerWeaponHolder.GetWeaponInHand(hand);
            switch (weapon.GetWeaponType())
            {
                case EWeapon_Type.MELEE:
                {
                    UseMeleeWeapon(weapon, hand);
                }
                break;
                case EWeapon_Type.RANGED:
                {
                    UseRangedWeapon(weapon, hand);
                }
                break;
            }
        }
    }

    protected override void SendMeleeAttack(in int AE_handIndex)
    {
        GameObject goHit = m_MeleeHitbox.GetFirstGameObjectCollided();
        if (goHit && goHit.CompareTag("Character"))
        {
            EPlayerHand hand = AE_handIndex == 1 ? EPlayerHand.HAND_LEFT : EPlayerHand.HAND_RIGHT;
            Weapon_Melee meleeWeapon = null;

            if (m_playerWeaponHolder.IsHoldingWeaponInHandOfType(hand, EWeapon_Type.MELEE))
            {
                meleeWeapon = (Weapon_Melee)m_playerWeaponHolder.GetWeaponInHand(hand);

                if (meleeWeapon)
                {
                    meleeWeapon.SendAttack(goHit.GetComponent<Character_Core>());
                }
            }
        }
    }

    private void UseMeleeWeapon(in Weapon_Base weaponToUse, in EPlayerHand hand)
    {
        uint animLayer = hand == EPlayerHand.HAND_RIGHT ? 1u : 2u;
        m_animator.Play("Attack_MeleeWeapon", (int)animLayer);
    }
    private void UseRangedWeapon(in Weapon_Base weaponToUse, in EPlayerHand hand)
    {
        uint animLayer = hand == EPlayerHand.HAND_RIGHT ? 1u : 2u;

        m_playerWeaponHolder.UpdateRangedWeaponAim();
        if(weaponToUse.Use())
        {
            m_animator.Play("Attack_RangedWeapon", (int)animLayer, 0.0f);
        }
    }
}
