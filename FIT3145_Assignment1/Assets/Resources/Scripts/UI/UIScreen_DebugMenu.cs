using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScreen_DebugMenu : UIScreenBase
{
    protected override void RegisterMethods()
    {
        
    }

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
        //DEBUG_UI_DisplaySpawnOptions();
        UI_DisplayInventory();
        UI_DisplayHands();
    }

    protected override void OnBack()
    {
        throw new System.NotImplementedException();
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
                    GamePlayManager.Instance.GetCurrentPlayer().m_playerInventory.AddItemToInventory(weapon);
                }
            }
        }
    }

    private void UI_DisplayInventoryElement_Augment(in Item item, in float x, in float y, float width, float height)
    {
        if (GUI.Button(new Rect(x, y + (height / 2), width, height / 2), "Equip to Spacebar"))
        {
            GamePlayManager.Instance.GetCurrentPlayer().AttachSpaceAugment(item as Augment);
            GamePlayManager.Instance.GetCurrentPlayer().m_playerInventory.RemoveItemFromInventory(item);
        }
    }

    private void UI_DisplayInventoryElement_Weapon(in Weapon_Base weapon, in float x, in float y, float width, float height)
    {
        for (int i = 0; i < (int)EPlayerHand.MAX; ++i)
        {
            if (GUI.Button(new Rect((x + width / 2) - (width / 2 * i), y + (height / 2), width / 2, height / 2), ((EPlayerHand)i) == EPlayerHand.HAND_RIGHT ? "Equip Right Hand" : "Equip Left Hand"))
            {
                if (GamePlayManager.Instance.GetCurrentPlayer().m_playerWeaponHolder.AttachWeaponToHand(((EPlayerHand)i), weapon))
                {
                    GamePlayManager.Instance.GetCurrentPlayer().m_playerInventory.RemoveItemFromInventory(weapon);
                }
            }
            if (GUI.Button(new Rect((x + width), y, 80, height), "Upgrade (" + weapon.GetUpgradePath().GetCurrentUpgradeIndex() + ")"))
            {
                //weapon.UpgradeWeapon();
                (UIScreen_Manager.Instance.GetUIScreen(EUIScreen.UPGRADE_MENU) as UIScreen_UpgradeMenu).SetWeaponToUpgrade(weapon);
                UIScreen_Manager.Instance.GoToUIScreen(EUIScreen.UPGRADE_MENU);
            }
        }
    }

    private void UI_DisplayInventoryElement(in Item item, in float x, in float y)
    {
        const float width = 250;
        const float height = 60;

        //display item
        GUI.Box(new Rect(x, y, width, height), item.GetComponent<Item>().GetItemName());

        //display Hand equips (For Weapons)
        switch(item.GetItemType())
        {
            case EItemType.WEAPON:
            {
                UI_DisplayInventoryElement_Weapon(item as Weapon_Base, x, y, width, height);
            }
            break;
            case EItemType.AUGMENT:
            {
                UI_DisplayInventoryElement_Augment(item, x, y, width, height);
            }
            break;
        }

    }

    public void UI_DisplayInventory()
    {
        List<Item> items = GamePlayManager.Instance.GetCurrentPlayer().m_playerInventory.AccessInventoryList();

        for (int i = 0; i < items.Count; ++i)
        {
            UI_DisplayInventoryElement(items[i], Screen.width - 400, 100 + (i * 60));
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
