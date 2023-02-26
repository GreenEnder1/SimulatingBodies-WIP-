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
        currVelocity += RK4(pos, timeStep, CalculateAcceleration);
        return currVelocity;

    }

    public void UpdatePosition(float timeStep)
    {
        posIndex++;
        if (posIndex > positions.Count-1)
        {
            positions.AddLast(transform.position + currVelocity*timeStep);
        }
        transform.position = positions.Last.Value;
        pos = transform.position;
        // UnityEngine.Debug.Log(this.name + " Remove At: " + positions.First);
    }

    Vector3 RK4(Vector3 y, float dt, Func<Vector3, Vector3> f)
    {
        Vector3 k1 = f(y);
        Vector3 k2 = f(y + (k1*dt/2));
        Vector3 k3 = f(y + (k2*dt/2));
        Vector3 k4 = f(y + (k3*dt));

        Vector3 dy = y + (dt/6)*(k1 + 2*k2 + 2*k3 + k4);
        UnityEngine.Debug.LogFormat("pos dy: {0} vel dy: {1}", dy[0], dy[1]);
        return dy;
    }

    private Vector3 CalculateAcceleration(Vector3 y)
    {
        Vector3 dydt = Vector3.zero;
        foreach (BodyScript otherBody in Bodies)
        {
            if (otherBody != this)
            {
                float distance = ((otherBody.pos - y) * AUtometer).sqrMagnitude;
                Vector3 acceleration = (otherBody.pos - y).normalized * (gravitationalConstant * otherBody.mass / distance);
                dydt += acceleration;
                // UnityEngine.Debug.Log("dist: " + distance);
                // UnityEngine.Debug.Log("currVel: " + currVelocity);
            }
        };
        return dydt;
    }
}