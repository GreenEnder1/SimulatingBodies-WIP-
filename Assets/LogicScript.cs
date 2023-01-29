using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogicScript : MonoBehaviour
{
    public GameObject Bodies;
    public GameObject bodyObj;
    private BodySpawnerScript bodySpawner;
    private BodyScript body;
    
    public Text posXText;
    public Text posYText;
    public Text posZText;
    public float posX;
    public float posY;
    public float posZ;
    public void CreateCustomObject()
    {
        posX = float.Parse(posXText.text);
        posY = float.Parse(posYText.text);
        posZ = float.Parse(posZText.text);
        Quaternion initRotation = Quaternion.identity;
        body = bodyObj.GetComponent<BodyScript>();
        bodySpawner = Bodies.GetComponent<BodySpawnerScript>();
        BodyScript newBody = Instantiate(body, new Vector3(posX, posY, posZ), initRotation, Bodies.transform);
        bodySpawner.bodies = FindObjectsOfType<BodyScript>();
        newBody.mass = Random.Range(10, 50);
        newBody.radius = newBody.mass / 10;
    }
}
