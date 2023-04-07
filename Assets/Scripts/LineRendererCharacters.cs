using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererCharacters : MonoBehaviour
{
    public Material lineMaterial; // Reference to the line material
    public float characterSize = 300f; // Define the size of the characters (exposed as a public variable)
    public float characterSpacing = 1.0f; // Define the spacing between characters (exposed as a public variable)
    public float lineWidth = 1f; // Define the width of the lines (exposed as a public variable)

    // Define a dictionary that holds line segment data for each character
    private Dictionary<char, List<Vector2>> charactersData;


    void Awake()
    {
        Debug.Log("LineRendererCharacters: Awake called");
        // Initialize the character data
        InitializeCharacterData();
        Debug.Log("LineRendererCharacters: charactersData initialized");
        Debug.Log("LineRendererCharacters: charactersData count = " + charactersData.Count);
    }

    // Optional: Define a setter method to set the characterSize and lineWidth programmatically
    public void SetCharacterSize(float size)
    {
        characterSize = size;
        lineWidth = size * 0.075f; // Set the lineWidth proportionally to the characterSize (you can adjust the factor as needed)
    }

    // Define line segments for each character in the InitializeCharacterData method
    private void InitializeCharacterData()
    {
        charactersData = new Dictionary<char, List<Vector2>>
    {
        { 'A', new List<Vector2> { new Vector2(0, 0), new Vector2(0, 2), new Vector2(1, 2), new Vector2(1, 1), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0) } },
        { 'B', new List<Vector2> { new Vector2(0, 0), new Vector2(0, 2), new Vector2(1, 2), new Vector2(1, 1), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 0) } },
        { 'C', new List<Vector2> { new Vector2(1, 0), new Vector2(0, 0), new Vector2(0, 2), new Vector2(1, 2) } },
        { 'D', new List<Vector2> { new Vector2(1, 0), new Vector2(0, 0), new Vector2(0, 2), new Vector2(1, 2), new Vector2(1, 0) } },
        { 'E', new List<Vector2> { new Vector2(1, 0), new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, 1), new Vector2(0, 2), new Vector2(1, 2) } },
        { 'F', new List<Vector2> { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, 1), new Vector2(0, 2), new Vector2(1, 2) } },
        { 'G', new List<Vector2> { new Vector2(0.5f, 1), new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 0), new Vector2(0, 2), new Vector2(1, 2) } },
        { 'H', new List<Vector2> { new Vector2(0, 0), new Vector2(0, 2), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), new Vector2(1, 2) } },
        { 'I', new List<Vector2> { new Vector2(0, 2), new Vector2(1, 2), new Vector2(0.5f, 2), new Vector2(0.5f, 0), new Vector2(0, 0), new Vector2(1, 0) } },
        { 'J', new List<Vector2> { new Vector2(0, 1), new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 2) } },
        { 'K', new List<Vector2> { new Vector2(0, 0), new Vector2(0, 2), new Vector2(0, 1), new Vector2(0.5f, 1), new Vector2(1, 2), new Vector2(0.5f, 1), new Vector2(1, 0) } },
        { 'L', new List<Vector2> { new Vector2(0, 2), new Vector2(0, 0), new Vector2(1, 0) } },
        { 'M', new List<Vector2> { new Vector2(0, 0), new Vector2(0, 2), new Vector2(0.5f, 1), new Vector2(1, 2), new Vector2(1, 0) } },
        { 'N', new List<Vector2> { new Vector2(0, 0), new Vector2(0, 2), new Vector2(1, 0), new Vector2(1, 2) } },
        { 'O', new List<Vector2> { new Vector2(0, 1), new Vector2(0, 2), new Vector2(1, 2), new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 0), new Vector2(0, 1) } },
        { 'P', new List<Vector2> { new Vector2(0, 0), new Vector2(0, 2), new Vector2(1, 2), new Vector2(1, 1), new Vector2(0, 1) } },
        { 'Q', new List<Vector2> { new Vector2(0, 1), new Vector2(0, 2), new Vector2(1, 2), new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0) } },
        { 'R', new List<Vector2> { new Vector2(0, 0), new Vector2(0, 2), new Vector2(1, 2), new Vector2(1, 1), new Vector2(0, 1), new Vector2(0.5f, 1), new Vector2(1, 0) } },
        { 'S', new List<Vector2> { new Vector2(1, 2), new Vector2(0, 2), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 0) } },
        { 'T', new List<Vector2> { new Vector2(0, 2), new Vector2(1, 2), new Vector2(0.5f, 2), new Vector2(0.5f, 0) } },
        { 'U', new List<Vector2> { new Vector2(0, 2), new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 2) } },
        { 'V', new List<Vector2> { new Vector2(0, 2), new Vector2(0, 1), new Vector2(0.5f, 0), new Vector2(1, 1), new Vector2(1, 2) } },
        { 'W', new List<Vector2> { new Vector2(0, 2), new Vector2(0, 0), new Vector2(0.5f, 1), new Vector2(1, 0), new Vector2(1, 2) } },
        { 'X', new List<Vector2> { new Vector2(0, 0), new Vector2(1, 2), new Vector2(0.5f, 1), new Vector2(0, 2), new Vector2(1, 0) } },
        { 'Y', new List<Vector2> { new Vector2(0, 2), new Vector2(0.5f, 1), new Vector2(1, 2), new Vector2(0.5f, 1), new Vector2(0.5f, 0) } },
        { 'Z', new List<Vector2> { new Vector2(0, 2), new Vector2(1, 2), new Vector2(0, 0), new Vector2(1, 0) } }
    };
    }

    public GameObject CreateCharacter(char character, Vector3 position, RectTransform parentTransform)
    {
        try
        {
            Debug.Log("Character: " + character);
            Debug.Log("Character Size: " + characterSize);
            Debug.Log("Line Width: " + lineWidth);
            Debug.Log("Position: " + position);
            Debug.Log("Target Canvas: " + (parentTransform != null ? parentTransform.name : "null"));

            // Check if charactersData is null
            if (charactersData == null)
            {
                Debug.LogError("charactersData dictionary is null");
                return null;
            }

            // Check if charactersData contains any entries
            if (charactersData.Count == 0)
            {
                Debug.LogError("charactersData dictionary is empty");
                return null;
            }

            if (!charactersData.TryGetValue(character, out List<Vector2> linePoints))
            {
                Debug.LogWarning($"Character '{character}' is not defined in charactersData.");
                return null;
            }

            GameObject characterObject = new GameObject("Character_" + character);
            Debug.Log("Character Object: " + (characterObject != null ? characterObject.name : "null"));

            // Set the createdCharacter object as a child of the parentTransform (LivesPanel)
            characterObject.transform.SetParent(parentTransform, false);
            // Set the local position of the characterObject relative to the parentTransform
            characterObject.transform.localPosition = position;

            LineRenderer lineRenderer = characterObject.AddComponent<LineRenderer>();
            if (lineRenderer == null)
            {
                Debug.LogError("lineRenderer is null");
                return null;
            }
            Debug.Log("Line Renderer: " + (lineRenderer != null ? lineRenderer.name : "null"));

            lineRenderer.useWorldSpace = false;
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
            //lineRenderer.startWidth = lineWidth; // Use the lineWidth variable for the startWidth
            //lineRenderer.endWidth = lineWidth; // Use the lineWidth variable for the endWidth
            lineRenderer.positionCount = linePoints.Count;

            Material testMaterial = new Material(Shader.Find("Unlit/Color"));
            testMaterial.color = Color.red;
            lineRenderer.material = testMaterial;

            for (int i = 0; i < linePoints.Count; i++)
            {
                lineRenderer.SetPosition(i, linePoints[i] * characterSize);
            }

            Debug.Log("Created characterObject: " + characterObject.name);
            return characterObject;
        }
        catch (Exception ex)
        {
            Debug.LogError("Exception in CreateCharacter: " + ex.Message);
        }
        return null;
    }

}