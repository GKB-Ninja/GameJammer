using UnityEngine;

public static class SaveUtility
{
    public static void SaveInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.Save();
    }

    public static int LoadInt(string key, int defaultValue = 0)
    {
        return PlayerPrefs.GetInt(key, defaultValue);
    }

    public static void SaveString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        PlayerPrefs.Save();
    }

    public static string LoadString(string key, string defaultValue = "")
    {
        return PlayerPrefs.GetString(key, defaultValue);
    }

    public static void SaveFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
        PlayerPrefs.Save();
    }

    public static float LoadFloat(string key, float defaultValue = 0f)
    {
        return PlayerPrefs.GetFloat(key, defaultValue);
    }

    public static void SaveMatrix(string key, int[,] matrix)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        string serializedMatrix = rows + "," + cols + ";";

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                serializedMatrix += matrix[i, j] + ",";
            }
        }

        PlayerPrefs.SetString(key, serializedMatrix.TrimEnd(','));
        PlayerPrefs.Save();
    }

    public static int[,] LoadMatrix(string key)
    {
        string serializedMatrix = PlayerPrefs.GetString(key);
        if (string.IsNullOrEmpty(serializedMatrix))
        {
            Debug.LogError($"No matrix found under key: {key}");
            return null;
        }

        string[] parts = serializedMatrix.Split(';');
        string[] dimensions = parts[0].Split(',');
        int rows = int.Parse(dimensions[0]);
        int cols = int.Parse(dimensions[1]);

        int[,] matrix = new int[rows, cols];
        string[] values = parts[1].Split(',');

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                matrix[i, j] = int.Parse(values[i * cols + j]);
            }
        }
        return matrix;
    }
}