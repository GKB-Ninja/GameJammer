using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EventManager : MonoBehaviour
{
    public TMP_Text textLog; // Reference to the TextMeshPro text log
    public PlayerStats playerStats; // Reference to the PlayerStats
    public GameObject eventPointPrefab; // Reference to the EventPoint prefab
    public int numberOfEventPoints = 3; // Number of event points to spawn
    public TMP_Text gameOverText; // Reference to the Game Over text
    public GameObject blackBackground; // Reference to the black background


    void Start()
    {
        textLog.text = "Event Log:\n";  // Initialize the text log
        SpawnEventPoints();
    }

    public void TriggerEvent(string eventDescription)
    {
        // Add the event description to the text log
        textLog.text += eventDescription + "\n";

        // Update player stats based on what event text contains on its description
        if (eventDescription.Contains("You encountered a rogue AI! Prepare for battle!"))
        {
            playerStats.TakeDamage(10); // Example: Lose 10 health in a battle
        }
        else if (eventDescription.Contains("You found a stash of credits! +20 Credits!"))
        {
            playerStats.AddCredits(20); // Example: Gain 20 credits
        }
        else if (eventDescription.Contains("You triggered a trap. You took 10 damage."))
        {
            playerStats.TakeDamage(10); // Example: Lose 10 health from a trap
        }

        // Check if the player's health is 0 or below
        if (playerStats.health <= 0)
        {
            GameOver(); // Trigger game over
        }
    }

    public void GameOver()
{
    // Show the Game Over text and black background
    gameOverText.gameObject.SetActive(true);
    blackBackground.SetActive(true);

    // Disable other UI elements (e.g., text log, event points)
    textLog.gameObject.SetActive(false);
    foreach (Transform child in transform)
    {
        child.gameObject.SetActive(false); // Disable all event points
    }
}

    public static void RestartGame()
    {
        SceneManager.LoadScene("MainMenu"); // Restart the game
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); // Return to the main menu
    }

    private void SpawnEventPoints()
    {
        Camera mainCamera = Camera.main; // Get the main camera
        float cameraHeight = 2f * mainCamera.orthographicSize; // Calculate the height of the camera view
        float cameraWidth = cameraHeight * mainCamera.aspect; // Calculate the width based on the aspect ratio

        Debug.Log($"Camera Width: {cameraWidth}, Camera Height: {cameraHeight}"); // Debug log

        for (int i = 0; i < numberOfEventPoints; i++)
        {
            Vector2 randomPosition = new Vector2(Random.Range(-cameraWidth / 2, cameraWidth / 2), Random.Range(-cameraHeight / 2, cameraHeight / 2));
            Debug.Log($"Spawning Event Point at: {randomPosition}"); // Debug log
            GameObject eventPoint = Instantiate(eventPointPrefab, randomPosition, Quaternion.identity);
            eventPoint.GetComponent<EventPoint>().eventManager = this; // Set the EventManager reference
        }
    }
}