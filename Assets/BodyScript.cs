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
    public Vector3 initVelocity;
    public Vector3 currVelocity;
    public List<Vector3> positions = new List<Vector3>();
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

    // Start is called before the first frame update
    public void Start()
    {
        positions.Add(transform.position);
        Body = bodyObj.GetComponent<BodyScript>();
        Spawner = spawnerObj.GetComponent<BodySpawnerScript>();
        Rigidbody = GetComponent<Rigidbody>();
        currVelocity = initVelocity;
        transform.localScale *= radius;
        pos = transform.position;
        gravitationalConstant = 6.6743f * Mathf.Pow(10, -11);
        AUtometer = 1.496f * Mathf.Pow(10,10);

        Renderer rend = bodyObj.GetComponent<Renderer> ();
        rend.material = new Material(Shader.Find("Standard"));
        rend.material.SetColor("_Color", new Color(Random.Range(0.5f, 1.0f), Random.Range(0.8f, 1.0f), Random.Range(0.5f, 1.0f), 1));
    }

    public void UpdateVelocity(BodyScript[] Bodies, float timeStep)
    {
        posIndex++;
        foreach (BodyScript otherBody in Bodies)
        {
            if (otherBody != this)
            {
                float distance = ((otherBody.pos - pos) * AUtometer).sqrMagnitude;
                Vector3 accelerationDir = (otherBody.pos - pos).normalized; 
                float accelerationMag = gravitationalConstant * otherBody.mass / distance;
                currVelocity += accelerationDir * accelerationMag * timeStep;
                UnityEngine.Debug.Log("dist: " + distance);
                UnityEngine.Debug.Log("currVel: " + currVelocity);
            }
        };
    }

    public void UpdatePosition(float timeStep)
    {
        if (posIndex > positions.Count-1)
        {
            positions.Add(transform.position + (currVelocity * (float)(timeStep/AUtometer)));
        }
        transform.position = positions[posIndex];
        pos = transform.position;
        UnityEngine.Debug.Log(this.name + " Remove At: " + positions[0]);
        if (positions.Count >= 10) 
        {
            positions.RemoveAt(0);
            posIndex--;
        }
    }
}