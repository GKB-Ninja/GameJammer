using UnityEngine;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    public int health = 100;
    public int credits = 50;

    public TMP_Text healthText;
    public TMP_Text creditsText;
    public EventManager eventManager; // Reference to the EventManager

    void Start()
    {
        credits = PlayerPrefs.GetInt("credits");
        health = PlayerPrefs.GetInt("hp");
        UpdateUI();
    }

    public void UpdateUI()
    {
        healthText.text = "Health: " + health;
        creditsText.text = "Credits: " + credits;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        PlayerPrefs.SetInt("hp", health);
        PlayerPrefs.Save();
        UpdateUI();

        // Check if health is 0 or below
        if (health <= 0)
        {
            health = 0; // Ensure health doesn't go below 0
            UpdateUI();
            eventManager.GameOver(); // Trigger game over directly
        }
    }

    public void AddCredits(int amount)
    {
        credits += amount;
        PlayerPrefs.SetInt("credits", credits);
        PlayerPrefs.Save();
        UpdateUI();
    }
}