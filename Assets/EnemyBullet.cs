using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float dmg = 100;
    //public EnemyHandler enemyHandlerRef;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("enemy bullet OnTrigger has run");

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