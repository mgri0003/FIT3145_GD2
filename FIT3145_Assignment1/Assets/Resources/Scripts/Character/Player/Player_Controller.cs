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
        if(GetInputEnabled() && !m_player.IsDead())
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                m_player.PrimaryAction();
            }
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                m_player.SecondaryAction();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                //Reload
                m_player.m_playerWeaponHolder.ReloadRangedWeapons();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                //Reload
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

            m_player.m_playerRotator.AddDesiredRotation(Input.GetAxis("Mouse X"));
            Camera_Main.GetMainCamera().MinusCurrentRotationY(Input.GetAxis("Mouse Y"));

            if (Input.GetMouseButtonDown(2))
            {
                Camera_Main.GetMainCamera().CycleCameraViewMode();
            }
        }
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
