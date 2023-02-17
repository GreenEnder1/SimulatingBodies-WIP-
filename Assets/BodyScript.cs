using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
public class BodyScript : MonoBehaviour
{
    public float mass; // kg
    public float radius;
    public string name;
    public Vector3 initVelocity;
    public Vector3 currVelocity;
    public LinkedList<Vector3> positions = new LinkedList<Vector3>();
    public LinkedList<Vector3> currRecordedPositions = new LinkedList<Vector3>();
    public LinkedList<Vector3> prevRecordedPositions = new LinkedList<Vector3>();
    public LinkedList<bool> matched = new LinkedList<bool>();
    public float distance; //Astronomical Units / 10 OR unity units
    public Vector3 accelerationDir;
    public Vector3 acceleration;
    public Vector3 Initpos;
    private float gravitationalConstant;
    private float AUtometer;
    public int posIndex;
    public bool accurate = false;
    public bool firstAccurate = false;
    
    public GameObject bodyObj;
    public GameObject spawnerObj;
    private BodyScript Body;
    private BodySpawnerScript Spawner;
    private Rigidbody Rigidbody;

    // Start is called before the first frame update
    public void Start()
    {
        positions.AddLast(transform.position);
        Body = bodyObj.GetComponent<BodyScript>();
        Spawner = spawnerObj.GetComponent<BodySpawnerScript>();
        Rigidbody = GetComponent<Rigidbody>();
        name = bodyObj.name;
        currVelocity = initVelocity;
        transform.localScale *= radius;
        Initpos = transform.position;
        gravitationalConstant = 6.6743f * Mathf.Pow(10, -11);
        AUtometer = 1.496f * Mathf.Pow(10,10);

        Renderer rend = bodyObj.GetComponent<Renderer> ();
        rend.material = new Material(Shader.Find("Standard"));
        rend.material.SetColor("_Color", new Color(Random.Range(0.5f, 1.0f), Random.Range(0.5f, 1.0f), Random.Range(0.5f, 1.0f), 1));
    }

    public void UpdateVelocity(BodyScript[] Bodies, float timeStep)
    {
        foreach (BodyScript otherBody in Bodies)
        {
            if (otherBody != this)
            {
                float distance = ((otherBody.transform.position - transform.position) * AUtometer).sqrMagnitude;
                float accelerationMag = gravitationalConstant * otherBody.mass / distance;
                currVelocity += (otherBody.transform.position - transform.position).normalized * accelerationMag * timeStep;
                // UnityEngine.Debug.Log("dist: " + distance);
                // UnityEngine.Debug.Log("currVel: " + currVelocity);
            }
        };
    }

    public void UpdatePosition(float timeStep)
    {
        posIndex++;
        if (posIndex > positions.Count-1)
        {
            positions.AddLast(transform.position + (currVelocity * (float)(timeStep/AUtometer)));
        }
        transform.position = positions.Last.Value;
        //UnityEngine.Debug.Log(this.name + " Remove At: " + positions.First);
    }

    public void RecordPosition()
    {
        currRecordedPositions.AddLast(positions.Last.Value);
    }

    public void MatchedTimestamp(bool value)
    {
        matched.AddLast(value);
    }

    public void ResetPosition()
    {
        prevRecordedPositions.Clear();
        foreach(Vector3 recordedPosition in currRecordedPositions)
        {
            prevRecordedPositions.AddLast(recordedPosition);
        }
        currRecordedPositions.Clear();
        transform.position = Initpos;
        currVelocity = initVelocity;
        posIndex = 0;
        positions.Clear();
    }

    public void GatherResults(LinkedList<float> timeStamps)
    {
        string[][] rawData = new string[currRecordedPositions.Count+1][];
        int Listindex = 0;
        int arrayIndex = 1;
        rawData[0] = new string[] {"Elapsed Time", "Previous X Position", "Previous Y Position" , "Previous Z Position" , "Current X Position" , "Current Y Position" , "Current Z Position" , "Matching Timestep"};
        while (Listindex != currRecordedPositions.Count)
        {
            if (matched.ElementAt(Listindex))
            {
                Vector3 matchedCurrent = currRecordedPositions.ElementAt(Listindex);
                Vector3 matchedPrevious = prevRecordedPositions.Find(matchedCurrent).Value;
                rawData[arrayIndex] = new string[] {timeStamps.ElementAt(Listindex).ToString(), matchedPrevious.x.ToString(), matchedPrevious.y.ToString(), matchedCurrent.z.ToString(), matchedCurrent.x.ToString(), matchedCurrent.y.ToString(), matchedCurrent.z.ToString(), matched.ElementAt(Listindex).ToString()};
                arrayIndex++;
            }
        }
    }
}