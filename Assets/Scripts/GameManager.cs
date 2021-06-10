using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool isGameActive = false;
    public int winScore;
    [SerializeField] private int gameDifficulty;
    public GameObject mainMenuScreen;
    public GameObject gameResultsScreen;
    public GameObject winMessage;
    public GameObject lossMessage;
    public GameObject inGameScreen;
    public TextMeshProUGUI playerScore;
    public TextMeshProUGUI enemyScore;
    public GameObject pressToStartText;
    public GameObject pausedScreen;
    public GameObject goalScreen;
    public Button easyButton;
    public Button normalButton;
    public Button hardButton;
    public Button exitButton;
    public Button restartButton;
    public Button mainMenuButton;

    public GameObject[] gameCameras;

    public Rigidbody pongBallRb;
    public BallController ballControllerScript;
    public EnemyController enemyControllerScript;
    public PlayerController playerControllerScript;
    public PowerupSpawnManager powerupSpawnScript;

    private TextMeshProUGUI[] inGameScreenTexts;

    private Vector3 ballResetPosition;
    private Vector3 playerPaddleResetPosition;
    private Vector3 enemyPaddleResetPosition;
    private float playerSpeedDefault;
    private float enemySpeedDefault;

    // Start is called before the first frame update
    void Start()
    {
        mainMenuScreen.SetActive(true);
        gameResultsScreen.SetActive(false);
        inGameScreen.SetActive(false);
        pausedScreen.SetActive(false);
        goalScreen.SetActive(false);
        pressToStartText.SetActive(false);

        gameCameras[0].SetActive(true);

        ballResetPosition = ballControllerScript.transform.position;
        playerPaddleResetPosition = playerControllerScript.transform.position;
        enemyPaddleResetPosition = enemyControllerScript.transform.position;

        pongBallRb = GameObject.Find("Pong_Ball").GetComponent<Rigidbody>();

        ballControllerScript = GameObject.Find("Pong_Ball").GetComponent<BallController>();
        enemyControllerScript = GameObject.Find("Enemy").GetComponent<EnemyController>();
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        powerupSpawnScript = GameObject.Find("PowerupSpawns").GetComponent<PowerupSpawnManager>();

        inGameScreenTexts = inGameScreen.GetComponentsInChildren<TextMeshProUGUI>();

        playerSpeedDefault = playerControllerScript.speed;
        enemySpeedDefault = enemyControllerScript.speed;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Debug.Log("Trying to Pause");
            PausedGame();
        }
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitEntireApplication()
    {
        //Debug.Log("EXITING");
        Application.Quit();
    }

    public void PausedGame()
    {
        if (!gameResultsScreen.activeSelf && !mainMenuScreen.activeSelf && !pausedScreen.activeSelf)
        {
            isGameActive = false;
            inGameScreen.SetActive(false);
            gameResultsScreen.SetActive(false);
            mainMenuScreen.SetActive(false);
            pausedScreen.SetActive(true);
        }
        else if (!isGameActive && pausedScreen.activeSelf)
        {
            pausedScreen.SetActive(false);
            inGameScreen.SetActive(true);
            isGameActive = true;
        }

    }

    public void GameResults()
    {
        isGameActive = false;
        inGameScreen.SetActive(false);
        gameResultsScreen.SetActive(true);
        mainMenuScreen.SetActive(false);
        pausedScreen.SetActive(false);

    }

    public void StartNewGame()
    {
        isGameActive = true;
        mainMenuScreen.SetActive(false);
        gameResultsScreen.SetActive(false);
        pausedScreen.SetActive(false);
        pressToStartText.SetActive(true);
        inGameScreen.SetActive(true);

        gameCameras[1].SetActive(true);
        gameCameras[0].SetActive(false);

        enemyControllerScript.speed += ((float)gameDifficulty / 2);
    }

    public void RestartGame()
    {
        playerScore.SetText("0");
        enemyScore.SetText("0");

        //Debug.Log("directionZ " + ballControllerScript.directionZ);
        if (ballControllerScript.directionZ < 0)
        {
            ballControllerScript.directionZ *= -1;
            ballControllerScript.directionX = ballControllerScript.standardX;
            ballControllerScript.movementDirection = new Vector3(ballControllerScript.directionX, 0, ballControllerScript.directionZ);
        }

        ballControllerScript.transform.position = ballResetPosition;
        playerControllerScript.transform.position = playerPaddleResetPosition;
        playerControllerScript.touchingWall = false;
        enemyControllerScript.transform.position = enemyPaddleResetPosition;

        ballControllerScript.transform.position = new Vector3(ballResetPosition.x, ballResetPosition.y, ballResetPosition.z);
        ballControllerScript.currentSpeed = ballControllerScript.normalSpeed;

        playerControllerScript.speed = playerSpeedDefault;
        enemyControllerScript.speed = enemySpeedDefault;

        isGameActive = true;
        mainMenuScreen.SetActive(false);
        gameResultsScreen.SetActive(false);
        pausedScreen.SetActive(false);
        pressToStartText.SetActive(true);
        inGameScreen.SetActive(true);

    }

    public void SetDifficulty(int difficulty)
    {
        gameDifficulty = difficulty;
        StartNewGame();
    }

    public void UpdateScore(int playerNumber)
    {
        if (!isGameActive)
        {
            //Debug.Log(pongBallRb.velocity);
            if (pongBallRb.velocity.z > 0)
            {
                Debug.Log("Ball is moving " + pongBallRb.velocity.z);
                pongBallRb.velocity = new Vector3(0, 0, 0);
            }

            if (pongBallRb.velocity.z < 0)
            {
                Debug.Log("Ball is moving " + pongBallRb.velocity.z);
                pongBallRb.velocity = new Vector3(0, 0, 0);
            }
        }

        int tempScore;

        switch (playerNumber)
        {
            case 0:
                tempScore = int.Parse(playerScore.text);
                tempScore++;
                playerScore.SetText("" + tempScore);
                StartCoroutine(DisplayGoalText(playerNumber, tempScore));
                break;
            case 1:
                tempScore = int.Parse(enemyScore.text);
                tempScore++;
                enemyScore.SetText("" + tempScore);
                StartCoroutine(DisplayGoalText(playerNumber, tempScore));
                break;
            default:
                Debug.Log("Score did not update");
                break;
        }
    }

    private void CheckForGameOver(int playerScore, int playerNumber)
    {
        if (playerScore >= winScore)
        {
            SetGameResultsScreen(playerNumber);
            GameResults();
        }
    }

    private void ResetAfterScore(int playerNumber)
    {
        // playerNumber == 0 --> Player 1 (player scored)
        // playerNumber == 1 --> Player 2 (enemy scored)
        ballControllerScript.transform.position = ballResetPosition;
        playerControllerScript.transform.position = playerPaddleResetPosition;
        playerControllerScript.touchingWall = false;
        enemyControllerScript.transform.position = enemyPaddleResetPosition;

        ballControllerScript.directionZ = -ballControllerScript.directionZ;
        ballControllerScript.directionX = ballControllerScript.standardX;
        ballControllerScript.movementDirection = new Vector3(ballControllerScript.directionX, 0, ballControllerScript.directionZ);

        ballControllerScript.currentSpeed = ballControllerScript.normalSpeed;

        if (playerNumber == 0)
        {
            ballControllerScript.transform.position = new Vector3(ballResetPosition.x, ballResetPosition.y, -ballResetPosition.z);
            StartCoroutine(EnemyStartsGameAfterScore());
        }

        if (playerNumber == 1)
        {
            pressToStartText.SetActive(true);
            if (Input.GetKeyUp(KeyCode.Space))
            {
                ballControllerScript.ballRolling = true;
            }
        }
    }

    private void SetGameResultsScreen(int playerNumber)
    {
        TextMeshProUGUI[] gameResultTexts = gameResultsScreen.GetComponentsInChildren<TextMeshProUGUI>();

        // set the results screen with players score
        foreach (TextMeshProUGUI textName in gameResultTexts)
        {
            if (textName.name == "Player One Score Text")
            {
                textName.SetText(playerScore.text);
            }

            if (textName.name == "Player Two Score Text")
            {
                textName.SetText(enemyScore.text);
            }
        }

        // player WON
        if (playerNumber == 0)
        {
            lossMessage.SetActive(false);
            winMessage.SetActive(true);
        }

        // player LOST
        if (playerNumber == 1)
        {
            winMessage.SetActive(false);
            lossMessage.SetActive(true);
        }
    }

    IEnumerator EnemyStartsGameAfterScore()
    {
        yield return new WaitForSeconds(2);
        ballControllerScript.ballRolling = true;
    }

    IEnumerator DisplayGoalText(int playerNumber, int playerScore)
    {
        isGameActive = false;
        ballControllerScript.ballRolling = false;
        goalScreen.SetActive(true);
        yield return new WaitForSeconds(3);
        goalScreen.SetActive(false);
        CheckForGameOver(playerScore, playerNumber);

        if (!gameResultsScreen.activeSelf)
        {
            isGameActive = true;
            ResetAfterScore(playerNumber);
        }
    }
}
