using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScreen_DebugMenu : UIScreenBase
{
    protected override void OnEnable()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    protected override void OnDisable()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    protected override void OnGUI()
    {
        if(GUI.Button(new Rect(100,50, 250,40), "Spawn Melee Weapon In Left Hand"))
        {
            DEBUG_SpawnWeaponInHand(EPlayerHand.HAND_LEFT, 0);
        }
        if (GUI.Button(new Rect(350, 50, 250, 40), "Spawn Melee Weapon In Right Hand"))
        {
            DEBUG_SpawnWeaponInHand(EPlayerHand.HAND_RIGHT, 0);
        }

        if (GUI.Button(new Rect(100, 90, 250, 40), "Spawn Ranged Weapon In Left Hand"))
        {
            DEBUG_SpawnWeaponInHand(EPlayerHand.HAND_LEFT, 1);
        }
        if (GUI.Button(new Rect(350, 90, 250, 40), "Spawn Ranged Weapon In Right Hand"))
        {
            DEBUG_SpawnWeaponInHand(EPlayerHand.HAND_RIGHT, 1);
        }
    }

    public void DEBUG_SpawnWeaponInHand(in EPlayerHand hand, in uint weaponID)
    {
        if (GamePlayManager.Instance.GetCurrentPlayer().m_playerWeaponHolder.IsHoldingWeaponInHand(hand))
        {
            GamePlayManager.Instance.GetCurrentPlayer().m_playerWeaponHolder.DetachWeaponFromHand(hand);
        }
        else
        {
            GamePlayManager.Instance.GetCurrentPlayer().m_playerWeaponHolder.AttachWeaponToHand(hand, WeaponsRepo.SpawnWeapon(weaponID).GetComponent<Weapon_Base>());
        }
    }
}
