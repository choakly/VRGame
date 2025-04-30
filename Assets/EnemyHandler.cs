//using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyHandler : MonoBehaviour
{
    [Header("This object needs 'Enemy' tag")]
    public GameObject spawner;
    public EnemySpawner script;
    public GameMenuManager menu;
    public GameObject gunRef;
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
    public AudioSource source;
    public AudioClip fireSound;

    [Header("I need these for testing")]
    public float killDelay = 2;
    public float killTimer;
    public bool hitByBullet = false;

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
            //New Look at function
            LookAtTarget();
            gunRef.GetComponent<EnemyGunHandler>().MoveGun(target);

            //Always look at player
            //transform.LookAt(target);

            //if(killTimer >= killDelay) DealDamage(100, true);
            //else killTimer += 1 * Time.deltaTime;
            
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

    

    private void LookAtTarget()
    {
        Vector3 lookPos = target.position - transform.position;
        lookPos.y = 0;
        Quaternion rot = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, 0.2f);
    }

    public void DealDamage(float dmg, bool hitType)
    {
        hitByBullet = hitType;
        Debug.Log("Dealt damage to enemy health.");
        health = health - dmg;
    }

    private void OnDestroy()
    {
        if(hitByBullet) script.EnemyKilled();
    }//END OnDestroy

    public void FireBullet()
    {
        GameObject spawnedBullet = Instantiate(bullet);
        source.PlayOneShot(fireSound);
        spawnedBullet.transform.position = bulletSpawnPoint.position;
        spawnedBullet.transform.rotation = bulletSpawnPoint.rotation;
        spawnedBullet.GetComponent<Rigidbody>().linearVelocity = bulletSpawnPoint.forward * bulletSpd;
        spawnedBullet.GetComponent<EnemyBullet>().menu = menu;
        spawnedBullet.GetComponent<EnemyBullet>().dmg = damage;

        fireRate = 0;
    }
}//END EnemyHandler