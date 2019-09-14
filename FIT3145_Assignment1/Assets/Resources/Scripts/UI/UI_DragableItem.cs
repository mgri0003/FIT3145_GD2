using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_DragableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Transform m_parentTransform = null;
    public Transform GetParentTransform() { return m_parentTransform; }

    private Item m_parentItem;
    public ref Item GetParentItem() { return ref m_parentItem; }
    public void SetParentItem(Item newItem) {  m_parentItem = newItem; InitItemElementSprite(); }

    public delegate void MyDelegate(UI_DragableItem dragableItem, PointerEventData eventData);
    public MyDelegate m_delegate_OnDrop; 

    private bool m_isDragging = false;
    public bool IsDragging() { return m_isDragging; }

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

    public void OnDrop(PointerEventData eventData)
    {
        m_delegate_OnDrop(this, eventData);
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
}
