using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#pragma warning disable 649

public class UIScreen_DebugMenu : UIScreenBase
{
    //UI Elements
    [SerializeField] private Button m_unequipAllWeaponsButton;
    [SerializeField] private Button[] m_unequipHandButtons = new Button[(int)EPlayerHand.MAX];
    [SerializeField] private Button[] m_detachAugmentButtons = new Button[(int)EAugmentSlot.MAX];
    [SerializeField] private Image[] m_loadout_hands = new Image[(int)EPlayerHand.MAX];
    [SerializeField] private Image[] m_loadout_handFrames = new Image[(int)EPlayerHand.MAX];
    [SerializeField] private Image m_upgradeSection;
    [SerializeField] private Image m_upgradeSectionFrame;

    [SerializeField] private Image[] m_augmentSlots = new Image[(int)EAugmentSlot.MAX];
    [SerializeField] private Image[] m_augmentSlots_Frame = new Image[(int)EAugmentSlot.MAX];

    //refs
    [SerializeField] private Sprite m_sprite_defaultEmptyHand;
    [SerializeField] private Sprite m_sprite_defaultEmptyAugment;
    [SerializeField] private GameObject m_spawnable_itemElement;
    [SerializeField] private GameObject m_inventoryScrollViewContentGO;

    //dynamic refs
    private Player_Core m_player = null;
    private List<GameObject> m_itemElements = new List<GameObject>();

    protected override void RegisterMethods()
    {
        m_unequipAllWeaponsButton.onClick.AddListener(() => { UnequipAllWeapons(); });

        m_unequipHandButtons[(int)EPlayerHand.HAND_RIGHT].onClick.AddListener(() => { UnequipPlayerHand(EPlayerHand.HAND_RIGHT); });
        m_unequipHandButtons[(int)EPlayerHand.HAND_LEFT].onClick.AddListener(() => { UnequipPlayerHand(EPlayerHand.HAND_LEFT); });



        m_detachAugmentButtons[(int)EAugmentSlot.Q].onClick.AddListener(() => { DetachPlayerAugment(EAugmentSlot.Q); UpdateAugmentSlots(); RepopulateItemElementsInScrollView(); });
        m_detachAugmentButtons[(int)EAugmentSlot.E].onClick.AddListener(() => { DetachPlayerAugment(EAugmentSlot.E); UpdateAugmentSlots(); RepopulateItemElementsInScrollView(); });
        m_detachAugmentButtons[(int)EAugmentSlot.SPACE].onClick.AddListener(() => { DetachPlayerAugment(EAugmentSlot.SPACE); UpdateAugmentSlots(); RepopulateItemElementsInScrollView(); });
        m_detachAugmentButtons[(int)EAugmentSlot.PASSIVE_1].onClick.AddListener(() => { DetachPlayerAugment(EAugmentSlot.PASSIVE_1); UpdateAugmentSlots(); RepopulateItemElementsInScrollView(); });
        m_detachAugmentButtons[(int)EAugmentSlot.PASSIVE_2].onClick.AddListener(() => { DetachPlayerAugment(EAugmentSlot.PASSIVE_2); UpdateAugmentSlots(); RepopulateItemElementsInScrollView(); });
    }

    protected override void OnEnable()
    {
        m_player = GamePlayManager.Instance.GetCurrentPlayer();

        RepopulateItemElementsInScrollView();
        UpdateLoadoutUIHands();
        UpdateAugmentSlots();

        m_upgradeSectionFrame.color = Color.white;
    }

    protected override void OnDisable()
    {
        m_player = null;
    }

    protected override void OnGUI()
    {
        //reset colours
        m_upgradeSectionFrame.color = Color.white;
        for (uint i = 0; i < (uint)EPlayerHand.MAX; i++)
        {
            m_loadout_handFrames[i].color = Color.white;
        }
        for (int i = 0; i < (int)EAugmentSlot.MAX; ++i)
        {
            m_augmentSlots_Frame[i].color = Color.white;
        }

        foreach (GameObject go in m_itemElements)
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

                    //Hovering over UPGRADE SECTION
                    if (UI_CanvasManager.IsPointInsideRect(m_upgradeSection.rectTransform, dragableItem.GetParentTransform().localPosition))
                    {
                        m_upgradeSectionFrame.color = Color.green;
                    }
                }
                else
                {
                    for (uint i = 0; i < (uint)EPlayerHand.MAX; i++)
                    {
                        m_loadout_handFrames[i].color = Color.red;
                    }
                    m_upgradeSectionFrame.color = Color.red;
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
                            if(CanAugmentBeSlotted(augSlot, aug.GetAugmentType()))
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
        if(dragableItem.GetParentItem().GetItemType() == EItemType.WEAPON)
        {
            bool weaponItemSlotted = false;

            //if item dropped on HAND SLOT
            for (uint i = 0; i < (uint)EPlayerHand.MAX; i++)
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

            //if item dropped on UPGRADE SECTION
            if (UI_CanvasManager.IsPointInsideRect(m_upgradeSection.rectTransform, dragableItem.GetParentTransform().localPosition))
            {
                (UIScreen_Manager.Instance.GetUIScreen(EUIScreen.UPGRADE_MENU) as UIScreen_UpgradeMenu).SetWeaponToUpgrade(dragableItem.GetParentItem() as Weapon_Base);
                UIScreen_Manager.Instance.GoToUIScreen(EUIScreen.UPGRADE_MENU);
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
                    if(CanAugmentBeSlotted(augSlot, aug.GetAugmentType()))
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
            UpdateAugmentSlots();
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

    private void UnequipPlayerHand(in EPlayerHand hand)
    {
        UnequipWeapon(hand);
        RepopulateItemElementsInScrollView();
        UpdateLoadoutUIHands();
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

    private void UpdateAugmentSlots()
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

    private bool CanAugmentBeSlotted(in EAugmentSlot augSlot, in EAugmentType augType)
    {
        return (augType == EAugmentType.ACTIVE && augSlot < EAugmentSlot.PASSIVE_1) || (augType == EAugmentType.PASSIVE && augSlot >= EAugmentSlot.PASSIVE_1);
    }
}
