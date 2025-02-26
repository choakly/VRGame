using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateHandOnInput : MonoBehaviour
{
    //Unity XRI stuff, basically the "variable" is an action, more class types rather than data types (i think)
    public InputActionProperty pinchAnimationAction;
    public InputActionProperty gripAnimationAction;
    public Animator handAnimator;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // for both the trigger and grip, they have floats (bc they're on axes), but we read the value, 
        // and the animator for the hand assigns it to either the trigger or grip, 
        // and then in they're BlendTrees (idk) assigns that read value. Kind of stagnant but it works for our purposes of VR looking stuff
        // Can also use these for when we want game stuff to happen ex: if triggerValue > 0.1 { shoot() }
        float triggerValue = pinchAnimationAction.action.ReadValue<float>();
        handAnimator.SetFloat("Trigger",triggerValue);

        float gripValue = gripAnimationAction.action.ReadValue<float>();
        handAnimator.SetFloat("Grip",gripValue);
    }
}
