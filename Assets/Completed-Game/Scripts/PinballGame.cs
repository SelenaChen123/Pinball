using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PinballGame : MonoBehaviour
{
    public Text currentLevelText;
    public Text ballsText;
    public Text remainingTimeText;
    public Text currentEquationText;
    public Text currentNumberText;
    public Text scoreText;
    public GameObject nextLevelText;
    public GameObject winText;
    public GameObject gameOverText;
    public KeyCode newGameKey;
    public KeyCode plungerKey;
    public KeyCode puzzlecameraKey;
    public AudioSource audioPlayer;
    public AudioClip plungerClip;
    public AudioClip gameOverClip;
    public AudioClip gameWonClip;
    public AudioClip nextLevelClip;

    private float offsetTime = 3f;
    private float plungerSpeed = 0.1f;
    private float timeForEachLevel = 300f;
    private int originalBallsLeft = 5;
    private int level = 1;
    private int ballsLeft;
    private float remainingTime;
    private string currentEquation = "";
    private int currentNumber = 0;
    private int solutionNumber;
    private int totalScore = 0;
    private bool moveToNextLevel = false;
    private float moveToNextLevelTimer = 0;
    private bool gameOver = false;
    private float gameOverTimer = 0;
    private bool gameWon = false;
    private float gameWonTimer = 0;
    private GameObject ball;
    private GameObject plunger;
    private GameObject drain;
    private GameObject maincam;
    private GameObject puzzleCamera;

    string[] equations = new string[] {
        "10 - 9 + 24 - 15 - 6 = ?",
        "( 14 + 6 ) - ( 2 + 7 ) - 5 = ?",
        "20 / 10 x 6 / 3 x 2 = ?",
        "64 / 32 x ( 2 x 2 ) = ?",
        "4 x 4 - 16 / 2 + 1 = ?",
        "6 x 7 - ( 17 + 11 ) - 28 / 4 = ?",
        "52 - 7^2 + 1 = ?",
        "9 x 18 / 3^3 = ?",
        "8 x 6 + 32 / 4^2 - 45 = ?",
        "12 + 2^4 - ( 9 + 1 ) x 75 / 5^2 = ?"
    };
    int[] solutions = new int[] { 4, 6, 8, 8, 9, 7, 4, 6, 5, 10 };

    // At the start of the game...
    void Start()
    {
        level = 1;
        ballsLeft = originalBallsLeft;
        remainingTime = timeForEachLevel;
        currentEquation = equations[level - 1];
        solutionNumber = solutions[level - 1];

        plunger = GameObject.Find("Plunger");
        drain = GameObject.Find("Drain");
        ball = GameObject.Find("Ball");
        maincam = GameObject.FindGameObjectWithTag("MainCamera");
        puzzleCamera = GameObject.Find("PuzzleCamera");
        puzzleCamera.SetActive(false);
        ball.SetActive(false);
        nextLevelText.SetActive(false);
        winText.SetActive(false);
        gameOverText.SetActive(false);

        audioPlayer = GetComponent<AudioSource>();
        audioPlayer.loop = true;
        audioPlayer.volume = 0.15f;
        audioPlayer.Play();
    }

    // Before rendering each frame...
    private void Update()
    {
        if (Input.GetKey(newGameKey) == true) NewGame();
        if (Input.GetKey(plungerKey) == true) Plunger();
        if (Input.GetKey(puzzlecameraKey) == true) switchCamera();

        // detect ball going past flippers into "drain"
        if (!gameOver && (ball.activeSelf == true) && (ball.transform.position.z < drain.transform.position.z))
        {
            ball.SetActive(false);
        }

        // detect whether the game is over, the game has been won, or the player should go to the next level
        if (!moveToNextLevel && !gameWon && ball.activeSelf == false && ballsLeft <= 0)
        {
            if (gameOver == false)
            {
                gameOver = true;
                audioPlayer.PlayOneShot(gameOverClip, 1f);
            }
        }
        else if(!moveToNextLevel && !gameOver && level == solutions.Length && currentNumber == solutionNumber)
        {
            if (gameWon == false)
            {
                gameWon = true;
                audioPlayer.PlayOneShot(gameWonClip, 1f);
            }
        }
        else if (!gameOver && !gameWon && currentNumber == solutionNumber)
        {
            if (moveToNextLevel == false)
            {
                moveToNextLevel = true;
                audioPlayer.PlayOneShot(nextLevelClip, 1f);
            }
        }

        // set the text on the canvas
        SetText();

        // move on to the next level, win the game, or end the game
        if (moveToNextLevel)
        {
            nextLevel();
        }
        else if(gameWon)
        {
            winGame();
        }
        else if (gameOver)
        {
            endGame();
        }
    }

    void NewGame()
    {
        winText.SetActive(false);
        nextLevelText.SetActive(false);
        gameOverText.SetActive(false);

        ball.SetActive(false);
        level = 1;
        ballsLeft = originalBallsLeft;
        remainingTime = timeForEachLevel;
        totalScore = 0;
        currentNumber = 0;
        currentEquation = equations[level - 1];
        solutionNumber = solutions[level - 1];
        moveToNextLevel = false;
        moveToNextLevelTimer = 0;
        gameWon = false;
        gameWonTimer = 0;
        gameOver = false;
        gameOverTimer = 0;

        // ensure each bumper is enabled
        GameObject[] bumpers;
        bumpers = GameObject.FindGameObjectsWithTag("Bumper");
        foreach (GameObject bumper in bumpers)
        {
            bumper.GetComponent<MeshRenderer>().enabled = true;
            bumper.GetComponent<BoxCollider>().enabled = true;
        }
    }

    void Plunger()
    {
        // only plunge ball if there are balls left
        if ((ballsLeft > 0) && (ball.activeSelf == false))
        {
            ball.SetActive(true);

            Rigidbody rb = ball.GetComponent<Rigidbody>();
            Vector3 movement = new Vector3(0.0f, 0.0f, 1.0f);
            rb.AddForce(movement * plungerSpeed);

            // set ball position to location of plunger
            ball.transform.position = plunger.transform.position;
            ballsLeft = ballsLeft - 1;

            audioPlayer.PlayOneShot(plungerClip);
        }
    }

    void switchCamera()
    {
        // use main camera defaultly, otherwise use puzzle camera
        if (maincam.activeSelf == true)
        {
            maincam.SetActive(false);
            puzzleCamera.SetActive(true);
        }
        else
        {
            puzzleCamera.SetActive(false);
            maincam.SetActive(true);
        }
    }

    void SetText()
    {
        // update the game text
        ballsText.text = ballsLeft.ToString();
        currentLevelText.text = level.ToString();
        currentEquationText.text = currentEquation;
        currentNumberText.text = currentNumber.ToString();
        scoreText.text = totalScore.ToString();

        // calculate the remaining time in the level
        if(!gameOver && !gameWon && !moveToNextLevel)
        {
            if (remainingTime > 0)
            {
                remainingTime -= Time.deltaTime;
            }
            else
            {
                remainingTime = 0;
            }
            remainingTimeText.text = ((int)(remainingTime / 60)).ToString() + ":" + ((int)(remainingTime % 60));
        }
    }

    private void nextLevel()
    {
        nextLevelText.SetActive(true);
        winText.SetActive(false);
        gameOverText.SetActive(false);
        ball.SetActive(false);
        gameWon = false;
        gameOver = false;

        // wait 3 seconds before starting a new game after the game is over
        moveToNextLevelTimer += Time.deltaTime;
        if (moveToNextLevelTimer > offsetTime)
        {
            // move on to the next level is the game hasn't been won, otherwise start a new game
            if (level <= solutions.Length)
            {
                nextLevelText.SetActive(false);
                ballsLeft = originalBallsLeft;
                totalScore = level * ((int)remainingTime);
                remainingTime = timeForEachLevel;
                level++;
                currentEquation = equations[level - 1];
                solutionNumber = solutions[level - 1];
                currentNumber = 0;
                moveToNextLevel = false;
                moveToNextLevelTimer = 0;
                gameWon = false;
                gameWonTimer = 0;
                gameOver = false;
                gameOverTimer = 0;
            }
            else
            {
                NewGame();
            }
        }
    }

    private void endGame()
    {
        nextLevelText.SetActive(false);
        winText.SetActive(false);
        gameOverText.SetActive(true);
        ball.SetActive(false);
        moveToNextLevel = false;
        gameWon = false;

        // wait 3 seconds before starting a new game after the game is over
        gameOverTimer += Time.deltaTime;
        if (gameOverTimer > offsetTime)
        {
            gameOverText.SetActive(false);
            gameOverTimer = 0;
            NewGame();
        }
    }

    private void winGame()
    {
        nextLevelText.SetActive(false);
        winText.SetActive(true);
        gameOverText.SetActive(false);
        ball.SetActive(false);
        moveToNextLevel = false;
        gameOver = false;
        
        // wait 3 seconds before starting a new game after the game has been won
        gameWonTimer += Time.deltaTime;
        if (gameWonTimer > offsetTime)
        {
            winText.SetActive(false);
            gameWonTimer = 0;
            NewGame();
        }
    }

    public void collided(int displayNumber)
    {
        // add bumper display number to the answer talley
        this.currentNumber += displayNumber;
    }
}
