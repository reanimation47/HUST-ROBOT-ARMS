using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//this is a prototype script

public class ArmController : MonoBehaviour
{
        
    #region Refs
    // gravity will work if this is enabled
    public bool gravityON;

    //toque applied to each part of the robot
    public float[] torque;
    //error in quaternion
    public float error = 0.001f;

    //drags
    public float drag = 0.1f;
    public float angDrag = 0.01f;

    //movement of the robot
    public float stepMovement;
    public bool automovement = false;


    //part fo the robot
    public Rigidbody part0;
    public Rigidbody part1;
    public Rigidbody part2;
    public Rigidbody part3;
    public Rigidbody gripLeft;
    public Rigidbody gripRight;

    public bool grip = false;

    //rigidbodies
    Rigidbody[] rbs;




    //quaternions that are used for displating the arm rotation
    public Quaternion t_arm, q1_arm, q2_arm, q3_arm;
    #endregion

    #region Movement State Variables
    public HoriontalMovement Part0_mState = HoriontalMovement.NONE;
    public VerticalMovement Part1_mState, Part2_mState, Part3_mState = VerticalMovement.NONE;
    #endregion

    #region  MonoBehavior
    void Start()
    {
        //create array of rigidbodies for future use
        rbs = new Rigidbody[6];
        rbs[0] = part0;
        rbs[1] = part1;
        rbs[2] = part2;
        rbs[3] = part3;
        rbs[4] = gripLeft;
        rbs[5] = gripRight;

        //show reative rotation on the inspector and set the initial rotations
        t_arm = part0.transform.rotation;
        q1_arm = part1.transform.rotation;
        q2_arm = part2.transform.rotation;
        q3_arm = part3.transform.rotation;

        //


        if (automovement)
        { StartCoroutine(FirstMove()); }
 

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //set gravity if changed
        if (gravityON)
        {
            for (int ii = 0; ii < rbs.Length; ii++)
            {
                rbs[ii].useGravity = true;
            }

        }
        else
        {
            for (int ii = 0; ii < rbs.Length; ii++)
            {
                rbs[ii].useGravity = false;
            }
        }


        //set drags

        for (int ii = 0; ii < rbs.Length; ii++)
        {
            rbs[ii].drag = drag;
            rbs[ii].angularDrag = angDrag;
        }





        //setting forces to zero
        //setFrictionToJoints();

        if (automovement == false)
        {
            //moving part 0
            if (Input.GetKey("a"))
            {
                Debug.LogWarning("aaa");
                part0.AddTorque(-torque[0] * part0.mass * part0.transform.forward);
            }
            if (Input.GetKey("d"))
            {
                part0.AddTorque(torque[0] * part0.mass * part0.transform.forward);
            }

            //moving part 1
            if (Input.GetKey("w"))
            {
                part1.AddTorque(-torque[1] * part1.mass * part1.transform.forward);
            }
            if (Input.GetKey("s"))
            {
                part1.AddTorque(torque[1] * part1.mass * part1.transform.forward);
            }

            //moving part 2
            if (Input.GetKey("q"))
            {
                part2.AddTorque(-torque[2] * part2.mass * part2.transform.forward);

            }
            if (Input.GetKey("e"))
            {
                part2.AddTorque(torque[2] * part2.mass * part2.transform.forward);
            }

            //moving part 3
            if (Input.GetKey("z"))
            {
                part3.AddTorque(-torque[3] * part3.mass * part3.transform.right);
            }
            if (Input.GetKey("c"))
            {
                part3.AddTorque(torque[3] * part3.mass * part3.transform.right);
            }


            //moving grips
            if (Input.GetKeyDown("x"))
            {
                grip = !grip;

            }



        }


        CheckForMovementStates();

        if (grip)
        {
            gripLeft.AddTorque(torque[4] * gripLeft.mass * gripLeft.transform.forward);
            gripRight.AddTorque(torque[4] * gripRight.mass * gripRight.transform.forward);

        }
        else
        {
            gripLeft.AddTorque(-torque[4] * gripLeft.mass * gripLeft.transform.forward);
            gripRight.AddTorque(-torque[4] * gripRight.mass * gripRight.transform.forward);

        }



        //show reative rotation on the inspector
        t_arm = part0.transform.localRotation;
        q1_arm = part1.transform.localRotation;
        q2_arm = part2.transform.localRotation;
        q3_arm = part3.transform.localRotation;


    }
    #endregion

    #region Customs
    private void CheckForMovementStates()
    {
        //Rotating part 0 // Arm's base
        switch(Part0_mState)
        {
            case HoriontalMovement.CLOCKWISE:
                part0.AddTorque(torque[0] * part0.mass * part0.transform.forward);
                break;
            case HoriontalMovement.COUNTERCLOCKWISE:
                part0.AddTorque(-torque[0] * part0.mass * part0.transform.forward);
                break;
            default:
                break;
        }
        
        switch(Part1_mState)
        {
            case VerticalMovement.UPWARDS:
                part1.AddTorque(-torque[1] * part1.mass * part1.transform.forward);
                break;
            case VerticalMovement.DOWNWARDS:
                part1.AddTorque(torque[1] * part1.mass * part1.transform.forward);
                break;
            default:
                break;
        }

        switch(Part2_mState)
        {
            case VerticalMovement.UPWARDS:
                part2.AddTorque(torque[2] * part2.mass * part2.transform.forward);
                break;
            case VerticalMovement.DOWNWARDS:
                part2.AddTorque(-torque[2] * part2.mass * part2.transform.forward);
                break;
            default:
                break;
        }
        switch(Part3_mState)
        {
            case VerticalMovement.UPWARDS:
                part3.AddTorque(-torque[3] * part3.mass * part3.transform.right);
                break;
            case VerticalMovement.DOWNWARDS:
                part3.AddTorque(torque[3] * part3.mass * part3.transform.right);
                break;
            default:
                break;
        }

    }

    #endregion

    IEnumerator FirstMove()
    {
        while (true)
        {


        }
    }


}


