using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePlayManager : Singleton<GamePlayManager>
{
    //--Variables--//
    private Transform m_spawnPoint = null;
    //private Transform[] m_enemySpawnPoints = null;

    //Spawnables (Do not change values in these objects)
    [SerializeField] private GameObject m_spawnable_mainPlayer = null;
    //[SerializeField] private GameObject m_spawnable_enemy = null;
    //[SerializeField] private GameObject m_spawnable_enemyDemo = null;
    [SerializeField] private GameObject m_spawnable_inventoryZone = null;

    //InGame GameObjects
    private GameObject m_current_mainPlayer;
    private GameObject m_current_inventoryZone;

    //states
    private bool m_gameplayActive = false;

    //gameplay vars
    private uint m_scrap;

    //--Methods--//

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(m_spawnable_mainPlayer, "Main Player Is Null");
        //Debug.Assert(m_spawnable_enemy, "Enemy Is Null");
        //Debug.Assert(m_spawnable_enemyDemo, "Enemy Demo Is Null");
        Debug.Assert(m_spawnable_inventoryZone, "Inventory Zone Is Null");
    }

    // Update is called once per frame
    void Update()
    {
        if(IsGameplayActive())
        {
            OnGameplayUpdate();
        }
    }

    public void SetScrap(uint newValue) { m_scrap = newValue; }
    public uint GetScrap() { return m_scrap; }
    public void AddScrap(uint newValue) { SetScrap(GetScrap() + newValue); }

    public void OnSceneLoadedToGameplay(Scene scene, LoadSceneMode mode)
    {
        GamePlayManager.Instance.SetupInGame();
        SceneManager.sceneLoaded -= GamePlayManager.Instance.OnSceneLoadedToGameplay;
        UIScreen_Manager.Instance.GetCanvasManager().Init();
    }

    public void SetupInGame()
    {
        m_gameplayActive = true;

        WeaponsRepo.Instance.LoadAllContent();
        SpawnInventoryZone();
        SetupSpawnPoints();
        SpawnPlayer();

        Enemy_Core[] enemies = GetAllEnemiesInScene();
        if(enemies != null)
        {
            foreach(Enemy_Core enemy in enemies)
            {
                enemy.FindPlayerToTarget();
            }
        }

        //if(m_enemySpawnPoints != null)
        //{
        //    for (int i = 0; i < m_enemySpawnPoints.Length; ++i)
        //    {
        //        SpawnEnemy(i);
        //    }
        //}


        UIScreen_Manager.Instance.GoToUIScreen(EUIScreen.INGAME_HUD);

        m_current_mainPlayer.GetComponent<Player_Controller>().SetEnableInput(true);
    }

    public void DisableAggroOnAllEnemies()
    {
        Enemy_Core[] enemies = GetAllEnemiesInScene();
        if (enemies != null)
        {
            foreach (Enemy_Core enemy in enemies)
            {
                enemy.DisableAggro();
            }
        }
    }

    public Enemy_Core[] GetAllEnemiesInScene()
    {
        return FindObjectsOfType<Enemy_Core>();
    }

    private void RespawnPlayer()
    {
        //revive player
        m_current_mainPlayer.GetComponent<Player_Core>().Revive();

        //reset position
        m_current_mainPlayer.transform.SetPositionAndRotation(m_spawnPoint.position, m_current_mainPlayer.transform.rotation);
    }

    void OnGameplayUpdate()
    {

    }

    public void OnPlayerDeath()
    {
        DisableAggroOnAllEnemies();
        Invoke("RespawnPlayer", 5.0f);
    }

    public void EndGame()
    {
        if(IsGameplayActive())
        {
            m_gameplayActive = false;

            m_current_mainPlayer = null;
            m_current_inventoryZone = null;

            SceneManager.LoadScene("MainMenu");

            UIScreen_Manager.Instance.GoToUIScreen(EUIScreen.TITLESCREEN_MENU);
        }
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

    //void SpawnEnemy(int spawnPointIndex)
    //{
    //    GameObject newEnemy = Instantiate(spawnPointIndex == 0? m_spawnable_enemyDemo : m_spawnable_enemy, m_enemySpawnPoints[spawnPointIndex].position, Quaternion.Euler(0, 0, 0));
    //    if(newEnemy)
    //    {
    //        if(m_current_mainPlayer)
    //        {
    //            newEnemy.GetComponent<Enemy_Core>().SetTargetCharacter(m_current_mainPlayer.GetComponent<Character_Core>());
    //        }
    //    }
    //}

    private void SetupSpawnPoints()
    {
        SetupPlayerSpawn();
        //SetupEnemySpawns();
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

    //private void SetupEnemySpawns()
    //{
    //    GameObject[] spawns = GameObject.FindGameObjectsWithTag("EnemySpawn");
    //    Debug.Assert(spawns.Length > 0, "No Enemy Spawn Set?");
    //    if (spawns.Length > 0)
    //    {
    //        Transform[] transforms = new Transform[spawns.Length];
    //        for(int i = 0; i < spawns.Length; ++i)
    //        {
    //            transforms[i] = spawns[i].transform;
    //        }
    //        m_enemySpawnPoints = transforms;
    //    }
    //}

    public Player_Core GetCurrentPlayer()
    {
        if(m_current_mainPlayer)
        {
            return m_current_mainPlayer.GetComponent<Player_Core>();
        }

        return null;
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
