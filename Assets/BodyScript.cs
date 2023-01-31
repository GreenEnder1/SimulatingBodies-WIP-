using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
public class BodyScript : MonoBehaviour
{
    public float mass; // kg * 10^24
    public float radius;
    public Vector3 initVelocity;
    public Vector3 currVelocity;
    public List<Vector3> positions = new List<Vector3>();
    public float distance; //Astronomical Units / 10 OR unity units
    public Vector3 accelerationDir;
    public Vector3 acceleration;
    public Vector3 pos;
    private float gravitationalConstant;
    
    public GameObject bodyObj;
    private BodyScript Body;
    private Rigidbody Rigidbody;

    // Start is called before the first frame update
    public void Start()
    {
        positions.Add(transform.position);
        Body = bodyObj.GetComponent<BodyScript>();
        Rigidbody = GetComponent<Rigidbody>();
        currVelocity = initVelocity;
        transform.localScale *= radius;
        pos = transform.position;
        UnityEngine.Debug.Log(this.name);
        UnityEngine.Debug.Log(initVelocity);
        gravitationalConstant = 6.6743f * Mathf.Pow(10, -11);

        Renderer rend = bodyObj.GetComponent<Renderer> ();
        rend.material = new Material(Shader.Find("Standard"));
        rend.material.SetColor("_Color", new Color(Random.Range(0.5f, 1.0f), Random.Range(0.8f, 1.0f), Random.Range(0.5f, 1.0f), 1));
        UnityEngine.Debug.Log("Mass: " + mass);
    }

    public void UpdateVelocity(BodyScript[] Bodies, float timeStep)
    {
        foreach (BodyScript otherBody in Bodies)
        {
            if (otherBody != this)
            {
                float distance = (otherBody.pos - pos).sqrMagnitude;
                Vector3 accelerationDir = (otherBody.pos - pos).normalized; 
                float accelerationMag = gravitationalConstant * otherBody.mass / distance;
                Vector3 acceleration = accelerationMag * accelerationDir;
                currVelocity += accelerationDir * accelerationMag * timeStep;
                UnityEngine.Debug.Log("dist: " + distance);
                UnityEngine.Debug.Log("Acceleration Mag" + accelerationMag);
                UnityEngine.Debug.Log("Acceleration Dir" + accelerationDir);
                UnityEngine.Debug.Log("Acc: " + acceleration);
                UnityEngine.Debug.Log("currVel: " + currVelocity);
            }
        };
    }

    public void UpdatePosition(int stepCount, float timeStep)
    {
        if (stepCount > positions.Count-1)
        {
            positions.Add(transform.position + (currVelocity * timeStep));
        }
        transform.position = positions[stepCount];
        pos = transform.position;
    }
}