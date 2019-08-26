using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIScreen_MainMenu : UIScreenBase
{
    protected override void OnEnable()
    {
        
    }

    protected override void OnDisable()
    {
        
    }

    protected override void OnGUI()
    {
        if (GUI.Button(new Rect((Screen.width / 2) - 125, Screen.height/2, 250, 40), "Go To Game Scene"))
        {
            SceneManager.sceneLoaded += GamePlayManager.Instance.OnSceneLoaded;
            SceneManager.LoadScene("Game");
        }
        if (GUI.Button(new Rect((Screen.width / 2) - 125, Screen.height / 2 + 40, 250, 40), "Go To Bridge_Hub Scene"))
        {
            SceneManager.LoadScene("Bridge_HUB");
            SceneManager.sceneLoaded += GamePlayManager.Instance.OnSceneLoaded;
        }
    }

}
