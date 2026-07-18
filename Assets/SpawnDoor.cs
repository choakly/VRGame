using UnityEngine;

public class SpawnDoor : MonoBehaviour
{
    [Header("Enemy spawns in front of this object.")]
    [Header("The blue arrow is forward.")]
    public bool active = false;

    /*
    * For stopping repeated spawns at the same door
    */
    public void JustSpawned(bool state, float time)
    {
        active = state;

        Invoke("ResetActive", time * 0.75f);
    }

    private void ResetActive()
    {
        active = false;
    }
}