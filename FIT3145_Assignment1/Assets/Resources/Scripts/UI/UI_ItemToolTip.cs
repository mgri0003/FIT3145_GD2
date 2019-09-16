using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 649

public class UI_ItemToolTip : MonoBehaviour
{
    [SerializeField] private Text m_nameText;
    [SerializeField] private Text m_typeText;
    [SerializeField] private Text m_typeDescriptionText;
    [SerializeField] private Text m_descriptionText;
    private Item m_parentItem;
    //private Vector2 m_offset = new Vector2(0,0);

    public void SetParentItem(in Item newItem) { m_parentItem = newItem; }

    private void Start()
    {
        Debug.Assert(m_parentItem, "Parent item Not Assigned to ItemToolTip!");

        RefreshDescription();
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = GetDesiredPosition();
    }

    void RefreshDescription()
    {
        //reset texts
        m_nameText.text = "";
        m_typeText.text = "";
        m_typeDescriptionText.text = "";
        m_descriptionText.text = "";

        //set texts
        m_nameText.text = "'" + m_parentItem.GetItemName() + "'";

        if (m_parentItem.GetItemType() == EItemType.WEAPON)
        {
            m_typeText.text = (m_parentItem as Weapon_Base).GetWeaponType().ToString() + " ";
        }
        else if (m_parentItem.GetItemType() == EItemType.AUGMENT)
        {
            m_typeText.text = (m_parentItem as Augment).GetAugmentType().ToString() + " ";
        }

        m_typeText.text += m_parentItem.GetItemType().ToString();

        m_typeDescriptionText.text = m_parentItem.GetItemTypeDescription();

        m_descriptionText.text = m_parentItem.GetItemDescription();
    }

    public Vector3 GetDesiredPosition()
    {
        return new Vector2(220, -180) + UI_CanvasManager.ConvertScreenPositionToCanvasLocalPosition(UI_CanvasManager.GetMousePositionFromScreenCentre());
    }
}
