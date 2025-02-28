using UnityEngine;
using System.Collections;

public class ItemSlotInterac : MonoBehaviour
{
    public GameObject specificEntity; // The specific entity to check collision with
    private bool[] childrenInTrigger; // Array to track the trigger state of each child
    private bool isDragging = false;
    private Vector3 startPosition; // To store the drag-start position
    private float transitionDuration = 0.5f; // Duration for the smooth transition

    void Start()
    {
        // Initialize the array based on the number of children
        childrenInTrigger = new bool[transform.childCount];
    }

    void Update()
    {
        // Handle dragging
        if (isDragging)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePosition.x, mousePosition.y, transform.position.z);
        }

        // Check for mouse button release to stop dragging
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            CheckAllChildrenInTrigger();
        }
    }

    private void OnMouseDown()
    {
        // Start dragging when the mouse button is pressed
        isDragging = true;
        startPosition = transform.position; // Store the drag-start position
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == specificEntity)
        {
            Debug.Log("Collision started with the specific entity.");
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject == specificEntity)
        {
            Debug.Log("Still colliding with the specific entity.");
            // Update the trigger state for the corresponding child
            for (int i = 0; i < transform.childCount; i++)
            {
                Collider2D childCollider = transform.GetChild(i).GetComponent<Collider2D>();
                if (childCollider != null && childCollider.IsTouching(other))
                {
                    childrenInTrigger[i] = true;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == specificEntity)
        {
            Debug.Log("Collision ended with the specific entity.");
            // Update the trigger state for the corresponding child
            for (int i = 0; i < transform.childCount; i++)
            {
                Collider2D childCollider = transform.GetChild(i).GetComponent<Collider2D>();
                if (childCollider != null && childCollider.IsTouching(other))
                {
                    childrenInTrigger[i] = false;
                }
            }
        }
    }

    private void CheckAllChildrenInTrigger()
    {
        // Check if all children are in the trigger
        foreach (bool inTrigger in childrenInTrigger)
        {
            if (!inTrigger)
            {
                StartCoroutine(SmoothReturnToStart());
                return;
            }
        }
        Debug.Log("All children are in the trigger.");
    }

    private IEnumerator SmoothReturnToStart()
    {
        float elapsedTime = 0f;
        Vector3 currentPos = transform.position;

        while (elapsedTime < transitionDuration)
        {
            transform.position = Vector3.Lerp(currentPos, startPosition, elapsedTime / transitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = startPosition;
    }
}