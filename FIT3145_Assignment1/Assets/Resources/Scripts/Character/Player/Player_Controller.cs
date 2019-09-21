using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    private Player_Core m_player = null;
    private bool m_inputEnabled = false;


    private void Awake()
    {
        m_player = GetComponent<Player_Core>();
        Debug.Assert(m_player, "Player_Controller | Awake() | Player is missing Player_Core component!?!?");
    }

    // Update is called once per frame
    void Update()
    {
        if(GetInputEnabled())
        {
            if(!m_player.IsDead())
            {
                if (GameOptions.GetSwapRightLeftClick() ? Input.GetKeyDown(KeyCode.Mouse0) : Input.GetKeyDown(KeyCode.Mouse1))
                {
                    m_player.PrimaryAction();
                }
                if (GameOptions.GetSwapRightLeftClick() ? Input.GetKeyDown(KeyCode.Mouse1) : Input.GetKeyDown(KeyCode.Mouse0))
                {
                    m_player.SecondaryAction();
                }
                if (GameOptions.GetSwapRightLeftClick() ? Input.GetKey(KeyCode.Mouse0) : Input.GetKey(KeyCode.Mouse1))
                {
                    m_player.AutoPrimaryAction();
                }
                if (GameOptions.GetSwapRightLeftClick() ? Input.GetKey(KeyCode.Mouse1) : Input.GetKey(KeyCode.Mouse0))
                {
                    m_player.AutoSecondaryAction();
                }


                //Reload
                if (Input.GetKeyDown(KeyCode.R))
                {
                    m_player.m_playerWeaponHolder.ReloadRangedWeapons();
                }

                //Pickup items
                if (Input.GetKeyDown(KeyCode.F))
                {
                    m_player.PickupNearbyItems();
                }

                if (Input.GetKeyDown(KeyCode.Q))
                {
                    m_player.m_playerAugmentHandler.UseAugment(EAugmentSlot.Q);
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    m_player.m_playerAugmentHandler.UseAugment(EAugmentSlot.E);
                }
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    m_player.m_playerAugmentHandler.UseAugment(EAugmentSlot.SPACE);
                }

                m_player.SetMovementValues(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                m_player.SetDirectionInput(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

                //camera input
                if (Input.GetMouseButtonDown(2))
                {
                    Camera_Main.GetMainCamera().CycleCameraViewMode();
                }

                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    Camera_Main.GetMainCamera().SetForceZoom(true);
                }
                else if (Input.GetKeyUp(KeyCode.LeftShift))
                {
                    Camera_Main.GetMainCamera().SetForceZoom(false);
                }
            }
        }


        //loadout
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (UIScreen_Manager.Instance.GetCurrentUIScreenAsEnum() == EUIScreen.LOADOUT_MENU)
            {
                UIScreen_Manager.Instance.GoToUIScreen(EUIScreen.INGAME_HUD);
            }
            else if (!m_player.IsDead())
            {
                UIScreen_Manager.Instance.GoToUIScreen(EUIScreen.LOADOUT_MENU);
            }
        }

        //updated desired rotation (should work whilst dead)
        if(IsHUDActive())
        {
            m_player.m_playerRotator.AddDesiredRotation(Input.GetAxis("Mouse X"));
            Camera_Main.GetMainCamera().MinusCurrentRotationY(Input.GetAxis("Mouse Y"));
        }

        //freecam mode
        if (Input.GetKeyDown(KeyCode.C))
        {
            m_player.m_playerRotator.SetDisabledRotation(!m_player.m_playerRotator.GetDisabledRotation());
            SetEnableInput(!m_player.m_playerRotator.GetDisabledRotation());
        }
    }

    public bool IsHUDActive()
    {
        return UIScreen_Manager.Instance.GetCurrentUIScreenAsEnum() == EUIScreen.INGAME_HUD;
    }

    public void SetEnableInput(in bool enabled)
    {
        m_inputEnabled = enabled;
    }
    public bool GetInputEnabled()
    {
        return m_inputEnabled;
    }
}
