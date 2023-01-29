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
    public GameObject constantsObj;
    private Constants Constants;
    private Quaternion initRotation;
    private float timer;
    public int stepCount;
    private bool active;
    // Start is called before the first frame update
    void Start()
    {
        initRotation = Quaternion.identity;
        body = bodyObj.GetComponent<BodyScript>();
        Constants = constantsObj.GetComponent<Constants>();
        bodies = FindObjectsOfType<BodyScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > Constants.trigger && active == true)
        {
            stepCount++;
            foreach (BodyScript updateBody in bodies)
            {
                updateBody.UpdateVelocity(bodies);
            };
            foreach (BodyScript updateBody in bodies)
            {
                updateBody.UpdatePosition(stepCount);
            }
            timer = 0;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (active) {active = false;}
            else {active = true;}
            timer = 0;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            stepCount++;
            Parallel.ForEach(bodies, updateBody =>
            {
                updateBody.UpdateVelocity(bodies);
            });
            foreach (BodyScript updateBody in bodies)
            {
                updateBody.UpdatePosition(stepCount);
            }
            timer = 0;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            stepCount--;
            foreach (BodyScript updateBody in bodies)
            {
                updateBody.UpdatePosition(stepCount);
            }
            timer = 0;
        }
        timer += Time.deltaTime;
    }
}
