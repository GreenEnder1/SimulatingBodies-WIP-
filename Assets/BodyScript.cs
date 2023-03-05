using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System;
using UnityEngine;
public class BodyScript : MonoBehaviour
{
    public float mass; // kg
    public float radius;
    public Vector3 initVelocity;
    public Vector3 currVelocity;
    public LinkedList<Vector3> positions = new LinkedList<Vector3>();
    public float distance; //Astronomical Units / 10 OR unity units
    public Vector3 accelerationDir;
    public Vector3 acceleration;
    public Vector3 pos;
    private float gravitationalConstant;
    private float AUtometer;
    public int posIndex;
    
    public GameObject bodyObj;
    public GameObject spawnerObj;
    private BodyScript Body;
    private BodySpawnerScript Spawner;
    private Rigidbody Rigidbody;
    private BodyScript[] Bodies;

    // Start is called before the first frame update
    public void Start()
    {
        positions.AddLast(transform.position);
        Body = bodyObj.GetComponent<BodyScript>();
        Bodies = FindObjectsOfType<BodyScript>();
        Spawner = spawnerObj.GetComponent<BodySpawnerScript>();
        Rigidbody = GetComponent<Rigidbody>();
        currVelocity = initVelocity;
        transform.localScale *= radius;
        pos = transform.position;
        gravitationalConstant = 6.6743f * Mathf.Pow(10, -11);
        AUtometer = 1.496f * Mathf.Pow(10,10);

        Renderer rend = bodyObj.GetComponent<Renderer> ();
        rend.material = new Material(Shader.Find("Standard"));
        rend.material.SetColor("_Color", new Color(UnityEngine.Random.Range(0.5f, 1.0f), UnityEngine.Random.Range(0.5f, 1.0f), UnityEngine.Random.Range(0.5f, 1.0f), 1));
    }

    public Vector3 UpdateVelocity(float timeStep)
    {
        currVelocity += RK4(pos, timeStep) * timeStep;
        return currVelocity;

    }

    public void UpdatePosition(float timeStep)
    {
        posIndex++;
        if (posIndex > positions.Count-1)
        {
            positions.AddLast(transform.position + currVelocity*(timeStep/AUtometer));
        }
        transform.position = positions.Last.Value;
        pos = transform.position;
        // UnityEngine.Debug.Log(this.name + " Remove At: " + positions.First);
    }

    Vector3 RK4(Vector3 y, float dt)
    {
        Vector3 acceleration = Vector3.zero;
        foreach (BodyScript otherBody in Bodies)
        {
            if (otherBody != this)
            {
                float distance = ((otherBody.pos - y) * AUtometer).sqrMagnitude;
                float accelerationMag = (gravitationalConstant * otherBody.mass / distance);
                Vector3 k1 = (otherBody.pos - y) * accelerationMag;

                Vector3 tempVel = partialStep(currVelocity, k1, 0.5f);
                Vector3 tempPos = partialStep(pos, tempVel, 0.5f);

                Vector3 k2 = (tempPos - (tempPos+(tempVel*0.5f*dt))) * accelerationMag;

                tempVel = partialStep(currVelocity, k2, 0.5f);

                Vector3 k3 = (tempPos - (tempPos+(tempVel*0.5f*dt))) * accelerationMag;

                tempVel = partialStep(currVelocity, k3, 1);
                tempPos = partialStep(pos, tempVel, 0.5f);

                Vector3 k4 = (otherBody.pos - (tempPos+(tempVel*0.5f*dt))) * accelerationMag;
                acceleration += (k1 + (2*k2) + (2*k3) + k4)/6;
            }
        };
        return acceleration;
    }
    Vector3 partialStep(Vector3 p1, Vector3 p2, float dt)
    {
        return (p1 + (p2*dt));
    }
}