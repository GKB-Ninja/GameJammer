using System.Collections.Generic;
using UnityEngine;

// This class is used to manage and retrieve item data in the game.
public static class Items
{
    private static readonly Dictionary<int, Item> database = new Dictionary<int, Item>();

    private class Item
    {
        public GameObject Prefab;

        public Item(GameObject prefab)
        {
            Prefab = prefab;
        }
        //public int[,] ItemTileMatris
        //{
        //    get
        //    {

        //        int matrixSize = Prefab.transform.childCount;
        //        int[,] orient = new int[matrixSize, matrixSize];
        //        float tileSize = Prefab.GetComponent<Grid>().cellSize.x;
        //        float tileGap = Prefab.GetComponent<Grid>().cellGap.x;
        //        foreach (Transform child in Prefab.transform)
        //        {
        //            Vector3 childPosition = child.localPosition;

        //        }
        //        return orient;
        //    }
        //}
    }

    static Items()
    {
        database[1] = new Item(Resources.Load<GameObject>("Items/Scanners/Radar"));
        database[5] = new Item(Resources.Load<GameObject>("Items/Scanners/Drone"));
        database[9] = new Item(Resources.Load<GameObject>("Items/Equipments/EMP"));
    }

    public static bool IsItemID(int ID)
    {
        int hash = (ID - 1) % 4;
        int itemID = ID - hash;
        return database.ContainsKey(itemID) ;
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

        Debug.LogError($"Couldn't retrieve the item with ID: {ID}\nDoesn't exist in 'Items.database'.");
        rotation = 0;
        return null;
    }

    public static int RetrieveID(string name)
    {
        foreach (KeyValuePair<int, Item> kvp in database)
        {
            if (name == kvp.Value.Prefab.name || name == kvp.Value.Prefab.name + "(Clone)")
            {
                return kvp.Key;
            }
        }

        Debug.LogError($"Couldn't retrieve the ID for the item with name: {name}\n" +
            "Doesn't exist in 'Items.database'." +
            "Possibly an item prefab in a scene has a different name than the one in Resources folder.");

        return -1;
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
}