using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

// Load the inventory from PlayerPrefs
public class LoadInventory : MonoBehaviour
{
    public GameObject inventory;
    public Tilemap biggerTilemap;
    public Tilemap slotsTilemap;
    public Vector3 adjustBiggerTilemapPosition;
    public float biggerTilemapVerticalAbsVal;
    public float biggerTilemapHorizontalAbsVal;
    public static int[,] inventoryMatrix;
    private readonly List<Vector3Int> itemsToLoad = new List<Vector3Int>();

    /* In inventory matrix;
     * -2: occupied inventory slot (although currently not implemented to any code)
     * -1: empty inventory slot
     * 1: Radar's center (upright), 2: Radar's center (turned 90 to right)...
     * 5: Drone's center (upright)
     * 9: EMP's center (upright)
     * ... more at Items.cs
     */

    void Start()
    {
        inventoryMatrix = SaveUtility.LoadMatrix("inventory");
        LoadInventorySlots();
    }

    private void LoadInventorySlots()
    {
        if (inventoryMatrix == null)
        {
            return;
        }

        // Iterate through each child Tilemap of the inventory GameObject
        foreach (Tilemap childTilemap in inventory.GetComponentsInChildren<Tilemap>())
        {
            TileBase thatTilemapsTile = childTilemap.GetTile(new Vector3Int(0, 0, 0));

            childTilemap.ClearAllTiles();

            for (int x = 0; x < inventoryMatrix.GetLength(0); x++)
            {
                for (int y = 0; y < inventoryMatrix.GetLength(1); y++)
                {
                    // Ignore 0s and build inventory based on tileIDs
                    int tileID = inventoryMatrix[x, y];
                    if (tileID == 0) continue;
                    Vector3Int cellPosition = new Vector3Int(y, -x, 0);

                    childTilemap.SetTile(cellPosition, thatTilemapsTile);

                    if (childTilemap == slotsTilemap && tileID > 0)
                    {
                        itemsToLoad.Add(cellPosition);
                    }
                }
            }

            if (childTilemap == slotsTilemap)
            {
                foreach (Vector3Int itemLoc in itemsToLoad)
                {
                    int itemID = inventoryMatrix[-itemLoc.y, itemLoc.x];
                    GameObject item = Instantiate(
                        Items.RetrievePrefab(itemID, out int rotation),
                        childTilemap.GetCellCenterWorld(itemLoc),
                        Quaternion.Euler(0, 0, rotation),
                        childTilemap.transform
                    );
                    List<Vector3> singleSpot = new List<Vector3> { Vector3.zero }; // placeholder so PlaceItem only checks the
                                                                                   // exact cell and not other potential spots
                    StartCoroutine(DelayedPlaceItem(item, singleSpot, itemID, itemLoc));
                }
            }

            // Reset the local position of the childTilemaps into the center of the inventory prefab
            childTilemap.transform.localPosition -= childTilemap.localBounds.center;
            // Scale biggerTilemap as if it is a background frame around the slotsTilemap
            if (childTilemap == biggerTilemap)
            {
                childTilemap.transform.localScale = new Vector3(
                    biggerTilemapHorizontalAbsVal / childTilemap.cellBounds.size.x + 1,
                    biggerTilemapVerticalAbsVal / childTilemap.cellBounds.size.y + 1,
                    1
                );
                childTilemap.transform.localPosition += adjustBiggerTilemapPosition;
            }
        }
    }

    private System.Collections.IEnumerator DelayedPlaceItem(GameObject item, List<Vector3> a, int itemID, Vector3Int itemLoc)
    {
        // Wait for the end of the current frame to ensure physics/colliders are initialized
        yield return new WaitForFixedUpdate();
        if (!gameObject.transform.GetComponentInChildren<InventoryOrganization>().PlaceItem(item, a))
        {
            Debug.LogError($"Couldn't place the item with ID: {itemID} at tilemap position {itemLoc}.\n" +
                "The item doesn't fit in the inventory during loading phase.");
        }
        itemsToLoad.Remove(itemLoc);
        if (itemsToLoad.Count == 0)
        {
            ItemDrag.SetInventoryProximityPoints();
            Destroy(this);
        }
    }
}