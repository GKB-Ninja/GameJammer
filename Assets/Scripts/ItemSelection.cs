using UnityEngine;
using TMPro;

public class ItemSelection : MonoBehaviour
{
    public int credits = 50;
    public int day = 1;
    public bool debugMode = false; // For us developers to test ItemSelection scene over and over
    public TMP_Text creditsText; // UI Text to display credits
    public TMP_Text dayCount; // UI Text to display day


    void Start()
    {
        // Load the credits and day from the PlayerPrefs or set them to the default values in debug mode

        if (debugMode)
        {
            PlayerPrefs.SetInt("credits", credits);
            PlayerPrefs.SetInt("day", day);
        }
        else
        {
            credits = PlayerPrefs.GetInt("credits");
            day = PlayerPrefs.GetInt("day");
        }

        UpdateUIs();
    }

    public void BuyWeapon()
    {
        if (credits >= 10)
        {
            credits -= 10;
            Debug.Log("You bought a Weapon!");
            UpdateUIs();
            SaveUtility.SaveInt("credits", credits);
            PlayerPrefs.SetInt("weaponItem", 1);
            Debug.Log("Weapon Item: " + PlayerPrefs.GetInt("weaponItem"));
        }
        else
        {
            Debug.Log("Not enough credits!");
        }
    }

    public void BuyArmor()
    {
        if (credits >= 15)
        {
            credits -= 15;
            Debug.Log("You bought Armor!");
            UpdateUIs();
            SaveUtility.SaveInt("credits", credits);
            PlayerPrefs.SetInt("armorItem", 1);
            Debug.Log("Armor Item: " + PlayerPrefs.GetInt("armorItem"));
        }
        else
        {
            Debug.Log("Not enough credits!");
        }
    }

    public void BuyRadar()
    {
        if (credits >= 20)
        {
            credits -= 20;
            Debug.Log("You bought a Radar!");
            UpdateUIs();
            SaveUtility.SaveInt("credits", credits);
            PlayerPrefs.SetInt("radarItem", 1);
            Debug.Log("Radar Item: " + PlayerPrefs.GetInt("radarItem"));
        }
        else
        {
            Debug.Log("Not enough credits!");
        }
    }

    void UpdateUIs()
    {
        creditsText.text = "Credits: " + credits;
        dayCount.text = "Day: " + day;
    }
}