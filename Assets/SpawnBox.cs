using UnityEngine;

public class SpawnBox : MonoBehaviour
{
    public EnemySpawner spawner;
    [Header("Spawn door objects need to be added manually to the array")]
    public Transform[] spawnDoors;

    //Get all doors in box, make spawner change what box is active, only spawn enemies in active box/doors

    void Start()
    {
        spawner = GameObject.FindAnyObjectByType<EnemySpawner>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            //Debug.LogWarning("spawn box on trigger has run");
            spawner.ChangeActiveBox(gameObject);
        }
    }

    public Transform[] GetSpawnDoors()
    {
        return spawnDoors;
    }
}