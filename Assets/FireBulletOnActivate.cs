using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;


public class FireBulletOnActivate : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public enum FireMode { SemiAuto, FullAuto }
    public FireMode fireMode = FireMode.SemiAuto;
    public float fireRate = 0.2f;
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
    public AudioSource source;
    public AudioClip fireSound;
    public Upgrades upgrades;
    public float baseDamage;
    private bool isFiring = false;
    private float nextFireTime = 0f;
    void Start()
    {
        
        directInteractor = FindAnyObjectByType<XRDirectInteractor>();
        rayInteractor = FindAnyObjectByType<XRRayInteractor>();
        grabbable = GetComponent<XRGrabInteractable>();
        XRGrabInteractableTwoAttached hoverable = GetComponent<XRGrabInteractableTwoAttached>();
        
        hoverable.hoverEntered.AddListener(HoverEnteredThing);
        hoverable.hoverExited.AddListener(HoverExitedThing);
        grabbable.activated.AddListener(FireBulletXR);
        grabbable.deactivated.AddListener(ctx => isFiring = false);
        socket.selectEntered.AddListener(ClipInserted);
        socket.selectExited.AddListener(ClipRemoved);
        socket.hoverEntered.AddListener(HandNearSocket);
        socket.hoverExited.AddListener(HandLeftSocket);
        //reloadButton.action.started += Reload;

    }

    // Update is called once per frame
    void Update()
    {
        if (fireMode == FireMode.FullAuto && isFiring && Time.time >= nextFireTime)
        {
            FireBullet();
            nextFireTime = Time.time + fireRate;
        }
    }
    public float GetDamage()
    {
        return baseDamage * (upgrades != null ? upgrades.damageMultiplier : 1f);
    }

    public void FireBulletXR(ActivateEventArgs args)
    {
        if (fireMode == FireMode.SemiAuto)
        {
            FireBullet();
        }
        else if (fireMode == FireMode.FullAuto)
        {
            isFiring = true;
            
        }
    }

    public void FireBullet()
    {
        if (ammo > 0 && hasClip)
        {

            GameObject spawnedBullet = Instantiate(bullet);
            spawnedBullet.transform.position = spawnPoint.position;
            spawnedBullet.GetComponent<Rigidbody>().linearVelocity = spawnPoint.forward * fireSpeed;
            BulletCollision bulletScript = spawnedBullet.GetComponent<BulletCollision>();
            if (bulletScript != null)
            {
                bulletScript.sourceGun = this;
            }
            source.PlayOneShot(fireSound);
            Destroy(spawnedBullet, 5);
            ammo--;
        }
        else if (!hasClip)
        {
            Debug.Log("No clip");
        }
        else
        {
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
