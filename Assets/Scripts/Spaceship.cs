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

    private Rigidbody2D rb;

    void Start()
    {
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