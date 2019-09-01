using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIScreen_MainMenu : UIScreenBase
{
    [SerializeField] Texture2D m_titleLogo;

    protected override void OnEnable()
    {
        
    }

    protected override void OnDisable()
    {
        
    }

    protected override void OnGUI()
    {
        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), m_titleLogo, GUIStyle.none);
        //if (GUI.Button(new Rect((Screen.width / 2) - 125, Screen.height/2, 250, 40), "Go To Game Scene"))
        //{
        //    SceneManager.sceneLoaded += GamePlayManager.Instance.OnSceneLoadedToGameplay;
        //    SceneManager.LoadScene("Game");
        //}
        if (GUI.Button(new Rect((Screen.width / 2) - 125, Screen.height - 150, 250, 40), "Start Game"))
        {
            SceneManager.LoadScene("Bridge_HUB");
            SceneManager.sceneLoaded += GamePlayManager.Instance.OnSceneLoadedToGameplay;
        }
    }
}
