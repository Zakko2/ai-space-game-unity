using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MyGame
{
    public class Bullet : MonoBehaviour
    {
        public float bulletSize = 0.1f;
        public Material lineMaterial;
        public float speed = 10f;
        public float lifetime = 3f;

        private Rigidbody2D rb;

        void Start()
        {
            // Create a LineRenderer component to render the bullet
            LineRenderer lr = gameObject.AddComponent<LineRenderer>();
            lr.useWorldSpace = false;
            lr.loop = false;
            lr.positionCount = 2; // 2 vertices for a small line
            lr.startWidth = 0.015f;
            lr.endWidth = 0.015f;
            lr.material = lineMaterial;

            // Define the vertices for the small bullet line
            Vector3[] positions = new Vector3[2];
            positions[0] = new Vector3(-bulletSize / 2, 0.0f, 0.0f);
            positions[1] = new Vector3(bulletSize / 2, 0.0f, 0.0f);

            // Set the positions of the LineRenderer
            lr.SetPositions(positions);

            // Add a CircleCollider2D component to the bullet
            CircleCollider2D circleCollider = gameObject.AddComponent<CircleCollider2D>();

            // Set the radius of the CircleCollider2D based on the bullet size
            circleCollider.radius = bulletSize / 2f;

            // Set the isTrigger property to true for detecting collisions
            circleCollider.isTrigger = true;

            // Get the existing Rigidbody2D component from the bullet
            rb = gameObject.GetComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.velocity = transform.right * speed;

            // Destroy the bullet after the lifetime expires
            Destroy(gameObject, lifetime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Asteroid"))
            {
                // Get the Asteroid script from the other GameObject
                Asteroid asteroidScript = other.gameObject.GetComponent<Asteroid>();

                if (asteroidScript != null)
                {
                    // Destroy the asteroid and the bullet
                    Destroy(other.gameObject);
                    Destroy(gameObject);
                }
                else
                {
                    Debug.LogError("Asteroid script is missing on the collided object.");
                }
            }
        }

    }
}