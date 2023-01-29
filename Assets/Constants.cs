using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
    public float gravitationalConstants = 1.9936f * Mathf.Pow(10, -17); // unity^3 / kg s^2
    public float trigger; // s
    public float timeStep; // seconds/step
}
