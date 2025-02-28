using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSelection : MonoBehaviour
{
    public int credits = 50; // Starting credits
    public TMP_Text creditsText; // UI Text to display credits
    public TMP_Text dayCount;


    void Start()
    {
        // Load credits from PlayerPrefs
        if (PlayerPrefs.GetInt("day") != 1)
        {
            credits = PlayerPrefs.GetInt("credits", credits);
            dayCount.text = "Day: " + PlayerPrefs.GetInt("day");
        }
        else
        {
            dayCount.text = "Day: 1";
        }   
        SaveCredits();
        UpdateCreditsUI();
    }

    public void BuyWeapon()
    {
        if (credits >= 10)
        {
            credits -= 10;
            Debug.Log("You bought a Weapon!");
            UpdateCreditsUI();
            SaveCredits();
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
            UpdateCreditsUI();
            SaveCredits();
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
            UpdateCreditsUI();
            SaveCredits();
        }
        else
        {
            Debug.Log("Not enough credits!");
        }
    }

    void UpdateCreditsUI()
    {
        creditsText.text = "Credits: " + credits;
    }

    void SaveCredits()
    {
        PlayerPrefs.SetInt("credits", credits);
        PlayerPrefs.Save();
    }
}