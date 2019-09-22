using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 649

public class UIScreen_Controls : UIScreenBase
{
    //--UI Elements--//
    [SerializeField] private Button m_backButton;

    protected override void RegisterMethods()
    {
        m_backButton.onClick.AddListener(() => { OnBack(); });
    }

    protected override void OnDisable()
    {
        
    }

    protected override void OnEnable()
    {
        
    }

    protected override void OnGUI()
    {
        
    }

    protected override void OnBack()
    {
        UIScreen_Manager.Instance.GoToUIScreen(EUIScreen.TITLESCREEN_MENU);
    }
}
