using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneChanger : MonoBehaviour
{
    public string SceneToLoad; // Variable to hold the scene name to load
    public float delay = 0f; // Variable to hold the delay duration

    // Method to change the scene with a delay
    public void ChangeScene()
    {
        if (!string.IsNullOrEmpty(SceneToLoad))
        {
            StartCoroutine(ChangeSceneWithDelay());
        }
        else
        {
            Debug.LogError("Scene to load is not set.");
        }
    }

    private IEnumerator ChangeSceneWithDelay()
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneToLoad);
    }
}