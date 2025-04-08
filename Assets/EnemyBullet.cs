using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float dmg = 100;
    public GameMenuManager menu;
    public Vector3 lastVel;
    public float delay;
    public float deathTimerMax = 20;
    public float deathTimerRem;

    private void Start()
    {
        lastVel = GetComponent<Rigidbody>().linearVelocity;
        delay = 0;
        deathTimerRem = 0;
    }

    private void Update()
    {
        if(menu.gamePaused)
        {
            GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            Debug.Log("Bullet paused?");
            delay = 0.5f;
        }
        else if(delay <= 0)
        {
            GetComponent<Rigidbody>().linearVelocity = lastVel;
            DeathByTimer();
        }
        else delay -= Time.deltaTime;
    }

    public void DeathByTimer()
    {
        if(deathTimerRem >= deathTimerMax)
        {
            Destroy(gameObject);
            //Debug.Log("DeathByTimer");
        }
        deathTimerRem += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("enemy bullet OnTrigger has run");

        if(other.CompareTag("Player"))
        {
            Debug.Log("player hit by bullet");

            //Deal damage to collider (enemy)
            //enemyHandlerRef = other.gameObject.GetComponent<EnemyHandler>();
            //enemyHandlerRef.DealDamage(dmg);

            //Destroy bullet if enemy is hit
            Destroy(gameObject);
        }
    }
}