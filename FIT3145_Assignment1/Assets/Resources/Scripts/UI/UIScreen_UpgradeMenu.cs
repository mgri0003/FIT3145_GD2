using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

#pragma warning disable 649

public class UIScreen_UpgradeMenu : UIScreenBase
{
    //ui elements
    [SerializeField] private UnityEngine.UI.Button m_backButton;
    [SerializeField] private UnityEngine.UI.Button m_removeAllUpgradesButton;

    //refs
    [SerializeField] Canvas m_canvas;
    [SerializeField] private Transform m_WeaponDisplay;
    [SerializeField] private GameObject m_upgradeScrollViewContentGO;
    [SerializeField] private GameObject m_spawnable_upgradeElement;
    [SerializeField] private UnityEngine.UI.Image m_upgradeApplyArea;

    //dynamic refs
    private Player_Core m_player = null;
    private List<GameObject> m_upgradeElements = new List<GameObject>();
    private Weapon_Base m_weaponToUpgrade = null;

    //constants
    const float m_displayWeaponSize = 400.0f;
    const float m_displayWeaponRotationSpeed = 0.15f;

    public void SetWeaponToUpgrade(Weapon_Base newWeapon)
    {
        m_weaponToUpgrade = newWeapon;
    }

    protected override void RegisterMethods()
    {
        m_backButton.onClick.AddListener(() => { OnBack(); });
        m_removeAllUpgradesButton.onClick.AddListener(() => { RemoveAllAttachedUpgrades(); });
    }

    protected override void OnEnable()
    {
        m_player = GamePlayManager.Instance.GetCurrentPlayer();
        if(m_player)
        {
            //Debug.Assert(m_weaponToUpgrade, "Weapon To Upgrade is null, set it before you enter this screen!");
            SetupWeaponDisplay();

            //Populate upgrade scroll view
            RepopulateUpgradeElementsInScrollView();
        }
    }

    protected override void OnDisable()
    {
        RemoveWeaponDisplay();
        CleanUpUpgradeElementsInScrollView();

        m_weaponToUpgrade = null;
        m_player = null;
    }

    protected override void OnGUI()
    {
        //rotate weapon for pzaz
        if(m_weaponToUpgrade)
        {
            m_weaponToUpgrade.transform.Rotate(Vector3.up, m_displayWeaponRotationSpeed);
        }

        //check which upgrade element is being dragged
        m_upgradeApplyArea.color = Color.white;
        foreach (GameObject go in m_upgradeElements)
        {
            UI_DragableItem dragableItem = go.GetComponentInChildren<UI_DragableItem>();
            if(dragableItem)
            {
                if(dragableItem.IsDragging())
                {
                    Rect canvasSize = m_canvas.pixelRect;
                    dragableItem.GetParentTransform().SetParent(m_canvas.transform);
                    dragableItem.GetParentTransform().localPosition = Input.mousePosition - new Vector3(canvasSize.width / 2, canvasSize.height / 2, 0.0f);

                    if(IsInsideUpgradeApplyArea(Input.mousePosition))
                    {
                        m_upgradeApplyArea.color = Color.green;
                    }
                    else
                    {
                        m_upgradeApplyArea.color = Color.white;
                    }
                }
            }
        }
    }

    protected override void OnBack()
    {
        UIScreen_Manager.Instance.GoToUIScreen(EUIScreen.DEBUG_MENU);
    }

    private void SetupWeaponDisplay()
    {
        if (m_weaponToUpgrade)
        {
            m_weaponToUpgrade.transform.SetParent(m_WeaponDisplay);
            m_weaponToUpgrade.transform.localPosition = Vector3.zero;
            m_weaponToUpgrade.transform.localScale = new Vector3(1, 1, 1) * m_displayWeaponSize;
            m_weaponToUpgrade.transform.rotation = m_WeaponDisplay.rotation;

            m_weaponToUpgrade.SetAllAttachedUpgradeParticleEffects(0.2f);
        }
    }

    private void RemoveWeaponDisplay()
    {
        if (m_weaponToUpgrade)
        {
            m_weaponToUpgrade.transform.SetParent(null);
            m_weaponToUpgrade.transform.localScale = new Vector3(1, 1, 1);
            m_weaponToUpgrade.MoveItemToInventoryZone();

            m_weaponToUpgrade.SetAllAttachedUpgradeParticleEffects(1.0f);
        }
    }

    private void RepopulateUpgradeElementsInScrollView()
    {
        CleanUpUpgradeElementsInScrollView();

        foreach (Item item in m_player.m_playerInventory.AccessInventoryList())
        {
            if (item.GetItemType() == EItemType.UPGRADE)
            {
                //Debug.Log("Upgrade Element Created");
                GameObject go = Instantiate(m_spawnable_upgradeElement);

                go.transform.SetParent(m_upgradeScrollViewContentGO.transform, false);

                go.GetComponentInChildren<UI_DragableItem>().m_delegate_OnDrop = OnUpgradeElementDropped;
                go.GetComponentInChildren<UI_DragableItem>().SetParentItem(item);

                m_upgradeElements.Add(go);
            }
        }

        RepositionUpgradeElementsInScrollView();
    }

    private void RepositionUpgradeElementsInScrollView()
    {
        float upgradePositionY = 0;
        foreach(GameObject go in m_upgradeElements)
        {
            go.transform.localPosition = new Vector3(100, -30 - upgradePositionY, 0);

            upgradePositionY += 50.0f;
        }
    }

    private void CleanUpUpgradeElementsInScrollView()
    {
        while(m_upgradeElements.Count > 0)
        {
            Destroy(m_upgradeElements[0]);
            m_upgradeElements.RemoveAt(0);
        }
    }

    private void RemoveAllAttachedUpgrades()
    {
        List<Upgrade> originalUpgrades = new List<Upgrade>();
        originalUpgrades.AddRange(m_weaponToUpgrade.AccessCurrentUpgrades());

        m_weaponToUpgrade.RemoveAllUpgrades();

        foreach (Upgrade up in originalUpgrades)
        {
            m_player.m_playerInventory.AddItemToInventory(up);
        }

        RepopulateUpgradeElementsInScrollView();
    }

    private void OnUpgradeElementDropped(UI_DragableItem dragableItem, PointerEventData eventData)
    {
        //Debug.Log("OnUpgradeElementReleased");
        if (IsInsideUpgradeApplyArea(Input.mousePosition))
        {
            Upgrade newUpgrade = dragableItem.GetParentItem() as Upgrade;
            if(newUpgrade)
            {
                if(m_weaponToUpgrade.AddUpgrade(newUpgrade))
                {
                    //Debug.Log("Added Upgrade!");

                    //fix up scale of upgrades
                    newUpgrade.transform.localScale = new Vector3(1, 1, 1);

                    m_player.m_playerInventory.RemoveItemFromInventory(newUpgrade);
                    m_upgradeElements.Remove(dragableItem.transform.parent.gameObject);

                    SetupWeaponDisplay();
                }
            }
        }

        //set the parent of the dragable element back to the scroll view
        dragableItem.GetParentTransform().SetParent(m_upgradeScrollViewContentGO.transform);

        //reposition the elements in scroll view
        RepositionUpgradeElementsInScrollView();
    }

    private bool IsInsideUpgradeApplyArea(Vector2 pos)
    {
        //Reference : https://stackoverflow.com/questions/40566250/unity-recttransform-contains-point?rq=1

        // Get the rectangular bounding box of your UI element
        Rect rect = m_upgradeApplyArea.rectTransform.rect;
        //convert position to bottom left being 0,0
        Rect canvasSize = m_canvas.pixelRect;
        pos -= new Vector2(canvasSize.width / 2, canvasSize.height / 2);
        
        // Get the left, right, top, and bottom boundaries of the rect
        float leftSide = m_upgradeApplyArea.rectTransform.anchoredPosition.x - rect.width / 2;
        float rightSide = m_upgradeApplyArea.rectTransform.anchoredPosition.x + rect.width / 2;
        float topSide = m_upgradeApplyArea.rectTransform.anchoredPosition.y + rect.height / 2;
        float bottomSide = m_upgradeApplyArea.rectTransform.anchoredPosition.y - rect.height / 2;
        
        //Debug.Log(leftSide + ", " + rightSide + ", " + topSide + ", " + bottomSide + " | " + pos.x + ", " + pos.y);
        
        //Check to see if the point is in the calculated bounds
        if (pos.x >= leftSide && pos.x <= rightSide &&
            pos.y >= bottomSide && pos.y <= topSide)
        {
            return true;
        }

        return false;
    }


}
