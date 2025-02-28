using UnityEngine;

public class DayInitializer : MonoBehaviour
{
    void Start()
    {
        PlayerPrefs.SetInt("day", 1);
        PlayerPrefs.SetInt("hp", 100);
        PlayerPrefs.Save();
    }
}