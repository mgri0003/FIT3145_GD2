using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#pragma warning disable 649

public class UIScreen_MainMenu : UIScreenBase
{
    [SerializeField] private Button m_startGameButton;

    protected override void RegisterMethods()
    {
        m_startGameButton.onClick.AddListener(() => { OnStartGamePressed(); });
    }

    protected override void OnEnable()
    {
        
    }

    protected override void OnDisable()
    {
        
    }

    protected override void OnGUI()
    {

    }

    void OnStartGamePressed()
    {
        SceneManager.LoadScene("Bridge_HUB");
        SceneManager.sceneLoaded += GamePlayManager.Instance.OnSceneLoadedToGameplay;
    }
}
