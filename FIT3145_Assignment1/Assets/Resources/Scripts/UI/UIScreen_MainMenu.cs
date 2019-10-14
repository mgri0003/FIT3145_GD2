﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#pragma warning disable 649

public class UIScreen_MainMenu : UIScreenBase
{
    //--UI ELEMENTS--//
    [SerializeField] private Button m_startGameButton;
    [SerializeField] private Button m_optionsButton;
    [SerializeField] private Button m_controlsButton;
    [SerializeField] private Button m_quitButton;
    [SerializeField] private UI_Fader m_fader;
    [SerializeField] private Image m_title;
    [SerializeField] private Image m_cutscene_top;
    [SerializeField] private Image m_cutscene_bottom;
    [SerializeField] private AudioSource m_as_titleScreen;


    //other elements
    GameObject m_mainMenuModel = null;

    //states
    bool m_gameStartRequested = false;


    protected override void RegisterMethods()
    {
        m_startGameButton.onClick.AddListener(() => { OnStartGamePressed(); });
        m_optionsButton.onClick.AddListener(() => { OnOptionsButtonPressed(); });
        m_controlsButton.onClick.AddListener(() => { OnControlsButtonPressed(); });
        m_quitButton.onClick.AddListener(() => { OnQuitButtonPressed(); });
    }

    protected override void OnEnable()
    {
        m_title.color = new Color(1, 1, 1, 1);

       m_cutscene_top.color = new Color(1,1, 1, 0);
       m_cutscene_bottom.color = new Color(1, 1, 1, 0);

        m_gameStartRequested = false;
        m_fader.Reset();
        Invoke("StartMainScreenFadeOut", 0.5f);

        m_as_titleScreen.Play();
    }

    protected override void OnDisable()
    {
        m_as_titleScreen.Stop();
    }

    protected override void OnGUI()
    {

    }

    protected override void OnBack()
    {
        throw new System.NotImplementedException();
    }

    void OnStartGamePressed()
    {
        if(!m_gameStartRequested)
        {
            m_gameStartRequested = true;

            m_mainMenuModel = GameObject.Find("MainMenuModel") as GameObject;
            if (m_mainMenuModel)
            {
                m_mainMenuModel.GetComponent<Animator>().Play("Anim_SpaceShipStart");
            }

            m_title.color = new Color(1, 1, 1, 0);

            m_cutscene_top.color = new Color(1, 1, 1, 1);
            m_cutscene_bottom.color = new Color(1, 1, 1, 1);

            Invoke("RequestStartGame", 4.0f);

            StartCoroutine(IE_AudioFadeOut(m_as_titleScreen, 2.0f));
        }
    }

    void RequestStartGame()
    {
        UIScreen_Manager.Instance.StartTransition();
        Invoke("DoActualStartGame", 1.5f);
    }

    void DoActualStartGame()
    {
        SceneManager.LoadScene("Bridge_HUB");
        SceneManager.sceneLoaded += GamePlayManager.Instance.OnSceneLoadedToGameplay;
    }

    private void StartMainScreenFadeOut()
    {
        m_fader.StartFade();
    }

    void OnControlsButtonPressed()
    {
        UIScreen_Manager.Instance.GoToUIScreen(EUIScreen.CONTROLS_MENU);
    }
    void OnOptionsButtonPressed()
    {
        UIScreen_Manager.Instance.GoToUIScreen(EUIScreen.GAMEOPTIONS_MENU);
    }

    void OnQuitButtonPressed()
    {
        Application.Quit(0);
    }

    public IEnumerator IE_AudioFadeOut(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;
            audioSource.volume = Mathf.Clamp(audioSource.volume, 0, 1);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }
}
