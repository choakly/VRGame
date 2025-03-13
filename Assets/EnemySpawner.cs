using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("This object needs 'Spawner' tag")]
    public int maxSpawns = 10;
    public int aliveEnemies = 0;
    public int killCount = 0;
    public GameObject enemy;
    public GameObject[] totalEnemies;
    //public Vector3 enemyPos;

    [Header("Min = player area (blue sphere)" +  "\n" + "Max = furthest out enemies can be (green)")]
    public float maxRadius = 20;
    public float minRadius = 10;

    [Header("Initial Enemy Spawn Variables")]
    public float spawnTime = 5;
    public float spawnRem = 0;
    public float timeReducePerSet = 0.1f;
    public float minDistToE = 2;
    //public float distToE;

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

    #region Trying to get closest enemy

    /*Transform GetClosestEnemy(Transform[] enemies)
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (Transform t in enemies)
        {
            float dist = Vector3.Distance(t.position, currentPos);
            if (dist < minDist)
            {
                tMin = t;
                minDist = dist;
            }
        }
        return tMin;
    }*/

    public Vector3 ClosestEnemy(Vector3 randomPos)
    {
        Vector3 closest = new Vector3(0,0,0);
        float minDist = Mathf.Infinity;

        for(int i = 0; i < totalEnemies.Length; i++)
        {
            float tempDist = Vector3.Distance(randomPos, totalEnemies[i].transform.position);
            if(tempDist < minDist)
            {
                tempDist = minDist;
            }
        }

        return closest;
    }

    #endregion

    public void SpawnInRadius()
    {
        //Generate random position
        Vector3 randomPos = Random.insideUnitSphere * maxRadius;

        //Set the y to height of enemy
        randomPos.y = 1;

        #region Check distance compared to player area

        float dist = Vector3.Distance(randomPos, transform.position);

        //distToE = GetClosestEnemy(randomPos);
        //Debug.Log("dist to e: " + distToE);

        if (dist <= minRadius) Debug.Log("dist is less than minRadius");
        //if (distToE <= minDistToE) Debug.Log("Enemy too close");

        while(dist <= minRadius)// && (distToE <= minDistToE))
        {
            //Generate random position
            randomPos = Random.insideUnitSphere * maxRadius;

            //Height Adjustments
            randomPos.y = 1;

            dist = Vector3.Distance(randomPos, transform.position);

            //if (dist > minRadius) Debug.Log("while: dist is good: " + dist);
        }

        #endregion

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