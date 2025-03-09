using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    [SerializeField] private GameObject spawner;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("bullet trigger");
        Destroy(other.gameObject);
        spawner.GetComponent<EnemySpawner>().EnemyKilled();
        Destroy(this.gameObject);
    }
}
