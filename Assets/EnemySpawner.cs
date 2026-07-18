using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
using Color = UnityEngine.Color;
using System.Collections;

public enum SpawnerState
{
    PRE_WAVE,   //idle and before the wave begins
    WAVE,       //the actual wave spawning
    END_WAVE,   //when all enemies are killed
}

public enum SpawnType
{
    RADIUS,
    DOORS_RANDOM,
    SPAWN_BOXES
}

public class EnemySpawner : MonoBehaviour
{
    [Header("State machine vars")]
    public SpawnerState currentState;
    private SpawnerState lastState;
    [SerializeField] SpawnType spawnType;

    [Header("This object needs 'Spawner' tag")]
    [Header("Initial Enemy Spawn Variables")]
    private GameObject enemy;
    //private int totalTypes = 3;
    [SerializeField] GameObject[] enemyTypes;

    [Header("Spawn boxes contain their own doors.\nThe first box is active.")]
    [SerializeField] GameObject[] spawnBoxes; //Areas that contain the door that spawn enemies
    [SerializeField] Transform[] doorsPos;   //the doors contain within the spawn box
    [SerializeField] GameObject activeSpawnBox = null;   //the active spawn box/area
    private Coroutine boxRoutine;   //contains coroutine for changing the spawn box/area

    [Header("SpawnDoors are not related to the spawn boxes\nbut for the Doors_Random spawn type.")]
    [SerializeField] GameObject[] spawnDoors; //keeps track of all doors for spawning doors random(not related to the spawn boxes)

    public int maxSpawns = 10;          //Enemies spawned at once
    public int maxEnemyPerWave = 10;    //Total enemies that spawn per wave
    public int totalEnemiesSpawned;
    public int wave;
    public float spawnTime = 5;
    public float timeReducePerSet = 0.5f;
    public float minDistForEnemies = 4;
    public float preWaveTimer = 30f;
    public bool timerIsRunning = false;
    public TMP_Text timerText;
    public TMP_Text waveText;

    [Header("Min = player area (blue sphere)" + "\n" + "Max = furthest out enemies can be (green)")]
    public float maxRadius = 20;
    public float minRadius = 10;

    [Header("Do not adjust.")]
    private bool setBoxDelay = false;      //For delaying changing the box switch until an enemy is not actively spawning
    public int aliveEnemies = 0;        //Goes up as enemies spawn
    public int killCount = 0;           //Goes up as enemies are killed
    public int waveKillCount = 0;
    public GameObject[] totalEnemies;
    public float spawnRem = 0;
    public bool canSpawn = true;
    public GameObject justSpawned;
    public Transform playerTransform;
    public GameMenuManager menuScript;
    public HighScoreHandler scoreHandler;
    public Upgrades upgrades;
    
    //[SerializeField] private float maxHeight = 1;

    void Start()
    {
        switch (spawnType)
        {
            case SpawnType.DOORS_RANDOM:
            {
                spawnDoors = GameObject.FindGameObjectsWithTag("SpawnDoor");
                break;
            }
            case SpawnType.SPAWN_BOXES:
            {
            
                spawnBoxes = GameObject.FindGameObjectsWithTag("SpawnBox");

                if(spawnBoxes != null)
                {
                    ChangeActiveBox(spawnBoxes[0]);
                }
                break;
            }
        }

        currentState = SpawnerState.PRE_WAVE;
        lastState = currentState;

        aliveEnemies = 0;
        wave = 1;

        GameObject cam = GameObject.FindGameObjectWithTag("Player");
        playerTransform = cam.transform;

        /*GameObject gameMenu*/ menuScript = GameObject.FindAnyObjectByType<GameMenuManager>(); //FindGameObjectWithTag("Menu");
        //menuScript = gameMenu.GetComponent<GameMenuManager>();

        /*GameObject score*/ //scoreHandler = GameObject.FindAnyObjectByType<HighScoreHandler>(); //FindGameObjectWithTag("HighscoreObj");
        //scoreHandler = score.GetComponent<HighScoreHandler>();

        if(scoreHandler != null)
        {
            scoreHandler.Invoke("RemoveTempScore", 0.1f);
        }
        else
        {
            Debug.LogWarning("HighScoreHandler not found");
        }
    }

    private void Update()
    {
        switch(currentState)
        {
            case SpawnerState.PRE_WAVE:
            {
                ResetVars();
                timerText.gameObject.SetActive(true);
                waveText.gameObject.SetActive(true);

                if (preWaveTimer > 0)
                {
                    preWaveTimer -= Time.deltaTime;
                    DisplayTime(preWaveTimer);
                }
                else
                {
                    Debug.Log("times up");
                    preWaveTimer = 0;
                    ChangeState(SpawnerState.WAVE);
                }
                    
                break;
            }
            case SpawnerState.WAVE:
            {
                //Spawn enemies
                timerText.gameObject.SetActive(false);
                waveText.gameObject.SetActive(false);
                WaveIsCurrent();
                break;
            }
            case SpawnerState.END_WAVE:
            {
                //Upgrades
                upgrades.UpgradeMenu();
                Debug.Log("END_WAVE State: Activated");
                break;
            }
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        waveText.text = "Wave: " + wave;
    }

    public void WaveIsCurrent()
    {
        if(!menuScript.gamePaused)
        {
            //Get a list of all enemies(gameobject)
            totalEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            if ((aliveEnemies < maxSpawns) && (totalEnemies.Length < maxSpawns) && (totalEnemiesSpawned < maxEnemyPerWave) && (waveKillCount != maxEnemyPerWave))
            {
                if (spawnRem >= spawnTime)
                {
                    switch(spawnType)
                    {
                        case SpawnType.RADIUS: SpawnInRadius(); break;
                        case SpawnType.DOORS_RANDOM: SpawnByDoorsRandom(); break;
                        case SpawnType.SPAWN_BOXES: SpawnByBox(); break;
                        default: spawnType = SpawnType.RADIUS; break;
                    }
                }
                else
                {
                    spawnRem += Time.deltaTime;
                }
            }
            if (waveKillCount >= maxEnemyPerWave)
            {
                scoreHandler.AddMaxWave(wave);
                wave += 1;
                ChangeState(SpawnerState.END_WAVE);
            }

            //Every 1 sec find the closest doors

        }
    }
    public void ChangeState(SpawnerState newState)
    {
        lastState = currentState;
        currentState = newState;
    }

    public void SpawnByDoorsRandom()
    {
        //Temporary point to spawn at
        Vector3 point = Vector3.zero;

        //Get door position
        int element = Random.Range(0,spawnDoors.Length);
        Debug.Log(element);
        point = spawnDoors[element].transform.position + (spawnDoors[element].transform.forward * 0.5f);
        point.y = 0;

        //Choose what type of enemy spawns next
        int rand = Random.Range(1, 100);

        if(rand >= 70) enemy = enemyTypes[1];
        else enemy = enemyTypes[0];

        //Spawn enemy and adjust vars
        justSpawned = Instantiate(enemy, point, Quaternion.identity);
        SetStats(justSpawned);
        HasSpawned();
    }

    public void SpawnByBox()
    {
        if(activeSpawnBox == null)
        {
            ChangeActiveBox(spawnBoxes[0]);
        }

        setBoxDelay = true; //if true then changing the boxes should be stopped to prioritize the enemy spawn

        //Temporary point to spawn at
        Vector3 point;

        //Get door position
        int element = Random.Range(0,doorsPos.Length);

        //Dont spawn enemy at a different door each run
        /*if(doorsPos[element].GetComponent<SpawnDoor>().active)
        {
            element = Random.Range(0, doorsPos.Length - 1);
        }*/

        point = doorsPos[element].position + (doorsPos[element].forward * 0.5f);
        //point.y = 0;

        //Choose what type of enemy spawns next
        int rand = Random.Range(1, 100);

        if(rand >= 70) enemy = enemyTypes[1];
        else enemy = enemyTypes[0];

        //Spawn enemy and adjust vars
        justSpawned = Instantiate(enemy, point, Quaternion.identity);
        SetStats(justSpawned);
        HasSpawned();

        //doorsPos[element].GetComponent<SpawnDoor>().JustSpawned(true, spawnTime);
        
    }

    private IEnumerator ChangeBoxRoutine(GameObject newBox)
    {
        if(!setBoxDelay)
        {
            Debug.LogWarning("box coroutine has run");
            ChangeActiveBox(newBox);
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void ChangeActiveBox(GameObject newBox)
    {
        if(!setBoxDelay)
        {
            if(newBox != null)
            {
                activeSpawnBox = newBox;
                SpawnBox temp = activeSpawnBox.GetComponent<SpawnBox>();

                doorsPos = temp.GetSpawnDoors();

                if(doorsPos.Length < 1)
                {
                    Debug.LogWarning("Add more doors to the current box: " + activeSpawnBox.name);
                }
            }

            if(boxRoutine != null)
            {
                StopCoroutine(boxRoutine);
                Debug.LogWarning("box coroutine has stopped");
                boxRoutine = null;
            }
        }
        else if(boxRoutine == null)
        {
            boxRoutine = StartCoroutine(ChangeBoxRoutine(newBox));
        }
    }

    public void SpawnInRadius()
    {
        //Generate random position
        Vector3 randomPos = Random.insideUnitSphere * maxRadius;

        //Set the y to height of enemy
        randomPos.y = 0;

        #region Check distance compared to player area and between enemies

        float dist = Vector3.Distance(randomPos, transform.position);
        if(dist <= minRadius) canSpawn = false;

        float distToE = minDistForEnemies + 1;
        if (totalEnemies.Length > 0)
        {
            for(int i = 0; i < totalEnemies.Length; i++)
            {
                Vector3 closestVector = totalEnemies[i].transform.position;
                distToE = Vector3.Distance(randomPos, closestVector);

                if(distToE <= minDistForEnemies)
                {
                    canSpawn = false;
                    break;
                }
            }
        }

        while(!canSpawn)
        {
            //Generate random position
            randomPos = Random.insideUnitSphere * maxRadius;

            //Height Adjustments
            randomPos.y = 0;

            dist = Vector3.Distance(randomPos, transform.position);

            if(totalEnemies.Length > 0)
            {
                for(int i = 0; i < totalEnemies.Length; i++)
                {
                    Vector3 closestVector = totalEnemies[i].transform.position;
                    distToE = Vector3.Distance(randomPos, closestVector);

                    if (distToE <= minDistForEnemies)
                    {
                        //Debug.Log("While. Enemy too close");
                        break;
                    }
                }

                if((dist > minRadius) && (distToE > minDistForEnemies))
                {
                    canSpawn = true;
                    break;
                }
            }
            else
            {
                if(dist > minRadius)
                {
                    canSpawn = true;
                    break;
                }
            }
        }//END While

        #endregion

        //Choose what type of enemy spawns next
        int rand = Random.Range(1, 100);

        if(rand >= 70) enemy = enemyTypes[1];
        else enemy = enemyTypes[0];

        justSpawned = Instantiate(enemy, randomPos, Quaternion.identity);
        SetStats(justSpawned);
        HasSpawned();
    }//END SpawnInRadius

    private void OnDrawGizmos() //To display the radius the enemy/target would spawn in
    {
        //blue represents the min range at which enemies spawn
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(this.transform.position, minRadius);

        //Green represents the range at which enemies spawn
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, maxRadius);
    }

    public void EnemyKilled()
    {
        aliveEnemies -= 1;
        killCount += 1;
        waveKillCount += 1;
        scoreHandler.AddPoints(killCount * 10);
        //Debug.Log(scoreHandler.highscore.playerScore.ToString());
        //Debug.Log("after after: " + aliveEnemies);

        //Every 10 kills reduce the spawn timer
        if((killCount % 10) == 0) spawnTime = Mathf.Clamp(spawnTime - timeReducePerSet, 1, spawnTime);
    }//END EnemuKilled

    private void HasSpawned()
    {
        aliveEnemies += 1;
        spawnRem = 0;
        totalEnemiesSpawned++;
        setBoxDelay = false;
    }

    public void SetStats(GameObject enemy)
    {
        //Get enemy script
        EnemyHandler enemyHandler = enemy.GetComponent<EnemyHandler>();
        enemyHandler.target = playerTransform;
        enemyHandler.menu = menuScript;
        enemyHandler.spawner = gameObject;

        //Set stats (variables)
        //damage, health, etc
    }

    private void ResetVars()
    {
        totalEnemiesSpawned = 0;
        waveKillCount = 0;
    }
}//END EnemySpawner