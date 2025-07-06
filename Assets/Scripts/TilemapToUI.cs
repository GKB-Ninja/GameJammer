using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class TilemapToUI : MonoBehaviour
{
    public Tilemap Tilemap;
    public GameObject Canvas;
    public GameObject ImagePrefab; // Prefab for the UI Image, which should have an empty Image component and "InvUI" tag
    private HashSet<Vector3Int> tilePositions = new HashSet<Vector3Int>();

    void Start()
    {
        // Subscribe to the tilemapTileChanged event
        Tilemap.tilemapTileChanged += OnTileChanged;
    }

    void OnDestroy()
    {
        // Unsubscribe from the tilemapTileChanged event to prevent memory leaks after destruction
        Tilemap.tilemapTileChanged -= OnTileChanged;
    }

    // Create a UI Image version of given tile's current sprite
    public void CreateUIImage(Vector3Int tilePosition)
    {
        Sprite sprite = Tilemap.GetSprite(tilePosition);
        if (sprite == null) return;

        Vector3 spriteSize = GetTileSpriteWorldSize(tilePosition);
        GameObject tileUI = Instantiate(ImagePrefab, Canvas.transform);
        RectTransform rectTransform = tileUI.GetComponent<RectTransform>();

        tileUI.GetComponent<Image>().sprite = sprite;
        rectTransform.sizeDelta = spriteSize;
        rectTransform.position = GetRealCellToWorld(tilePosition);
        tileUI.transform.SetParent(Tilemap.transform, true);

        tilePositions.Add(tilePosition);
    }

    public void RemoveUIImage(Vector3Int tilePosition)
    {
        if (!tilePositions.Contains(tilePosition))
        {
            Debug.Log($"No UI Image found for tile position {tilePosition}. Cannot remove.");
            return;
        }

        foreach (GameObject child in GameObject.FindGameObjectsWithTag("InvUI"))
        {
            if (child.transform.position == GetRealCellToWorld(tilePosition) && child.transform.parent == Tilemap.transform)
            {
                Destroy(child);
                tilePositions.Remove(tilePosition);
                return;
            }
        }
    }

    // This method is called whenever a tile is changed in any Tilemap
    private void OnTileChanged(Tilemap tilemap, Tilemap.SyncTile[] syncTiles)
    {
        if (tilemap != Tilemap) return;

        foreach (Tilemap.SyncTile syncTile in syncTiles)
        {
            Vector3Int changedTilePos = syncTile.position;
            if (!tilePositions.Contains(changedTilePos) && syncTile.tile != null)
            {
                tilePositions.Add(changedTilePos);
                // Create a new UI Image after the current frame has ended
                // Because Auto Tile Orientation (Merge) Rules haven't applied yet
                StartCoroutine(CreateUIImageAfterFrame(changedTilePos));
            }
            else if (tilePositions.Contains(changedTilePos) && syncTile.tile == null)
            {
                // If the tile is already in the UI but now it's null, remove the UI Image
                RemoveUIImage(changedTilePos);
            }
        }
    }

    // Clone the tile as a UI Image after the current frame has ended
    private IEnumerator CreateUIImageAfterFrame(Vector3Int tilePosition)
    {
        yield return new WaitForEndOfFrame();
        CreateUIImage(tilePosition);
    }

    private Vector3 GetTileSpriteWorldSize(Vector3Int tilePosition)
    {
        float tileSpriteWorldWidth = Tilemap.cellSize.x * Tilemap.transform.localScale.x;
        float tileSpriteWorldHeight = Tilemap.cellSize.y * Tilemap.transform.localScale.y;
        float tileSpriteWorldDepth = Tilemap.cellSize.z * Tilemap.transform.localScale.z;
        return new Vector3(tileSpriteWorldWidth, tileSpriteWorldHeight, tileSpriteWorldDepth);
    }

    // Get the world position of the cell, taking into account the tile anchor and orientation which CellToWorld doesn't
    private Vector3 GetRealCellToWorld(Vector3Int cellPosition)
    {
        Vector3 spriteSize = GetTileSpriteWorldSize(cellPosition);
        Vector3 worldPos = Tilemap.CellToWorld(cellPosition);
        Vector3 anchor = Tilemap.tileAnchor;
        Matrix4x4 orientation = Tilemap.orientationMatrix;
        // Calculate the final position because CellToWorld does not take the tile anchor into account
        return new Vector3(
            anchor.x * spriteSize.x + worldPos.x + orientation.m03, 
            anchor.y * spriteSize.y + worldPos.y + orientation.m13,
            anchor.z * spriteSize.z + worldPos.z + orientation.m23
        );
    }
}