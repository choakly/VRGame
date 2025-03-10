using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            Debug.Log("bullet collision detected");
            Destroy(other.gameObject);  //should destroy enemy
        }
    }
}
