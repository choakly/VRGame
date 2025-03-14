using UnityEngine;

public class EnemyHandler : MonoBehaviour
{
    [Header("This object needs 'Enemy' tag")]
    public GameObject spawner;
    public EnemySpawner script;
    //public float timeUntilDeath = 120;
    //public float countdown = 0;
    public float maxHealth = 100;
    public float health;
    public float distToOther;

    void Start()
    {
        health = maxHealth;

        //Get spawner reference
        spawner = GameObject.FindGameObjectWithTag("Spawner");
        if (spawner != null) script = spawner.GetComponent<EnemySpawner>();
        else Debug.Log("no spawner found");
    }

    void Update()
    {
        //Debug.Log("countdown" + countdown);
        //if (countdown >= timeUntilDeath) Destroy(this.gameObject);
        //else countdown += 1 * Time.deltaTime;

        if(health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void DealDamage(float dmg)
    {
        Debug.Log("Dealt damage to enemy health.");
        health = health - dmg;
    }

    private void OnDestroy()
    {
        Debug.Log("Enemy OnDestroy");
        script.EnemyKilled();
        //spawner.GetComponent<EnemySpawner>().DecayEnemyAlive();
    }//END OnDestroy
}//END EnemyHandler