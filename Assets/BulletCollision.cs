using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    public float dmg = 100;
    public EnemyHandler enemyHandlerRef;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("bullet collision detected");

        if (other.CompareTag("Enemy"))
        {
            Debug.Log("enemy hit by bullet");

            //Deal damage to collider (enemy)
            enemyHandlerRef = other.gameObject.GetComponent<EnemyHandler>();
            enemyHandlerRef.DealDamage(dmg);

            //Destroy bullet if enemy is hit
            Destroy(gameObject);
        }
    }
}