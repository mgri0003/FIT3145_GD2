using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private string m_name = "Default_Item";

    public string GetItemName() { return m_name; }
}
