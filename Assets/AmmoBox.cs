using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class AmmoBox : MonoBehaviour
{
    public GameObject pistolClip;
    public Transform spawnPosition;
    public GameObject spawnedClip;
    public XRController leftController;
    public InputActionReference grabButton;
    public XRGrabInteractable grabInteractable;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        grabButton.action.started += GrabAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        if(grabButton.action.WasPressedThisFrame() && spawnedClip == null)
        {
            SpawnClip();
        }
    }
    void SpawnClip()
    {
        spawnedClip = Instantiate(pistolClip, spawnPosition.position, spawnPosition.rotation);
        spawnedClip.transform.position += new Vector3(0f, 0.05f, 0f);
        grabInteractable = spawnedClip.AddComponent<XRGrabInteractable>();
        grabInteractable.interactionLayers = LayerMask.GetMask("Default");
        grabInteractable.selectEntered.AddListener(OnClipGrabbed);
        grabInteractable.selectExited.AddListener(OnClipReleased);
        grabInteractable.interactionLayers = LayerMask.GetMask("Default");
    }

    void OnClipGrabbed(SelectEnterEventArgs args)
    {

    }

    void OnClipReleased(SelectExitEventArgs args)
    {

    }

    void GrabAmmo(InputAction.CallbackContext context)
    {
        SpawnClip();
    }
}
