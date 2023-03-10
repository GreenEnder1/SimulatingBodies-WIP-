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
    public Vector3 k1;
    public Vector3 k2;
    public Vector3 k3;
    public Vector3 k4;
    public Vector3 k1Acc;
    public Vector3 k2Acc;
    public Vector3 k3Acc;
    public Vector3 k4Acc;
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

    public void UpdatePosition(float timeStep)
    {
        posIndex++;
        k3Acc = calculateAcceleration(k3);
        k4Acc = calculateAcceleration(k4);
        pos = k1 + (timeStep/AUtometer)*currVelocity + timeStep*timeStep*(k1Acc + k2Acc + k3Acc)/(6*AUtometer);
        currVelocity += (timeStep/6)*(k1Acc + (2*k2Acc) + (2*k3Acc) + k4Acc);
        if (posIndex > positions.Count-1)
        {
            positions.AddLast(pos);
        }
        transform.position = positions.Last.Value;
        // UnityEngine.Debug.Log(this.name + " Remove At: " + positions.First);
    }

    public Vector3 Calculatek2 (float timeStep)
    {
        k1 = pos;
        k2 = pos + 0.5f*(timeStep/AUtometer)*currVelocity;
        return k2;
    }
    public Vector3 Calculatek3(float timeStep)
    {
        k1Acc = calculateAcceleration(k1);
        k3 = pos + 0.5f*(timeStep/AUtometer)*(currVelocity + 0.5f*timeStep*k1Acc);
        return k3;
    }

    public Vector3 Calculatek4(float timeStep)
    {
        k2Acc = calculateAcceleration(k2);
        k4 = pos + (timeStep/AUtometer)*(currVelocity + 0.5f*timeStep*k2Acc);
        return k4;
    }

    public Vector3 calculateAcceleration(Vector3 y)
    {
        Vector3 acceleration = Vector3.zero;
        foreach (BodyScript otherBody in Bodies)
        {
            if (otherBody != this)
            {
                float distance = ((otherBody.pos - y) * AUtometer).sqrMagnitude;
                float accelerationMag = (gravitationalConstant * otherBody.mass / distance);
                acceleration += (otherBody.pos - y).normalized * accelerationMag;
            }
        }
        return acceleration;
    }
}