using UnityEngine;

public abstract class Entity : MonoBehaviour {
    // SaveState is called to save the state of an entity at the start of a loop.
    public abstract void SaveState();
    // LoadState is called to load the state (when resetting the loop).
    public abstract void LoadState();
}