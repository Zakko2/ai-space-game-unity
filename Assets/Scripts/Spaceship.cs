using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spaceship : MonoBehaviour
{
    public float accelerationRate = 2.0f;
    public float maxAcceleration = 5.0f;
    public float turnSpeed = 100.0f;
    public float decelerationFactor = 2.0f;
    public GameObject bulletPrefab;
    public float shootForce = 10f;
    public float bulletSpeed = 10f;
    public Asteroids asteroidManager; // Reference to the Asteroids script
    public GameManager gameManager;    // Reference to the GameManager script
    //public ParticleSystem explosionEffect; // Reference to the Particle System for the explosion

    private bool isExploding = false;
    private Rigidbody2D rb;
    private AudioSource audioSource;

    [SerializeField] private ParticleSystem spaceshipExplosionEffectPrefab;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Rotate the spaceship
        float rotation = -horizontalInput * turnSpeed * Time.deltaTime;
        transform.Rotate(new Vector3(0, 0, rotation));

        // Accelerate the spaceship in the direction it's facing
        if (verticalInput > 0)
        {
            rb.AddForce(transform.up * accelerationRate, ForceMode2D.Force);
        }
        else
        {
            // Decelerate the spaceship when not accelerating
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, decelerationFactor * Time.deltaTime);
        }

        // Clamp the spaceship's velocity
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxAcceleration);

        // Screen wrapping logic
        ScreenWrapping();

        // Shooting
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
            // Play the shooting sound effect
            audioSource.Play();
        }
    }

    public void DeactivateSpaceship()
    {
        // Disable spaceship controls, renderer, and collider
        this.enabled = false;
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
    }

    public void ReactivateSpaceship()
    {
        // Re-enable spaceship controls, renderer, and collider
        this.enabled = true;
        GetComponent<Renderer>().enabled = true;
        GetComponent<Collider2D>().enabled = true;
    }


    void Shoot()
    {
        // Calculate the offset for spawning the bullet in front of the spaceship
        LineRenderer lr = GetComponent<LineRenderer>();
        Vector3 offset = (lr.GetPosition(0) + lr.GetPosition(1) + lr.GetPosition(2)) / 3.0f;

        // Instantiate a new bullet and set its position and rotation to match the spaceship, with a 90-degree offset in the z-axis
        GameObject bullet = Instantiate(bulletPrefab, transform.TransformPoint(offset), transform.rotation * Quaternion.Euler(0, 0, 90));

        // Calculate the bullet's direction based on the spaceship's rotation
        Vector2 direction = transform.up;

        // Set the bullet's velocity
        bullet.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;
    }



    void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore collision if the spaceship is currently exploding
        if (isExploding)
            return;

        // Check if the spaceship collided with an asteroid
        if (other.CompareTag("Asteroid") && !isExploding)
        {
            // Instantiate the spaceship explosion effect
            ParticleSystem explosionInstance = Instantiate(spaceshipExplosionEffectPrefab, transform.position, Quaternion.identity);
            explosionInstance.Play();
            // Set the StopAction to automatically destroy the explosion instance after playing
            var mainModule = explosionInstance.main;
            mainModule.stopAction = ParticleSystemStopAction.Destroy;

            StartCoroutine(ExplodeAndRespawn());
        }
    }

    IEnumerator ExplodeAndRespawn()
    {

        // Check if the game is over, and if so, exit the coroutine early
        if (gameManager.isGameOver)
        {
            yield break;
        }

        // Set isExploding to true to prevent multiple explosions
        isExploding = true;

        // Disable spaceship controls and renderer
        this.enabled = false;
        GetComponent<Renderer>().enabled = false;

        // Get the spaceship's collider and disable it
        Collider2D spaceshipCollider = GetComponent<Collider2D>();
        spaceshipCollider.enabled = false; // Disable the collider

        // Play the explosion effect
        //explosionEffect.transform.position = transform.position; // Set the position to match the player's ship
        //explosionEffect.Play();
        spaceshipExplosionEffectPrefab.Play();

        // Call the LoseLife method from the GameManager script
        gameManager.LoseLife();

        // Wait for 1 second before respawning the player
        yield return new WaitForSeconds(1f);

        // Check again if the game is over before respawning
        if (gameManager.isGameOver)
        {
            yield break;
        }

        // Stop the explosion effect
        //explosionEffect.Stop();
        spaceshipExplosionEffectPrefab.Stop();

        // Set the player's velocity to 0
        rb.velocity = Vector2.zero;

        // Respawn the player at the safest position
        // Use the reference to asteroidManager to access the asteroids list
        Vector3 safestPosition = FindSafestRespawnPosition(asteroidManager.asteroids);
        transform.position = safestPosition;

        // Re-enable spaceship controls and renderer
        this.enabled = true;
        GetComponent<Renderer>().enabled = true;

        // Re-enable the spaceship's collider
        spaceshipCollider.enabled = true; // Enable the collider

        // Set isExploding back to false to allow for future explosions
        isExploding = false;
    }



    void ScreenWrapping()
    {
        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(transform.position);
        bool wrapHorizontal = false;
        bool wrapVertical = false;

        if (viewportPosition.x < 0 || viewportPosition.x > 1)
        {
            viewportPosition.x = viewportPosition.x < 0 ? 1 - 0.01f : 0.01f;
            wrapHorizontal = true;
        }

        if (viewportPosition.y < 0 || viewportPosition.y > 1)
        {
            viewportPosition.y = viewportPosition.y < 0 ? 1 - 0.01f : 0.01f;
            wrapVertical = true;
        }

        if (wrapHorizontal || wrapVertical)
        {
            Vector3 newPosition = Camera.main.ViewportToWorldPoint(viewportPosition);
            newPosition.z = transform.position.z;
            transform.position = newPosition;
        }
    }

    public Vector3 FindSafestRespawnPosition(List<GameObject> asteroids)
    {
        // Get the camera's bounds in world space
        Camera mainCamera = Camera.main;
        float halfHeight = mainCamera.orthographicSize;
        float halfWidth = mainCamera.aspect * halfHeight;
        float leftBound = mainCamera.transform.position.x - halfWidth;
        float rightBound = mainCamera.transform.position.x + halfWidth;
        float topBound = mainCamera.transform.position.y + halfHeight;
        float bottomBound = mainCamera.transform.position.y - halfHeight;

        // Define a margin inside the screen bounds
        float margin = 1.0f; // Adjust the margin value as needed

        // Define the search area within the margin inside the screen bounds
        float searchAreaLeftBound = leftBound + margin;
        float searchAreaRightBound = rightBound - margin;
        float searchAreaTopBound = topBound - margin;
        float searchAreaBottomBound = bottomBound + margin;

        // Initialize variables to keep track of the safest position and maximum distance
        Vector3 safestPosition = Vector3.zero;
        float maxDistance = float.MinValue;

        // Iterate through possible respawn positions within the search area
        for (float x = searchAreaLeftBound; x <= searchAreaRightBound; x += margin)
        {
            for (float y = searchAreaBottomBound; y <= searchAreaTopBound; y += margin)
            {
                // Calculate the minimum distance to all asteroids from the current position
                Vector3 currentPosition = new Vector3(x, y, 0);
                float minDistance = float.MaxValue;
                foreach (GameObject asteroid in asteroids)
                {
                    float distance = Vector3.Distance(currentPosition, asteroid.transform.position);
                    minDistance = Mathf.Min(minDistance, distance);
                }

                // Update the safest position if the current position has a greater minimum distance
                if (minDistance > maxDistance)
                {
                    maxDistance = minDistance;
                    safestPosition = currentPosition;
                }
            }
        }

        // Return the safest respawn position
        return safestPosition;
    }

}