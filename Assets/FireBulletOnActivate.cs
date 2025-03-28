using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;


public class FireBulletOnActivate : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject bullet;
    public Transform spawnPoint;
    public float fireSpeed = 20;
    public float ammo = 8;
    protected bool thing = false;
    public float maxAmmo = 8;
    public InputActionReference reloadButton;
    public bool hasClip = true;

    public XRSocketInteractorTag socket;
    void Start()
    {
        XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
        XRGrabInteractableTwoAttached hoverable = GetComponent<XRGrabInteractableTwoAttached>();
        hoverable.hoverEntered.AddListener(HoverEnteredThing);
        hoverable.hoverExited.AddListener(HoverExitedThing);
        grabbable.activated.AddListener(FireBullet);
        socket.selectEntered.AddListener(ClipInserted);
        socket.selectExited.AddListener(ClipRemoved);
        //reloadButton.action.started += Reload;

    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void FireBullet(ActivateEventArgs arg)
    {
        if(ammo > 0 && hasClip)
        {
            GameObject spawnedBullet = Instantiate(bullet);
            spawnedBullet.transform.position = spawnPoint.position;
            spawnedBullet.GetComponent<Rigidbody>().linearVelocity = spawnPoint.forward * fireSpeed;
            Destroy(spawnedBullet, 5);
            ammo--;
        }
        else if (!hasClip) {
            Debug.Log("No clip");
        }
        else{
            Debug.Log("Out of ammo");
        }
    }
    void ClipInserted(SelectEnterEventArgs args)
    {
        hasClip = true;
        ammo = maxAmmo;
    }

    void ClipRemoved(SelectExitEventArgs args)
    {
        hasClip = false;
    }
        
        
    void HoverEnteredThing(HoverEnterEventArgs arg)
    {
        thing = true;
    }
    void HoverExitedThing(HoverExitEventArgs arg)
    {
        thing = false;
    }
    /*void Reload(InputAction.CallbackContext context)
    {
        if(ammo < 8 && thing == true)
        {
            ammo = maxAmmo;
        }
    }
    */
    
}
