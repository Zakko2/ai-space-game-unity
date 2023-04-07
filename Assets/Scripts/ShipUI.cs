using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipUI : MonoBehaviour
{
    public float spaceshipSize = 1.0f;
    public Material lineMaterial;

    void Start()
    {
        // Get the existing LineRenderer component instead of adding a new one
        LineRenderer lr = GetComponent<LineRenderer>();

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
    }
}
