using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePlayManager : Singleton<GamePlayManager>
{
    //--Variables--//
    private Transform m_spawnPoint = null;
    private Transform m_enemySpawnPoint = null;

    //Spawnables (Do not change values in these objects)
    [SerializeField] private GameObject m_spawnable_mainPlayer = null;
    [SerializeField] private GameObject m_spawnable_enemy = null;

    //InGame GameObjects
    private GameObject m_current_mainPlayer;


    //--Methods--//

    // Start is called before the first frame update
    void Start()
    {
        //debug call as GamePlayManager would of already been created in a previous scene :P
        if (SceneManager.GetActiveScene().name == "Game")
        {
            SetupInGame();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            if(UIScreen_Manager.Instance.GetCurrentUIScreen() == EUIScreen.DEBUGMENU)
            {
                UIScreen_Manager.Instance.GoToUIScreen(EUIScreen.INGAMEHUD);
            }
            else
            {
                UIScreen_Manager.Instance.GoToUIScreen(EUIScreen.DEBUGMENU);
            }
        }
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GamePlayManager.Instance.SetupInGame();
        SceneManager.sceneLoaded -= GamePlayManager.Instance.OnSceneLoaded;
    }

    public void SetupInGame()
    {
        SetupSpawnPoints();
        SpawnPlayer();
        SpawnEnemy();

        UIScreen_Manager.Instance.GoToUIScreen(EUIScreen.INGAMEHUD);

        m_current_mainPlayer.GetComponent<Player_Controller>().SetEnableInput(true);
    }

    void SpawnPlayer()
    {
        m_current_mainPlayer = Instantiate(m_spawnable_mainPlayer, m_spawnPoint.position, Quaternion.Euler(0, 0, 0));
        Debug.Assert(m_current_mainPlayer, "Player Failed to Instantiate?!!??!");
        if (m_current_mainPlayer)
        {
            //set name
            m_current_mainPlayer.name = "MainPlayer";

            //Setup Camera vars for player
            Camera_Main.GetMainCamera().SetTarget(m_current_mainPlayer.transform);
        }
    }

    void SpawnEnemy()
    {
        GameObject newEnemy = Instantiate(m_spawnable_enemy, m_enemySpawnPoint.position, Quaternion.Euler(0, 0, 0));
        if(newEnemy)
        {
            if(m_current_mainPlayer)
            {
                newEnemy.GetComponent<Enemy_Core>().SetTargetCharacter(m_current_mainPlayer.GetComponent<Character_Core>());
            }
        }
    }

    private void SetupSpawnPoints()
    {
        SetupPlayerSpawn();
        SetupEnemySpawn();
    }

    private void SetupPlayerSpawn()
    {
        GameObject spawn = GameObject.FindGameObjectWithTag("PlayerSpawn");
        Debug.Assert(spawn, "No Player Spawn Set?");
        if (spawn)
        {
            m_spawnPoint = spawn.transform;
        }
    }

    private void SetupEnemySpawn()
    {
        GameObject spawn = GameObject.FindGameObjectWithTag("EnemySpawn");
        Debug.Assert(spawn, "No Enemy Spawn Set?");
        if (spawn)
        {
            m_enemySpawnPoint = spawn.transform;
        }
    }

    public Player_Core GetCurrentPlayer()
    {
        return m_current_mainPlayer.GetComponent<Player_Core>();
    }
}
