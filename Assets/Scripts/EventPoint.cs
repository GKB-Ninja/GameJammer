using UnityEngine;

public class EventPoint : MonoBehaviour
{
    public EventManager eventManager; // Reference to the EventManager

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Check for left mouse button click
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (GetComponent<Collider2D>().OverlapPoint(mousePosition))
            {
                TriggerRandomEvent();
            }
        }
    }

    void TriggerRandomEvent()
    {
        // Randomly choose an event type
        int eventType = Random.Range(0, 3); // 0: Battle, 1: Reward, 2: Trap

        switch (eventType)
        {
            case 0:
                eventManager.TriggerEvent("You encountered a rogue AI! Prepare for battle!");
                break;
            case 1:
                eventManager.TriggerEvent("You found a stash of credits! +20 Credits!");
                break;
            case 2:
                eventManager.TriggerEvent("You triggered a trap. You took 10 damage.");
                break;
        }
    }
}