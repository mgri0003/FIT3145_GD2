using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameOptions
{
    //--Variables--//
    private static bool m_swapRightLeftClick = false;

    //--Getters--//
    public static bool GetSwapRightLeftClick() { return m_swapRightLeftClick; }

    //--Setters--//
    public static void SetSwapRightLeftClick(bool newValue) { m_swapRightLeftClick = newValue; }

    //--Toggles--//
    public static void ToggleSwapRightLeftClick() { m_swapRightLeftClick = !m_swapRightLeftClick; }
}
