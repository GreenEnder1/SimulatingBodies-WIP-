using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Linq;
using UnityEngine;

public class BodySpawnerScript : MonoBehaviour
{
    public GameObject Bodies;
    public GameObject bodyObj;
    private BodyScript body;
    public BodyScript[] bodies;
    public LinkedList<float> currRecordedTimeStamps = new LinkedList<float>();
    public LinkedList<float> prevRecordedTimeStamps = new LinkedList<float>();
    public LinkedList<float> timeSteps = new LinkedList<float>();
    public LinkedList<float> results = new LinkedList<float>();
    private float timer;
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
        if (prevRecordedTimeStamps.Count > 0 && currRecordedTimeStamps.Count > 0)
        {
            bool matchedTimeStamp = false;
            foreach (float value in prevRecordedTimeStamps)
            {
                if (value == currRecordedTimeStamps.Last.Value)
                {
                    matchedTimeStamp = true;
                    break;
                }
            }
            if (matchedTimeStamp)
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
            timeSteps.AddLast(timeStep);
            if(accuracy >= bodies.Length)
            {
                ExportResults();
                active = false;
            }
            else
            {
                timeStep = (timeStep/2) - (float)(machineEpsilon * 8);
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
                updateBody.Calculatek2(timeStep);
                updateBody.Calculatek3(timeStep);
                updateBody.Calculatek4(timeStep);
            };
            foreach (BodyScript updateBody in bodies)
            {
                updateBody.UpdatePosition(timeStep);
            }
            foreach (BodyScript updateBody in bodies)
            {
                updateBody.RecordPosition();
            }
            currRecordedTimeStamps.AddLast(stepCount*timeStep);
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
        string[][] distanceData = new string[timeSteps.Count+1][];
        string[] titles = new string[bodies.Length + 1];
        titles[0] = "Timestep";
        for(int i = 1; i <= bodies.Length; i++)
        {
            titles[i] = bodies[i-1].name;
        }
        distanceData[0] = titles;
        for (int i = 1; i <= timeSteps.Count; i++)
        {
            string[] simRun = new string[bodies.Length + 1];
            simRun[0] = timeSteps.ElementAt(i-1).ToString();
            for(int j = 1; j <= bodies.Length; j++)
            {
                simRun[j] = bodies[j-1].avgDistanceDiffs.ElementAt(i-1).ToString();
            }
            distanceData[i] = simRun;
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
