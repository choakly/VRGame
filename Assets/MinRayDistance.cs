using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MinRayDistance : MonoBehaviour
{
    public XRRayInteractor leftrayInteractor;
    public XRRayInteractor rightrayInteractor;
    public float minRayDistance = 1.2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(leftrayInteractor == null && rightrayInteractor == null)
        {
            leftrayInteractor = GetComponent<XRRayInteractor>();
            rightrayInteractor = GetComponent<XRRayInteractor>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hitInfo;
        Vector3 leftrayDirection = leftrayInteractor.transform.forward;
        Vector3 rightrayDirection = rightrayInteractor.transform.forward;

        if(Physics.Raycast(leftrayInteractor.transform.position + leftrayDirection * minRayDistance, leftrayDirection, out hitInfo, Mathf.Infinity) || Physics.Raycast(leftrayInteractor.transform.position + rightrayDirection * minRayDistance, rightrayDirection, out hitInfo, Mathf.Infinity))
        {

        }
    }
}
