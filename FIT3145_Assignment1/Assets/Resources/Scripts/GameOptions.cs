using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameOptions
{
    //--Variables--//
    private static bool m_swapRightLeftClick = false;
    private static int m_qualitySetting = QualitySettings.GetQualityLevel();

    //--Getters--//
    public static bool GetSwapRightLeftClick() { return m_swapRightLeftClick; }
    public static int GetQualitySetting() { return m_qualitySetting; }
    public static string GetQualitySettingName() { return QualitySettings.names[m_qualitySetting]; }

    //--Setters--//
    public static void SetSwapRightLeftClick(bool newValue) { m_swapRightLeftClick = newValue; }
    public static void SetQualitySetting(int newValue)
    {
        m_qualitySetting = newValue;
        QualitySettings.SetQualityLevel(GetQualitySetting());
    }

    //--Toggles--//
    public static void ToggleSwapRightLeftClick() { m_swapRightLeftClick = !m_swapRightLeftClick; }
}
