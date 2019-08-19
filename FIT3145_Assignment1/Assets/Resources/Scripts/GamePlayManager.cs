using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayManager : MonoBehaviour
{
    //--Variables--//

    //Spawnables
    [SerializeField] private GameObject m_spawnable_mainPlayer;

    private Transform m_spawnPoint;

    //--Methods--//

    // Start is called before the first frame update
    void Start()
    {
        SetupSpawnPoint();
        SpawnPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnPlayer()
    {
        GameObject newPlayer = Instantiate(m_spawnable_mainPlayer, m_spawnPoint.position, Quaternion.Euler(0, 0, 0));

        //Setup Camera vars for player
        Camera_Main.GetMainCamera().SetTarget(newPlayer.transform);
    }

    private void SetupSpawnPoint()
    {
        GameObject spawn = GameObject.FindGameObjectWithTag("PlayerSpawn");
        Debug.Assert(spawn, "No Player Spawn Set?");
        if(spawn)
        {
            m_spawnPoint = spawn.transform;
        }
    }
}
