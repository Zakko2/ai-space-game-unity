using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public Asteroids asteroidManager;
    public Vector2 velocity;



    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            if (transform.localScale.x > 0.5f)
            {
                Split(velocity); // Call the Split method to create smaller asteroids and pass the original velocity
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
        if (transform.localScale.x > 0.3f) // Prevent infinite splitting
        {
            for (int i = 0; i < 2; i++)
            {
                // Instantiate a new smaller asteroid at the current position
                GameObject newAsteroid = asteroidManager.NewAsteroid(transform.position.x, transform.position.y);
                newAsteroid.transform.localScale = transform.localScale * 0.5f; // Set the new asteroid's scale to half of the original

                // Calculate the new velocity based on the original velocity
                float angle = Random.Range(30, 60); // Set a random angle between 30 and 60 degrees
                if (i == 1) angle = -angle; // Invert the angle for the second asteroid
                Vector2 newVelocity = Quaternion.Euler(0, 0, angle) * originalVelocity;
                newAsteroid.GetComponent<Asteroid>().velocity = newVelocity; // Set the new asteroid's velocity
                Debug.Log(originalVelocity);
                Debug.Log(newVelocity);

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
