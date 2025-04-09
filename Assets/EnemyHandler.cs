using UnityEngine;

public class EnemyHandler : MonoBehaviour
{
    [Header("This object needs 'Enemy' tag")]
    public GameObject spawner;
    public EnemySpawner script;
    public GameMenuManager menu;
    public float maxHealth = 100;
    public float health;
    public float distToOther;

    //Gun Variables
    public GameObject bullet;
    public Transform bulletSpawnPoint;
    public float fireDelay;
    public float fireRate;
    public float random;
    public float damage = 1;
    public float bulletSpd = 2;
    public Transform target;

    [Header("I need these for testing")]
    public float killDelay = 2;
    public float killTimer;

    //This is a start
    void Start()
    {
        health = maxHealth;
        killTimer = 0;

        random = Random.Range(2, 8);
        fireDelay = 5 + random;

        fireRate = 0;

        //Get spawner reference
        spawner = GameObject.FindGameObjectWithTag("Spawner");
        if (spawner != null) script = spawner.GetComponent<EnemySpawner>();
        else Debug.Log("no spawner found");
    }

    void Update()
    {
        if(!menu.gamePaused)
        {
            //Always look at player
            transform.LookAt(target);

            /*if(killTimer >= killDelay)
                DealDamage(100);
            else
                killTimer += 1 * Time.deltaTime;
            */
            if(health <= 0)
            {
                Destroy(gameObject);
            }

            if(fireRate >= fireDelay)
                FireBullet();
            else
                fireRate += 1 * Time.deltaTime;
        }
    }

    public void DealDamage(float dmg)
    {
        Debug.Log("Dealt damage to enemy health.");
        health = health - dmg;
    }

    private void OnDestroy()
    {
        //Debug.Log("Enemy OnDestroy");
        script.EnemyKilled();
    }//END OnDestroy

    public void FireBullet()
    {
        GameObject spawnedBullet = Instantiate(bullet);
        spawnedBullet.transform.position = bulletSpawnPoint.position;
        spawnedBullet.GetComponent<Rigidbody>().linearVelocity = bulletSpawnPoint.forward * bulletSpd;
        spawnedBullet.GetComponent<EnemyBullet>().menu = menu;
        //Destroy(spawnedBullet, 20);

        fireRate = 0;
    }
}//END EnemyHandler