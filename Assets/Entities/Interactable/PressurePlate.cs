using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : Activator
{
    [SerializeField] private bool onlyPlayerCollidersTrigger = true;
    private float minTriggerMass = 10;
    
    private List<GameObject> currentTriggerers = new();
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (onlyPlayerCollidersTrigger && !other.CompareTag("Player"))
            return;

        Rigidbody2D rb = other.attachedRigidbody;
        if (rb == null || rb.mass < minTriggerMass) { // Don't really even need to check the mass. This is just to ignore projectiles.
            return;
        }

        Activate();
        currentTriggerers.Add(other.gameObject);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        currentTriggerers.Remove(other.gameObject);
        if (currentTriggerers.Count == 0)
        {
            Deactivate();
        }
    }
}
