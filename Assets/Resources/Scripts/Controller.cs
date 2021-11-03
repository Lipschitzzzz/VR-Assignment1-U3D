using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    private Vector3 previousFramePosition = Vector3.zero; // controller previous position
    private float speed = 0f; // controller moving speed
    private Vector3 direction; // controller moving direction

    public static Controller Instance;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public void OnTriggerStay(Collider other)
    {
        var inputDevices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevices(inputDevices);
        float triggerValue;
        foreach (var device in inputDevices)
        {
            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.trigger, out triggerValue) && triggerValue > 0.7f)
            {
                if (other.attachedRigidbody)
                {
                    other.attachedRigidbody.useGravity = false;
                    other.attachedRigidbody.velocity = Vector3.zero;
                    other.attachedRigidbody.angularVelocity = Vector3.zero;
                }
                other.transform.parent = this.transform.parent.transform;
                // Debug.Log("collided");
            }
            else if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.trigger, out triggerValue) && triggerValue < 0.7f)
            {
                int childCountOnController = this.transform.parent.transform.childCount;

                if (childCountOnController >= 1)
                {
                    for (int i = 1; i < childCountOnController; i++)
                    {
                        Transform cubeGrabbed = this.transform.parent.transform.GetChild(i);
                        cubeGrabbed.parent = null;
                        Collider collider = cubeGrabbed.GetComponent<Collider>();
                        collider.attachedRigidbody.useGravity = true;
                        collider.attachedRigidbody.AddForce(direction * speed * 100f);
                    }
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame

    void Update()
    {

        // Get direction
        Vector3 posA = previousFramePosition;
        Vector3 posB = transform.position;

        //Destination - Origin
        this.direction = (posB - posA).normalized;

        // Get speed
        float movementPerFrame = Vector3.Distance(previousFramePosition, transform.position);
        this.speed = movementPerFrame / Time.deltaTime;
        previousFramePosition = transform.position;

    }
}
