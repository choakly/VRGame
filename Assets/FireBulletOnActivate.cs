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
    private XRGrabInteractable grabbable;
    public XRGrabInteractable clipGrabbable;
    public XRSocketInteractorTag socket;
    public XRDirectInteractor directInteractor;
    public XRRayInteractor rayInteractor;
    void Start()
    {
        directInteractor = FindAnyObjectByType<XRDirectInteractor>();
        rayInteractor = FindAnyObjectByType<XRRayInteractor>();
        grabbable = GetComponent<XRGrabInteractable>();
        XRGrabInteractableTwoAttached hoverable = GetComponent<XRGrabInteractableTwoAttached>();
        hoverable.hoverEntered.AddListener(HoverEnteredThing);
        hoverable.hoverExited.AddListener(HoverExitedThing);
        grabbable.activated.AddListener(FireBullet);
        socket.selectEntered.AddListener(ClipInserted);
        socket.selectExited.AddListener(ClipRemoved);
        socket.hoverEntered.AddListener(HandNearSocket);
        socket.hoverExited.AddListener(HandLeftSocket);
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
    void HandNearSocket(HoverEnterEventArgs arg)
    {
        if(grabbable.isSelected)
        {
            return;
        }
        directInteractor.enabled = false;
        rayInteractor.enabled = false;
    }
    void HandLeftSocket(HoverExitEventArgs arg)
    {
        directInteractor.enabled = true;
        rayInteractor.enabled = true;
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
