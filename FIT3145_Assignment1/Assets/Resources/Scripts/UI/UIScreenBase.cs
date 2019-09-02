using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIScreenBase : MonoBehaviour
{
    private void Awake()
    {
        RegisterMethods();
    }
    protected abstract void RegisterMethods();
    protected abstract void OnEnable();
    protected abstract void OnDisable();
    protected abstract void OnGUI();
}
