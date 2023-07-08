using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ZoomableScrollRect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float minZoom = 1f;
    [SerializeField] private float maxZoom = 2f;
    [SerializeField] private float zoomSpeed = 0.1f;

    private ScrollRect scrollRect;
    [SerializeField] private float currentZoom = 1.5f;

    private float lastDistance = 0f;

    [SerializeField] private bool isHovering = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }

    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    private void Start()
    {
        scrollRect.content.localScale = currentZoom * Vector2.one;
    }

    private void Update()
    {
        ZoomWithMouseScroll();
        ZoomWithTouches();
    }

    private void ZoomWithMouseScroll()
    {
        if (!isHovering) return;

        float zoomDelta = -Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * 6;

        if (Mathf.Abs(zoomDelta) > 0.01f)
        {
            currentZoom = Mathf.Clamp(currentZoom - zoomDelta, minZoom, maxZoom);
            scrollRect.content.localScale = currentZoom * Vector2.one;
        }
    }
    private void ZoomWithTouches()
    {
        if (Input.touchCount == 2)
        {
            // Get the first and second touches
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            scrollRect.enabled = false;

            // Calculate the distance between the touches
            float distance = Vector2.Distance(touch1.position, touch2.position);

            if (lastDistance == 0f)
            {
                lastDistance = distance;
            }
            else
            {
                // Calculate the change in distance since last frame
                float deltaDistance = distance - lastDistance;

                // Update the zoom level based on the change in distance
                float zoomDelta = -deltaDistance * zoomSpeed * Time.deltaTime;
                currentZoom = Mathf.Clamp(currentZoom - zoomDelta, minZoom, maxZoom);
                scrollRect.content.localScale = new Vector3(currentZoom, currentZoom, 1f);

                // Update the last distance
                lastDistance = distance;
            }
        }
        else
        {
            scrollRect.enabled = true;

            lastDistance = 0f;
        }
    }
}
