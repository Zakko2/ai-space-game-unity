using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int startingLives = 3;
    public int pointsPerExtraLife = 10000;
    public GameObject gameOverScreen; // Reference to the Game Over UI
    public bool isGameOver = false;     // Boolean variable to track if the game is over

    private int currentLives;
    private int currentScore;
    private int nextExtraLifeThreshold;

    [SerializeField]
    private RectTransform livesPanelTransform;

    [SerializeField]
    private TextMeshProUGUI livesText;

    [SerializeField]
    private TextMeshProUGUI scoreText;

    [SerializeField]
    private GameObject gameOverPanel;

    [SerializeField]
    private TextMeshProUGUI gameOverText;

    
    void Start()
    {
        currentLives = startingLives;
        UpdateLivesText();
        currentScore = 0;
        nextExtraLifeThreshold = pointsPerExtraLife; // Initialize the threshold for the first extra life
        UpdateScoreText();
        gameOverPanel.SetActive(false); // Hide the game over panel at start
    }

    public void LoseLife()
    {

        currentLives--;
        UpdateLivesText();

        if (currentLives <= 0)
        {
            // Handle game over
            GameOver();
        }
    }

    private void GameOver()
    {
        isGameOver = true;
        // Deactivate the spaceship when the game is over
        Spaceship spaceship = FindObjectOfType<Spaceship>();
        if (spaceship != null)
        {
            spaceship.DeactivateSpaceship();
        }

        // Show the game over message and ask the player if they want to try again
        gameOverPanel.SetActive(true);
        gameOverText.text = "Game Over\nWould you like to try again?";
    }


    public void OnTryAgainButton()
    {
        // Reactivate the spaceship before restarting the game
        Spaceship spaceship = FindObjectOfType<Spaceship>();
        if (spaceship != null)
        {
            spaceship.ReactivateSpaceship();
        }

        // Get the reference to the Asteroids script
        Asteroids asteroidManager = FindObjectOfType<Asteroids>();
        if (asteroidManager != null)
        {
            // Clear all existing asteroids and initialize new ones
            asteroidManager.ClearAllAsteroids();
            asteroidManager.InitializeAsteroids();
        }

        // Reset the player's score and lives
        currentScore = 0;
        currentLives = 3;
        UpdateScoreText();
        UpdateLivesText();
        // Restart the game (reload the current scene)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    public void OnQuitButton()
    {
        // Quit the game
        Application.Quit();
    }


    private void UpdateLivesText()
    {
        livesText.text = "Lives: " + currentLives.ToString();
    }

    private void UpdateScoreText()
    {
        scoreText.text = "Score: " + currentScore.ToString();
    }

    public void AddScore(int points)
    {
        // Update the player's score and the UI
        currentScore += points;
        scoreText.text = $"Score: {currentScore}";

        // Check if the player has reached the threshold for an extra life
        if (currentScore >= nextExtraLifeThreshold)
        {
            // Award an extra life
            currentLives++;
            UpdateLivesText();

            // Update the threshold for the next extra life
            nextExtraLifeThreshold += pointsPerExtraLife;
        }
    }
}
