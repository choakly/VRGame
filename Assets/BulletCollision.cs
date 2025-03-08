using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    [SerializeField] private GameObject spawner;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger");
        Destroy(other.gameObject);
        spawner.GetComponent<EnemySpawner>().EnemyKilled();
        Destroy(this.gameObject);
    }
}
