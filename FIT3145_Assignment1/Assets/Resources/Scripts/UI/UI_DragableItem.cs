using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_DragableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerUpHandler
{
    [SerializeField] private Transform m_parentTransform = null;
    public Transform GetParentTransform() { return m_parentTransform; }

    private Item m_parentItem;
    public ref Item GetParentItem() { return ref m_parentItem; }
    public void SetParentItem(Item newItem) {  m_parentItem = newItem; InitItemElementSprite(); }

    public delegate void ItemDelegate(UI_DragableItem dragableItem, PointerEventData eventData);
    public ItemDelegate m_delegate_OnDrop;
    public ItemDelegate m_delegate_OnClicked;
    public ItemDelegate m_delegate_OnHoverEnter;
    public ItemDelegate m_delegate_OnHoverExit;

    private bool m_isDragging = false;
    public bool IsDragging() { return m_isDragging; }

    private bool m_isHovered = false;
    public bool IsHovered() { return m_isHovered; }

    public void OnDrag(PointerEventData eventData)
    {

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        m_isDragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        m_isDragging = false;
    }

    private void InitItemElementSprite()
    {
        Image image = GetComponentInChildren<Image>();
        if (image)
        {
            if (GetParentItem())
            {
                if(GetParentItem().GetItemSprite())
                {
                    image.sprite = GetParentItem().GetItemSprite();
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_isHovered = true;
        m_delegate_OnHoverEnter?.Invoke(this, eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_isHovered = false;
        m_delegate_OnHoverExit?.Invoke(this, eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        m_delegate_OnClicked?.Invoke(this, eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_delegate_OnDrop?.Invoke(this, eventData);
    }
}
