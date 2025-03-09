using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemInvOrgMng : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public GameObject specificEntity; // The specific entity to check collision with
    private bool[] childrenInTrigger; // Array to track the trigger state of each child
    private Vector3 startPosition; // To store the drag-start position
    private float transitionDuration = 0.5f; // Duration for the smooth transition
    private RectTransform rectTransform; // To store the RectTransform component
    private bool isLerping = false; // To track if lerp is in progress
    private Canvas canvas; // To store the Canvas component

    void Start()
    {
        // Initialize
        childrenInTrigger = new bool[transform.childCount];
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>(); // Get the Canvas component in the parent
        startPosition = rectTransform.position; // Set the start position at the beginning
    }

    void Update()
    {

    }

    // Prevent a second drag until the current lerp is complete
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isLerping) return;
    }

    // GameObject location is updated as the mouse is dragged relative to the canvas
    public void OnDrag(PointerEventData eventData)
    {
        // Convert the mouse position to canvas position
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out localPoint);
        rectTransform.localPosition = localPoint;
    }

    // Check if all children are in the trigger when the drag ends
    public void OnEndDrag(PointerEventData eventData)
    {
        CheckAllChildrenInTrigger();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
    }

    // Update the trigger state for each children per frame
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject == specificEntity)
        {
            // Update the trigger state for the corresponding child
            for (int i = 0; i < transform.childCount; i++)
            {
                Collider2D childCollider = transform.GetChild(i).GetComponent<Collider2D>();
                if (childCollider != null && childCollider.IsTouching(other))
                {
                    childrenInTrigger[i] = true;
                }
                else if (childCollider != null && !childCollider.IsTouching(other))
                {
                    childrenInTrigger[i] = false;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {

    }

    private void CheckAllChildrenInTrigger()
    {
        foreach (bool inTrigger in childrenInTrigger)
        {
            if (!inTrigger)
            {
                childrenInTrigger = new bool[transform.childCount]; // Reset the array
                StartCoroutine(SmoothReturnToStart());
                return;
            }
            else
            {
                // Do something when all children are in the trigger
            }
        }
    }

    private IEnumerator SmoothReturnToStart()
    {
        isLerping = true; // Set lerping flag to true
        float elapsedTime = 0f;
        Vector3 currentPos = rectTransform.position;

        while (elapsedTime < transitionDuration)
        {
            rectTransform.position = Vector3.Lerp(currentPos, startPosition, elapsedTime / transitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rectTransform.position = startPosition;
        isLerping = false; // Reset lerping flag
    }
}
