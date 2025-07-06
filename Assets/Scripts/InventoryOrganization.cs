using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

// This script must be attached to the tilemap that represents the inventory.
public class InventoryOrganization : MonoBehaviour
{
    public bool DebugProximityPoints;
    private int[,] inventoryState; // is retrieved from PlayerPrefs under the key "inventory"
    private Tilemap inventoryTilemap;
    private TileBase inventorySlotTile;

    void Awake()
    {
        inventoryTilemap = GetComponent<Tilemap>();
        inventorySlotTile = inventoryTilemap.GetTile(Vector3Int.zero);
    }

    void Start()
    {
        inventoryState = SaveUtility.LoadMatrix("inventory");
    }

    // Checks inventory background tilemap so that if given world position is within the bounds of the inventory + range, returns true.
    public bool IsWithinInvBounds(Vector3 worldPos, int range = 0)
    {
        Tilemap invSurface = transform.parent.GetComponentsInChildren<Tilemap>()[0];
        if (range == 0)
        {
            return invSurface.HasTile(invSurface.WorldToCell(worldPos));
        }

        Vector3Int cellPos = invSurface.WorldToCell(worldPos);
        Vector3Int checkPos = cellPos;
        for (int y = -range; y <= range; y++)
        {
            checkPos.y = cellPos.y + y;
            for (int x = -range; x <= range; x++)
            {
                checkPos.x = cellPos.x + x;
                if (invSurface.HasTile(checkPos))
                {
                    return true;
                }
            }
        }

        return false;
    }

    // Based on given cell range, returns a Vector3 list of world positions of the cells outside the inventory bounds.
    public List<Vector3> GetInvProximityPoints(int range = 1)
    {
        HashSet<Vector3> outerBounds = new HashSet<Vector3>();
        Tilemap invSurface = transform.parent.GetComponentsInChildren<Tilemap>()[0];
        BoundsInt bounds = invSurface.cellBounds;
        if (bounds.size.x == 0 || bounds.size.y == 0)
        {
            return new List<Vector3>();
        }

        int xMin = bounds.xMin;
        int yMin = bounds.yMin;
        int xMax = bounds.xMax - 1;
        int yMax = bounds.yMax - 1;
        Vector3Int currPosition = new Vector3Int(xMin, yMin, bounds.zMin);

        int resetFactorX = xMin;
        int resetFactorY = yMin;

        // Scan bottom/left then top/right edges
        for (int parallelFactor = 1; parallelFactor > -3; parallelFactor -= 2)
        {
            // Horizontal scan
            for (currPosition.y = resetFactorY; currPosition.x <= xMax; currPosition.x++)
            {
                while (!invSurface.HasTile(currPosition))
                {
                    currPosition.y += parallelFactor;
                }
                currPosition.y -= range * parallelFactor;
                outerBounds.Add(invSurface.GetCellCenterWorld(currPosition));
                currPosition.y = resetFactorY;
            }
            currPosition.y = yMin;

            // Vertical scan
            for (currPosition.x = resetFactorX; currPosition.y <= yMax; currPosition.y++)
            {
                while (!invSurface.HasTile(currPosition))
                {
                    currPosition.x += parallelFactor;
                }
                currPosition.x -= range * parallelFactor;
                outerBounds.Add(invSurface.GetCellCenterWorld(currPosition));
                currPosition.x = resetFactorX;
            }

            // Before calculating top/right edges, move iteration factors' side to their parallel sides
            resetFactorX = xMax;
            resetFactorY = yMax;
        }

        if (DebugProximityPoints)
        {
            DisplayProximityDebugSquares(outerBounds.ToList());
        }

        return outerBounds.ToList();
    }

    // Check if all colliders of the itemToCheck are touching any inventory slots
    public bool AreAllChildrenTouching(GameObject itemToCheck)
    {
        foreach (Collider2D collider2D in itemToCheck.GetComponentsInChildren<Collider2D>())
        {
            if (!collider2D.IsTouching(GetComponent<TilemapCollider2D>()))
            {
                return false;
            }
        }
        return true;
    }

    // Place the item into the inventory based on its child's center position.
    public bool PlaceItem(GameObject item, List<Vector3> customTargetSlots = null, Vector2 customSearchRange = new Vector2())
    {
        if (AreAllChildrenTouching(item))
        {
            Vector3 itemsWorldPosition = item.transform.Find("center").transform.position;
            Vector3 snapLocation = inventoryTilemap.GetCellCenterWorld(inventoryTilemap.WorldToCell(itemsWorldPosition));

            return SearchAndSnapRecursive(item, snapLocation, customTargetSlots, customSearchRange);
        }
        else
        {
            return false;
        }
    }

    public void RemoveItem(GameObject item)
    {
        // Update the inventoryState matrix to remove the item
        Vector3Int itemCenterToCellPos = inventoryTilemap.WorldToCell(item.transform.Find("center").position);
        if (inventoryState[-itemCenterToCellPos.y, itemCenterToCellPos.x] == Items.RetrieveID(item))
        {
            inventoryState[-itemCenterToCellPos.y, itemCenterToCellPos.x] = -1;
            if (item.TryGetComponent(out ItemDrag itemAdjustments))
            {
                // Let OnEndDrag() handle 'ItemStored' if it's being removed by dragging
                itemAdjustments.ItemStored = itemAdjustments.IsDragged;
            }
            else
            {
                Debug.LogError($"Item {item.name} does not have an ItemDrag component. All items must have one.");
                return;
            }

            Vector3Int cellPosToRecover = Vector3Int.zero;
            foreach (Transform child in item.GetComponentsInChildren<Transform>().Where(t => t != item.transform))
            {
                cellPosToRecover = inventoryTilemap.WorldToCell(child.position);

                if (!inventoryTilemap.HasTile(cellPosToRecover))
                {
                    inventoryTilemap.SetTile(cellPosToRecover, inventorySlotTile);
                }
                else
                {
                    Debug.LogWarning($"Cannot remove {item.name}'s item tile '{child.name}' from inventory tilemap position " +
                        $"{cellPosToRecover} because it wasn't placed on the inventory in the first place.");
                }
            }
        }

        else
        {
            Debug.LogWarning($"Cannot remove {item.name} from inventory because 'inventoryState' did not match up.");
        }
    }

    /* Check if the item can fit into slots by imagining as if the item's center is already snapped at targetSlotCenterWorldPos.
     * If so, snap the item, mark occupied slots and return true.
     * If not, scan for potential spots (recursive) within the range of snapRange and return false if all fails. 
     * If no range is given, the range is the item's center collider bounds extents. (item's hitbox)
     * If specific potential snap targets are given, snapRange is ignored.
     */
    private bool SearchAndSnapRecursive(GameObject itemToSnap, Vector3 targetSlotCenterWorldPos, List<Vector3> potSnapTargets = null, Vector2 snapRange = new Vector2())
    {
        Transform iTransform = itemToSnap.transform;
        Vector3 iCenterWorldPos = iTransform.Find("center").position;
        Vector3 diff = targetSlotCenterWorldPos - iCenterWorldPos;
        Vector3[] iChildrensSnappedWorldPos = new Vector3[iTransform.childCount];

        int wronglySnappedItemTiles = 0;
        for (int i = 0; i < iTransform.childCount; i++)
        {
            iChildrensSnappedWorldPos[i] = iTransform.GetChild(i).position + diff;
            Vector3Int cellPositionToCheck = inventoryTilemap.WorldToCell(iChildrensSnappedWorldPos[i]);

            if (!inventoryTilemap.HasTile(cellPositionToCheck))
            {
                wronglySnappedItemTiles++;
                break;
            }
        }

        // If all children are snapped correctly; update both ReturnLocationAfterDrag and inventoryState, remove newly occupied tiles and return true
        if (wronglySnappedItemTiles == 0)
        {
            foreach (Vector3 cellPositionToBeFilled in iChildrensSnappedWorldPos)
            {
                Vector3Int cellToBeFilled = inventoryTilemap.WorldToCell(cellPositionToBeFilled);
                inventoryTilemap.SetTile(cellToBeFilled, null);
            }
            Vector3Int occMainTile = inventoryTilemap.WorldToCell(iCenterWorldPos + diff);
            ItemDrag itemAdjustments = itemToSnap.GetComponent<ItemDrag>();

            inventoryState[-occMainTile.y, occMainTile.x] = Items.RetrieveID(itemToSnap);
            itemToSnap.transform.SetParent(inventoryTilemap.transform);
            itemAdjustments.ReturnLocationAfterDrag = targetSlotCenterWorldPos;
            itemAdjustments.ItemStored = true;
            return true;
        }

        // If the item cannot fit into the target slot, check vicinity. Or return false if exact snap was requested.
        else
        {
            if (snapRange == Vector2.zero)
            {
                if (itemToSnap.transform.Find("center").TryGetComponent<Collider2D>(out var itemCollider))
                {
                    snapRange = itemCollider.bounds.extents;
                    if (2 * snapRange.x % 128 != 0)
                    {
                        Debug.LogWarning($"Item {itemToSnap.name}'s children Collider2Ds should be in sizes of 128 * k " +
                            $"\nRight now, it is {2 * snapRange.x}");
                    }
                }
                else
                {
                    Debug.LogError($"Item {itemToSnap.name}'s 'center' tile child does not have Collider2D component. " +
                        "Please ensure all items' children have Collider2Ds.");
                    return false;
                }
            }

            if (potSnapTargets == null || !potSnapTargets.Any())
            {
                potSnapTargets = new List<Vector3>();
                for (float x = -snapRange.x; x <= snapRange.x + 1; x += inventoryTilemap.cellSize.x) // + 1s for float raunding problems
                {
                    for (float y = -snapRange.y; y <= snapRange.y + 1; y += inventoryTilemap.cellSize.y)
                    {
                        Vector3 newSnapTarget = targetSlotCenterWorldPos + new Vector3(x, y, 0);
                        if (!potSnapTargets.Contains(newSnapTarget))
                        {
                            potSnapTargets.Add(newSnapTarget);
                        }
                    }
                }
                potSnapTargets = potSnapTargets.OrderBy(pos => Vector3.Distance(pos, targetSlotCenterWorldPos)).ToList();
            }

            potSnapTargets.RemoveAt(0);
            if (potSnapTargets.Count != 0)
            {
                return SearchAndSnapRecursive(itemToSnap, potSnapTargets[0], potSnapTargets, snapRange);
            }
            return false;
        }
    }

    // for debugging, put white square UI images on the found outer bounds
    private void DisplayProximityDebugSquares(List<Vector3> outerBounds)
    {
        foreach (Vector3 outerBound in outerBounds)
        {
            GameObject debugSquare = new GameObject("DebugSquare");
            debugSquare.transform.parent = GameObject.Find("Canvas").transform;
            debugSquare.transform.position = outerBound;
            RectTransform rectTransform = debugSquare.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(64f, 64f);
            Image image = debugSquare.AddComponent<Image>();
            image.color = new Color(1, 1, 1, 0.5f);
        }
    }
}