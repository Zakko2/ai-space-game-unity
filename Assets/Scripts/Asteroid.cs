using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MyGame
{
    public class Asteroid : MonoBehaviour
    {
        public Asteroids asteroidManager;
        public Vector2 velocity;
        public int size;
        private GameManager gameManager; // Reference to the GameManager
        public GameObject Explosion;

        void Start()
        {
            // Find the GameManager in the scene
            gameManager = GameObject.FindObjectOfType<GameManager>();
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Bullet"))
            {
                // Get the original asteroid's velocity
                Vector2 originalVelocity = asteroidManager.asteroidVelocities[gameObject];

                // Call the Split method to create smaller asteroids
                Split(originalVelocity);

                // Calculate the points based on the asteroid's size
                int points = 0;
                switch (size)
                {
                    case 3:
                        points = 20; // Large Asteroid
                        break;
                    case 2:
                        points = 50; // Medium Asteroid
                        break;
                    case 1:
                        points = 100; // Small Asteroid
                        break;
                }

                // Add points to the player's score
                if (gameManager != null)
                {
                    gameManager.AddScore(points);
                }

                // Remove this asteroid from the asteroidManager's lists
                asteroidManager.asteroids.Remove(gameObject);
                asteroidManager.asteroidVelocities.Remove(gameObject);

                // Destroy the projectile and the asteroid
                Destroy(other.gameObject);
                Destroy(gameObject);
            }
        }

        public void Split(Vector2 originalVelocity)
        {
            // Instantiate the explosion effect at the current position
            ParticleSystem explosion = Instantiate(asteroidManager.explosionEffectPrefab, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();

            // Play the explosion effect
            explosion.Play();

            if (transform.localScale.x > 0.3f) // Prevent infinite splitting
            {
                // Destroy the explosion effect after its duration
                Destroy(explosion.gameObject, explosion.main.duration);

                for (int i = 0; i < 2; i++)
                {
                    // Instantiate a new smaller asteroid at the current position
                    GameObject newAsteroid = asteroidManager.NewAsteroid(transform.position.x, transform.position.y);
                    newAsteroid.transform.localScale = transform.localScale * 0.5f; // Set the new asteroid's scale to half of the original

                    // Set the size of the new smaller asteroids based on its scale
                    float scale = newAsteroid.transform.localScale.x;
                    if (scale >= 0.9f)
                    {
                        newAsteroid.GetComponent<Asteroid>().size = 3; // Large Asteroid
                    }
                    else if (scale >= 0.45f)
                    {
                        newAsteroid.GetComponent<Asteroid>().size = 2; // Medium Asteroid
                    }
                    else
                    {
                        newAsteroid.GetComponent<Asteroid>().size = 1; // Small Asteroid
                    }

                    // Calculate the new velocity based on the original velocity
                    float angle = Random.Range(30, 60); // Set a random angle between 30 and 60 degrees
                    if (i == 1) angle = -angle; // Invert the angle for the second asteroid
                    Vector2 newVelocity = Quaternion.Euler(0, 0, angle) * originalVelocity;
                    newAsteroid.GetComponent<Asteroid>().velocity = newVelocity; // Set the new asteroid's velocity

                    // Add the new asteroid to the asteroids list
                    asteroidManager.asteroids.Add(newAsteroid);

                    // Add the new asteroid to the asteroidVelocities dictionary
                    asteroidManager.asteroidVelocities.Add(newAsteroid, newVelocity);
                }

                // Remove the original asteroid from the dictionary
                asteroidManager.asteroidVelocities.Remove(gameObject);
            }
        }


    }
}