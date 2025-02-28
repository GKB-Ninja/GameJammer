using UnityEngine;

public class MoveImage : MonoBehaviour
{
    public float speed = 1f; // Speed at which the image moves
    private float distanceMoved = 0f; // Distance moved by the image
    private bool movingDown = true; // Direction of movement

    void Update()
    {
        // Calculate the movement step
        float step = speed * Time.deltaTime;

        // Move the image and update the distance moved
        if (movingDown)
        {
            transform.Translate(Vector3.down * step);
            distanceMoved += step;
        }
        else
        {
            transform.Translate(Vector3.up * step);
            distanceMoved -= step;
        }

        // Check if the image has moved 460 units and switch direction
        if (Mathf.Abs(distanceMoved) >= 430f)
        {
            movingDown = !movingDown;
            distanceMoved = 0f; // Reset the distance moved
        }
    }
}