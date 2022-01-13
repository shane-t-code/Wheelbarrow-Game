using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WB_Controller : MonoBehaviour
{
    //Movement vars
    public Rigidbody RB;

    [SerializeField] public float forwardAccel = 8f;
    [SerializeField] private float reverseAccel = 4f;
    //[SerializeField] private float maxSpeed = 50f;
    [SerializeField] private float turnStrength = 180f;
    [SerializeField] private float gravityForce = 10f;
    [SerializeField] private float dragOnGround = 3f;


    private float speedInput;
    private float turnInput;

    private bool grounded;

    public LayerMask WhatIsGround;
    public float groundRayLength = .5f;
    public Transform groundRayPoint;

    public Transform frontWheel;
    public float maxWheelTurn = 25f;

    //Boost vars
    [SerializeField] private float boostWearOff = 10f;
    [SerializeField] private float boostAmount = 1000;

    //Point vars
    public float pointTotal = 0f;
    [SerializeField] private TextMeshProUGUI totalPointsText;

    //Speed Line vars
    [SerializeField] ParticleSystem speedLines;

    void Start()
    {
        RB.transform.parent = null;
    }

    void Update()
    {
        //Movement Things
        speedInput = 0f;
        if (Input.GetAxis("Vertical") > 0)
        {
            speedInput = Input.GetAxis("Vertical") * forwardAccel * -1;
        }
        //else if (Input.GetAxis("Vertical") < 0)
        //{
        //    speedInput = Input.GetAxis("Vertical") * reverseAccel * -1;
        //}

        turnInput = Input.GetAxis("Horizontal");

        if (grounded)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrength * Time.deltaTime * Input.GetAxis("Vertical"), 0f));
            //tiltWhileTurning();
        }



        frontWheel.localRotation = Quaternion.Euler(frontWheel.localRotation.eulerAngles.x, turnInput * maxWheelTurn, frontWheel.localRotation.eulerAngles.z);

        transform.position = RB.transform.position;

        //Points Things
        totalPointsText.text = pointTotal.ToString();

        //Speed Line things
        if(forwardAccel > 250)
        {
            speedLines.Play();
        }
        else
        if(forwardAccel <= 250)
        {
            speedLines.Stop();
        }
    }

    private void FixedUpdate()
    {
        //Check if is on ground (Movement Things)
        grounded = false;
        RaycastHit hit;

        if (Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLength, WhatIsGround))
        {
            grounded = true;

            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        }

        if (grounded)
        {
            RB.drag = dragOnGround;

            if (Mathf.Abs(speedInput) > 0)
            {
                RB.AddForce(transform.forward * speedInput);
            }
            else
            {
                RB.drag = 0.1f;

                RB.AddForce(Vector3.up * -gravityForce * 100);
            }
        }

    }

    private void tiltWhileTurning()
    {
            //Tilt while turning (not working yet)
            float z = Input.GetAxis("Horizontal") * 15;
            Vector3 euler = transform.localEulerAngles;
            euler.z = Mathf.Lerp(euler.z, z, 2.0f * Time.deltaTime);
            transform.localEulerAngles = euler;
    }

    private void OnTriggerEnter(Collider collision)
    {
        //Collect Boost Item and Point Item
        if (collision.CompareTag("boostItem"))
        {
            Destroy(collision.gameObject);
            forwardAccel += boostAmount;
            StartCoroutine(boostTime());
        }
        else
        if (collision.CompareTag("pointItem"))
        {
            pointTotal += 10f;
            Destroy(collision.gameObject);
            Debug.Log(pointTotal);
        }
    }

    IEnumerator boostTime()
    {
        //Boost wear off time
        yield return new WaitForSeconds(boostWearOff);
        forwardAccel -= boostAmount;
    }

}
