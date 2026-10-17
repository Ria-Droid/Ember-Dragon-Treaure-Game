using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    [Serializable]
    public class Wave
    {
        public string waveName = "Wave";
        public GameObject[] enemyPrefabs;
        public int enemyCount = 5;
        public float spawnInterval = 1.5f;
    }

    [Header("Setup")]
    public Transform[] spawnPoints;
    public Transform treasureTarget;
    public List<Wave> waves = new List<Wave>();

    [Header("Timing")]
    public float timeBetweenWaves = 2f;

    public event Action<int> OnWaveStarted;
    public event Action OnAllWavesComplete;

    private int currentWaveIndex = -1;
    private int enemiesAliveThisWave = 0;
    private bool wavesRunning = false;

    
    private readonly List<GameObject> spawnedEnemies = new List<GameObject>();

    void OnEnable()
    {
        EnemyBase.OnAnyEnemyDied += NotifyEnemyRemoved;
    }

    void OnDisable()
    {
        EnemyBase.OnAnyEnemyDied -= NotifyEnemyRemoved;
    }

    void Start()
    {
        
        if (treasureTarget == null)
        {
            Debug.LogError("WaveManager: Treasure Target is NOT assigned in the Inspector!");
        }
        else if (treasureTarget.GetComponentInParent<TreasureHealth>() == null &&
                 treasureTarget.GetComponentInChildren<TreasureHealth>() == null)
        {
            Debug.LogError("WaveManager: '" + treasureTarget.name +
                           "' has no TreasureHealth on it, its parents, or its children!");
        }

        StartWaves();
    }

    

    void Update()
    {
        // N = Skip to Wave 3
        if (Input.GetKeyDown(KeyCode.N))
        {
            SkipToWave(3);
        }

        // M = Skip to Wave 6
        if (Input.GetKeyDown(KeyCode.M))
        {
            SkipToWave(6);
        }
    }

    

    public void StartWaves()
    {
        // Prevents two wave systems running together
        if (wavesRunning)
            return;

        if (waves == null || waves.Count == 0)
        {
            Debug.LogWarning("WaveManager: No waves have been configured.");
            return;
        }

        wavesRunning = true;
        StartCoroutine(RunWaves());
    }

   

    public void SkipToWave(int waveNumber)
    {
        if (waves == null || waves.Count == 0)
        {
            Debug.LogWarning("WaveManager: No waves configured.");
            return;
        }

        
        int targetIndex = waveNumber - 1;

        
        if (targetIndex < 0 || targetIndex >= waves.Count)
        {
            Debug.LogWarning(
                "Wave " + waveNumber +
                " does not exist. You currently have " +
                waves.Count + " waves."
            );

            return;
        }

        
        Debug.Log("SKIPPING TO WAVE " + waveNumber);
        

        
        StopAllCoroutines();

        
        foreach (GameObject enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }

        spawnedEnemies.Clear();

        
        enemiesAliveThisWave = 0;

        
        currentWaveIndex = targetIndex;

        
        wavesRunning = true;

        
        StartCoroutine(RunWavesFromIndex(targetIndex));
    }

   

    private IEnumerator RunWaves()
    {
        yield return StartCoroutine(RunWavesFromIndex(0));
    }

    private IEnumerator RunWavesFromIndex(int startIndex)
    {
        for (int i = startIndex; i < waves.Count; i++)
        {
            currentWaveIndex = i;

            Debug.Log("Starting Wave " + (i + 1));

            
            OnWaveStarted?.Invoke(i);

            
            yield return StartCoroutine(SpawnWave(waves[i]));

            
            
            yield return new WaitUntil(() => enemiesAliveThisWave <= 0);

            spawnedEnemies.RemoveAll(e => e == null);

            
            if (i < waves.Count - 1)
            {
                yield return new WaitForSeconds(timeBetweenWaves);
            }
        }

        wavesRunning = false;

        OnAllWavesComplete?.Invoke();
    }

    

    private IEnumerator SpawnWave(Wave wave)
    {
        enemiesAliveThisWave = wave.enemyCount;

        for (int i = 0; i < wave.enemyCount; i++)
        {
            bool enemySpawned = SpawnEnemy(wave);

            
            if (!enemySpawned)
            {
                enemiesAliveThisWave =
                    Mathf.Max(0, enemiesAliveThisWave - 1);
            }

            yield return new WaitForSeconds(wave.spawnInterval);
        }
    }

  

    private bool SpawnEnemy(Wave wave)
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("WaveManager: No spawn points assigned!");
            return false;
        }

        if (wave.enemyPrefabs == null ||
            wave.enemyPrefabs.Length == 0)
        {
            Debug.LogError("WaveManager: No enemy prefabs assigned!");
            return false;
        }

        Transform spawnPoint =
            spawnPoints[
                UnityEngine.Random.Range(
                    0,
                    spawnPoints.Length
                )
            ];

        GameObject prefab =
            wave.enemyPrefabs[
                UnityEngine.Random.Range(
                    0,
                    wave.enemyPrefabs.Length
                )
            ];

        if (spawnPoint == null || prefab == null)
        {
            Debug.LogWarning(
                "WaveManager: Missing spawn point or enemy prefab."
            );

            return false;
        }

        
        Quaternion spawnRotation = spawnPoint.rotation;

        if (treasureTarget != null)
        {
            Vector3 direction =
                treasureTarget.position -
                spawnPoint.position;

            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                spawnRotation =
                    Quaternion.LookRotation(direction);
            }
        }

       
        GameObject enemy =
            Instantiate(
                prefab,
                spawnPoint.position,
                spawnRotation
            );

        
        spawnedEnemies.Add(enemy);

        
        EnemyBase enemyBase =
            enemy.GetComponent<EnemyBase>();

        if (enemyBase != null)
        {
            
            enemyBase.treasureTarget = treasureTarget;

            
            enemyBase.spawnPoints = spawnPoints;
        }
        else
        {
            Debug.LogWarning(
                enemy.name +
                " does not have EnemyBase attached."
            );
        }

        return true;
    }

    
    public void NotifyEnemyRemoved()
    {
        enemiesAliveThisWave =
            Mathf.Max(
                0,
                enemiesAliveThisWave - 1
            );
    }

    
    public void StopWaves()
    {
        StopAllCoroutines();

        wavesRunning = false;

        foreach (GameObject enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }

        spawnedEnemies.Clear();
    }
}