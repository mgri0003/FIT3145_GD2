using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScreen_InGameHud : UIScreenBase
{
    //--variables--//
    private Player_Core m_player = null;

    //--Methods--//
    protected override void OnEnable()
    {
        m_player = GamePlayManager.Instance.GetCurrentPlayer();
        if(m_player)
        {
            m_player.GetComponent<Player_Controller>().SetEnableInput(true);
        }
    }

    protected override void OnDisable()
    {
        if(m_player)
        {
            m_player.GetComponent<Player_Controller>().SetEnableInput(false);
        }
    }

    protected override void OnGUI()
    {
        GUI.Box(new Rect(0, 0, 100, 30), "Health: " + m_player.m_characterStats.AccessHealthStat().GetCurrent());

        UI_DisplayWeaponForHand(EPlayerHand.HAND_RIGHT);
        UI_DisplayWeaponForHand(EPlayerHand.HAND_LEFT);
        UI_DisplayPickupable();
    }

    private void UI_DisplayPickupable()
    {
        if (m_player.IsItemNearby())
        {
            GUI.Box(new Rect(Screen.width / 2 - 100, Screen.height / 2, 200, 30), "Pick Nearby Item By Pressing 'E'");
        }
    }

    private void UI_DisplayWeaponForHand(in EPlayerHand hand)
    {
        bool isRightLeftHand = (hand == EPlayerHand.HAND_RIGHT);

        string handDisplay = isRightLeftHand ? "Right Hand: " : "Left Hand: ";
        string weaponDisplay = "Empty";
        Weapon_Base weapon = m_player.m_playerWeaponHolder.GetWeaponInHand(hand);
        if (weapon)
        {
            weaponDisplay = weapon.GetItemName();
            if (weapon.GetWeaponType() == EWeaponType.RANGED)
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

    public void SetPlayer(in Player_Core newPlayer)
    {
        m_player = newPlayer;
    }
}
