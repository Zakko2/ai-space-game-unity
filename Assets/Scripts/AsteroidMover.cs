using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AsteroidMover : MonoBehaviour
{
    private RectTransform rectTransform;

    [SerializeField]
    private float speed = 100f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector2 newPosition = rectTransform.anchoredPosition + new Vector2(horizontal, vertical) * speed * Time.deltaTime;
        rectTransform.anchoredPosition = newPosition;
    }
}
