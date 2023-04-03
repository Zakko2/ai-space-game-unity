using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Asteroids : MonoBehaviour
{
    public int numberOfAsteroids = 10;
    private int startingNumberOfAsteroids; // Add a variable to store the starting number of asteroids
    public float asteroidSize = 100f;
    public float asteroidSpeed = 50f;
    public int asteroidVertices = 10;
    public float asteroidJaggedness = 0.4f;
    public Material lineMaterial;
    public LineRenderer asteroidLinePrefab;
    public Canvas canvas;
    public GameObject explosionEffectPrefab;
    public List<GameObject> asteroids;
    public Dictionary<GameObject, Vector2> asteroidVelocities; // Replace the List<GameObject> and List<Vector2>
    public int maxNumberOfAsteroids = 50; // Set a maximum limit on the number of asteroids

    private int additionalAsteroids = 0;
    private bool respawningAsteroids = false; // Add a flag to track respawning

    void Start()
    {
        asteroidVelocities = new Dictionary<GameObject, Vector2>();
        startingNumberOfAsteroids = numberOfAsteroids; // Store the initial value set in the Unity UI
        CreateAsteroidBelt();
        Debug.Log("Ran start routine");
    }

    void Update()
    {
        // Existing code for moving asteroids
        MoveAsteroids();

        // Check if the "Q" key is pressed
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // Quit the game
            Application.Quit();
        }

        // Call the method to check if all asteroids are destroyed and respawn if needed
        CheckAndRespawnAsteroids();
    }

    void CheckAndRespawnAsteroids()
    {
        // If there are no active asteroids and there are no asteroids waiting to be removed
        if (asteroids.Count == 0 && asteroidVelocities.Count == 0 && !respawningAsteroids)
        {
            StartCoroutine(RespawnAsteroidsAfterDelay());
        }
    }

    IEnumerator RespawnAsteroidsAfterDelay()
    {
        // Set the respawning flag to true
        respawningAsteroids = true;

        // Wait for 3 seconds
        yield return new WaitForSeconds(3f);

        // Increase the number of additional asteroids to spawn
        additionalAsteroids++;

        // Update the numberOfAsteroids based on the starting number and additionalAsteroids
        numberOfAsteroids = startingNumberOfAsteroids + additionalAsteroids;

        // Apply the limit to the number of asteroids (optional)
        numberOfAsteroids = Mathf.Min(numberOfAsteroids, maxNumberOfAsteroids);

        // Call the CreateAsteroidBelt method to spawn new asteroids
        CreateAsteroidBelt();

        // Set the respawning flag to false
        respawningAsteroids = false;
    }

    void CreateAsteroidBelt()
    {
        // Get the camera's bounds in world space
        Camera mainCamera = Camera.main;
        float halfHeight = mainCamera.orthographicSize;
        float halfWidth = mainCamera.aspect * halfHeight;
        float leftBound = mainCamera.transform.position.x - halfWidth;
        float rightBound = mainCamera.transform.position.x + halfWidth;
        float topBound = mainCamera.transform.position.y + halfHeight;
        float bottomBound = mainCamera.transform.position.y - halfHeight;

        for (int i = 0; i < numberOfAsteroids; i++)
        {
            // Randomly choose one of the four edges for spawning
            int edge = Random.Range(0, 4);
            float x, y;

            switch (edge)
            {
                case 0: // Top edge
                    x = Random.Range(leftBound, rightBound);
                    y = topBound;
                    break;
                case 1: // Bottom edge
                    x = Random.Range(leftBound, rightBound);
                    y = bottomBound;
                    break;
                case 2: // Left edge
                    x = leftBound;
                    y = Random.Range(bottomBound, topBound);
                    break;
                case 3: // Right edge
                    x = rightBound;
                    y = Random.Range(bottomBound, topBound);
                    break;
                default:
                    x = 0;
                    y = 0;
                    break;
            }

            // Create the asteroid at the calculated position
            GameObject asteroid = NewAsteroid(x, y);

            asteroids.Add(asteroid);

            // Set a random velocity for the asteroid while avoiding (0, 0)
            Vector2 velocity;
            do
            {
                velocity = new Vector2(Random.Range(-asteroidSpeed, asteroidSpeed), Random.Range(-asteroidSpeed, asteroidSpeed));
            } while (velocity == Vector2.zero);
            asteroidVelocities.Add(asteroid, velocity); // Add the asteroid and its velocity to the dictionary
        }
    }



    public GameObject NewAsteroid(float x, float y)
    {
        int vert = Mathf.FloorToInt(Random.Range(asteroidVertices / 2, asteroidVertices * 1.5f));
        GameObject asteroid = new GameObject("Asteroid");
        asteroid.layer = LayerMask.NameToLayer("Asteroid"); // Set the asteroid's layer
        asteroid.transform.position = new Vector3(x, y, 0); // Set the asteroid's position

        // Add the Asteroid script to the asteroid GameObject
        Asteroid asteroidScript = asteroid.AddComponent<Asteroid>();

        // Set the asteroidManager reference in the Asteroid script
        asteroidScript.asteroidManager = this;
        
        asteroidScript.size = 3;

        asteroid.AddComponent<Rigidbody2D>().gravityScale = 0;
        asteroid.tag = "Asteroid";

        // Create the LineRenderer component on the asteroid GameObject
        LineRenderer lr = asteroid.AddComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.loop = true;
        lr.positionCount = vert + 1;
        lr.startWidth = 0.015f;
        lr.endWidth = 0.015f;
        lr.material = lineMaterial;

        Vector3[] positions = new Vector3[vert + 1];
        float angleStep = 360f / vert;
        float angle = 0;

        for (int i = 0; i < vert; i++)
        {
            float radius = asteroidSize / 2 * (1 + Random.Range(-asteroidJaggedness, asteroidJaggedness));
            float xPos = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            float yPos = radius * Mathf.Sin(angle * Mathf.Deg2Rad);
            positions[i] = new Vector3(xPos, yPos, 0);
            angle += angleStep;
        }

        positions[vert] = positions[0];
        lr.SetPositions(positions);

        // Add a PolygonCollider2D component to the asteroid
        PolygonCollider2D polygonCollider = asteroid.AddComponent<PolygonCollider2D>();

        // Convert the Vector3 array of positions to a Vector2 array
        Vector2[] colliderPath = new Vector2[positions.Length];
        for (int i = 0; i < positions.Length; i++)
        {
            colliderPath[i] = new Vector2(positions[i].x, positions[i].y);
        }

        // Set the path of the PolygonCollider2D to match the asteroid's shape
        polygonCollider.SetPath(0, colliderPath);


        // Create glowing particles at each vertex
        CreateGlowParticles(asteroid, positions);

        return asteroid;
    }





    private void CreateGlowParticles(GameObject asteroid, Vector3[] positions)
    {
        // Create a new particle system for the asteroid
        ParticleSystem glowParticles = asteroid.AddComponent<ParticleSystem>();

        // Configure the particle system
        var main = glowParticles.main;
        main.startColor = Color.white;
        main.startSize = 0.1f; // Adjust size as needed
        main.startLifetime = Mathf.Infinity; // Particle lifetime set to infinity
        main.loop = false; // Disable looping
        main.playOnAwake = false; // Disable play on awake

        // Disable the continuous emission of particles
        var emission = glowParticles.emission;
        emission.enabled = false;

        // Set the material for the particles
        var renderer = glowParticles.GetComponent<ParticleSystemRenderer>();
        renderer.material = Resources.Load<Material>("GlowParticleMaterial");

        // Emit particles only at each vertex position
        foreach (Vector3 position in positions)
        {
            glowParticles.Emit(new ParticleSystem.EmitParams { position = position }, 1);
        }
    }


    void MoveAsteroids()
    {
        // Get the camera's bounds in world space
        Camera mainCamera = Camera.main;
        float halfHeight = mainCamera.orthographicSize;
        float halfWidth = mainCamera.aspect * halfHeight;
        float leftBound = mainCamera.transform.position.x - halfWidth;
        float rightBound = mainCamera.transform.position.x + halfWidth;
        float topBound = mainCamera.transform.position.y + halfHeight;
        float bottomBound = mainCamera.transform.position.y - halfHeight;

        List<GameObject> asteroidsToBeRemoved = new List<GameObject>();

        foreach (KeyValuePair<GameObject, Vector2> entry in asteroidVelocities)
        {
            GameObject asteroid = entry.Key;

            if (asteroid == null)
            {
                asteroidsToBeRemoved.Add(asteroid);
                continue;
            }

            Vector2 velocity = entry.Value;

            // Update the position based on the velocity
            asteroid.transform.position += (Vector3)velocity * Time.deltaTime;

            // Update particle positions to match asteroid vertices
            LineRenderer lr = asteroid.GetComponent<LineRenderer>();
            ParticleSystem glowParticles = asteroid.GetComponent<ParticleSystem>();
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[glowParticles.particleCount];
            glowParticles.GetParticles(particles);
            for (int j = 0; j < particles.Length; j++)
            {
                particles[j].position = lr.GetPosition(j);
            }
            glowParticles.SetParticles(particles, particles.Length);

            // Edge detection and wrapping
            Vector3 pos = asteroid.transform.position;
            if (pos.x < leftBound)
            {
                pos.x = rightBound;
            }
            else if (pos.x > rightBound)
            {
                pos.x = leftBound;
            }
            if (pos.y < bottomBound)
            {
                pos.y = topBound;
            }
            else if (pos.y > topBound)
            {
                pos.y = bottomBound;
            }
            asteroid.transform.position = pos;
        }

        foreach (GameObject asteroid in asteroidsToBeRemoved)
        {
            asteroidVelocities.Remove(asteroid);
        }
    }

    public void ClearAllAsteroids()
    {
        // Loop through all asteroids and destroy them
        foreach (GameObject asteroid in asteroids)
        {
            Destroy(asteroid);
        }
        // Clear the lists and dictionary
        asteroids.Clear();
        asteroidVelocities.Clear();
        additionalAsteroids = 0;
    }

    public void InitializeAsteroids()
    {
        // Call the CreateAsteroidBelt method to spawn new asteroids
        CreateAsteroidBelt();
    }

    public void RemoveAsteroid(GameObject asteroid)
    {
        // Remove the asteroid from the lists and dictionary
        if (asteroids.Contains(asteroid))
        {
            asteroids.Remove(asteroid);
        }
        if (asteroidVelocities.ContainsKey(asteroid))
        {
            asteroidVelocities.Remove(asteroid);
        }
    }
}