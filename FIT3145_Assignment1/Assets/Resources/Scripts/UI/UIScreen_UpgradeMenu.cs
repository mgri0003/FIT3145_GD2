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
    [SerializeField] private UnityEngine.UI.Button m_upgradeButton;
    [SerializeField] private UnityEngine.UI.Text m_weaponInformationText;
    [SerializeField] private UnityEngine.UI.Text m_weaponInformationText_Upgrades;

    //refs
    [SerializeField] Canvas m_canvas;
    [SerializeField] private Transform m_WeaponDisplay;
    [SerializeField] private GameObject m_upgradeScrollViewContentGO;
    [SerializeField] private Transform m_upgradeSegmentsListTransform;
    [SerializeField] private GameObject m_spawnable_upgradeElement;
    [SerializeField] private GameObject m_spawnable_upgradeSegment;
    [SerializeField] private UnityEngine.UI.Image m_upgradeApplyArea;

    //dynamic refs
    private Player_Core m_player = null;
    private List<GameObject> m_upgradeElements = new List<GameObject>();
    private List<GameObject> m_upgradeSegments = new List<GameObject>();
    private Weapon_Base m_weaponToUpgrade = null;

    //constants
    const float m_displayWeaponSize = 400.0f;
    const float m_displayWeaponRotationSpeed = 10.00f;

    public void SetWeaponToUpgrade(Weapon_Base newWeapon)
    {
        m_weaponToUpgrade = newWeapon;
    }

    protected override void RegisterMethods()
    {
        m_backButton.onClick.AddListener(() => { OnBack(); });
        m_removeAllUpgradesButton.onClick.AddListener(() => { RemoveAllAttachedUpgrades(); });
        m_upgradeButton.onClick.AddListener(() => {
            m_weaponToUpgrade.ImproveWeapon();
            RefreshUpgradeSegments();
            RefreshWeaponInformation();
        });
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

            //populate the upgrade segments
            PopulateUpgradeSegments();

            //update the weapon information
            RefreshWeaponInformation();
        }
    }

    protected override void OnDisable()
    {
        RemoveWeaponDisplay();
        CleanUpUpgradeElementsInScrollView();
        CleanUpUpgradeSegments();

        m_weaponToUpgrade = null;
        m_player = null;
    }

    protected override void OnGUI()
    {
        //rotate weapon for pzaz
        if (m_weaponToUpgrade)
        {
            m_weaponToUpgrade.transform.Rotate(Vector3.up, m_displayWeaponRotationSpeed * Time.deltaTime);
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

                    dragableItem.GetParentTransform().localPosition = UI_CanvasManager.ConvertScreenPositionToCanvasLocalPosition(UI_CanvasManager.GetMousePositionFromScreenCentre());

                    if(IsInsideUpgradeApplyArea(UI_CanvasManager.ConvertScreenPositionToCanvasLocalPosition(UI_CanvasManager.GetMousePositionFromScreenCentre())))
                    {
                        if(m_weaponToUpgrade.CanAddUpgrade())
                        {
                            m_upgradeApplyArea.color = Color.green;
                        }
                        else
                        {
                            m_upgradeApplyArea.color = Color.red;
                        }
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

            m_weaponToUpgrade.SetAllAttachedUpgradeParticleEffectsScale(0.2f);
        }
    }

    private void RemoveWeaponDisplay()
    {
        if (m_weaponToUpgrade)
        {
            m_weaponToUpgrade.transform.SetParent(null);
            m_weaponToUpgrade.transform.localScale = new Vector3(1, 1, 1);
            m_weaponToUpgrade.MoveItemToInventoryZone();

            m_weaponToUpgrade.SetAllAttachedUpgradeParticleEffectsScale(1.0f);
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
        RefreshWeaponInformation();
    }

    private void OnUpgradeElementDropped(UI_DragableItem dragableItem, PointerEventData eventData)
    {
        if (IsInsideUpgradeApplyArea(UI_CanvasManager.ConvertScreenPositionToCanvasLocalPosition(UI_CanvasManager.GetMousePositionFromScreenCentre())))
        {
            Upgrade newUpgrade = dragableItem.GetParentItem() as Upgrade;
            if(newUpgrade)
            {
                if(m_weaponToUpgrade.AddUpgrade(newUpgrade))
                {
                    //fix up scale of upgrade visually
                    newUpgrade.transform.localScale = new Vector3(1, 1, 1);

                    //remove the upgrade from the inventory!
                    m_player.m_playerInventory.RemoveItemFromInventory(newUpgrade);

                    //remove the UI upgrade element
                    m_upgradeElements.Remove(dragableItem.transform.parent.gameObject);

                    //reset the weapon display!
                    SetupWeaponDisplay();

                    //refresh weapon info!!
                    RefreshWeaponInformation();
                }
            }
        }

        //set the parent of the dragable element back to the scroll view
        dragableItem.GetParentTransform().SetParent(m_upgradeScrollViewContentGO.transform);

        //reposition the elements in scroll view
        RepositionUpgradeElementsInScrollView();

        //refresh the weapon info just incase
        RefreshWeaponInformation();
    }

    private bool IsInsideUpgradeApplyArea(Vector2 pos)
    {
        //Reference : https://stackoverflow.com/questions/40566250/unity-recttransform-contains-point?rq=1

        // Get the rectangular bounding box of your UI element
        Rect rect = m_upgradeApplyArea.rectTransform.rect;

        //convert position to bottom left being 0,0
        // canvasSize = m_canvas.pixelRect;
        //pos -= new Vector2(canvasSize.width / 2, canvasSize.height / 2);
        
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

    private void PopulateUpgradeSegments()
    {
        if (m_spawnable_upgradeSegment)
        {
            float positionY = 0.0f;
            foreach (ImprovementSegment upSeg in m_weaponToUpgrade.GetImprovementPath().GetImprovementSegments())
            {
                //Debug.Log("Upgrade Element Created");
                GameObject go = Instantiate(m_spawnable_upgradeSegment);
                go.transform.SetParent(m_upgradeSegmentsListTransform.transform, false);
                go.transform.localPosition = new Vector3(0, positionY, 0);
                m_upgradeSegments.Add(go);
                positionY -= 35.0f;
            }

            RefreshUpgradeSegments();
        }
    }

    private void RefreshUpgradeSegments()
    {
        int upgradeIndex = 0;
        foreach (GameObject upSegGO in m_upgradeSegments)
        {
            //Debug.Log("Upgrade Element Created");
            UI_ImprovementSegment uiUpSeg = upSegGO.GetComponent<UI_ImprovementSegment>();
            ImprovementSegment upSeg = m_weaponToUpgrade.GetImprovementPath().GetImprovementSegments()[upgradeIndex];
            if (uiUpSeg)
            {
                uiUpSeg.AccessImprovementSegmentText().text = (upSeg.StatToImprove != EWeaponStat.MAX) ? (m_weaponToUpgrade.AccessWeaponStat(upSeg.StatToImprove).GetName() + ((upSeg.Value > 0) ? (" +") : (" ") ) + upSeg.Value.ToString()) : "";

                if (upSeg.UpgradeSlotIncrease)
                {
                    uiUpSeg.AccessImprovementSegmentText().text += ((uiUpSeg.AccessImprovementSegmentText().text == "") ? "" : " | " ) + "Upgrade Slot +1";
                }

                if (upSeg.ScrapCost > 0)
                {
                    uiUpSeg.AccessImprovementSegmentCostText().text = upSeg.ScrapCost.ToString();
                }
                else
                {
                    uiUpSeg.AccessImprovementSegmentCostText().text = "FREE";
                }

                if(m_weaponToUpgrade.GetImprovementPath().GetCurrentImprovementIndex() >= upgradeIndex)
                {
                    uiUpSeg.SetUISegmentOpacity(1.0f);

                    if(m_weaponToUpgrade.GetImprovementPath().GetCurrentImprovementIndex() > upgradeIndex)
                    {
                        uiUpSeg.AccessImprovementSegmentText().color = Color.green;
                        uiUpSeg.AccessImprovementSegmentCostText().color = Color.green;
                    }
                }
                else
                {
                    uiUpSeg.SetUISegmentOpacity(0.1f);
                }
            }
            ++upgradeIndex;
        }

        //reposition Upgrade Button to latest position
        if(m_weaponToUpgrade.GetImprovementPath().GetCurrentImprovementIndex() < m_weaponToUpgrade.GetImprovementPath().GetImprovementSegmentCount())
        {
            m_upgradeButton.gameObject.SetActive(true);

            m_upgradeButton.transform.localPosition = new Vector3(
                m_upgradeSegmentsListTransform.localPosition.x - 50.0f, 
                m_upgradeSegmentsListTransform.transform.localPosition.y - (0.0f) - (m_weaponToUpgrade.GetImprovementPath().GetCurrentImprovementIndex() * 35.0f), 
                0);
        }
        else
        {
            m_upgradeButton.gameObject.SetActive(false);
        }
    }

    private void CleanUpUpgradeSegments()
    {
        while (m_upgradeSegments.Count > 0)
        {
            Destroy(m_upgradeSegments[0]);
            m_upgradeSegments.RemoveAt(0);
        }
    }

    private void RefreshWeaponInformation()
    {
        if(m_weaponInformationText && m_weaponInformationText_Upgrades)
        {
            //weapon name & stats
            m_weaponInformationText.text =  "Name: " + m_weaponToUpgrade.GetItemName();
            m_weaponInformationText.text += "\n";

            for(int i = 0; i < (int)EWeaponStat.MAX; ++i)
            {
                bool isMeleeStat = ((EWeaponStat)i).ToString().Contains("MELEE_");
                bool isRangedStat = ((EWeaponStat)i).ToString().Contains("RANGED_");

                if( ((EWeaponStat)i).ToString().Contains("ALL_") 
                    || (m_weaponToUpgrade.GetWeaponType() == EWeaponType.MELEE && isMeleeStat) 
                    || (m_weaponToUpgrade.GetWeaponType() == EWeaponType.RANGED && isRangedStat))
                {
                    Stat stat = m_weaponToUpgrade.AccessWeaponStat((EWeaponStat)i);
                    m_weaponInformationText.text += stat.GetName() + ": " + stat.GetCurrent().ToString() + "\n";
                }
            }

            //upgrades info

            m_weaponInformationText_Upgrades.text = "==UPGRADES==";
            m_weaponInformationText_Upgrades.text += "\n";
            m_weaponInformationText_Upgrades.text += "(SLOTS AVAILABLE: " + m_weaponToUpgrade.GetUpgradesAvailableCount() + ")";
            m_weaponInformationText_Upgrades.text += "\n";

            if (m_weaponToUpgrade.AccessCurrentUpgrades().Count > 0)
            {
                foreach (Upgrade up in m_weaponToUpgrade.AccessCurrentUpgrades())
                {
                    m_weaponInformationText_Upgrades.text += up.GetItemName() + "\n";
                }
            }
            else
            {
                m_weaponInformationText_Upgrades.text += "none";
            }
        }
    }

}
