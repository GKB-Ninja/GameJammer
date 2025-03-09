using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class TilemapToUI : MonoBehaviour
{
    public Tilemap tilemap; // Reference to the Tilemap
    private GameObject canvas; // Reference to the Canvas
    public GameObject imagePrefab; // Prefab for the UI Image

    void Start()
    {
        canvas = tilemap.transform.parent.parent.gameObject;

        ConvertTilesToUI();
    }

    // Iterate through all the tiles in the Tilemap and create a UI Image for each one
    void ConvertTilesToUI()
    {
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null)
                {
                    Vector3Int tilePosition = new Vector3Int(x + bounds.xMin, y + bounds.yMin, 0);
                    Sprite tileSprite = tilemap.GetSprite(tilePosition);
                    
                    if (tileSprite != null)
                    {
                        float tileWidth = (tileSprite.rect.width / tileSprite.pixelsPerUnit) * tilemap.transform.localScale.x;
                        float tileHeight = (tileSprite.rect.height / tileSprite.pixelsPerUnit) * tilemap.transform.localScale.y;
                        Vector2 TileSize = new Vector2(tileWidth, tileHeight);
                        CreateUIImage(tileSprite, tilePosition, TileSize);
                    }
                }
            }
        }
    }

    void CreateUIImage(Sprite sprite, Vector3Int tilePosition, Vector2 TileSize)
    {
        // Instantiate a new UI Image
        GameObject newImage = Instantiate(imagePrefab, canvas.transform);
        Image imageComponent = newImage.GetComponent<Image>();
        imageComponent.sprite = sprite;
        newImage.GetComponent<RectTransform>().sizeDelta = TileSize;

        // Set the initial position of the UI Image
        RectTransform rectTransform = newImage.GetComponent<RectTransform>();
        rectTransform.position = new Vector3(tilemap.tileAnchor.x * TileSize.x + tilemap.CellToWorld(tilePosition).x, tilemap.tileAnchor.y * TileSize.y + tilemap.CellToWorld(tilePosition).y, tilemap.CellToWorld(tilePosition).z);

        // Make the UI Image a child of the corresponding tile
        newImage.transform.SetParent(tilemap.transform, true);
    }
}
