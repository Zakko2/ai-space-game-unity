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
            Split();
            Destroy(other.gameObject); // Destroy the bullet
            Destroy(gameObject); // Destroy the original asteroid
        }
    }

    public void Split()
    {
        if (transform.localScale.x > 0.5f) // Prevent infinite splitting
        {
            for (int i = 0; i < 2; i++)
            {
                // Instantiate a new smaller asteroid at the current position
                GameObject newAsteroid = asteroidManager.NewAsteroid(transform.position.x, transform.position.y);
                newAsteroid.transform.localScale = transform.localScale * 0.5f; // Set the new asteroid's scale to half of the original

                // Calculate the new velocity
                float angle = Random.Range(30, 60); // Set a random angle between 30 and 60 degrees
                if (i == 1) angle = -angle; // Invert the angle for the second asteroid
                Vector2 newVelocity = Quaternion.Euler(0, 0, angle) * velocity;
                newAsteroid.GetComponent<Asteroid>().velocity = newVelocity; // Set the new asteroid's velocity
            }

            // Remove the original asteroid from the list
            asteroidManager.asteroids.Remove(gameObject);
        }
    }
}
