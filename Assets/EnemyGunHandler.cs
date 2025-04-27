using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyGunHandler : MonoBehaviour
{
    public void MoveGun(Transform target)
    {
        Vector3 lookPos = target.position - transform.position;
        //lookPos.y = 0;
        Quaternion rot = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, 0.2f);
    }
}
