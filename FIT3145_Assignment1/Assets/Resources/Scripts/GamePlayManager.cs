﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayManager : MonoBehaviour
{
    //--Variables--//

    private Transform m_spawnPoint = null;
    private Transform m_enemySpawnPoint = null;

    //Spawnables
    [SerializeField] private GameObject m_spawnable_mainPlayer = null;
    [SerializeField] private GameObject m_spawnable_enemy = null;

    //InGame GameObjects
    private GameObject m_current_mainPlayer;


    //--Methods--//

    // Start is called before the first frame update
    void Start()
    {
        SetupSpawnPoints();
        SpawnPlayer();
        SpawnEnemy();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnPlayer()
    {
        m_current_mainPlayer = Instantiate(m_spawnable_mainPlayer, m_spawnPoint.position, Quaternion.Euler(0, 0, 0));

        //Setup Camera vars for player
        Camera_Main.GetMainCamera().SetTarget(m_current_mainPlayer.transform);
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
}
