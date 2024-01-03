using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;


//this is a prototype script

public class ArmController : MonoBehaviour
{
        
    #region Refs
    public Transform targetBoxDir;//Same pos as box to pick up
    public Transform targetBoxDir_drop;//Same pos as drop off 
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

    public int CurrentSequenceState = 0;
    public float MoveSpeed = 2; // Less -> More accuracy



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


        //{ StartCoroutine(FirstMove()); }
        //Manager.Instance.respawnRobot(this.gameObject);
 

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

        if (!automovement == false)
        {
            //ManualControls();
        }


        UpdateRotationValues();
        CheckForMovementStates();

        if (grip)
        {
            gripLeft.AddTorque(torque[4] * gripLeft.mass * gripLeft.transform.forward * MoveSpeed);
            gripRight.AddTorque(torque[4] * gripRight.mass * gripRight.transform.forward * MoveSpeed);

        }
        else
        {
            gripLeft.AddTorque(-torque[4] * gripLeft.mass * gripLeft.transform.forward* MoveSpeed);
            gripRight.AddTorque(-torque[4] * gripRight.mass * gripRight.transform.forward* MoveSpeed);

        }

        DetectBox();

        //AdjustPerfectRotation();
        PickAndDropSequence();



        //Debug.LogWarning(q3_arm.x);

            // var fixedRotation = part0.transform.localRotation.eulerAngles;
            // Debug.LogWarning(fixedRotation);

    }
    #endregion

    #region Movements
    private void CheckForMovementStates()
    {
        //Rotating part 0 // Arm's base
        switch(Part0_mState)
        {
            case HoriontalMovement.CLOCKWISE:
                part0.AddTorque(torque[0] * part0.mass * part0.transform.forward * MoveSpeed);
                break;
            case HoriontalMovement.COUNTERCLOCKWISE:
                part0.AddTorque(-torque[0] * part0.mass * part0.transform.forward * MoveSpeed);
                break;
            default:
                break;
        }
        
        switch(Part1_mState)
        {
            case VerticalMovement.UPWARDS:
                part1.AddTorque(-torque[1] * part1.mass * part1.transform.forward * MoveSpeed);
                break;
            case VerticalMovement.DOWNWARDS:
                part1.AddTorque(torque[1] * part1.mass * part1.transform.forward * MoveSpeed);
                break;
            default:
                break;
        }

        switch(Part2_mState)
        {
            case VerticalMovement.UPWARDS:
                part2.AddTorque(torque[2] * part2.mass * part2.transform.forward * MoveSpeed);
                break;
            case VerticalMovement.DOWNWARDS:
                part2.AddTorque(-torque[2] * part2.mass * part2.transform.forward * MoveSpeed);
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

    private void ManualControls()
    {
        //moving part 0
        if (Input.GetKey("a"))
        {
            part0.AddTorque(-torque[0] * part0.mass * part0.transform.forward * MoveSpeed);
        }
        if (Input.GetKey("d"))
        {
            part0.AddTorque(torque[0] * part0.mass * part0.transform.forward * MoveSpeed);
        }

        //moving part 1
        if (Input.GetKey("w"))
        {
            part1.AddTorque(-torque[1] * part1.mass * part1.transform.forward * MoveSpeed);
        }
        if (Input.GetKey("s"))
        {
            part1.AddTorque(torque[1] * part1.mass * part1.transform.forward * MoveSpeed);
        }

        //moving part 2
        if (Input.GetKey("q"))
        {
            part2.AddTorque(-torque[2] * part2.mass * part2.transform.forward * MoveSpeed);

        }
        if (Input.GetKey("e"))
        {
            part2.AddTorque(torque[2] * part2.mass * part2.transform.forward * MoveSpeed);
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

    private void UpdateRotationValues()
    {
        //show reative rotation on the inspector
        t_arm = part0.transform.localRotation;
        q1_arm = part1.transform.localRotation;
        q2_arm = part2.transform.localRotation;
        q3_arm = part3.transform.localRotation;

    }

    private void PickAndDropSequence()
    {
        Debug.LogWarning(q3_arm.x);

        //Leans toward the cube
        if(t_arm.y <= 0.001f && CurrentSequenceState == 1)
        {
            Part0_mState = HoriontalMovement.NONE;
            //RotateToDir(part0.transform, targetBoxDir); // Adjust to perfect dir
            CurrentSequenceState += 1;
            Part1_mState = VerticalMovement.UPWARDS;
            Invoke("AdjustPerfectRotation", 0.1f);

            // var fixedRotation = part0.transform.localEulerAngles;
            // Debug.LogWarning(fixedRotation);
            // fixedRotation.z = 80;
            // part0.transform.localRotation = Quaternion.Euler(fixedRotation);
        }

        //Lowers the grip 
        if(q1_arm.x >= 0.6f && CurrentSequenceState == 2)
        {
            Part1_mState = VerticalMovement.NONE;
            CurrentSequenceState += 1;
            Part3_mState = VerticalMovement.DOWNWARDS;
        }

        //Grabs the cube
        if(q3_arm.x <= -0.601f && CurrentSequenceState == 3)
        {
            CurrentSequenceState += 1;
            Part3_mState = VerticalMovement.NONE;
            var fixedRotation = part3.transform.localEulerAngles;

            //Readjust to fix small errors
            fixedRotation.x = 63.30f;
            part3.transform.localRotation = Quaternion.Euler(fixedRotation);
            //Debug.LogWarning(fixedRotation);
            grip = true;
            Invoke("NextStep",1f);
        }

        //Raises hand back to neutral position
        if(CurrentSequenceState == 5)
        {
            CurrentSequenceState +=1;
            Part1_mState = VerticalMovement.DOWNWARDS;
        }

        //Rotates hand to drop off position
        if(Mathf.Abs(Mathf.Abs(q1_arm.x) - 0f) <= 0.02f && CurrentSequenceState == 6)
        {
            CurrentSequenceState += 1;
            Part1_mState = VerticalMovement.NONE;
            Part0_mState = HoriontalMovement.CLOCKWISE;
        }

        //Leans hand toward drop off position
        if(t_arm.y < -0.706 && CurrentSequenceState == 7)
        {
            CurrentSequenceState += 1;
            Part0_mState = HoriontalMovement.NONE;
            Part1_mState = VerticalMovement.UPWARDS;
            Invoke("AdjustPerfectRotation", 0.1f);

            // var fixedRotation = part0.transform.localRotation.eulerAngles;
            // Debug.LogWarning(fixedRotation);
            // fixedRotation.z = 264;
            // part0.transform.localRotation = Quaternion.Euler(fixedRotation);
        }

        //Lowers the grip
        if(Mathf.Abs(Mathf.Abs(q1_arm.x) - 0.56f) <= 0.001f && CurrentSequenceState == 8)
        {
            CurrentSequenceState += 1;
            Part1_mState = VerticalMovement.NONE;
            Part3_mState = VerticalMovement.DOWNWARDS;
        }

        //Drops off the cube
        if(q3_arm.x >= 0.59f && CurrentSequenceState == 9)
        {
            var fixedRotation = part3.transform.localEulerAngles;
            //Debug.LogWarning(fixedRotation);
            fixedRotation.x = 66.30f;
            part3.transform.localRotation = Quaternion.Euler(fixedRotation);

            CurrentSequenceState += 1;
            Part3_mState = VerticalMovement.NONE;
            grip = false;
            Invoke("NextStep", 1f);
        }

        //Raises hand back to neutral position
        if(CurrentSequenceState == 11)
        {
            CurrentSequenceState += 1;
            Part1_mState = VerticalMovement.DOWNWARDS;
        }

        //Rotates hand back to starting position
        if(Mathf.Abs(Mathf.Abs(q1_arm.x) - 0f) <= 0.002f && CurrentSequenceState == 12)
        {
            CurrentSequenceState += 1;
            Part1_mState = VerticalMovement.NONE;
            Part0_mState = HoriontalMovement.COUNTERCLOCKWISE;
        }

        //Stops - 1 cycle done
        if(t_arm.y >= -0.49f && CurrentSequenceState == 13)
        {
            CurrentSequenceState = 0;
            Part0_mState = HoriontalMovement.NONE;
            Manager.Instance.respawnRobot(this.gameObject);
        }

    }

    private void NextStep()
    {
        CurrentSequenceState += 1;

    }

    public void StartSequence()
    {
        if(CurrentSequenceState != 0){return;}
        //Starts rotating base to cube's direction
        CurrentSequenceState = 1;
        Part0_mState = HoriontalMovement.COUNTERCLOCKWISE;
    }
    #endregion

    #region Box Detection
    private void DetectBox()
    {
        RaycastHit hit;
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward)*4, Color.yellow);
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 5f))
        {
            if(hit.transform.gameObject.layer != 3){return;}
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);
            Debug.Log("Did Hit");
            StartSequence();
        }
    }
    #endregion

    #region Others
    private void AdjustPerfectRotation()
    {
        if(CurrentSequenceState == 2)
        {
            RotateToDir(part0.transform, targetBoxDir); // Adjust to perfect dir
        }else if(CurrentSequenceState == 8)
        {
            RotateToDir(part0.transform, targetBoxDir_drop); // Adjust to perfect dir
        }

    }
    private void RotateToDir(Transform start, Transform target, int dir = 1)
    {
        float x = start.rotation.eulerAngles.x;
        float z = start.rotation.eulerAngles.z;
        Quaternion tempRotation = Quaternion.LookRotation(target.position - start.position);
        start.rotation = Quaternion.Euler(x, dir *tempRotation.eulerAngles.y, z);

    }
    #endregion
    // IEnumerator FirstMove()
    // {
    //     //Rotates Arm to correct position
    //     Part0_mState = HoriontalMovement.CLOCKWISE;
    //     yield return null;
    // }



}


