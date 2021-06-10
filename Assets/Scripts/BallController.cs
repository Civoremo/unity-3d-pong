using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public GameManager gameManagerScript;
    public EnemyController enemyControllerScript;
    public PlayerController playerControllerScript;
    public PowerupSpawnManager powerupSpawnScript;
    private Rigidbody ballRb;
    public ParticleSystem trackParticles;
    public GameObject enemyGoal;
    public GameObject playerGoal;
    public float normalSpeed;
    public float currentSpeed;
    public float speedBoost;
    public float directionX = .5f;
    public float directionZ = 1f;
    public bool zDirectionChange = false;
    public Vector3 movementDirection;
    public bool ballRolling = false;
    public string[] paddleHitAreas;
    public float standardX;
    public float offsetXLimitMax;
    public float offsetXLimitMin;
    public float outsideHotX;
    public float outsideColdX;
    public bool paddleZonesEntered = false;
    public int powerupIndex;
    public float powerupSpeedBoost;
    public float powerupSpeedBoostDuration;
    public float powerupSlowPaddleDuration;

    public bool checkTrigger = true;

    // Start is called before the first frame update
    void Start()
    {
        ballRb = GetComponent<Rigidbody>();

        Vector2 randomStartXDirection = new Vector2(Random.Range(.4f, .7f), 0);
        movementDirection = new Vector3(randomStartXDirection.x, 0, directionZ);
        gameManagerScript = GameObject.Find("Game Manager").GetComponent<GameManager>();
        enemyControllerScript = GameObject.Find("Enemy").GetComponent<EnemyController>();
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        powerupSpawnScript = GameObject.Find("PowerupSpawns").GetComponent<PowerupSpawnManager>();
        trackParticles = GetComponentInChildren<ParticleSystem>();
        trackParticles.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManagerScript.isGameActive)
        {
            if (Input.GetKeyUp(KeyCode.Space) && !ballRolling)
            {
                ballRolling = !ballRolling;
                gameManagerScript.pressToStartText.SetActive(false);
            }

            // if ball has started moving
            if (ballRolling)
            {
                // move ball
                transform.Translate(movementDirection * (currentSpeed) * Time.deltaTime);

                // if ball is moving away from our paddle after a collision
                if (directionZ > 0 && paddleZonesEntered && transform.position.z > -4f)
                {
                    //paddleZonesEntered = false;
                }

                NormalizeBallSpeed();

                SpeedParticles();

            }

        }
    }

    // after the inital ball speed boost; reduce the boosted speed back to our normal speed
    private void NormalizeBallSpeed()
    {
        if (normalSpeed < currentSpeed)
        {
            currentSpeed -= Time.deltaTime;
        }
    }

    // playing particles if ball is going faster than its normal speed
    private void SpeedParticles()
    {
        if (currentSpeed > 4)
        {
            trackParticles.Play();
        }

        if (currentSpeed <= 4)
        {
            trackParticles.Stop();
        }
    }

    // Ball speed changes
    private void BallSpeed(int pickedSpeed)
    {
        switch (pickedSpeed)
        {
            case 0:
                // ball hit HOT ZONE on paddle and player added boost
                if (!playerControllerScript.boostingBallSpeed)
                {
                    currentSpeed += (speedBoost + playerControllerScript.ballSpeedBoost);
                    //Debug.Log("player boost " + playerControllerScript.ballSpeedBoost);
                    //Debug.Log("with player speed boost " + currentSpeed);
                }
                break;
            case 1:
                // ball did NOT hit HOT ZONE on paddle and player added boost

                if (!playerControllerScript.boostingBallSpeed)
                {
                    //Debug.Log("player boost " + playerControllerScript.ballSpeedBoost);
                    currentSpeed += playerControllerScript.ballSpeedBoost;
                    //Debug.Log("with player speed boost " + currentSpeed);
                    //Debug.Log("with player speed boost /w no speedboost " + currentSpeed);
                }
                break;
            default:
                break;
        }
    }

    // Ball and Paddle triggers
    private void TriggerBallToPaddle(Collider other)
    {
        //Debug.Log(other.gameObject.name);
        //Debug.Log(other.gameObject.name + " " + movementDirection.x + " " + checkTrigger);

        // [__ <-- o  for player
        if (other.gameObject.name == paddleHitAreas[0] && movementDirection.x < 0)
        {
            directionX = -outsideHotX - randomXpositionFactor();
            //Debug.Log("P - Hot " + directionX + " / 1.175 - 1.375");
            BallSpeed(0);
        }
        // o --> [__ for player
        else if (other.gameObject.name == paddleHitAreas[0] && movementDirection.x > 0)
        {
            directionX = outsideColdX + randomXpositionFactor();
            //Debug.Log("P - Cold " + directionX + " / .775 - .975");
            //Debug.Log("Hit left of paddle weak");
            BallSpeed(1);
        }
        // __] <-- o for player
        else if (other.gameObject.name == paddleHitAreas[1] && movementDirection.x < 0)
        {
            directionX = -outsideColdX - randomXpositionFactor();
            //Debug.Log("P - Cold " + directionX + " / .775 - .975");
            //Debug.Log("Hit right of paddle weak");
            BallSpeed(1);
        }
        // o --> __] for player
        else if (other.gameObject.name == paddleHitAreas[1] && movementDirection.x > 0)
        {
            directionX = outsideHotX + randomXpositionFactor();
            // Debug.Log("P - Hot " + directionX + " / 1.175 - 1.375");
            //Debug.Log("Hit right of paddle");
            BallSpeed(0);
        }
        // o --> __] for enemy
        else if (other.gameObject.name == paddleHitAreas[2] && movementDirection.x > 0)
        {
            // Debug.Log("strong left");
            directionX = outsideHotX + randomXpositionFactor();
            //Debug.Log("E - Hot " + directionX + " / 1.175 - 1.375");
            BallSpeed(0);
        }
        // __] <-- o for enemy
        else if (other.gameObject.name == paddleHitAreas[2] && movementDirection.x < 0)
        {
            //Debug.Log("weak left");
            directionX = -outsideColdX - randomXpositionFactor();
            // Debug.Log("E - Cold " + directionX + " / .775 - .975");
            BallSpeed(1);
        }
        // o --> [__ for enemy 
        else if (other.gameObject.name == paddleHitAreas[3] && movementDirection.x > 0)
        {
            //Debug.Log("weak right");
            directionX = outsideColdX + randomXpositionFactor();
            //Debug.Log("E - Cold " + directionX + " / .775 - .975");
            BallSpeed(1);
        }
        // [__ <-- for enemy
        else if (other.gameObject.name == paddleHitAreas[3] && movementDirection.x < 0)
        {
            //Debug.Log("strong right");
            directionX = -outsideHotX - randomXpositionFactor();
            //Debug.Log("E - Hot " + directionX + " / 1.175 - 1.375");
            BallSpeed(0);
        }
        // o --> __middle__
        else if (other.gameObject.name == "playerCenterCollider")
        {
            //Debug.Log("Hit center of paddle");
            if (directionX < 0)
            {
                directionX = -standardX;
            }
            if (directionX > 0)
            {
                directionX = standardX;
            }

            if (!playerControllerScript.boostingBallSpeed)
            {
                currentSpeed = (normalSpeed + playerControllerScript.ballSpeedBoost);
            }
            else
            {
                currentSpeed = normalSpeed;
            }
            //Debug.Log("Standard " + directionX + " / .85");

            // freeze enemy for some time after ball hits the paddle
            //enemyControllerScript.allowToMove = false;
            //StartCoroutine(FreezeEnemyAfterPaddleCollision());

        }
        else
        {
            //Debug.Log("Error with paddle trigger " + other.gameObject);
            if (directionX < 0)
            {
                directionX = -standardX;
            }
            if (directionX > 0)
            {
                directionX = standardX;
            }
            currentSpeed = normalSpeed;
            //gameManagerScript.isGameActive = false;
        }


        directionZ = -(movementDirection.z);
        movementDirection = new Vector3(directionX, 0, directionZ);

        //paddleZonesEntered = true;
        //Debug.Log(directionZ);

        // freeze enemy for some time after ball hits the paddle
        //enemyControllerScript.allowToMove = false;
        StartCoroutine(FreezeEnemyAfterPaddleCollision());
    }


    // Ball and Powerup triggers
    private void TriggerBallToPowerup(Collider other)
    {
        // find the index of the powerup collected
        for (int i = 0; i < powerupSpawnScript.powerupPrefabs.Length; i++)
        {
            if (other.gameObject.name == (powerupSpawnScript.powerupPrefabs[i].name + "(Clone)"))
            {
                powerupIndex = i;
                switch (powerupIndex)
                {
                    // ball speed boost
                    case 0:
                        StartCoroutine(PowerupSpeedBoostRoutine(other.gameObject.GetComponent<Powerup_BallSpeedUp>().ballBoostSpeed));
                        break;
                    // change ball x direction
                    case 1:
                        directionX *= other.gameObject.GetComponent<Powerup_ChangeBallXdirection>().posXChange;
                        movementDirection = new Vector3(directionX, 0, directionZ);
                        break;
                    // slow down opposing players paddle speed
                    case 2:
                        //Debug.Log("ES powerup");
                        StartCoroutine(SlowDownPaddlePowerupRoutine(other.gameObject.GetComponent<Powerup_SlowDownEnemyPaddle>().slowDownSpeed));
                        break;
                    case 3:
                        //Debug.Log("fill energy");
                        StartCoroutine(RefillEnergyMeterRoutine(other.gameObject.GetComponent<Powerup_FillEnergyMeter>().energyFillUp));
                        break;
                    default:
                        break;
                }
            }
        }

        for (int i = 0; i < powerupSpawnScript.occupiedSpawnPositions.Count; i++)
        {
            if (other.gameObject.transform.position == powerupSpawnScript.occupiedSpawnPositions[i])
            {
                powerupSpawnScript.occupiedSpawnPositions.RemoveAt(i);
            }
        }

        // destroy the collected powerup
        Destroy(other.gameObject);
        // start a coroutine to spawn a new powerup
        StartCoroutine(powerupSpawnScript.SpawnNewRandomPowerupRoutine());
    }

    private void GoalScored(Collider other)
    {
        trackParticles.Stop();
        if (other.gameObject.name == enemyGoal.name)
        {
            // player scored
            gameManagerScript.UpdateScore(0);
            gameManagerScript.isGameActive = false;
            ballRb.velocity = new Vector3(0, 0, 0);
        }

        if (other.gameObject.name == playerGoal.name)
        {
            // enemy scored
            gameManagerScript.UpdateScore(1);
            gameManagerScript.isGameActive = false;
            ballRb.velocity = new Vector3(0, 0, 0);
        }
        currentSpeed = 0;
    }

    // Ball Collisions

    private void OnCollisionEnter(Collision collision)
    {
        // if the ball collides with our side walls, change the X direction of our ball
        if (collision.gameObject.CompareTag("Sidewall"))
        {
            directionX = -directionX;
            movementDirection = new Vector3(directionX, 0, directionZ);
        }

        /*
        // if the ball collides with the player or enemy paddle
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy"))
        {
            // incoming X direction is negative and NOT collided with defined HOT hit ZONE
            if (directionX < 0 && !paddleZonesEntered)
            {
                directionX = -standardX;
            }

            // incoming X direction is positive and NOT collided with defined HOT hit ZONE
            if (directionX > 0 && !paddleZonesEntered)
            {
                directionX = standardX;
            }

            // after collision, invert our Z direction
            //directionZ = -directionZ;
            // change our ball speed
            //currentSpeed += playerControllerScript.ballSpeedBoost;
            BallSpeed(1);
            // set our new ball direction
            movementDirection = new Vector3(directionX, 0, directionZ);

            // freeze enemy for some time after ball hits the paddle
            enemyControllerScript.allowToMove = false;
            StartCoroutine(FreezeEnemyAfterPaddleCollision());

        }
        */
    }

    IEnumerator PauseTrigger()
    {
        checkTrigger = false;
        yield return new WaitForSeconds(1);
        checkTrigger = true;
    }

    // Ball triggers
    private void OnTriggerEnter(Collider other)
    {
        //if (!paddleZonesEntered && !other.gameObject.CompareTag("Powerup"))
        if (other.gameObject.CompareTag("PaddleHitArea") && checkTrigger)
        {
            TriggerBallToPaddle(other);
            StartCoroutine(PauseTrigger());
        }

        else if (other.gameObject.CompareTag("Powerup"))
        {
            TriggerBallToPowerup(other);
        }

        else if (other.gameObject.CompareTag("Goalwall"))
        {
            GoalScored(other);
        }

        else
        {
            // checkTrigger preventing paddle trigger
            //Debug.Log("Error with trigger " + checkTrigger);
            //Debug.Log(other.gameObject.name);
        }


    }

    // Energy Fillup from Powerup
    IEnumerator RefillEnergyMeterRoutine(float energyFillUp)
    {
        if (directionZ > 0)
        {
            //Debug.Log("fill players energy");
            playerControllerScript.energy = energyFillUp;
            yield return new WaitForSeconds(1);
        }
        else if (directionZ < 0)
        {
            //Debug.Log("fill enemy energy");

            // once we allow the computer to have and be able to use ENERGY
            yield return new WaitForSeconds(1);
        }
        yield return new WaitForSeconds(1);
    }

    // Paddle Slowdown from Powerup
    IEnumerator SlowDownPaddlePowerupRoutine(float paddleSpeed)
    {
        if (directionZ > 0)
        {
            //Debug.Log("slow enemy");
            float tempOriginalPaddleSpeed = enemyControllerScript.speed;
            enemyControllerScript.speed -= paddleSpeed;
            //Debug.Log(enemyControllerScript.speed);
            yield return new WaitForSeconds(powerupSlowPaddleDuration);
            enemyControllerScript.speed = tempOriginalPaddleSpeed;
        }

        else if (directionZ < 0)
        {
            //Debug.Log("slow player");
            float tempOriginalPaddleSpeed = playerControllerScript.speed;
            playerControllerScript.speed -= paddleSpeed;
            //Debug.Log(playerControllerScript.speed);
            yield return new WaitForSeconds(powerupSlowPaddleDuration);
            playerControllerScript.speed = tempOriginalPaddleSpeed;
        }
    }

    // SubRoutine for collecting ball boost speed powerup
    IEnumerator PowerupSpeedBoostRoutine(float ballBoostSpeed)
    {
        powerupSpeedBoost = ballBoostSpeed;
        currentSpeed += powerupSpeedBoost;
        yield return new WaitForSeconds(powerupSpeedBoostDuration);
        powerupSpeedBoost = 0f;
    }

    // create a bit of random range to our BALL X Value
    private float randomXpositionFactor()
    {
        float randomOffset = Random.Range(offsetXLimitMin, offsetXLimitMax);
        int randomDirection = Random.Range(0, 1);

        if (randomDirection == 0)
        {
            return randomOffset;
        }

        return randomOffset;
    }

    // SubRoutine to stall the enemy paddle for defined time after we hit the ball
    IEnumerator FreezeEnemyAfterPaddleCollision()
    {
        enemyControllerScript.allowToMove = false;
        yield return new WaitForSeconds(enemyControllerScript.waitTimeBeforeMove);
        enemyControllerScript.allowToMove = true;
    }

}
