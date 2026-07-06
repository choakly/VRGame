using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Color = UnityEngine.Color;

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
    DOORS_CLOSEST
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
    public GameObject[] enemyTypes;
    public GameObject[] spawnDoors; //keeps track of all doors
    public GameObject[] closestDoors = new GameObject[3];

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
    private float oldDist = 9999;       //For comparing closest doors
    private float maxSDelay = 120;
    private float searchDelay = 0;      //For stopping the closest door search every frame
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
        spawnDoors = GameObject.FindGameObjectsWithTag("SpawnDoor");
        closestDoors[0] = spawnDoors[0];
        closestDoors[1] = spawnDoors[1];
        closestDoors[2] = spawnDoors[2];

        currentState = SpawnerState.PRE_WAVE;
        lastState = currentState;

        aliveEnemies = 0;
        wave = 1;

        GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
        playerTransform = cam.transform;

        GameObject gameMenu = GameObject.FindGameObjectWithTag("Menu");
        menuScript = gameMenu.GetComponent<GameMenuManager>();

        GameObject score = GameObject.FindGameObjectWithTag("HighscoreObj");
        scoreHandler = score.GetComponent<HighScoreHandler>();

        scoreHandler.Invoke("RemoveTempScore",0.5f);
    }

    private void Update()
    {
        //if(!menuScript.gamePaused)
        //{
        switch(currentState)
        {
            case SpawnerState.PRE_WAVE:
            {
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
                if((spawnType == SpawnType.DOORS_CLOSEST) && (searchDelay <= 0))
                {
                    Debug.Log("Start search");
                    int o = 0;
                    //Find closest doors to player
                    for(int i = 0; i < spawnDoors.Length; i++)
                    {
                        float dist = (playerTransform.position - spawnDoors[i].transform.position).sqrMagnitude;

                        if(dist <= oldDist)
                        {
                            closestDoors[o] = spawnDoors[i];
                            oldDist = dist;
                            o++;
                        }
                    }

                    searchDelay = maxSDelay;
                }

                searchDelay -= Time.deltaTime;

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
        //}
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
        if (!menuScript.gamePaused)
        {
            //Get a list of all enemies(gameobject)
            totalEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            if ((aliveEnemies < maxSpawns) && (totalEnemies.Length < maxSpawns) && (totalEnemiesSpawned <= maxEnemyPerWave) && (waveKillCount != maxEnemyPerWave))
            {
                if (spawnRem >= spawnTime)
                {
                    switch(spawnType)
                    {
                        case SpawnType.RADIUS: SpawnInRadius(); break;
                        case SpawnType.DOORS_RANDOM: SpawnByDoorsRandom(); break;
                        case SpawnType.DOORS_CLOSEST: SpawnByDoorsClosest(); break;
                        default: spawnType = SpawnType.RADIUS; break;
                    }
                }
                else
                {
                    spawnRem += 1 * Time.deltaTime;
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
        int element = Random.Range(0,spawnDoors.Length-1);
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

    public void SpawnByDoorsClosest()   //is same as doors random atm
    {
        //Temporary point to spawn at
        Vector3 point = Vector3.zero;

        if(closestDoors.Length != 0)
        {
            int element = Random.Range(0, closestDoors.Length);
            point = closestDoors[element].transform.position + (closestDoors[element].transform.forward * 0.5f);
            point.y = 0;

            //Choose what type of enemy spawns next
            int rand = Random.Range(1, 100);
            if(rand >= 70) enemy = enemyTypes[1];
            else enemy = enemyTypes[0];

            //Spawn enemy and adjust vars
            justSpawned = Instantiate(enemy, point, Quaternion.identity);
            SetStats(justSpawned);
            HasSpawned();

            /*for(int i = 0; i < closestDoors.Length; i++)
            {
                //Get door position
                point = closestDoors[i].transform.position + (closestDoors[i].transform.forward * 0.5f);
                point.y = 0;

                //Choose what type of enemy spawns next
                int rand = Random.Range(1, 100);
                if(rand >= 70) enemy = enemyTypes[1];
                else enemy = enemyTypes[0];

                //Spawn enemy and adjust vars
                justSpawned = Instantiate(enemy, point, Quaternion.identity);
                SetStats(justSpawned);
                HasSpawned();
            }*/
        }
        else Debug.Log("closest doors array is empty");
        /*//Temporary point to spawn at
        Vector3 point = Vector3.zero;
        float doorDist = 50;
        float lastDist = doorDist;

        for(int cd = 0; cd < closestDoors.Length; cd++)
        {
            for(int sd = 0; sd < spawnDoors.Length - 1; sd++)
            {
                doorDist = Vector3.Distance(playerTransform.position, spawnDoors[sd].transform.position);
                if(doorDist < lastDist)
                {
                    closestDoors[cd] = spawnDoors[sd];
                }
                //float dist = Vector3.Distance(playerTransform.position, spawnDoors[0].transform.position) | Vector3.Distance(playerTransform.position, spawnDoors[1].transform.position);
            }
        }
        //Get door position
        int element = Random.Range(0, spawnDoors.Length - 1);
        //Debug.Log(element);
        point = spawnDoors[element].transform.position + (spawnDoors[element].transform.forward * 0.5f);
        point.y = 0;

        //Choose what type of enemy spawns next
        int rand = Random.Range(1, 100);

        if(rand >= 70) enemy = enemyTypes[1];
        else enemy = enemyTypes[0];

        //Spawn enemy and adjust vars
        justSpawned = Instantiate(enemy, point, Quaternion.identity);
        SetStats(justSpawned);*/
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
                    //Debug.Log("Enemy too close");
                    //Debug.Log("dist to e: " + distToE);
                    canSpawn = false;
                    break;
                }
            }
        }

        while(!canSpawn)
        {
            //Generate random position
            randomPos = Random.insideUnitSphere * maxRadius;
            //Debug.Log("New spawn position");

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
                        //Debug.Log("While. dist to e: " + distToE);
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

    public void EnemyTooClose()
    {
        aliveEnemies -= 1;
    }

    public void EnemyKilled()
    {
        //Debug.Log("EnemyKilled has run");
        //Debug.Log("before killed: " + aliveEnemies);
        aliveEnemies -= 1;
        killCount += 1;
        waveKillCount += 1;
        scoreHandler.AddPoints(killCount * 10);
        //Debug.Log(scoreHandler.highscore.playerScore.ToString());
        //Debug.Log("after after: " + aliveEnemies);

        //Every 10 kills reduce the spawn timer
        if((killCount % 10) == 0) spawnTime = spawnTime - timeReducePerSet;
    }//END EnemuKilled

    private void HasSpawned()
    {
        aliveEnemies += 1;
        spawnRem = 0;
        totalEnemiesSpawned++;
    }

    public void SetStats(GameObject enemy)
    {
        //Get enemy script
        EnemyHandler enemyHandler = enemy.GetComponent<EnemyHandler>();
        enemyHandler.target = playerTransform;
        enemyHandler.menu = menuScript;

        //Set stats (variables)
        //damage, health, etc
    }
}//END EnemySpawner