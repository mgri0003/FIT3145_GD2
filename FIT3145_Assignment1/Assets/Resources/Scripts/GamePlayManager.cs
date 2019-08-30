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
    [SerializeField] private GameObject m_spawnable_inventoryZone = null;

    //InGame GameObjects
    private GameObject m_current_mainPlayer;
    private GameObject m_current_inventoryZone;

    //states
    private bool m_gameplayActive = false;


    //--Methods--//

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(m_spawnable_mainPlayer, "Main Player Is Null");
        Debug.Assert(m_spawnable_enemy, "Enemy Is Null");
        Debug.Assert(m_spawnable_inventoryZone, "Inventory Zone Is Null");
    }

    // Update is called once per frame
    void Update()
    {
        if(IsGameplayActive())
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                if (UIScreen_Manager.Instance.GetCurrentUIScreen() == EUIScreen.DEBUGMENU)
                {
                    UIScreen_Manager.Instance.GoToUIScreen(EUIScreen.INGAMEHUD);
                }
                else
                {
                    UIScreen_Manager.Instance.GoToUIScreen(EUIScreen.DEBUGMENU);
                }
            }

            if(m_current_mainPlayer.GetComponent<Player_Core>().IsDead())
            {
                EndGame();
            }
        }
    }

    public void OnSceneLoadedToGameplay(Scene scene, LoadSceneMode mode)
    {
        GamePlayManager.Instance.SetupInGame();
        SceneManager.sceneLoaded -= GamePlayManager.Instance.OnSceneLoadedToGameplay;
    }

    public void SetupInGame()
    {
        m_gameplayActive = true;

        WeaponsRepo.Instance.LoadAllContent();
        SpawnInventoryZone();
        SetupSpawnPoints();
        SpawnPlayer();
        SpawnEnemy();

        UIScreen_Manager.Instance.GoToUIScreen(EUIScreen.INGAMEHUD);

        m_current_mainPlayer.GetComponent<Player_Controller>().SetEnableInput(true);
    }

    public void EndGame()
    {
        m_gameplayActive = false;
        SceneManager.LoadScene("MainMenu");
        m_current_mainPlayer = null;
        m_current_inventoryZone = null;
        UIScreen_Manager.Instance.GoToUIScreen(EUIScreen.MAINMENU);
    }

    public bool IsGameplayActive()
    {
        return m_gameplayActive;
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

    public void SpawnInventoryZone()
    {
        m_current_inventoryZone = Instantiate(m_spawnable_inventoryZone, new Vector3(0, 1000 ,0), Quaternion.Euler(0, 0, 0));
    }

    public Vector3 GetInventoryZonePosition()
    {
        Debug.Assert(m_current_inventoryZone, "Inventory Zone not created!?!?!");
        return m_current_inventoryZone.transform.position;
    }
}
