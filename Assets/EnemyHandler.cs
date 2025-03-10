using UnityEngine;

public class EnemyHandler : MonoBehaviour
{
    [Header("This object needs 'Enemy' tag")]
    public GameObject spawner;
    public EnemySpawner script;
    public float timeUntilDeath = 120;
    public float countdown = 0;

    void Start()
    {
        spawner = GameObject.FindGameObjectWithTag("Spawner");
        if (spawner != null) script = spawner.GetComponent<EnemySpawner>();
        else Debug.Log("no spawner found");

        float dist = Vector3.Distance(transform.position, spawner.transform.position);
        Debug.Log("distance between enemy and spawner: " + dist);
    }

    void Update()
    {
        //Debug.Log("countdown" + countdown);
        if (countdown >= timeUntilDeath) Destroy(this.gameObject);
        else countdown += 1 * Time.deltaTime;
    }

    private void OnDestroy()
    {
        Debug.Log("Enemy OnDestroy");
        script.EnemyKilled();
        //spawner.GetComponent<EnemySpawner>().DecayEnemyAlive();
    }
}