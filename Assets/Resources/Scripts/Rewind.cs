using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rewind : MonoBehaviour
{
    private List<Queue> wallCubeRotationList; // The list save all cube rotation info for revert in the future
    private List<Queue> wallCubePositionList; // The list save all cube position info for revert in the future
    private GameObject[] wallCubeSet; // The set save every object cube for obtain the position
    private int width, height; // width height of wall
    private int frameRewind; // rewind frame
    private bool onRewinding; // is on rewinding
    private int rewindFrame; // var to adjust rewinding speed

    /// <summary>
    /// Data Structure
    /// List<Queue>    [queue1 queue2 ... queueN ]
    /// queueN = [position before 20s position before 19s...position before 1s]
    ///          [rotation before 20s rotation before 19s...rotation before 1s]
    ///          [Vector3 Vector3...Vector3]
    ///          [Quaternion Quaternion...Quaternion]
    /// </summary>



    // Start is called before the first frame update
    void Start()
    {
        // initialize var
        width = 10;
        height = 4;
        frameRewind = 250;
        onRewinding = false;

        // initialize var list queue
        wallCubeSet = GameObject.FindGameObjectsWithTag("wallcube");
        wallCubePositionList = new List<Queue>();
        wallCubeRotationList = new List<Queue>();
        for(int i = 0; i < width * height; i ++)
        {
            wallCubePositionList.Add(new Queue());
            wallCubeRotationList.Add(new Queue());
        }
    }

    // Add one frame but do not delete one frame
    void AddOneFrame()
    {
        for (int i = 0; i < width * height; i++)
        {
            wallCubePositionList[i].Enqueue(wallCubeSet[i].transform.position);
            wallCubeRotationList[i].Enqueue(wallCubeSet[i].transform.rotation);
        }

    }

    // Add one frame but delete one frame as well
    void UpdateOneFrame()
    {
        for (int i = 0; i < width * height; i++)
        {
            wallCubePositionList[i].Enqueue(wallCubeSet[i].transform.position);
            wallCubePositionList[i].Dequeue();
            wallCubeRotationList[i].Enqueue(wallCubeSet[i].transform.rotation);
            wallCubeRotationList[i].Dequeue();
        }
    }

    // wall rewind
    void rewind()
    {
        for (int i = 0; i < width * height; i++)
        {
            object[] wallCubeArray = wallCubePositionList[i].ToArray();
            object[] wallRotationCubeArray = wallCubeRotationList[i].ToArray();
            wallCubeSet[i].transform.position = (Vector3)wallCubeArray[rewindFrame];
            wallCubeSet[i].transform.rotation = (Quaternion)wallRotationCubeArray[rewindFrame];
        }
        rewindFrame--;
        if(rewindFrame < 0)
        {
            onRewinding = false;

            //Open Gravity
            SetWallGravity(true);
        }
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        // record wall cube state
        if (wallCubePositionList[0].Count < frameRewind && onRewinding == false)
        {
            AddOneFrame();
        }
        else if (wallCubePositionList[0].Count == frameRewind && onRewinding == false)
        {
            UpdateOneFrame();
        }
        else
        {
            ;
        }



        var inputDevices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevices(inputDevices);
        bool primary2DAxisClickValue;
        foreach (var device in inputDevices)
        {
            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxisClick, out primary2DAxisClickValue) && primary2DAxisClickValue)
            {
                if (!onRewinding)
                {
                    onRewinding = true;
                    rewindFrame = wallCubePositionList[0].Count - 1;
                    Debug.Log(rewindFrame);

                    //Close Gravity
                    SetWallGravity(false);
                }
            }
            
        }

        if (onRewinding)
        {
            rewind();
        }
    }

    void SetWallGravity(bool enable)
    {
        for(int i = 0;i< wallCubeSet.Length; i++)
        {
            var rigid = wallCubeSet[i].GetComponent<Rigidbody>();
            rigid.useGravity = enable;
            rigid.isKinematic = !enable;
        }
    }
}
