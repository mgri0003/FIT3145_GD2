using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

#pragma warning disable 649

public enum EUIScreen
{
    NONE = -1,
    LOADOUT_MENU,
    INGAME_HUD,
    TITLESCREEN_MENU,
    UPGRADE_MENU,
    GAMEOPTIONS_MENU,
    MAX
}

public class UIScreen_Manager : Singleton<UIScreen_Manager>
{
    //--Variables--//
    [SerializeField] public UIScreenBase[] m_UIScreens = new UIScreenBase[(int)EUIScreen.MAX - 1];
    EUIScreen m_currentUIScreen = EUIScreen.NONE;
    private UI_CanvasManager m_canvasManager = null;

    //spawnables
    [SerializeField] private GameObject m_spawnable_itemToolTip;

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

    public void CreateItemToolTip(UI_DragableItem dragableItem, PointerEventData eventData)
    {
        //Debug.Log("Tooltip Created");
        GameObject go = Instantiate(m_spawnable_itemToolTip, UI_CanvasManager.GetCanvas().transform);

        dragableItem.m_currentToolTip = go.GetComponent<UI_ItemToolTip>();
        if (dragableItem.m_currentToolTip)
        {
            dragableItem.m_currentToolTip.SetParentItem(dragableItem.GetParentItem());
        }

        go.transform.localPosition = dragableItem.m_currentToolTip.GetDesiredPosition();
        go.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    public void DestroyItemToolTip(UI_DragableItem dragableItem, PointerEventData eventData)
    {
        //Debug.Log("Tooltip Destroyed");
        dragableItem.DestroyToolTip();
    }
}
