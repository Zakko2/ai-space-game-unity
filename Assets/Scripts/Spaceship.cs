using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spaceship : MonoBehaviour
{
    public float spaceshipSize = 1.0f;
    public float accelerationRate = 2.0f;
    public float maxAcceleration = 5.0f;
    public float turnSpeed = 100.0f;
    public float decelerationFactor = 2.0f;
    public Material lineMaterial;
    public GameObject bulletPrefab;
    public float shootForce = 10f;
    public float bulletSpeed = 10f;

    private Rigidbody2D rb;

    void Start()
    {
        // Create a LineRenderer component to render the spaceship
        LineRenderer lr = gameObject.AddComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.loop = true;
        lr.positionCount = 4; // 3 vertices + closing the loop
        lr.startWidth = 0.015f;
        lr.endWidth = 0.015f;
        lr.material = lineMaterial;

        // Define the vertices for the triangular spaceship
        Vector3[] positions = new Vector3[4];
        positions[0] = new Vector3(0.0f, spaceshipSize, 0.0f); // Top vertex
        positions[1] = new Vector3(-spaceshipSize / 2, -spaceshipSize / 2, 0.0f); // Bottom-left vertex
        positions[2] = new Vector3(spaceshipSize / 2, -spaceshipSize / 2, 0.0f); // Bottom-right vertex
        positions[3] = positions[0]; // Close the loop

        gameObject.transform.position = new Vector3(0, 0, 1); // Set the z position to 1

        // Set the positions of the LineRenderer
        lr.SetPositions(positions);

        rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
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
        }
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
}



//Embark on the most electrifying space adventure of the century: Astro Blasters! Join Captain Starburn, the galaxy's bravest pilot, as he battles through a treacherous asteroid field to save his beloved planet, Galaxium Prime, and the enchanting space siren, Aurora Supernova. With the fate of the universe hanging in the balance, defeat the sinister Dark Nebula Armada and become the ultimate cosmic hero!

//Experience edge-of-your-seat excitement as you navigate the deadly asteroid field, dodge colossal space rocks, and engage in intense laser battles against the relentless forces of the Dark Nebula Armada. Unleash your inner daredevil and prove that you have what it takes to save the day in this high-octane space epic.

//Astro Blasters boasts state-of-the-art, awe-inspiring graphics that transport you to the far reaches of the cosmos. Get ready to groove to the synth-heavy soundtrack that'll have players' heads bopping as they navigate this thrilling cosmic rollercoaster. Astro Blasters is the out-of-this-world adventure that no gamer can afford to miss!