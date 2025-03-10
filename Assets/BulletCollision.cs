using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            Debug.Log("bullet collision detected");
            Destroy(other.gameObject);  //should destroy enemy
        }
        else Destroy(this.gameObject);
    }
}
