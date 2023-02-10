using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class BodySpawnerScript : MonoBehaviour
{
    public GameObject Bodies;
    public GameObject bodyObj;
    private BodyScript body;
    public BodyScript[] bodies;
    public LinkedList<float> currRecordedTimeStamps = new LinkedList<float>();
    public LinkedList<float> prevRecordedTimeStamps = new LinkedList<float>();
    private float timer;
    private float trigger = 0.01f;
    public float timeStep;
    public int stepCount;
    private bool active = true;
    // Start is called before the first frame update
    void Start()
    {
        body = bodyObj.GetComponent<BodyScript>();
        bodies = FindObjectsOfType<BodyScript>();
        timeStep *= trigger;
    }

    // Update is called once per frame
    void Update()
    {
        if (stepCount*timeStep % 1000 == 0)
        {
            foreach (BodyScript updateBody in bodies)
            {
                updateBody.RecordPosition();
            }
            currRecordedTimeStamps.AddLast(stepCount*timeStep);
        }
        if ((prevRecordedTimeStamps.Count > 0 && currRecordedTimeStamps.Count > 0))
        {
            if (prevRecordedTimeStamps.Find(currRecordedTimeStamps.Last.Value) != null)
            {
                foreach (BodyScript updateBody in bodies)
                {
                    updateBody.CalculatePercentError();
                }
            }
        }
        if (stepCount*timeStep >= Mathf.Pow(10, 7))
        {
            active = false;
            foreach (BodyScript updateBody in bodies)
            {
                updateBody.ResetPosition();
            }
            timeStep /= 2;
            stepCount = 0;
            active = true;
            prevRecordedTimeStamps.Clear();
            foreach(float recordedTimeStamp in currRecordedTimeStamps)
            {
                prevRecordedTimeStamps.AddLast(recordedTimeStamp);
            }
            currRecordedTimeStamps.Clear();

        }
        if (active == true)
        {
            stepCount++;
            foreach (BodyScript updateBody in bodies)
            {
                updateBody.UpdateVelocity(bodies, timeStep);
            };
            foreach (BodyScript updateBody in bodies)
            {
                updateBody.UpdatePosition(timeStep);
            }
            //timer = 0;
        }
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     if (active) {active = false;}
        //     else {active = true;}
        //     timer = 0;
        // }
        // if (Input.GetKeyDown(KeyCode.RightArrow))
        // {
        //     stepCount++;
        //     Parallel.ForEach(bodies, updateBody =>
        //     {
        //         updateBody.UpdateVelocity(bodies, timeStep);
        //     });
        //     foreach (BodyScript updateBody in bodies)
        //     {
        //         updateBody.UpdatePosition(timeStep);
        //     }
        //     timer = 0;
        // }
        // if (Input.GetKeyDown(KeyCode.LeftArrow))
        // {
        //     stepCount--;
        //     foreach (BodyScript updateBody in bodies)
        //     {
        //         updateBody.UpdatePosition(timeStep);
        //     }
        //     timer = 0;
        // }
        timer += Time.deltaTime;
    }
}
