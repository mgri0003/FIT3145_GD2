using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Core : Character_Core
{
    //-Variables-
    private float m_movementValueHorizontal = 0.0f;
    private float m_movementValueVertical = 0.0f;

    //Components
    [HideInInspector] public Player_Rotator m_playerRotator;
    [HideInInspector] public Player_WeaponHolder m_playerWeaponHolder;
    [HideInInspector] public Player_Inventory m_playerInventory;
    [SerializeField] public Hitbox m_playerItemPickupArea;


    //-Methods-

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

        m_playerInventory = GetComponent<Player_Inventory>();
        Debug.Assert(m_playerInventory != null, "Player Inventory Is Null");

        Debug.Assert(m_playerItemPickupArea != null, "Player Item Pickup Are Is Null");
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement();
    }

    void InitPlayer()
    {
        m_playerWeaponHolder.Init();
        m_characterAimer.Init(Camera_Main.GetMainCamera().GetAimTarget());
    }

    private void UpdateMovement()
    {
        bool isMoving = (m_movementValueHorizontal != 0 || m_movementValueVertical != 0);

        if (isMoving)
        {
            MoveCharacter(new Vector3(m_movementValueHorizontal, 0, m_movementValueVertical), Space.Self);
            m_playerRotator.RefreshCurrentPlayerRotation();
        }

        m_animator.SetBool("AP_isMoving", isMoving);
        ResetMovementValues();
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

    public void SetMovementValues(in float horizontal, in float vertical)
    {
        m_movementValueHorizontal = horizontal;
        m_movementValueVertical = vertical;
    }

    public void ResetMovementValues()
    {
        m_movementValueHorizontal = 0;
        m_movementValueVertical = 0;
    }

    public void PrimaryAction()
    {
        ExecuteHandAction(EPlayerHand.HAND_RIGHT);
    }
    public void SecondaryAction()
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
                case EWeaponType.MELEE:
                {
                    UseMeleeWeapon(weapon, hand);
                }
                break;
                case EWeaponType.RANGED:
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

            if (m_playerWeaponHolder.IsHoldingWeaponInHandOfType(hand, EWeaponType.MELEE))
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

    public bool IsItemNearby()
    {
        return m_playerItemPickupArea.IsColliding();
    }
    
    public void PickupNearbyItems()
    {
        if(IsItemNearby())
        {
            //add items to inventory
            m_playerInventory.AddItemsToInventory(m_playerItemPickupArea.GetAllGameObjectsCollided());
        }
    }
}
