using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EUIScreen
{
    NONE = -1,
    DEBUGMENU,
    INGAMEHUD,
    MAINMENU,
    MAX
}

public class UIScreen_Manager : Singleton<UIScreen_Manager>
{
    //--Variables--//
    [SerializeField] public UIScreenBase[] m_UIScreens = new UIScreenBase[(int)EUIScreen.MAX - 1];
    EUIScreen m_currentUIScreen = EUIScreen.NONE;

    //--Methods--//
    void Awake()
    {
        DisableAllScreens();
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
                screen.enabled = true;
            }
        }
        m_currentUIScreen = screenToEnable;
    }

    private void DisableAllScreens()
    {
        foreach (UIScreenBase screen in m_UIScreens)
        {
            if (screen)
            {
                screen.enabled = false;
            }
        }
    }

    public EUIScreen GetCurrentUIScreen()
    {
        return m_currentUIScreen;
    }

    public void FindPlayerForUIScreens()
    {
        Player_Core player = null;
        GameObject playerGO = GameObject.Find("MainPlayer");
        Debug.Assert(playerGO, "UIScreen_Manager | FindPlayerForUIScreens() | Player was not found!");
        if (playerGO)
        {
            player = playerGO.GetComponent<Player_Core>();
        }

        if(player)
        {
            //Assign player ref to uiscreens

        }
    }
}
