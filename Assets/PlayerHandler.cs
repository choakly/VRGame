using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    public CharacterController charCont;
    public CapsuleCollider capsuleCollider;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        capsuleCollider.radius = charCont.radius;
        capsuleCollider.height = charCont.height;
        capsuleCollider.center = charCont.center;

    }

    // Update is called once per frame
    void Update()
    {
        capsuleCollider.radius = charCont.radius;
        capsuleCollider.height = charCont.height;
        capsuleCollider.center = charCont.center;
    }
}
