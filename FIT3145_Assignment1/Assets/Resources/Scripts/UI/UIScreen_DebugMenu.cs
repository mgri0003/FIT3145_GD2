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
        DEBUG_UI_DisplaySpawnOptions();
        UI_DisplayInventory();
        UI_DisplayHands();
    }

    private void UI_DisplayHands()
    {
        for (uint i = 0; i < (uint)EPlayerHand.MAX; ++i)
        {
            Weapon_Base weapon = GamePlayManager.Instance.GetCurrentPlayer().m_playerWeaponHolder.GetWeaponInHand((EPlayerHand)i);

            string handDisplay = "Empty Hand";
            if(weapon)
            {
                handDisplay = weapon.GetItemName();
            }

            if (GUI.Button(new Rect(300 - (150 * i), 300, 150, 150), handDisplay))
            {
                if(GamePlayManager.Instance.GetCurrentPlayer().m_playerWeaponHolder.IsHoldingWeaponInHand((EPlayerHand)i))
                {
                    GamePlayManager.Instance.GetCurrentPlayer().m_playerWeaponHolder.DetachWeaponFromHand((EPlayerHand)i);
                    GamePlayManager.Instance.GetCurrentPlayer().m_playerInventory.AddItemToInventory(weapon.gameObject);
                }
            }
        }
    }

    private bool UI_DisplayInventoryElement(in GameObject item, in float x, in float y)
    {
        if (GUI.Button(new Rect(x, y, 200, 60), item.GetComponent<Item>().GetItemName()))
        {
            if (GamePlayManager.Instance.GetCurrentPlayer().m_playerWeaponHolder.AttachWeaponToHand(EPlayerHand.HAND_RIGHT, item.GetComponent<Weapon_Base>()))
            {
                GamePlayManager.Instance.GetCurrentPlayer().m_playerInventory.RemoveItemFromInventory(item);
                return true;
            }
        }

        return false;
    }

    public void UI_DisplayInventory()
    {
        List<GameObject> items = GamePlayManager.Instance.GetCurrentPlayer().m_playerInventory.AccessInventoryList();

        for (int i = 0; i < items.Count; ++i)
        {
            UI_DisplayInventoryElement(items[i], Screen.width - 300, 100 + (i * 60));
        }
    }

    private void DEBUG_UI_DisplaySpawnOptions()
    {
        if (GUI.Button(new Rect(100, 50, 250, 40), "Spawn Melee Weapon In Left Hand"))
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
