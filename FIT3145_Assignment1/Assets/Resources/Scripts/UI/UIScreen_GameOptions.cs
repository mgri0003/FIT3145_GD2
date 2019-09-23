using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 649

enum EGameOption
{
    TOGGLE_SWAPLEFTRIGHTCLICK,
    DROPDOWN_QUALITYSETTING,

    MAX
}


public class UIScreen_GameOptions : UIScreenBase
{
    //--UI Elements--//
    [SerializeField] private Toggle m_toggle_SwapLeftRightClick;
    [SerializeField] private Dropdown m_dropdown_qualitySettings;

    [SerializeField] private Button m_backButton;

    protected override void RegisterMethods()
    {
        m_backButton.onClick.AddListener(() => { OnBack(); });

        m_toggle_SwapLeftRightClick.onValueChanged.AddListener((bool val) => { OnOptionChanged(EGameOption.TOGGLE_SWAPLEFTRIGHTCLICK); });
        m_dropdown_qualitySettings.onValueChanged.AddListener((int val) => { OnOptionChanged(EGameOption.DROPDOWN_QUALITYSETTING); });
    }

    protected override void OnDisable()
    {
        
    }

    protected override void OnEnable()
    {
        PopulateQualitySettingsDropDown();

        RefreshOptionsUI();
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
        m_dropdown_qualitySettings.value = GameOptions.GetQualitySetting();
    }

    private void OnOptionChanged(EGameOption gameOption)
    {
        switch (gameOption)
        {
            case EGameOption.TOGGLE_SWAPLEFTRIGHTCLICK:
            {
                GameOptions.SetSwapRightLeftClick(m_toggle_SwapLeftRightClick.isOn);
                //Debug.Log("Option Changed to : " + GameOptions.GetSwapRightLeftClick());
            }
            break;

            case EGameOption.DROPDOWN_QUALITYSETTING:
            {
                GameOptions.SetQualitySetting(m_dropdown_qualitySettings.value);
                //Debug.Log("Option Changed to : " + GameOptions.GetQualitySettingName() + "(" + GameOptions.GetQualitySetting() + ")"); 
            }
            break;
        }

        RefreshOptionsUI();
    }

    private void PopulateQualitySettingsDropDown()
    {
        m_dropdown_qualitySettings.ClearOptions();
        List<Dropdown.OptionData> newOptions = new List<Dropdown.OptionData>();
        foreach (string qualityOptionName in QualitySettings.names)
        {
            Dropdown.OptionData newOption = new Dropdown.OptionData
            {
                text = qualityOptionName
            };

            newOptions.Add(newOption);
        }
        m_dropdown_qualitySettings.AddOptions(newOptions);
    }
}
