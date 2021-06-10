using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed;
    public float xMovementLimit;
    public GameManager gameManagerScript;
    public GameObject pongBall;
    public BallController pongBallScript;
    public bool touchingWall = false;
    public float horizontalMovement;
    public float waitTimeBeforeMove;
    public bool allowToMove = true;

    // Start is called before the first frame update
    void Start()
    {
        gameManagerScript = GameObject.Find("Game Manager").GetComponent<GameManager>();
        // to have a referance of our ball position
        pongBall = GameObject.Find("Pong_Ball");
        pongBallScript = pongBall.GetComponent<BallController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManagerScript.isGameActive)
        {
            float ballPositionX = pongBall.transform.position.x;

            // if enemy paddle position X is not the same as the BALL POSITION X and the paddle is allowed to move
            if (transform.position.x < ballPositionX && allowToMove && transform.position.x < xMovementLimit)
            {
                transform.Translate(Vector3.right * speed * -horizontalMovement * Time.deltaTime);
            }

            // if enemy paddle position X is not the same as the BALL POSITION X and the paddle is allowed to move
            if (transform.position.x > ballPositionX && allowToMove && transform.position.x > -xMovementLimit)
            {
                transform.Translate(Vector3.left * speed * -horizontalMovement * Time.deltaTime);
            }

            if (touchingWall)
            {
                if (transform.position.x < ballPositionX && pongBallScript.directionX > 0)
                {
                    touchingWall = false;
                }

                if (transform.position.x > ballPositionX && pongBallScript.directionX < 0)
                {
                    touchingWall = false;
                }
            }

        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Sidewall"))
        {
            //Debug.Log("Enemy touching wall");
            touchingWall = true;
        }
    }

}
