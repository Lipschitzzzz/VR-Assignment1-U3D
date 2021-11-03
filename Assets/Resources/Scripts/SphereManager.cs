using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereManager : MonoBehaviour
{
    public GameObject sphere;

    // Generate sphere under the controller
    void Generate_Sphere()
    {
        var inputDevices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevices(inputDevices);
        bool menuButtonValue;
        foreach (var device in inputDevices)
        {
            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.menuButton, out menuButtonValue) && menuButtonValue)
            {
                Instantiate(sphere, Controller.Instance.transform.position + new Vector3(0.0f, -0.2f, 0.0f), transform.rotation,transform);
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
        Generate_Sphere();
    }
}
