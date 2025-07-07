using System.Collections.Generic;
using UnityEngine;

public enum ItemSfx
{
    Grab,
    Drag,
    Drop,
    Intervene,
    ReturnBack
}

// This class is used to manage and retrieve item data in the game.
public static class Items
{
    private static readonly Dictionary<int, Item> database = new Dictionary<int, Item>();

    private class Item
    {
        public GameObject Prefab;
        public SfxItem Sfx;

        public Item(GameObject prefab, SfxItem sfx)
        {
            Prefab = prefab;
            Sfx = sfx;
        }
    }

    static Items()
    {
        database[1] = new Item(
            Resources.Load<GameObject>("Items/Scanners/Radar/Radar"),
            Resources.Load<SfxItem>("Items/Scanners/Radar/Radar Sfx")
        );

        database[5] = new Item(
            Resources.Load<GameObject>("Items/Scanners/Drone/Drone"),
            Resources.Load<SfxItem>("Items/Scanners/Drone/Drone Sfx")
        );

        database[9] = new Item(
            Resources.Load<GameObject>("Items/Equipments/EMP/EMP"),
            Resources.Load<SfxItem>("Items/Equipments/EMP/EMP Sfx")
        );
    }

    public static bool IsItemID(int ID)
    {
        int hash = (ID - 1) % 4;
        int itemID = ID - hash;

        return database.ContainsKey(itemID) ;
    }

    public static int RetrieveID(GameObject go)
    {
        foreach (KeyValuePair<int, Item> kvp in database)
        {
            if (go.name == kvp.Value.Prefab.name || go.name == kvp.Value.Prefab.name + "(Clone)")
            {
                return kvp.Key;
            }
        }

        Debug.LogError($"Couldn't retrieve the ID for the GameObject with name: {go.name}\n" +
            "Doesn't exist in 'Items.database'." +
            "Possibly an item prefab in a scene has a different name than the one in Resources folder.");

        return -1;
    }
    
    public static GameObject RetrievePrefab(int ID, out int rotation)
    {
        int hash = (ID - 1) % 4;
        int itemID = ID - hash;

        if (database.TryGetValue(itemID, out Item item))
        {
            rotation = 90 * hash;
            return item.Prefab;
        }

        Debug.LogError($"Couldn't retrieve the GameObject for the item with ID: {ID}\nDoesn't exist in 'Items.database'.");
        rotation = 0;
        return null;
    }

    public static SfxItem RetrieveSfx(int ID)
    {
        int hash = (ID - 1) % 4;
        int itemID = ID - hash;

        if (database.TryGetValue(itemID, out Item item))
        {
            return item.Sfx;
        }

        Debug.LogError($"Couldn't retrieve the Sfx for the item with ID: {ID}\nDoesn't exist in 'Items.database'.");
        return null;
    }
}