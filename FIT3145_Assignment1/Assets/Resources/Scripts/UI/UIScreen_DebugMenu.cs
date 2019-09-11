using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIScreen_DebugMenu : UIScreenBase
{
    //UI Elements
    [SerializeField] private Button m_unequipAllWeaponsButton;
    [SerializeField] private Image[] m_loadout_hands = new Image[(int)EPlayerHand.MAX];
    [SerializeField] private Image[] m_loadout_handFrames = new Image[(int)EPlayerHand.MAX];

    //refs
    [SerializeField] private Sprite m_sprite_defaultEmptyHand;
    [SerializeField] private GameObject m_spawnable_itemElement;
    [SerializeField] private GameObject m_inventoryScrollViewContentGO;

    //dynamic refs
    private Player_Core m_player = null;
    private List<GameObject> m_itemElements = new List<GameObject>();

    protected override void RegisterMethods()
    {
        m_unequipAllWeaponsButton.onClick.AddListener(() => { UnequipAllWeapons(); });
    }

    protected override void OnEnable()
    {
        m_player = GamePlayManager.Instance.GetCurrentPlayer();

        RepopulateItemElementsInScrollView();
        UpdateLoadoutUIHands();
    }

    protected override void OnDisable()
    {
        m_player = null;
    }

    protected override void OnGUI()
    {
        foreach (GameObject go in m_itemElements)
        {
            UI_DragableItem dragableItem = go.GetComponentInChildren<UI_DragableItem>();
            if (dragableItem)
            {
                if (dragableItem.IsDragging())
                {
                    //Move the element!
                    Rect canvasSize = UI_CanvasManager.GetCanvas().pixelRect;
                    dragableItem.GetParentTransform().SetParent(UI_CanvasManager.GetCanvas().transform);
                    dragableItem.GetParentTransform().localPosition = UI_CanvasManager.ConvertScreenPositionToCanvasLocalPosition(UI_CanvasManager.GetMousePositionFromScreenCentre());

                    if(dragableItem.GetParentItem().GetItemType() == EItemType.WEAPON)
                    {
                        for (uint i = 0; i < (uint)EPlayerHand.MAX; i++)
                        {
                            //if your hovering over a UI hand
                            if (UI_CanvasManager.IsPointInsideRect(m_loadout_hands[i].rectTransform, dragableItem.GetParentTransform().localPosition))
                            {
                                m_loadout_handFrames[i].color = Color.green;
                            }
                            else //if your not hovering over a UI Hand
                            {
                                //reset UI hand frame colour
                                m_loadout_handFrames[i].color = Color.white;
                            }
                        }
                    }
                }
            }
        }
    }

    protected override void OnBack()
    {
        throw new System.NotImplementedException();
    }

    private void RepopulateItemElementsInScrollView()
    {
        CleanUpItemElementsInScrollView();

        if(m_player)
        {
            foreach (Item item in m_player.m_playerInventory.AccessInventoryList())
            {
                //if (item.GetItemType() == EItemType.UPGRADE)
                {
                    GameObject go = Instantiate(m_spawnable_itemElement);

                    go.transform.SetParent(m_inventoryScrollViewContentGO.transform, false);

                    go.GetComponentInChildren<UI_DragableItem>().m_delegate_OnDrop = OnItemElementDropped;
                    go.GetComponentInChildren<UI_DragableItem>().SetParentItem(item);

                    m_itemElements.Add(go);
                }
            }
        }

        RepositionItemElementsInScrollView();
    }

    private void RemoveItemElement(GameObject itemElement)
    {
        if(m_itemElements.Contains(itemElement))
        {
            m_itemElements.Remove(itemElement);
            Destroy(itemElement);
        }
    }

    private void CleanUpItemElementsInScrollView()
    {
        while (m_itemElements.Count > 0)
        {
            Destroy(m_itemElements[0]);
            m_itemElements.RemoveAt(0);
        }
    }

    private void RepositionItemElementsInScrollView()
    {
        float upgradePositionY = 0;
        foreach (GameObject go in m_itemElements)
        {
            go.transform.localPosition = new Vector3(100, -30 - upgradePositionY, 0);

            upgradePositionY += 50.0f;
        }
    }

    private void OnItemElementDropped(UI_DragableItem dragableItem, PointerEventData eventData)
    {
        bool weaponItemSlotted = false;
        if(dragableItem.GetParentItem().GetItemType() == EItemType.WEAPON)
        {
            for(uint i = 0; i < (uint)EPlayerHand.MAX; i++)
            {
                if(UI_CanvasManager.IsPointInsideRect(m_loadout_hands[i].rectTransform, dragableItem.GetParentTransform().localPosition))
                {
                    weaponItemSlotted = true;

                    //if a weapon is already being held, its about to get replaced, so detach it and put it back into inventory
                    if (m_player.m_playerWeaponHolder.IsHoldingWeaponInHand((EPlayerHand)i))
                    {
                        UnequipWeapon((EPlayerHand)i);
                    }
                    
                    //attach new weapon to hand!
                    m_player.m_playerWeaponHolder.AttachWeaponToHand((EPlayerHand)i, dragableItem.GetParentItem() as Weapon_Base);

                    //remove the weapon from the inventory!
                    m_player.m_playerInventory.RemoveItemFromInventory(dragableItem.GetParentItem());

                    //remove the UI item element
                    RemoveItemElement(dragableItem.GetParentTransform().gameObject);
                }
            }
        }

        if(!weaponItemSlotted)
        {
            //set the parent of the dragable element back to the scroll view
            dragableItem.GetParentTransform().SetParent(m_inventoryScrollViewContentGO.transform);
        }

        //Repopulate the elements in scroll view
        RepopulateItemElementsInScrollView();

        //update ui hands
        UpdateLoadoutUIHands();

        //reset hand frames
        for (uint i = 0; i < (uint)EPlayerHand.MAX; i++)
        {
            //reset UI hand frame colour
            m_loadout_handFrames[i].color = Color.white;
        }
    }

    private void UpdateLoadoutUIHands()
    {
        if(m_player)
        {
            for (uint i = 0; i < (uint)EPlayerHand.MAX; i++)
            {
                //set hand sprite to default hand sprite
                m_loadout_hands[i].sprite = m_sprite_defaultEmptyHand;

                //if there is a weapon equipped, set the hand sprite to the weapon sprite
                if (m_player.m_playerWeaponHolder.IsHoldingWeaponInHand((EPlayerHand)i))
                {
                    Weapon_Base weapon = m_player.m_playerWeaponHolder.GetWeaponInHand((EPlayerHand)i);
                    if (weapon)
                    {
                        m_loadout_hands[i].sprite = weapon.GetItemSprite();
                    }
                }
            }
        }
    }

    private void UnequipWeapon(in EPlayerHand hand)
    {
        if (m_player.m_playerWeaponHolder.IsHoldingWeaponInHand(hand))
        {
            Weapon_Base weapon = m_player.m_playerWeaponHolder.GetWeaponInHand(hand);

            //remove weapon from hand
            m_player.m_playerWeaponHolder.DetachWeaponFromHand(hand);

            //add weapon back into inventory
            if (weapon)
            {
                m_player.m_playerInventory.AddItemToInventory(weapon);
            }
        }
    }

    private void UnequipAllWeapons()
    {
        for (uint i = 0; i < (uint)EPlayerHand.MAX; i++)
        {
            UnequipWeapon((EPlayerHand)i);
        }

        RepopulateItemElementsInScrollView();

        UpdateLoadoutUIHands();
    }

    /*
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
            if (GUI.Button(new Rect((x + width), y, 80, height), "Upgrade (" + weapon.GetImprovementPath().GetCurrentImprovementIndex() + ")"))
            {
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
    */
}
