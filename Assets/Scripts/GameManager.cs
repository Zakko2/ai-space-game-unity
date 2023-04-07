using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyGame
{
    public class GameManager : MonoBehaviour
    {
        public int startingLives = 3;
        public int pointsPerExtraLife = 10000;
        public GameObject gameOverScreen; // Reference to the Game Over UI
        public bool isGameOver = false;     // Boolean variable to track if the game is over                                        
                                            //public LineRendererCharacters lineRendererCharacters; // Reference to the LineRendererCharacters script

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

        [SerializeField]
        private LineRendererCharacters lineRendererCharacters; // Reference to the LineRendererCharacters script

        GameObject testGameObject;
        private float horizontalSpacing = 0.25f; // Spacing between letters



        void Start()
        {
            currentLives = startingLives;
            UpdateLivesText();
            currentScore = 0;
            nextExtraLifeThreshold = pointsPerExtraLife;
            UpdateScoreText();
            gameOverPanel.SetActive(false);

            // Find the LineRendererManager GameObject and get the LineRendererCharacters component
            GameObject lineRendererManager = GameObject.Find("LineRendererManager");
            LineRendererCharacters lineRendererCharacters = null;
            if (lineRendererManager != null)
            {
                lineRendererCharacters = lineRendererManager.GetComponent<LineRendererCharacters>();
            }
            else
            {
                Debug.LogError("LineRendererManager GameObject not found.");
            }

            // Find the target canvas
            GameObject canvasObject = GameObject.Find("LivesPanel");
            Canvas targetCanvas = null;
            if (canvasObject != null)
            {
                targetCanvas = canvasObject.GetComponent<Canvas>();
            }
            else
            {
                Debug.LogError("Canvas GameObject not found.");
            }

            // Find the LivesPanel GameObject and get the RectTransform component
            GameObject livesPanelObject = GameObject.Find("LivesPanel");
            RectTransform livesPanelRectTransform = null;
            if (livesPanelObject != null)
            {
                livesPanelRectTransform = livesPanelObject.GetComponent<RectTransform>();
            }
            else
            {
                Debug.LogError("LivesPanel GameObject not found.");
            }

            // Define the letters to be printed
            //string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string letters = "A";
            float characterScale = 40f; // Adjust the scale of the characters
            lineRendererCharacters.SetCharacterSize(characterScale);
            // Call CreateCharacter method from the LineRendererCharacters script
            if (lineRendererCharacters != null && livesPanelRectTransform != null)
            {
                // Get the height of the LivesPanel
                float panelHeight = livesPanelRectTransform.rect.height;
                // Calculate the vertical position (center the characters within the panel)
                float verticalPosition = -panelHeight / 2.0f;

                // Loop through each letter and create the corresponding character
                for (int i = 0; i < letters.Length; i++)
                {
                    char letter = letters[i];
                    // Calculate the position for each letter based on the index and spacing
                    Vector3 position = new Vector3(i * horizontalSpacing, verticalPosition, 0);
                    // Create the character
                    GameObject createdCharacter = lineRendererCharacters.CreateCharacter(letter, position, livesPanelRectTransform);

                    // Set the createdCharacter object as a child of the LivesPanel
                    //if (createdCharacter != null)
                    //{
                    //    createdCharacter.transform.SetParent(livesPanelRectTransform, false);
                    //}
                }
            }
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

}