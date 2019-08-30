﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    //--Variables--//
    [SerializeField] private Rigidbody m_physicsRigidbody = null;
    [SerializeField] private BoxCollider m_physicsCollider = null;

    [SerializeField] private string m_name = "Default_Item";


    //--Methods--//
    public string GetItemName() { return m_name; }

    public void SetPhysicsActive(bool enablePhysics)
    {
        Debug.Assert(m_physicsRigidbody, "Missing Rigidbody!?!?");
        Debug.Assert(m_physicsCollider, "Missing Collider!?!?");

        if (enablePhysics)
        {
            if (m_physicsRigidbody)
            {
                m_physicsRigidbody.useGravity = true;
                m_physicsRigidbody.constraints = RigidbodyConstraints.None;
            }
            if (m_physicsCollider)
            {
                m_physicsCollider.enabled = true;
            }
        }
        else
        {
            if (m_physicsRigidbody)
            {
                m_physicsRigidbody.useGravity = false;
                m_physicsRigidbody.constraints = RigidbodyConstraints.FreezeAll;
            }
            if (m_physicsCollider)
            {
                m_physicsCollider.enabled = false;
            }
        }
    }
}