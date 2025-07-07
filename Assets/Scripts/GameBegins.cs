using System.Collections.Generic;
using UnityEngine;

public class GameBegins : MonoBehaviour
{
    public int StarterDay;
    public int StarterHP;
    public int StarterCredits;
    private readonly int[,] Starter_Inventory = new int[31, 31];
    public bool DebugInventory;
    public int DebugInvWidth;
    public int DebugInvHeight;
    public List<Vector3Int> DebugItemRow_Col_ID;

    void Start()
    {
        // If it's the first day, or DebugInventory is enabled initialize all data
        if (PlayerPrefs.GetInt("day", 1) != 1 && !DebugInventory) return;
        PlayerPrefs.SetInt("day", StarterDay);
        PlayerPrefs.SetInt("hp", StarterHP);
        PlayerPrefs.SetInt("credits", StarterCredits);

        // if debugMode is enabled, use DebugInv attributes otherwise use 3x4 inventory matrix
        int matrixWidth = DebugInventory ? DebugInvWidth : 4;
        int matrixHeight = DebugInventory ? DebugInvHeight : 3;
        for (int x = 0; x < matrixHeight; x++)
        {
            for (int y = 0; y < matrixWidth; y++)
            {
                Starter_Inventory[x, y] = -1;
            }
        }

        // For debugging purposes, fill the inventory matrix with items from DebugItemRow_Col_ID
        if (DebugInventory && DebugItemRow_Col_ID != null)
        {
            foreach (var debugItem in DebugItemRow_Col_ID)
            {
                if (debugItem.x < 0 || debugItem.x >= matrixHeight || debugItem.y < 0 || debugItem.y >= matrixWidth)
                {
                    Debug.LogWarning(
                        $"'DebugItemRow_Col_ID' variable row = {debugItem.x} and column = {debugItem.y} values are out of bounds " +
                        $"for the inventory matrix of size row = {matrixHeight} and column = {matrixWidth}.");
                    continue;
                }
                if (!Items.IsItemID(debugItem.z))
                {
                    Debug.LogWarning(
                        $"'DebugItemRow_Col_ID' variable Item ID = {debugItem.z} does not exist in 'Items.database'.");
                    continue;
                }

                Starter_Inventory[debugItem.x, debugItem.y] = debugItem.z;
            }
        }

        SaveUtility.SaveMatrix("inventory", Starter_Inventory);
    }
}