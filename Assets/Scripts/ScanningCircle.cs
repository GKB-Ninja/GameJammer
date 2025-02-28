using UnityEngine;

public class ScanningCircle : MonoBehaviour
{
    public float maxSize = 5f; // Maximum size of the circle
    public float duration = 3f; // Duration to reach the maximum size
    public LayerMask collisionLayer; // Layer to check for collisions

    private float currentSize = 0f;
    private float growthRate;
    private bool isScanning = true;

    void Start()
    {
        growthRate = maxSize / duration;
        Debug.Log("Scanning started. Growth rate: " + growthRate);
    }

    void Update()
    {
        if (isScanning)
        {
            // Grow the circle
            currentSize += growthRate * Time.deltaTime;
            transform.localScale = new Vector3(currentSize, currentSize, 1f);

            // Check for collisions
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, currentSize / 2, collisionLayer);
            foreach (Collider2D collider in colliders)
            {
                if (collider.gameObject != gameObject) // Ensure it doesn't detect itself
                {
                    Debug.Log("Collider detected: " + collider.gameObject.name);
                    OnPrefabCollision();
                    isScanning = false;
                    break;
                }
            }

            // Stop scanning if the maximum size is reached
            if (currentSize >= maxSize)
            {
                isScanning = false;
                Debug.Log("Maximum size reached. Scanning stopped.");
            }
        }
    }

    void OnPrefabCollision()
    {
        // Trigger the function when a collision with the prefab occurs
        Debug.Log("Collision with prefab detected!");
        // Add your custom function call here
    }
}