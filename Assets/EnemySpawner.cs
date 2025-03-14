using UnityEngine;

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

    //[SerializeField] private float maxHeight = 1;

    void Start()
    {
        aliveEnemies = 0;
    }

    private void Update()
    {
        //Get a list of all enemies(gameobject)
        totalEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        //Debug.Log("spawnRem: " + spawnRem);
        //Debug.Log("update alive enemies: " + aliveEnemies);
        if ((aliveEnemies < maxSpawns) && (totalEnemies.Length < maxSpawns))
        {
            if (spawnRem >= spawnTime) SpawnInRadius();
            else spawnRem += 1 * Time.deltaTime;
        }
    }

    public void SpawnInRadius()
    {
        //Generate random position
        Vector3 randomPos = Random.insideUnitSphere * maxRadius;

        //Set the y to height of enemy
        randomPos.y = 1;

        //#region Check distance compared to player area

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
                    Debug.Log("Enemy too close");
                    Debug.Log("dist to e: " + distToE);
                    canSpawn = false;
                    break;
                }
            }
        }

        //Debug.Log("Dist: " + dist);

        while(!canSpawn)
        {
            //Generate random position
            randomPos = Random.insideUnitSphere * maxRadius;
            Debug.Log("New spawn position");

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
                        Debug.Log("While. Enemy too close");
                        Debug.Log("While. dist to e: " + distToE);
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

        Instantiate(enemy, randomPos, Quaternion.identity);

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
        Debug.Log("EnemyKilled has run");
        //Debug.Log("before killed: " + aliveEnemies);
        aliveEnemies -= 1;
        killCount += 1;
        //Debug.Log("after after: " + aliveEnemies);

        //Every 10 kills reduce the spawn timer
        if((killCount % 10) == 0) spawnTime = spawnTime - timeReducePerSet;
    }
}//END EnemySpawner