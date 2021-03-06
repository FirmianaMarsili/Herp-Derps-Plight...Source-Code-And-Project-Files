﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : Singleton<EnemySpawner>
{
    // Properties + Component References
    #region
    [Header("Testing Properties")]
    public bool runTestWaveOnly;
    public EnemyWaveSO testingWave;

    [Header("Enemy Encounter Lists")]
    public EnemyWaveSO[] basicEnemyWavesActOneHalfOne;
    public EnemyWaveSO[] basicEnemyWavesActOneHalfTwo;
    public EnemyWaveSO[] eliteEnemyWaves;
    public EnemyWaveSO[] bossEnemyWaves;
    public EnemyWaveSO[] storyEventEnemyWaves;

    [Header("Current Viable Encounters Lists")]
    [HideInInspector] public List<EnemyWaveSO> viableBasicEnemyActOneHalfOneWaves;
    [HideInInspector] public List<EnemyWaveSO> viableBasicEnemyActOneHalfTwoWaves;
    [HideInInspector] public List<EnemyWaveSO> viableEliteEnemyWaves;

    #endregion


    // Initialization + Setup
    #region   
    private void Start()
    {
        PopulateWaveList(viableBasicEnemyActOneHalfOneWaves, basicEnemyWavesActOneHalfOne);
        PopulateWaveList(viableBasicEnemyActOneHalfTwoWaves, basicEnemyWavesActOneHalfTwo);
        PopulateWaveList(viableEliteEnemyWaves, eliteEnemyWaves);
    }
    #endregion

    // Enemy Spawning + Related
    #region
    public void SpawnEnemyWave(string enemyType = "Basic", EnemyWaveSO enemyWave = null)
    {
        Debug.Log("SpawnEnemyWave() Called....");
       // PopulateEnemySpawnLocations();
        EnemyWaveSO enemyWaveSO = enemyWave;

        // for testing
        if (runTestWaveOnly)
        {
            enemyWaveSO = testingWave;
        }

        /*
        // If we have not given a specific enemy wave to spawn, get a random one
        else if(enemyWaveSO == null)
        {
            // select a random enemyWaveSO
            if (enemyType == "Basic" &&
                WorldManager.Instance.playerColumnPosition <= 5)
            {
                if (viableBasicEnemyActOneHalfOneWaves.Count == 0)
                {
                    PopulateWaveList(viableBasicEnemyActOneHalfOneWaves, basicEnemyWavesActOneHalfOne);
                }

                enemyWaveSO = GetRandomWaveSO(viableBasicEnemyActOneHalfOneWaves, true);
            }
            else if (enemyType == "Basic" &&
                WorldManager.Instance.playerColumnPosition > 5)
            {
                if (viableBasicEnemyActOneHalfTwoWaves.Count == 0)
                {
                    PopulateWaveList(viableBasicEnemyActOneHalfTwoWaves, basicEnemyWavesActOneHalfTwo);
                }

                enemyWaveSO = GetRandomWaveSO(viableBasicEnemyActOneHalfTwoWaves, true);
            }

            else if (enemyType == "Elite")
            {
                if (viableEliteEnemyWaves.Count == 0)
                {
                    PopulateWaveList(viableEliteEnemyWaves, eliteEnemyWaves);
                }

                enemyWaveSO = GetRandomWaveSO(viableEliteEnemyWaves, true);
            }

            else if (enemyType == "Boss")
            {
                enemyWaveSO = GetRandomWaveSO(bossEnemyWaves);
            }
        }   
        */

        // Create all enemies in wave
        foreach (EnemyGroup enemyGroup in enemyWaveSO.enemyGroups)
        {
            // Random choose enemy data
            int randomIndex = Random.Range(0, enemyGroup.possibleEnemies.Count);
            EnemyDataSO data = enemyGroup.possibleEnemies[randomIndex];

            CharacterEntityController.Instance.CreateEnemyCharacter(data, LevelManager.Instance.GetNextAvailableEnemyNode());

        }

    }
    public void PopulateWaveList(List <EnemyWaveSO> waveListToPopulate, IEnumerable<EnemyWaveSO> wavesCopiedIn)
    {
        waveListToPopulate.AddRange(wavesCopiedIn);
    }    
    /*
    public EnemyWaveSO GetRandomWaveSO(List<EnemyWaveSO> enemyWaves, bool removeWaveFromList = false)
    {
        EnemyWaveSO enemyWaveReturned = enemyWaves[Random.Range(0, enemyWaves.Count)];        
        if(removeWaveFromList == true && enemyWaveReturned != null && enemyWaves.Count >= 1)
        {
            enemyWaves.Remove(enemyWaveReturned);
        }
        return enemyWaveReturned;
    }
    */
    /*
    public EnemyWaveSO GetEnemyWaveByName(string name)
    {
        List<EnemyWaveSO> allWaves = new List<EnemyWaveSO>();
        EnemyWaveSO waveReturned = null;

        allWaves.AddRange(basicEnemyWavesActOneHalfOne);
        allWaves.AddRange(eliteEnemyWaves);
        allWaves.AddRange(bossEnemyWaves);
        allWaves.AddRange(storyEventEnemyWaves);

        foreach(EnemyWaveSO wave in allWaves)
        {
            if(wave.encounterName == name)
            {
                waveReturned = wave;
                break;
            }
        }

        return waveReturned;
    }
    */
    #endregion



}
