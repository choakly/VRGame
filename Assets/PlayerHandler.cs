using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    public float maxHealth = 100;
    public float health;
    public string playerName;
    public CharacterController charCont;
    public CapsuleCollider capsuleCollider;
    public SceneTransitionManager transitionManager;
    public bool isDead = false;
    
    void Start()
    {
        capsuleCollider.radius = charCont.radius;
        capsuleCollider.height = charCont.height;
        capsuleCollider.center = charCont.center;

        health = maxHealth;
        playerName = "";
    }

    // Update is called once per frame
    void Update()
    {
        capsuleCollider.radius = charCont.radius;
        capsuleCollider.height = charCont.height;
        capsuleCollider.center = charCont.center;

        if((health <= 0) && (!isDead))
        {
            isDead = true;
            transitionManager.GoToSceneAsync(0);
        }
    }

    public void DamagePlayer(float dmg)
    {
        health = health - dmg;
    }
}