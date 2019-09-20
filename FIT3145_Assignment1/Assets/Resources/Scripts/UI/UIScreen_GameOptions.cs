using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 649

enum EGameOption
{
    TOGGLE_SWAPLEFTRIGHTCLICK,

    MAX
}


public class UIScreen_GameOptions : UIScreenBase
{
    //--UI Elements--//
    [SerializeField] private Toggle m_toggle_SwapLeftRightClick;

    [SerializeField] private Button m_backButton;

    protected override void RegisterMethods()
    {
        m_backButton.onClick.AddListener(() => { OnBack(); });

        m_toggle_SwapLeftRightClick.onValueChanged.AddListener((bool val) => { ChangeOption(EGameOption.TOGGLE_SWAPLEFTRIGHTCLICK); });
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

    private void RefreshOptionsUI()
    {
        m_toggle_SwapLeftRightClick.isOn = GameOptions.GetSwapRightLeftClick();
    }

    private void ChangeOption(EGameOption gameOption)
    {
        switch (gameOption)
        {
            case EGameOption.TOGGLE_SWAPLEFTRIGHTCLICK:
            {
                GameOptions.SetSwapRightLeftClick(m_toggle_SwapLeftRightClick.isOn);
                Debug.Log("Option Changed to : " + GameOptions.GetSwapRightLeftClick());
            }
            break;
        }
    }
}
