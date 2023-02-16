using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.IO;
using UnityEngine;

public class BodySpawnerScript : MonoBehaviour
{
    public GameObject Bodies;
    public GameObject bodyObj;
    private BodyScript body;
    public BodyScript[] bodies;
    public LinkedList<float> currRecordedTimeStamps = new LinkedList<float>();
    public LinkedList<float> prevRecordedTimeStamps = new LinkedList<float>();
    public LinkedList<float> results = new LinkedList<float>();
    private float timer;
    private float trigger = 0.01f;
    public float timeStep;
    public int stepCount;
    private bool active = true;
    private int accuracy = 0;
    private double machineEpsilon = 1.11022302462516E-16;
    // Start is called before the first frame update
    void Start()
    {
        body = bodyObj.GetComponent<BodyScript>();
        bodies = FindObjectsOfType<BodyScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((stepCount*timeStep % 1000) - (machineEpsilon * 8) <= 0 && active == true)
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
                int index = 0;
                foreach (float timeStamps in prevRecordedTimeStamps)
                {
                    if (timeStamps.Equals(currRecordedTimeStamps.Last.Value))
                    {
                        break;
                    }
                    index++;
                }
                foreach (BodyScript updateBody in bodies)
                {
                    updateBody.CalculatePercentError(index);
                }
            }
        }
        if (stepCount*timeStep >= Mathf.Pow(10, 7) && active == true)
        {
            active = false;
            foreach (BodyScript updateBody in bodies)
            {
                updateBody.ResetPosition();
                if(updateBody.accurate == true && updateBody.firstAccurate == false)
                {
                    accuracy++;
                    UnityEngine.Debug.Log("Accurate: " + updateBody.name);
                    updateBody.firstAccurate = true;
                }
            }
            if(accuracy >= bodies.Length)
            {
                foreach (BodyScript updateBody in bodies)
                {
                    results.AddLast(updateBody.GatherResults());
                }
                ExportResults();
                active = false;
            }
            else
            {
                timeStep /= 2;
                stepCount = 0;
                active = true;
            }
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

    public void ExportResults()
    {
        string[][] distanceData = new string [bodies.Length+2][];
        distanceData[0] = new string[] {"Timestep", "", "", ""};
        distanceData[1] = new string[] {timeStep.ToString(), "", "", "Distance Difference"};
        int index = 2;
        LinkedListNode<float> bodyResult = results.First;
        foreach (BodyScript updateBody in bodies)
        {
            distanceData[index] = new string[] {"", "", updateBody.name, bodyResult.Value.ToString()};
            UnityEngine.Debug.Log("Distance Data: " + distanceData[index][2] + " Distance Diff - " + distanceData[index][3]);
            index++;
            bodyResult = bodyResult.Next;
        }
        using (StreamWriter writer = new StreamWriter("data.csv"))
        {
            foreach (string[] row in distanceData)
            {
                writer.WriteLine(string.Join(",", row));
            }
        }
    }
}
