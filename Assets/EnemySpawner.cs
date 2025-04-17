using UnityEngine;
using UnityEngine.Rendering;

public class EnemySpawner : MonoBehaviour
{
    [Header("This object needs 'Spawner' tag")]
    [Header("Initial Enemy Spawn Variables")]
    public GameObject enemy;
    public int maxSpawns = 10;
    public float spawnTime = 5;
    public float timeReducePerSet = 0.1f;
    public float minDistForEnemies = 2;

    [Header("Min = player area (blue sphere)" +  "\n" + "Max = furthest out enemies can be (green)")]
    public float maxRadius = 20;
    public float minRadius = 10;

    [Header("Do not adjust.")]
    public int aliveEnemies = 0;
    public int killCount = 0;
    public GameObject[] totalEnemies;
    public float spawnRem = 0;
    public bool canSpawn = true;
    public GameObject justSpawned;
    public Transform playerTransform;
    public GameMenuManager menuScript;
    
    //[SerializeField] private float maxHeight = 1;

    void Start()
    {
        aliveEnemies = 0;

        GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
        playerTransform = cam.transform;

        GameObject gameMenu = GameObject.FindGameObjectWithTag("Menu");
        menuScript = gameMenu.GetComponent<GameMenuManager>();
    }

    private void Update()
    {
        if(!menuScript.gamePaused)
        {
            //Get a list of all enemies(gameobject)
            totalEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            if((aliveEnemies < maxSpawns) && (totalEnemies.Length < maxSpawns))
            {
                if(spawnRem >= spawnTime) SpawnInRadius();
                else spawnRem += 1 * Time.deltaTime;
            }
        }
    }

    public void SpawnInRadius()
    {
        //Generate random position
        Vector3 randomPos = Random.insideUnitSphere * maxRadius;

        //Set the y to height of enemy
        randomPos.y = 1;

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
            randomPos.y = 1;

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