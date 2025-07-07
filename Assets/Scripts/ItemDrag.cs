using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

// TODO: add rotatablity
// For managing, dragging and storing game items to the inventory
public class ItemDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    public bool ItemStored { get; set; }
    public bool IsDragged { get; set; }
    public Vector3 ReturnLocationAfterDrag { get; set; }

    // The geometric center of the item, calculated as the average position of all child item tiles
    public Vector3 GeometricCenter
    {
        get
        {
            Vector3 perfectCenter = Vector3.zero;
            foreach (RectTransform childRectT in transform.GetComponentsInChildren<RectTransform>())
            {
                if (childRectT != rectTransform)
                {
                    perfectCenter += childRectT.position;
                }
            }
            return perfectCenter /= transform.childCount;
        }
    }

    private static GameObject inventory;
    private static InventoryOrganization inventoryOrganization;
    private static List<Vector3> invProximityPoints; // Points around the inventory to which the item can be pushed back upon fails
    private float transitionDuration = 0.25f; // Time gap from drag release position to 'ReturnLocationAfterDrag' position
    private RectTransform rectTransform;
    private Image img;
    private Canvas canvas;
    private AudioSource[] audios;
    private Vector3 prevPointerPosition;

    // TODO: make intervened items play sfx instead of this item
    /* An item must have its AudioSource components' indexing as follows;
     * [0]: sfx for when another item intervenes its position in the inventory
     */

    void Start()
    {
        if (inventory == null)
        {
            inventory = GameObject.FindGameObjectWithTag("Inventory");
            if (inventory == null)
            {
                Debug.LogError("GameObject tagged \"inventory\" cannot be found. Please ensure it exists in the scene.");
                return;
            }

            inventoryOrganization = inventory.GetComponentInChildren<InventoryOrganization>();
            if (inventoryOrganization == null)
            {
                Debug.LogError("InventoryOrganization component not found in the inventory GameObject. Please ensure it exists.");
                return;
            }
        }

        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("RectTransform component not found on the item. Please ensure it exists.");
            return;
        }

        audios = GetComponents<AudioSource>();
        if (audios.Length == 0)
        {
            Debug.LogWarning("No AudioSource components found on the item. See ItemDrag documentation for details.");
        }

        img = GetComponent<Image>();
        if (img == null)
        {
            Debug.LogError("Image component not found on the item. Please ensure it exists for raycasting.");
            return;
        }

        canvas = transform.root.GetComponent<Canvas>();
        ReturnLocationAfterDrag = rectTransform.position;
    }

    public static void SetInventoryProximityPoints()
    {
        invProximityPoints = GameObject.FindGameObjectWithTag("Inventory").GetComponentInChildren<InventoryOrganization>().GetInvProximityPoints(1);
    }




    public void OnPointerDown(PointerEventData eventData)
    {
        prevPointerPosition = eventData.position;
    }

    // If the item is stored in the inventory, it will be removed from the inventory when dragging starts
    public void OnBeginDrag(PointerEventData eventData)
    {
        IsDragged = true;

        if (ItemStored)
        {
            inventoryOrganization.RemoveItem(gameObject);
        }
        UpdateDragPosition(eventData);
    }

    // GameObject location is updated as the mouse is dragged relative to the canvas
    public void OnDrag(PointerEventData eventData)
    {
        UpdateDragPosition(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        img.raycastTarget = false; // Prevent another drag

        if (inventoryOrganization.IsWithinInvBounds(GeometricCenter))
        {
            if (!inventoryOrganization.PlaceItem(gameObject))
            {
                audios[0].Play();
                StartCoroutine(SmoothReturnToOrigin(ItemStored));
            }
            else StartCoroutine(SmoothReturnToOrigin(false));
        }
        else
        {
            rectTransform.SetParent(canvas.transform);
            ItemStored = false;
            ReturnLocationAfterDrag = rectTransform.position;
            img.raycastTarget = true; // Allow dragging again
        }

        IsDragged = false;
    }




    public void AddForceTowardsPoint(Vector3 target, float forceMagnitude)
    {
        Vector2 direction = (target - rectTransform.position).normalized;
        if (TryGetComponent<Rigidbody2D>(out var rb2d))
        {
            rb2d.AddForce(direction * forceMagnitude, ForceMode2D.Impulse);
        }
        else
        {
            Debug.LogWarning($"Rigidbody2D component not found on the item {gameObject.name}. Cannot apply force.");
        }
    }

    private void UpdateDragPosition(PointerEventData eventData)
    {
        Vector3 currentPointerPosition = eventData.position;
        Vector3 delta = currentPointerPosition - prevPointerPosition;
        rectTransform.position += delta;
        prevPointerPosition = currentPointerPosition;
    }

    private IEnumerator SmoothReturnToOrigin(bool shouldItemRestored)
    {
        float elapsedTime = 0f;
        Vector3 currentPos = rectTransform.position;
        while (elapsedTime < transitionDuration)
        {
            rectTransform.position = Vector3.Lerp(currentPos, ReturnLocationAfterDrag, elapsedTime / transitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        rectTransform.position = ReturnLocationAfterDrag;

        if (shouldItemRestored)
        {
            // If the item fails to return back its original position on inventory, add random force
            // towards a random point just outside the edges of inventory
            if (!inventoryOrganization.PlaceItem(gameObject, new List<Vector3> { Vector3.zero }))
            {
                rectTransform.SetParent(canvas.transform);
                ItemStored = false;
                audios[0].Play();
                ReturnLocationAfterDrag = invProximityPoints[Random.Range(0, invProximityPoints.Count)];
                AddForceTowardsPoint(ReturnLocationAfterDrag, 1000);
            }
        }

        img.raycastTarget = true; // Allow dragging again
    }
}