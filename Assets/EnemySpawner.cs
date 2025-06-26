using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum SpawnerState
{
    PRE_WAVE,   //idle and before the wave begins
    WAVE,       //the actual wave spawning
    END_WAVE,   //when all enemies are killed
}

public class EnemySpawner : MonoBehaviour
{
    [Header("State machine vars")]
    public SpawnerState currentState;
    private SpawnerState lastState;

    [Header("This object needs 'Spawner' tag")]
    [Header("Initial Enemy Spawn Variables")]
    private GameObject enemy;
    //private int totalTypes = 3;
    public GameObject[] enemyList;

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
                    SpawnInRadius();
                    totalEnemiesSpawned++;
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
        }
    }
    public void ChangeState(SpawnerState newState)
    {
        lastState = currentState;
        currentState = newState;
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

        if(rand >= 70) enemy = enemyList[1];
        else enemy = enemyList[0];

        justSpawned = Instantiate(enemy, randomPos, Quaternion.identity);
        SetStats(justSpawned);

        aliveEnemies += 1;
        spawnRem = 0;
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