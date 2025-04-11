using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameMenuManager : MonoBehaviour
{
    public GameObject menu;
    public InputActionProperty showButton;
    public Transform head;
    public float spawnDistance = 2;
    public bool gamePaused = false;
    public GameObject gun;
    private FireBulletOnActivate fireGun;

    void Start()
    {
        fireGun = gun.GetComponent<FireBulletOnActivate>();
    }

    // Update is called once per frame
    void Update()
    {
        if(showButton.action.WasPerformedThisFrame())
        {
            menu.SetActive(!menu.activeSelf);
            menu.transform.position = head.position + new Vector3(head.forward.x,0,head.forward.z).normalized * spawnDistance;
            gamePaused = !gamePaused;

            if(fireGun.isActiveAndEnabled) fireGun.enabled = false;
            else fireGun.enabled = true;
        }

        menu.transform.LookAt(new Vector3 (head.position.x, menu.transform.position.y, head.position.z));
        menu.transform.forward *= -1;
    }
}