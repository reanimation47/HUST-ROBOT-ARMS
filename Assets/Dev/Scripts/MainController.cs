using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
    public RoboticArm roboticArm;
    void Start()
    {
        roboticArm.rotatePart0(120);

    }
}
