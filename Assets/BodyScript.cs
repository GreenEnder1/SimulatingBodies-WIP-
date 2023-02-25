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

    // Start is called before the first frame update
    public void Start()
    {
        positions.AddLast(transform.position);
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
        rend.material.SetColor("_Color", new Color(Random.Range(0.5f, 1.0f), Random.Range(0.5f, 1.0f), Random.Range(0.5f, 1.0f), 1));
    }

    public void UpdateVelocity(BodyScript[] Bodies, float timeStep)
    {
        foreach (BodyScript otherBody in Bodies)
        {
            if (otherBody != this)
            {
                float distance = ((otherBody.pos - pos) * AUtometer).sqrMagnitude;
                float accelerationMag = gravitationalConstant * otherBody.mass / distance;
                Vector3[] y = {pos, currVelocity };
                Vector3[] dy = RK4(y, timeStep, acceleration);
                transform.position = y[0] + dy[0];
                currVelocity = y[1] + dy[1];
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
            positions.AddLast(transform.position);
        }
        pos = transform.position;
        // UnityEngine.Debug.Log(this.name + " Remove At: " + positions.First);
    }

    Vector3[] RK4(Vector3[] y, float dt, Vector3 a)
    {

        Vector3[] f(Vector3[] y)
        {
            Vector3[] dydt = new Vector3[2];
            dydt[0] = y[1];
            dydt[1] = a;
            return dydt;
        };


        Vector3[] k1 = f(y);
        Vector3[] k2 = f(AddArrays(y, MultiplyArray(k1, dt / 2)));
        Vector3[] k3 = f(AddArrays(y, MultiplyArray(k2, dt / 2)));
        Vector3[] k4 = f(AddArrays(y, MultiplyArray(k3, dt)));

        Vector3[] dy = MultiplyArray(AddArrays(AddArrays(k1, MultiplyArray(k2, 2)), AddArrays(MultiplyArray(k3, 2), k4)), dt / 6);

        return dy;
    }

    Vector3[] AddArrays(Vector3[] a, Vector3[] b)
    {
        Vector3[] result = new Vector3[a.Length];
        for (int i = 0; i < a.Length; i++)
        {
            result[i] = a[i] + b[i];
        }
        return result;
    }

    Vector3[] MultiplyArray(Vector3[] a, float b)
    {
        Vector3[] result = new Vector3[a.Length];
        for (int i = 0; i < a.Length; i++)
        {
            result[i] = a[i] * b;
        }
        return result;
    }
}