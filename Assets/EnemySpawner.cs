using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("This object needs 'Spawner' tag")]
    public float maxRadius = 20;
    public float minRadius = 10;
    public int maxSpawns = 10;
    public int aliveEnemies = 0;
    public int killCount = 0;
    public GameObject enemy;
    public GameObject[] totalEnemies;

    public float spawnTime = 5;
    public float spawnRem = 0;
    public float timeReducePerSet = 0.1f;

    //[Header("Initial Spawn Variables")]
    //[SerializeField] private float maxHeight = 1;
    //private Renderer rend;

    void Start()
    {
        //rend = GetComponent<Renderer>();
        //rend.enabled = false;

        aliveEnemies = 0;
    }

    private void Update()
    {
        //Get a list of all enemies(gameobject)
        totalEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        //Debug.Log("spawnRem: " + spawnRem);
        Debug.Log("update alive enemies: " + aliveEnemies);
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

        float dist = Vector3.Distance(randomPos, transform.position);
        Debug.Log("enemy distance to 0,0,0: " + dist);


        if (dist <= minRadius) Debug.Log("dist is less than minRadius");
        while (dist <= minRadius)
        {
            //Generate random position
            randomPos = Random.insideUnitSphere * maxRadius;

            //Height Adjustments
            randomPos.y = 1;

            dist = Vector3.Distance(randomPos, transform.position);

            if (dist > minRadius) Debug.Log("while: dist is good: " + dist);
        }

        Instantiate(enemy, randomPos, Quaternion.identity);

        aliveEnemies += 1;
        spawnRem = 0;
    }//END SpawnInRadius

    private void OnDrawGizmos()
    {
        //blue represents the min range at which enemies spawn
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(this.transform.position, minRadius);

        //To display the radius the enemy/target would spawn in

        //Green represents the range at which enemies spawn
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, maxRadius);
    }

    /*private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger");
        //Does not count as a kill
        Destroy(other.gameObject);
        enemyIsAlive--;
    }*/

    public void EnemyTooClose()
    {
        aliveEnemies -= 1;
    }

    public void EnemyKilled()
    {
        Debug.Log("EnemyKilled has run");
        Debug.Log("before killed: " + aliveEnemies);
        aliveEnemies -= 1;
        killCount += 1;
        Debug.Log("after after: " + aliveEnemies);

        //Every 10 kills reduce the spawn timer
        if((killCount % 10) == 0) spawnTime = spawnTime - timeReducePerSet;
    }

    /*private void OnCollisionEnter(Collision collision)
    {
        //Does not count as a kill
        Destroy(collision.gameObject);
        enemyIsAlive--;
    }*/
}//END EnemySpawner