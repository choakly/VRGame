using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    public FireBulletOnActivate sourceGun;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("bullet collision detected");

        if (other.CompareTag("Enemy"))
        {
            Debug.Log("enemy hit by bullet");

            //Deal damage to collider (enemy)
            EnemyHandler enemyHandlerRef = other.GetComponent<EnemyHandler>();
            if (enemyHandlerRef != null && sourceGun != null)
            {
                float dmg = sourceGun.GetDamage();
                enemyHandlerRef.DealDamage(dmg, true);
            }
            

            //Destroy bullet if enemy is hit
            Destroy(gameObject);
        }
    }
}