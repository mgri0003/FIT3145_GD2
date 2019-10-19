using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum EGameEvent
{
    LEVEL1_COMPLETE,

    MAX
}

public enum EEnemySpawnableType
{
    Troglodyte,
    Troglodyte_Demo,
    Troglodyte_Big,
    Imp,
    Imp_Big,

    MAX
}

public struct EnemySpawnData
{
    public EEnemySpawnableType EnemyTypeToSpawn;
    public Vector3 SpawnLocation;
    public GameObject[] ItemDrops;
}

public class GamePlayManager : Singleton<GamePlayManager>
{
    //--Variables--//
    public const uint NUMBER_OF_LEVELS = 2;
    private Transform m_spawnPoint = null;
    [SerializeField] GameObject[] m_spawnable_enemies = new GameObject[(int)EEnemySpawnableType.MAX];
    private List<EnemySpawnData> m_enemySpawnDatas = null;
    private ElevatorPanel[] m_elevatorPanels = new ElevatorPanel[NUMBER_OF_LEVELS];

    //Spawnables (Do not change values in these objects)
    [SerializeField] private GameObject m_spawnable_mainPlayer = null;
    [SerializeField] private GameObject m_spawnable_inventoryZone = null;

    //InGame GameObjects
    private GameObject m_current_mainPlayer;
    private GameObject m_current_inventoryZone;

    //states
    private bool m_gameplayActive = false;

    //gameplay vars
    private int m_scrap;

    //--Methods--//

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(m_spawnable_mainPlayer, "Main Player Is Null");
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

    public void SetScrap(int newValue) { m_scrap = newValue; }
    public int GetScrap() { return m_scrap; }
    public void AddScrap(int newValue) { SetScrap(GetScrap() + newValue); }

    public void OnSceneLoadedToGameplay(Scene scene, LoadSceneMode mode)
    {
        GamePlayManager.Instance.SetupInGame();
        SceneManager.sceneLoaded -= GamePlayManager.Instance.OnSceneLoadedToGameplay;
        UIScreen_Manager.Instance.GetCanvasManager().Init();
        UIScreen_Manager.Instance.EndTransition();

        MusicManager.Instance.PlayMusicTrack(EMusicTrack.INGAME, 0.15f);
    }

    public void SetupInGame()
    {
        m_gameplayActive = true;

        WeaponsRepo.Instance.LoadAllContent();
        SpawnInventoryZone();
        SetupSpawnPoints();
        SpawnPlayer();
        SetupElevators();

        RespawnEnemies(true);

        UIScreen_Manager.Instance.GoToUIScreen(EUIScreen.INGAME_HUD);

        m_current_mainPlayer.GetComponent<Player_Controller>().SetEnableInput(true);
    }


    public void RespawnEnemies(in bool includeBoss)
    {
        //generate spawn data based on what enemies are currently in the scene (should only occur once)
        GenerateSpawnData();
        RemoveAllEnemiesInScene();
        SpawnEnemiesFromSpawnData(includeBoss);
        InitEnemyTargets();
    }

    private void InitEnemyTargets()
    {
        Enemy_Core[] enemies = GetAllEnemiesInScene();
        if (enemies != null)
        {
            foreach (Enemy_Core enemy in enemies)
            {
                enemy.FindPlayerToTarget();
            }
        }
    }

    private void SpawnEnemiesFromSpawnData(in bool includeBoss)
    {
        foreach(EnemySpawnData esp in m_enemySpawnDatas)
        {
            if (includeBoss || esp.EnemyTypeToSpawn != EEnemySpawnableType.Imp_Big)
            {
                GameObject newEnemyGO = Instantiate(m_spawnable_enemies[(int)esp.EnemyTypeToSpawn], esp.SpawnLocation, Quaternion.Euler(0, 0, 0));

                Enemy_Core enemy = newEnemyGO.GetComponent<Enemy_Core>();
                if (enemy)
                {
                    enemy.SetItemDrops(esp.ItemDrops);
                }
            }
        }
    }

    private void RemoveAllEnemiesInScene()
    {
        Enemy_Core[] allEnemies = GetAllEnemiesInScene();
        for(int i = 0; i < allEnemies.Length; ++i)
        {
            Destroy(allEnemies[i].gameObject);
        }
        allEnemies = null;
    }

    public void AggroNearbyEnemies(Enemy_Core enemy, in float distance)
    {
        Enemy_Core[] allEnemies = GetAllEnemiesInScene();
        for (int i = 0; i < allEnemies.Length; ++i)
        {
            if(Vector3.Distance(enemy.transform.position, allEnemies[i].transform.position) < distance)
            {
                allEnemies[i].TriggerAggro();
            }
        }
    }

    private void GenerateSpawnData()
    {
        if(m_enemySpawnDatas == null)
        {
            m_enemySpawnDatas = new List<EnemySpawnData>();
            if(m_enemySpawnDatas != null)
            {
                Enemy_Core[] allEnemies = GetAllEnemiesInScene();

                foreach (Enemy_Core enemy in allEnemies)
                {
                    EnemySpawnData newSpawnData;
                    newSpawnData.EnemyTypeToSpawn = enemy.GetEnemyType();
                    newSpawnData.SpawnLocation = enemy.transform.position;
                    newSpawnData.ItemDrops = enemy.GetItemDrops();

                    m_enemySpawnDatas.Add(newSpawnData);
                }
            }
        }
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

    public ElevatorPanel[] GetAllElevatorsPanelsInScene()
    {
        return FindObjectsOfType<ElevatorPanel>();
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
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            EndGame();
        }
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

            ResetGameplayData();

            SceneManager.LoadScene("MainMenu");

            UIScreen_Manager.Instance.GoToUIScreen(EUIScreen.TITLESCREEN_MENU);
        }
    }

    private void ResetGameplayData()
    {
        m_current_mainPlayer = null;
        m_current_inventoryZone = null;
        SetScrap(0);
    }

    public bool IsGameplayActive()
    {
        return m_gameplayActive;
    }

    void SpawnPlayer()
    {
        m_current_mainPlayer = Instantiate(m_spawnable_mainPlayer, m_spawnPoint.position, m_spawnPoint.rotation);
        Debug.Assert(m_current_mainPlayer, "Player Failed to Instantiate?!!??!");
        if (m_current_mainPlayer)
        {
            //set name
            m_current_mainPlayer.name = "MainPlayer";

            //Setup Camera vars for player
            Camera_Main.GetMainCamera().SetTarget(m_current_mainPlayer.transform);
        }
    }

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

    private void SetupElevators()
    {
        ElevatorPanel[] elevators = GetAllElevatorsPanelsInScene();
        if (elevators != null)
        {
            foreach (ElevatorPanel ep in elevators)
            {
                if(ep)
                {
                    Debug.Assert(m_elevatorPanels[ep.m_levelFrom] == null, "Elevator Already Assigned for level!");
                    if (m_elevatorPanels[ep.m_levelFrom] == null)
                    {
                        m_elevatorPanels[ep.m_levelFrom] = ep;
                    }
                }
            }
        }
    }

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

    public void MovePlayerToLevel(uint level)
    {
        Debug.Assert(level < NUMBER_OF_LEVELS, "Invalid Level");
        if(level < NUMBER_OF_LEVELS)
        {
            m_current_mainPlayer.transform.position = m_elevatorPanels[level].GetPlayerTeleportTransform().position;
        }
    }

    public Vector3 GetElevatorTeleportLocation(uint level)
    {
        Debug.Assert(level < NUMBER_OF_LEVELS, "Invalid Level");
        if (level < NUMBER_OF_LEVELS)
        {
           return m_elevatorPanels[level].GetPlayerTeleportTransform().position;
        }

        return Vector3.zero;
    }

    public Vector3 GetElevatorPanelLocation(uint level)
    {
        Debug.Assert(level < NUMBER_OF_LEVELS, "Invalid Level");
        if (level < NUMBER_OF_LEVELS)
        {
            return m_elevatorPanels[level].transform.position;
        }

        return Vector3.zero;
    }

    
    public void ProcessGameEvent(in EGameEvent gameEvent)
    {
        Debug.Assert(gameEvent != EGameEvent.MAX, "Invalid Game Event Requested!");

        switch (gameEvent)
        {
            case EGameEvent.LEVEL1_COMPLETE:
            {
                RespawnEnemies(false);
            }
            break;
        }

    }

}
