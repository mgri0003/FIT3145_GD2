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
    [SerializeField] private UnityEngine.UI.Button m_improvementButton;
    [SerializeField] private UnityEngine.UI.Text m_weaponInformationText;
    [SerializeField] private UnityEngine.UI.Text m_weaponInformationText_Upgrades;
    [SerializeField] private UnityEngine.UI.Text m_scrapValueText;

    //refs
    [SerializeField] Canvas m_canvas;
    [SerializeField] private Transform m_WeaponDisplay;
    [SerializeField] private GameObject m_upgradeScrollViewContentGO;
    [SerializeField] private Transform m_upgradeSegmentsListTransform;
    [SerializeField] private GameObject m_spawnable_upgradeElement;
    [SerializeField] private GameObject m_spawnable_improvementSegment;
    [SerializeField] private UnityEngine.UI.Image m_upgradeApplyArea;

    //dynamic refs
    private Player_Core m_player = null;
    private List<GameObject> m_upgradeElements = new List<GameObject>();
    private List<GameObject> m_improvementSegments = new List<GameObject>();
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
        m_improvementButton.onClick.AddListener(() => { OnImprovementButtonPressed(); });
    }

    protected override void OnEnable()
    {
        m_player = GamePlayManager.Instance.GetCurrentPlayer();
        if(m_player)
        {
            Debug.Assert(m_weaponToUpgrade, "Weapon To Upgrade is null, set it before you enter this screen!");
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

                    if(UI_CanvasManager.IsPointInsideRect(m_upgradeApplyArea.rectTransform, UI_CanvasManager.ConvertScreenPositionToCanvasLocalPosition(UI_CanvasManager.GetMousePositionFromScreenCentre())))
                    {
                        if(m_weaponToUpgrade.CanAddUpgrade(dragableItem.GetParentItem() as Upgrade))
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

        UI_UpdateScrapValue();
    }

    protected override void OnBack()
    {
        UIScreen_Manager.Instance.GoToUIScreen(EUIScreen.LOADOUT_MENU);
    }

    private void OnImprovementButtonPressed()
    {
        if(CanAffordImprovement())
        {
            //spend scrap!
            SpendScrapOnNextImprovement();

            m_weaponToUpgrade.ImproveWeapon();
            RefreshImprovementSegments();
            RefreshWeaponInformation();
        }
    }

    private void SpendScrapOnNextImprovement()
    {
        int costOfNextImprovement = (int)m_weaponToUpgrade.GetImprovementPath().GetNextAvailableImprovementSegment().ScrapCost;
        GamePlayManager.Instance.AddScrap(-costOfNextImprovement);
    }

    private bool CanAffordImprovement()
    {
        if(!m_weaponToUpgrade.GetImprovementPath().IsFullyImproved())
        {
            return GamePlayManager.Instance.GetScrap() >= m_weaponToUpgrade.GetImprovementPath().GetNextAvailableImprovementSegment().ScrapCost;
        }

        return false;
    }

    private void UI_UpdateScrapValue()
    {
        m_scrapValueText.text = GamePlayManager.Instance.GetScrap().ToString();
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
                go.GetComponentInChildren<UI_DragableItem>().m_delegate_OnHoverEnter = UIScreen_Manager.Instance.CreateItemToolTip;
                go.GetComponentInChildren<UI_DragableItem>().m_delegate_OnHoverExit = UIScreen_Manager.Instance.DestroyItemToolTip;
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
        if (UI_CanvasManager.IsPointInsideRect(m_upgradeApplyArea.rectTransform, UI_CanvasManager.ConvertScreenPositionToCanvasLocalPosition(UI_CanvasManager.GetMousePositionFromScreenCentre())))
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

    private void PopulateUpgradeSegments()
    {
        if (m_spawnable_improvementSegment)
        {
            float positionY = 0.0f;
            foreach (ImprovementSegment upSeg in m_weaponToUpgrade.GetImprovementPath().GetImprovementSegments())
            {
                //Debug.Log("Upgrade Element Created");
                GameObject go = Instantiate(m_spawnable_improvementSegment);
                go.transform.SetParent(m_upgradeSegmentsListTransform.transform, false);
                go.transform.localPosition = new Vector3(0, positionY, 0);
                m_improvementSegments.Add(go);
                positionY -= 35.0f;
            }

            RefreshImprovementSegments();
        }
    }

    private void RefreshImprovementSegments()
    {
        int improvementIndex = 0;
        foreach (GameObject upSegGO in m_improvementSegments)
        {
            //Debug.Log("Upgrade Element Created");
            UI_ImprovementSegment uiImpSeg = upSegGO.GetComponent<UI_ImprovementSegment>();
            ImprovementSegment impSeg = m_weaponToUpgrade.GetImprovementPath().GetImprovementSegments()[improvementIndex];
            if (uiImpSeg)
            {
                uiImpSeg.AccessImprovementSegmentText().text = (impSeg.StatToImprove != EWeaponStat.MAX) ? (m_weaponToUpgrade.AccessWeaponStat(impSeg.StatToImprove).GetName() + ((impSeg.Value > 0) ? (" +") : (" ") ) + impSeg.Value.ToString()) : "";

                if (impSeg.UpgradeSlotIncrease)
                {
                    uiImpSeg.AccessImprovementSegmentText().text += ((uiImpSeg.AccessImprovementSegmentText().text == "") ? "" : " | " ) + "Upgrade Slot +1";
                }

                if (impSeg.ScrapCost > 0)
                {
                    uiImpSeg.AccessImprovementSegmentCostText().text = impSeg.ScrapCost.ToString();
                }
                else
                {
                    uiImpSeg.AccessImprovementSegmentCostText().text = "FREE";
                }

                if(m_weaponToUpgrade.GetImprovementPath().GetCurrentImprovementIndex() >= improvementIndex)
                {
                    uiImpSeg.SetUISegmentOpacity(1.0f);

                    if(m_weaponToUpgrade.GetImprovementPath().GetCurrentImprovementIndex() > improvementIndex)
                    {
                        uiImpSeg.AccessImprovementSegmentText().color = Color.green;
                        uiImpSeg.AccessImprovementSegmentCostText().color = Color.green;
                    }
                }
                else
                {
                    uiImpSeg.SetUISegmentOpacity(0.1f);
                }
            }
            ++improvementIndex;
        }

        //reposition Upgrade Button to latest position
        if(m_weaponToUpgrade.GetImprovementPath().GetCurrentImprovementIndex() < m_weaponToUpgrade.GetImprovementPath().GetImprovementSegmentCount())
        {
            m_improvementButton.gameObject.SetActive(true);

            m_improvementButton.transform.localPosition = new Vector3(
                m_upgradeSegmentsListTransform.localPosition.x - 50.0f, 
                m_upgradeSegmentsListTransform.transform.localPosition.y - (0.0f) - (m_weaponToUpgrade.GetImprovementPath().GetCurrentImprovementIndex() * 35.0f), 
                0);
        }
        else
        {
            m_improvementButton.gameObject.SetActive(false);
        }
    }

    private void CleanUpUpgradeSegments()
    {
        while (m_improvementSegments.Count > 0)
        {
            Destroy(m_improvementSegments[0]);
            m_improvementSegments.RemoveAt(0);
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
