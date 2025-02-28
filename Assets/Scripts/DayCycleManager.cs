using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DayCycleManager : MonoBehaviour
{
    public Button skipButton;
    public TMP_Text dayText;
    public Image screenOverlay;
    public EventManager eventManager;
    public PlayerStats playerStats;

    private int currentDay = 0;
    private Color overlayColor;

    void Start()
    {
        currentDay = PlayerPrefs.GetInt("day");
        overlayColor = screenOverlay.color;
        overlayColor.a = 0;
        screenOverlay.color = overlayColor;
        dayText.text = "";
    }

    public void SkipDay()
    {
        currentDay = currentDay + 1;
        PlayerPrefs.SetInt("day", currentDay);
        PlayerPrefs.Save();
        StartCoroutine(DayTransition());
    }

    private IEnumerator DayTransition()
    {
        // Darken the screen
        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            overlayColor.a = Mathf.Lerp(0, 1, t);
            screenOverlay.color = overlayColor;
            yield return null;
        }

        // Display "Day X" text
        dayText.text = "Day " + currentDay;
        yield return new WaitForSeconds(5);

        // Lighten the screen
        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            overlayColor.a = Mathf.Lerp(1, 0, t);
            screenOverlay.color = overlayColor;
            yield return null;
        }

        dayText.text = "";

        // Reset the game state (except credits, health, and combat text log)
        ResetGameState();
    }

    private void ResetGameState()
    {
        // Implement your logic to reset the game state here
        // For example, reset positions, clear enemies, etc.
        // Keep credits, health, and combat text log as they are
    }
}