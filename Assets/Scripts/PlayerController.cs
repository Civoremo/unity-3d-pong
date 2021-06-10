using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public GameManager gameManagerScript;
    public BallController ballControllerScript;
    public Image energyMeterImage;
    public GameObject energyMeterPosition;
    public Image boostMeterImage;
    public GameObject boostMeterPosition;
    public float speed;
    public int energyLimit;
    public float energy;
    public bool touchingWall = false;
    public float ballSpeedBoost;
    public float ballBoostMax;
    public float ballBoostMin;
    private bool ballBoostingLimit = false;
    public bool boostingBallSpeed = false;
    public float sideMovementInput;

    // Start is called before the first frame update
    void Start()
    {
        gameManagerScript = GameObject.Find("Game Manager").GetComponent<GameManager>();
        energy = 0;
        ballControllerScript = GameObject.Find("Pong_Ball").GetComponent<BallController>();
        EnergyMeterDisplay();
        BoostMeterDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManagerScript.isGameActive)
        {
            // our horizontal input value
            sideMovementInput = Input.GetAxis("Horizontal");

            // if we are not at our move limits (i.e. didnt collide with walls)
            if (!touchingWall && ballControllerScript.ballRolling && sideMovementInput != 0)
            {
                transform.Translate(Vector3.right * sideMovementInput * Time.deltaTime * speed);
            }

            // if we are hitting a wall while moving our player
            if (touchingWall)
            {
                if (sideMovementInput < 0 && transform.position.x > 0)
                {
                    transform.Translate(Vector3.right * sideMovementInput * Time.deltaTime * speed);
                    touchingWall = false;
                }

                if (sideMovementInput > 0 && transform.position.x < 0)
                {
                    transform.Translate(Vector3.right * sideMovementInput * Time.deltaTime * speed);
                    touchingWall = false;
                }
            }

            BallBoost();
            EnergyReplenisher();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Sidewall"))
        {
            touchingWall = true;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Sidewall"))
        {
            //touchingWall = true;
        }
    }

    // recharges the energy meter over time
    private void EnergyReplenisher()
    {
        if (energy <= energyLimit && ballControllerScript.ballRolling)
        {
            //energy += (Time.deltaTime / 3);
            energy += Time.deltaTime;
            EnergyMeterDisplay();
        }

        if (energy > energyLimit)
        {
            energy = (float)energyLimit;
        }
    }

    private void BoostMeterDisplay()
    {
        boostMeterImage.rectTransform.sizeDelta = new Vector2(ballSpeedBoost * 100, boostMeterImage.rectTransform.sizeDelta.y);
        boostMeterImage.rectTransform.transform.position = new Vector2(boostMeterPosition.transform.position.x + boostMeterImage.rectTransform.rect.width / 2, boostMeterPosition.transform.position.y);
    }

    private void EnergyMeterDisplay()
    {
        // resize the energy meter image to the amount of energy the player has
        energyMeterImage.rectTransform.sizeDelta = new Vector2(energy * 10, energyMeterImage.rectTransform.sizeDelta.y);
        // positioning the meter
        energyMeterImage.rectTransform.transform.position = new Vector2(energyMeterPosition.transform.position.x + energyMeterImage.rectTransform.rect.width / 2, energyMeterPosition.transform.position.y);
    }

    // Allowes the player to attempt a power shot on the return ball
    // boost meter will climb up and down the indicated limits until button is released
    public void BallBoost()
    {
        if (Input.GetKey(KeyCode.Space) && ballControllerScript.ballRolling)
        {
            boostingBallSpeed = true;
            // current ball boost speed is <= our boost MAX limit and we have not reached our limit
            if (ballSpeedBoost <= ballBoostMax && !ballBoostingLimit)
            {
                ballSpeedBoost += (Time.deltaTime * 5);

                // if we reach our MAX limit
                if (ballSpeedBoost >= ballBoostMax)
                {
                    ballBoostingLimit = true;
                }
            }

            // if our MAX limit BOOL is true; start reducing boost
            if (ballBoostingLimit)
            {
                ballSpeedBoost -= (Time.deltaTime * 5);

                // if ball speed MIN is reached; change our MAX limit BOOL to false
                if (ballSpeedBoost <= ballBoostMin)
                {
                    ballBoostingLimit = false;
                }
            }

            BoostMeterDisplay();


        }

        // if we release our boost button
        if (Input.GetKeyUp(KeyCode.Space))
        {
            boostingBallSpeed = false;
            if (ballControllerScript.ballRolling)
            {
                StartCoroutine(HoldBallBoostSpeed());
                ballBoostingLimit = false;
            }
        }
    }

    // keeps track of the set boost for a set amount of time after key is released before reseting to 0 (zero)
    IEnumerator HoldBallBoostSpeed()
    {
        int tempBallSpeedBoost = (int)(ballSpeedBoost * 10);

        // if our boost if larger than the current amount of energy
        // change our boost amount to the total amount of energy we have remaining
        if (tempBallSpeedBoost > energy)
        {
            energy -= energy;
            ballSpeedBoost = energy / 10;
        }
        else
        {
            energy -= tempBallSpeedBoost;
        }

        // wait an amount of time before resetting our boost
        yield return new WaitForSeconds(.7f);
        //Debug.Log("emptied boost");
        ballSpeedBoost = 0f;
        BoostMeterDisplay();
    }



}
