using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int startingLives = 3;
    public GameObject shipUIPrefab;
    public float shipUISpacing = 1f;
    public Vector3 shipUIStartPosition = new Vector3(-7f, 4f, 0f);

    private int currentLives;
    private GameObject[] shipUIInstances;
    private RectTransform livesPanelTransform;

    void Start()
    {
        currentLives = startingLives;
        livesPanelTransform = GameObject.Find("LivesPanel").GetComponent<RectTransform>();
        CreateShipUILives();
    }

    public void LoseLife()
    {
        currentLives--;
        UpdateShipUILives();

        if (currentLives <= 0)
        {
            // Handle game over, e.g. show a game over screen or restart the level
        }
        else
        {
            // Reset the game state, e.g. move the player's ship and asteroids to their starting positions
            // You can implement this in separate methods in the respective scripts and call them here
        }
    }

    private void CreateShipUILives()
    {
        shipUIInstances = new GameObject[startingLives];

        for (int i = 0; i < startingLives; i++)
        {
            GameObject shipUIInstance = Instantiate(shipUIPrefab, shipUIStartPosition + new Vector3(i * shipUISpacing, 0f, 0f), Quaternion.identity, livesPanelTransform);
            shipUIInstances[i] = shipUIInstance;
        }
    }

    private void UpdateShipUILives()
    {
        if (currentLives >= 0 && currentLives < shipUIInstances.Length)
        {
            Destroy(shipUIInstances[currentLives]);
        }
    }
}
