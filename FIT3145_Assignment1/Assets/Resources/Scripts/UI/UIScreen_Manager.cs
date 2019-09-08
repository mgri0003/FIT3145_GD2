﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum EUIScreen
{
    NONE = -1,
    DEBUG_MENU,
    INGAME_HUD,
    TITLESCREEN_MENU,
    UPGRADE_MENU,
    MAX
}

public class UIScreen_Manager : Singleton<UIScreen_Manager>
{
    //--Variables--//
    [SerializeField] public UIScreenBase[] m_UIScreens = new UIScreenBase[(int)EUIScreen.MAX - 1];
    EUIScreen m_currentUIScreen = EUIScreen.NONE;
    private UI_CanvasManager m_canvasManager = null;

    //--Methods--//
    protected override void Awake()
    {
        base.Awake();

        m_canvasManager = GetComponent<UI_CanvasManager>();
        Debug.Assert(m_canvasManager, "Missing Canvas Manager");

        DisableAllScreens();

        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            GoToUIScreen(EUIScreen.TITLESCREEN_MENU);
        }
    }

    public void GoToUIScreen(in EUIScreen newScreen)
    {
        DisableAllScreens();
        EnableScreen(newScreen);
    }

    private void EnableScreen(in EUIScreen screenToEnable)
    {
        if (screenToEnable != EUIScreen.NONE)
        {
            UIScreenBase screen = m_UIScreens[(int)screenToEnable];
            if (screen)
            {
                screen.gameObject.SetActive(true);
            }
        }
        m_currentUIScreen = screenToEnable;

        //disable Cursor Visibilty & Lock State based on screen
        switch(m_currentUIScreen)
        {
            case EUIScreen.INGAME_HUD:
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            break;

            default:
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            break;
        }
    }

    private void DisableAllScreens()
    {
        foreach (UIScreenBase screen in m_UIScreens)
        {
            if (screen)
            {
                screen.gameObject.SetActive(false);
            }
        }
    }

    public EUIScreen GetCurrentUIScreenAsEnum()
    {
        return m_currentUIScreen;
    }

    public UIScreenBase GetCurrentUIScreen()
    {
        if(m_currentUIScreen != EUIScreen.NONE || m_currentUIScreen != EUIScreen.MAX)
        {
            return m_UIScreens[(int)m_currentUIScreen];
        }

        return null;
    }

    public UIScreenBase GetUIScreen(EUIScreen uiScreen)
    {
        if (uiScreen != EUIScreen.NONE || uiScreen != EUIScreen.MAX)
        {
            return m_UIScreens[(int)uiScreen];
        }

        return null;
    }

    public UI_CanvasManager GetCanvasManager()
    {
        return m_canvasManager;
    }
}
