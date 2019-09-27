﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#pragma warning disable 649

public class UIScreen_Loadout : UIScreenBase
{
    //UI Elements
    [SerializeField] private Button m_unequipAllWeaponsButton;
    [SerializeField] private Button[] m_unequipHandButtons = new Button[(int)EPlayerHand.MAX];
    [SerializeField] private Button[] m_detachAugmentButtons = new Button[(int)EAugmentSlot.MAX];
    [SerializeField] private Image[] m_loadout_hands = new Image[(int)EPlayerHand.MAX];
    [SerializeField] private Image[] m_loadout_handFrames = new Image[(int)EPlayerHand.MAX];

    [SerializeField] private Image[] m_augmentSlots = new Image[(int)EAugmentSlot.MAX];
    [SerializeField] private Image[] m_augmentSlots_Frame = new Image[(int)EAugmentSlot.MAX];

    //refs
    [SerializeField] private Sprite m_sprite_defaultEmptyHand;
    [SerializeField] private Sprite m_sprite_defaultEmptyAugment;
    [SerializeField] private GameObject m_spawnable_itemElement;
    [SerializeField] private GameObject m_inventoryScrollViewContentGO;

    //dynamic refs
    private Player_Core m_player = null;
    private List<GameObject> m_inventoryitemElements = new List<GameObject>();
    private GameObject[] m_handItemElements = new GameObject[(int)EPlayerHand.MAX];

    protected override void RegisterMethods()
    {
        m_unequipAllWeaponsButton.onClick.AddListener(() => { UnequipAllWeapons(); });

        m_unequipHandButtons[(int)EPlayerHand.HAND_RIGHT].onClick.AddListener(() => { UnequipPlayerHand(EPlayerHand.HAND_RIGHT); });
        m_unequipHandButtons[(int)EPlayerHand.HAND_LEFT].onClick.AddListener(() => { UnequipPlayerHand(EPlayerHand.HAND_LEFT); });

        m_detachAugmentButtons[(int)EAugmentSlot.Q].onClick.AddListener(() => { OnAugmentElementDropped(EAugmentSlot.Q);});
        m_detachAugmentButtons[(int)EAugmentSlot.E].onClick.AddListener(() => { OnAugmentElementDropped(EAugmentSlot.E); });
        m_detachAugmentButtons[(int)EAugmentSlot.SPACE].onClick.AddListener(() => { OnAugmentElementDropped(EAugmentSlot.SPACE); });
        m_detachAugmentButtons[(int)EAugmentSlot.PASSIVE_1].onClick.AddListener(() => { OnAugmentElementDropped(EAugmentSlot.PASSIVE_1); });
        m_detachAugmentButtons[(int)EAugmentSlot.PASSIVE_2].onClick.AddListener(() => { OnAugmentElementDropped(EAugmentSlot.PASSIVE_2); });
    }

    protected override void OnEnable()
    {
        m_player = GamePlayManager.Instance.GetCurrentPlayer();

        RepopulateItemElementsInScrollView();
        RepopulateItemElementsInHands();
        UpdateAugmentSlotsUI();
    }

    protected override void OnDisable()
    {
        CleanUpItemElementsInScrollView();

        m_player = null;
    }

    protected override void OnGUI()
    {
        //reset colours
        for (uint i = 0; i < (uint)EPlayerHand.MAX; i++)
        {
            m_loadout_handFrames[i].color = Color.white;
        }
        for (int i = 0; i < (int)EAugmentSlot.MAX; ++i)
        {
            m_augmentSlots_Frame[i].color = Color.white;
        }

        foreach (GameObject go in m_inventoryitemElements)
        {
            UI_DragableItem dragableItem = go.GetComponentInChildren<UI_DragableItem>();
            HandleItemDragging(dragableItem);
        }
    }

    protected override void OnBack()
    {
        throw new System.NotImplementedException();
    }

    private void HandleItemDragging(UI_DragableItem dragableItem)
    {
        if (dragableItem)
        {
            if (dragableItem.IsDragging())
            {
                //Move the element!
                Rect canvasSize = UI_CanvasManager.GetCanvas().pixelRect;
                dragableItem.GetParentTransform().SetParent(UI_CanvasManager.GetCanvas().transform);
                dragableItem.GetParentTransform().localPosition = UI_CanvasManager.ConvertScreenPositionToCanvasLocalPosition(UI_CanvasManager.GetMousePositionFromScreenCentre());

                if (dragableItem.GetParentItem().GetItemType() == EItemType.WEAPON)
                {
                    //Hovering over HAND SLOTS
                    for (uint i = 0; i < (uint)EPlayerHand.MAX; i++)
                    {
                        //if your hovering over a UI hand
                        if (UI_CanvasManager.IsPointInsideRect(m_loadout_hands[i].rectTransform, dragableItem.GetParentTransform().localPosition))
                        {
                            m_loadout_handFrames[i].color = Color.green;
                        }
                    }
                }
                else
                {
                    for (uint i = 0; i < (uint)EPlayerHand.MAX; i++)
                    {
                        m_loadout_handFrames[i].color = Color.red;
                    }
                }

                if(dragableItem.GetParentItem().GetItemType() == EItemType.AUGMENT)
                {
                    Augment aug = dragableItem.GetParentItem() as Augment;

                    //Hovering over AUGMENTS
                    for (int i = 0; i < (int)EAugmentSlot.MAX; ++i)
                    {
                        EAugmentSlot augSlot = (EAugmentSlot)i;

                        if (UI_CanvasManager.IsPointInsideRect(m_augmentSlots[i].rectTransform, dragableItem.GetParentTransform().localPosition))
                        {
                            if(m_player.m_playerAugmentHandler.CanAttachAugmentToSlot(augSlot, aug))
                            {
                                m_augmentSlots_Frame[i].color = Color.green;
                            }
                            else
                            {
                                m_augmentSlots_Frame[i].color = Color.red;
                            }
                        }
                    }
                }
                else
                {
                    //display bad colours for aug slots
                    for (int i = 0; i < (int)EAugmentSlot.MAX; ++i)
                    {
                        m_augmentSlots_Frame[i].color = Color.red;
                    }
                }
            }
        }
    }

    private void OnAugmentElementDropped(in EAugmentSlot augSlot)
    {
        DetachPlayerAugment(augSlot);
        UpdateAugmentSlotsUI();
        RepopulateItemElementsInScrollView();
    }

    private void RepopulateItemElementsInHands()
    {
        CleanUpItemElementsInHands();

        if (m_player)
        {
            for(uint i = 0; i < (uint)EPlayerHand.MAX; ++i)
            {
                if(m_player.m_playerWeaponHolder.IsHoldingWeaponInHand((EPlayerHand)i))
                {
                    Item weaponItem = m_player.m_playerWeaponHolder.GetWeaponInHand((EPlayerHand)i);
                    PopulateItemElement(weaponItem, m_loadout_hands[i].transform);
                }
            }
        }

        RepositionItemElementsInHands();
    }

    private void RepopulateItemElementsInScrollView()
    {
        CleanUpItemElementsInScrollView();

        if(m_player)
        {
            foreach (Item item in m_player.m_playerInventory.AccessInventoryList())
            {
                PopulateItemElement(item, m_inventoryScrollViewContentGO.transform);
            }
        }

        RepositionItemElementsInScrollView();
    }

    private void PopulateItemElement(in Item item, in Transform parentTransform)
    {
        GameObject go = Instantiate(m_spawnable_itemElement);

        go.transform.SetParent(parentTransform, false);

        go.GetComponentInChildren<UI_DragableItem>().m_delegate_OnDrop = OnItemElementDropped;
        go.GetComponentInChildren<UI_DragableItem>().m_delegate_OnHoverEnter = UIScreen_Manager.Instance.CreateItemToolTip;
        go.GetComponentInChildren<UI_DragableItem>().m_delegate_OnHoverExit = UIScreen_Manager.Instance.DestroyItemToolTip;
        go.GetComponentInChildren<UI_DragableItem>().m_delegate_OnPointerDown = GoToWeaponUpgradeScreen;
        go.GetComponentInChildren<UI_DragableItem>().SetParentItem(item);

        m_inventoryitemElements.Add(go);
    }

    private void RemoveItemElement(GameObject itemElement)
    {
        if(m_inventoryitemElements.Contains(itemElement))
        {
            m_inventoryitemElements.Remove(itemElement);
            Destroy(itemElement);
        }
    }

    private void CleanUpItemElementsInHands()
    {
        for (uint i = 0; i < (uint)EPlayerHand.MAX; i++)
        {
            if(m_handItemElements[i])
            {
                Destroy(m_handItemElements[i]);
            }
        }
    }

    private void CleanUpItemElementsInScrollView()
    {
        while (m_inventoryitemElements.Count > 0)
        {
            Destroy(m_inventoryitemElements[0]);
            m_inventoryitemElements.RemoveAt(0);
        }
    }

    private void RepositionItemElementsInScrollView()
    {
        float upgradePositionY = 0;
        foreach (GameObject go in m_inventoryitemElements)
        {
            float sizeY = 0.0f;
            UI_DragableItem dragableItem = go.GetComponentInChildren<UI_DragableItem>();
            if (dragableItem)
            {
                RectTransform rectTransform = dragableItem.GetComponent<RectTransform>();
                if (rectTransform)
                {
                    sizeY = rectTransform.rect.height * 1.20f;
                }
            }

            go.transform.localPosition = new Vector3(100, -sizeY - upgradePositionY, 0);


            upgradePositionY += sizeY;
        }
    }

    private void RepositionItemElementsInHands()
    {
        for(uint i = 0; i < m_handItemElements.Length; ++i)
        {
            if(m_handItemElements[i])
            {
                m_handItemElements[i].transform.localPosition = Vector3.zero;
            }
        }
    }

    private void OnItemElementDropped(UI_DragableItem dragableItem, PointerEventData eventData)
    {
        if(dragableItem.GetParentItem().GetItemType() == EItemType.WEAPON)
        {
            bool weaponItemSlotted = false;

            //if this weapon was originally equipped to a hand
            for (uint i = 0; i < (uint)EPlayerHand.MAX; i++)
            {
                if (m_player.m_playerWeaponHolder.GetWeaponInHand((EPlayerHand)i) == (dragableItem.GetParentItem() as Weapon_Base))
                {
                    m_player.SimpleUnequipWeapon((EPlayerHand)i);
                }
            }

            //if item dropped on HAND SLOT
            for (uint i = 0; i < (uint)EPlayerHand.MAX; i++)
            {
                if(UI_CanvasManager.IsPointInsideRect(m_loadout_hands[i].rectTransform, dragableItem.GetParentTransform().localPosition))
                {
                    weaponItemSlotted = true;

                    //if a weapon is already being held, its about to get replaced, so detach it and put it back into inventory
                    if (m_player.m_playerWeaponHolder.IsHoldingWeaponInHand((EPlayerHand)i))
                    {
                        m_player.SimpleUnequipWeapon((EPlayerHand)i);
                    }

                    m_player.SimpleEquipWeapon(dragableItem.GetParentItem(), (EPlayerHand)i);

                    break;
                }
            }

            if (!weaponItemSlotted)
            {
                //set the parent of the dragable element back to the scroll view
                dragableItem.GetParentTransform().SetParent(m_inventoryScrollViewContentGO.transform);
            }
        }

        if (dragableItem.GetParentItem().GetItemType() == EItemType.AUGMENT)
        {
            for (uint i = 0; i < (uint)EAugmentSlot.MAX; i++)
            {
                EAugmentSlot augSlot = (EAugmentSlot)i;
                Augment aug = dragableItem.GetParentItem() as Augment;

                //if item dropped on AUGMENT SLOT
                if (UI_CanvasManager.IsPointInsideRect(m_augmentSlots[i].rectTransform, dragableItem.GetParentTransform().localPosition))
                {
                    if (m_player.m_playerAugmentHandler.CanAttachAugmentToSlot(augSlot, aug))
                    {
                        //if a augment is already slotted in slot, its about to get replaced, so detach it and put it back into inventory
                        if (m_player.m_playerAugmentHandler.HasAugment(augSlot))
                        {
                            DetachPlayerAugment(augSlot);
                        }

                        //attach augment
                        m_player.m_playerAugmentHandler.AttachAugment(augSlot, aug);

                        //remove the augment from the inventory!
                        m_player.m_playerInventory.RemoveItemFromInventory(dragableItem.GetParentItem());

                        break;
                    }
                }
            }

            //update ui of augments
            UpdateAugmentSlotsUI();
        }

        //Repopulate the elements in scroll view
        RepopulateItemElementsInScrollView();
        RepopulateItemElementsInHands();

        //reset hand frames
        for (uint i = 0; i < (uint)EPlayerHand.MAX; i++)
        {
            //reset UI hand frame colour
            m_loadout_handFrames[i].color = Color.white;
        }
    }

    private void UnequipAllWeapons()
    {
        for (uint i = 0; i < (uint)EPlayerHand.MAX; i++)
        {
            m_player.SimpleUnequipWeapon((EPlayerHand)i);
        }

        RepopulateItemElementsInScrollView();
    }

    private void UnequipPlayerHand(in EPlayerHand hand)
    {
        m_player.SimpleUnequipWeapon(hand);
        RepopulateItemElementsInScrollView();
    }

    private void DetachPlayerAugment(in EAugmentSlot augmentSlot)
    {
        if(m_player.m_playerAugmentHandler.HasAugment(augmentSlot))
        {
            Augment augment = m_player.m_playerAugmentHandler.GetAugment(augmentSlot);
            if(augment)
            {
                //detach augment from augment slot
                m_player.m_playerAugmentHandler.DetachAugment(augmentSlot);

                //put augment back into inventory
                m_player.m_playerInventory.AddItemToInventory(augment);
            }
        }
    }

    private void UpdateAugmentSlotsUI()
    {
        if(m_player)
        {
            for (uint i = 0; i < (uint)EAugmentSlot.MAX; i++)
            {
                if (m_player.m_playerAugmentHandler.HasAugment((EAugmentSlot)i))
                {
                    m_augmentSlots[i].sprite = m_player.m_playerAugmentHandler.GetAugment((EAugmentSlot)i).GetItemSprite();
                }
                else
                {
                    m_augmentSlots[i].sprite = m_sprite_defaultEmptyAugment;
                }
            }
        }
    }

    private void GoToWeaponUpgradeScreen(UI_DragableItem dragableItem, PointerEventData eventData)
    {
        //check if a right click occurred
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            //Only a weapon can go to the upgrade screen
            if (dragableItem.GetParentItem().GetItemType() == EItemType.WEAPON)
            {
                //find out if the weapon dropped was originall equipped
                bool wasWeaponEquipped = false;
                EPlayerHand whichHandEquipped = EPlayerHand.MAX;
                for (uint i = 0; i < (uint)EPlayerHand.MAX; i++)
                {
                    if (m_player.m_playerWeaponHolder.GetWeaponInHand((EPlayerHand)i) == (dragableItem.GetParentItem() as Weapon_Base))
                    {
                        wasWeaponEquipped = true;
                        whichHandEquipped = (EPlayerHand)i;
                        break;
                    }
                }

                (UIScreen_Manager.Instance.GetUIScreen(EUIScreen.UPGRADE_MENU) as UIScreen_UpgradeMenu).InitUpgradeMenuData(dragableItem.GetParentItem() as Weapon_Base, wasWeaponEquipped, whichHandEquipped);
                UIScreen_Manager.Instance.GoToUIScreen(EUIScreen.UPGRADE_MENU);
            }
        }
    }
}
