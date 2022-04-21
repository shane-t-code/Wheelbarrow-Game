using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//using EZCameraShake;

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
    public TextMeshProUGUI finalPointsText;

    //Speed Line vars
    [SerializeField] ParticleSystem speedLines;

    //StopOnCollision vars
    public static bool gameEnd;
    [SerializeField] private float gameOverTime = 100f;

    //Particles
    [SerializeField] private ParticleSystem boostParticles;
    [SerializeField] private ParticleSystem pointParticles;
    [SerializeField] private ParticleSystem obstacleCollisionParticles;

    //UI Text
    [SerializeField] private TextMeshProUGUI totalPointsText;
    [SerializeField] private GameObject gameOverPopup;
    [SerializeField] private GameObject gameWinPopup;

    //Clamp vars
    //[SerializeField] float offset = 180f;
    //[SerializeField] float yClampValue = 90f;

    //---------------------------------------------------------------------------------------------------------------------------------------------------------

    void Start()
    {
        //Seperate movement sphere from wheelbarrow
        RB.transform.parent = null;

        //Clamp turning

        //Set vars in before game starts
        gameEnd = false;
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------


    void Update()
    {
        //Movement Things
        if (gameEnd == false)
        {
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
            if (forwardAccel > 250)
            {
                speedLines.Play();
            }
            else
            if (forwardAccel <= 250)
            {
                speedLines.Stop();
            }
        }
        
        //float yValue = transform.eulerAngles.y;
        //float xValue = transform.eulerAngles.x;
        //float zValue = transform.eulerAngles.z;

        //yValue = Mathf.Clamp(yValue, 0, 180);

        ////yValue -= offset;


        //transform.eulerAngles = new Vector3(xValue, yValue, zValue);
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------

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

    //---------------------------------------------------------------------------------------------------------------------------------------------------------

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
        //Collect Boost Item and Point Item + Game Over Collision
        if (collision.CompareTag("boostItem"))
        {
            Instantiate(boostParticles, transform.position, transform.rotation);
            Destroy(collision.gameObject);
            forwardAccel += boostAmount;
            StartCoroutine(boostTime());
        }
        else
            if (collision.CompareTag("pointItem"))
        {
            Instantiate(pointParticles, transform.position, transform.rotation);
            pointTotal += 10f;
            Destroy(collision.gameObject);
            Debug.Log(pointTotal);
        }
        else
            if (collision.CompareTag("Obstacle"))
        {
            Instantiate(obstacleCollisionParticles, transform.position, transform.rotation);
            startGameOver();
        }
        else
            if (collision.CompareTag("winBoundary"))
        {
            startGameWin();
        }
    }

    IEnumerator boostTime()
    {
        //Boost wear off time
        yield return new WaitForSeconds(boostWearOff);
        forwardAccel -= boostAmount;
    }

    IEnumerator timeBeforeGameOver()
    {
        //Time before going to the Game Over scene
        yield return new WaitForSeconds(gameOverTime);
    }

    void startGameOver()
    {
        //Things to do for game over to start
        //CameraShaker.Instance.ShakeOnce(10f, 10f, 10f, 10f);
        gameEnd = true;
        //StartCoroutine(timeBeforeGameOver());
        gameOverPopup.SetActive(true);
    }

    void startGameWin()
    {
        //Things to do for game win to start
        gameEnd = true;
        gameWinPopup.SetActive(true);
        finalPointsText.text = "Final Points: " + pointTotal.ToString();
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------

}
