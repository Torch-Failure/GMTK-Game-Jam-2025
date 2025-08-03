using System.Collections.Generic;
using UnityEngine;

public abstract class Activator : MonoBehaviour
{
    [SerializeField] List<Activatable> activates;
    
    protected void Activate()
    {
        foreach (var activatable in activates)
            activatable.Activate();
    }

    protected void Deactivate()
    {
        foreach (var activatable in activates)
            activatable.Deactivate();
    }
}
