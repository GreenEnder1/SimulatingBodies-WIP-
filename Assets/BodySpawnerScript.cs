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
    private Quaternion initRotation;
    private float timer;
    private float trigger = 0.01f;
    public float timeStep;
    public int stepCount;
    private bool active = true;
    // Start is called before the first frame update
    void Start()
    {
        initRotation = Quaternion.identity;
        body = bodyObj.GetComponent<BodyScript>();
        bodies = FindObjectsOfType<BodyScript>();
        timeStep *= trigger;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > trigger && active == true)
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
            timer = 0;
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
