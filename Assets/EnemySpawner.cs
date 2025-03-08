using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Initial Spawn Variables")]
    [SerializeField] private float maxRadius = 30;
    [SerializeField] private float maxHeight = 1;
    [SerializeField] private int maxSpawns = 10;

    [Header("Enemy Related Variables")]
    [SerializeField] private GameObject enemy;
    [SerializeField] private float spawnTimer = 5;   //The time per spawn
    [SerializeField] private float timeReducePerSet = 0.1f;

    private float spawnRem; //remaining time to spawn
    private int enemyIsAlive = 0;
    private int killCount = 0;
    private Renderer rend;

    //private float minRadius = 5;    //for visual debug, should be same radius as cylinder

    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.enabled = false;

        spawnRem = 0;
    }

    private void Update()
    {
        //Debug.Log(killCount);
        if (enemyIsAlive < maxSpawns)
        {
            if (spawnRem >= spawnTimer)
            {
                SpawnInRadius();
                spawnRem = 0;
                enemyIsAlive++;

            }
            else
            {
                //Debug.Log(spawnRem);
                spawnRem += Time.deltaTime;
            }
        }
    }

    private void SpawnInRadius()
    {
        //Generate random position
        Vector3 randomPos = Random.insideUnitSphere * maxRadius;

        #region Horizontal Adjustments. None atm

        /*float xAbs = Mathf.Abs(randomPos.x);
        float xSign = Mathf.Sign(randomPos.x);
        float zAbs = Mathf.Abs(randomPos.z);
        float zSign = Mathf.Sign(randomPos.z);*/

        //randomPos.x = xSign * Mathf.Clamp(xAbs, minRadius, maxRadius);
        //randomPos.z = zSign * Mathf.Clamp(zAbs, minRadius, maxRadius);

        /*if (xAbs <= minRadius)
        {
            Debug.Log(xAbs);
            randomPos.x = xSign * Random.Range(xAbs, maxRadius);
            //randomPos.x = randomPos.x + (xSign * 5);
        }
        
        if (zAbs <= minRadius) randomPos.z = zSign * Random.Range(zAbs, maxRadius);*/
            //randomPos.z = randomPos.z + (zSign * 5);

        /*float dist = Vector3.Distance(randomPos, transform.position);
        
        while(dist <= 5)
        {
            Debug.Log(dist);
            randomPos = Random.insideUnitSphere * maxRadius;
            dist = Vector3.Distance(randomPos, transform.position);
        }*/

        #endregion

        #region Height Adjustments

        //If enemy would spawn below the ground, set it to a range from zero to a max spawn height.
        //I'm assuming the ground will be zero.
        if (randomPos.y < 1)
        {
            randomPos.y = Random.Range(1, maxHeight);
        }

        //If the position is above the desired height, bring them to whatever the max height should be. 
        if(randomPos.y > maxHeight)
        {
            randomPos.y = maxHeight;
        }

        #endregion

        //Create a clone of the enemy
        Instantiate(enemy, randomPos, Quaternion.identity);
    }

    /*
     * To display the radius the enemy/target would spawn in
     */
    private void OnDrawGizmos()
    {
        //Blue should be the no spawn zone/player play area
        //Gizmos.color = Color.blue;
        //Gizmos.DrawWireSphere(this.transform.position, minRadius);
        
        //Green represents the range at which enemies spawn
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, maxRadius);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger");
        //Does not count as a kill
        Destroy(other.gameObject);
        enemyIsAlive--;
    }

    public void EnemyKilled()
    {
        enemyIsAlive--;
        killCount++;

        //Every 10 kills reduce the spawn timer
        if((killCount % 10) == 0) spawnTimer = spawnTimer - timeReducePerSet;
    }

    /*private void OnCollisionEnter(Collision collision)
    {
        //Does not count as a kill
        Destroy(collision.gameObject);
        enemyIsAlive--;
    }*/
}