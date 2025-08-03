using System.Collections.Generic;
using UnityEngine;

public class Door : Activatable
{
    public override void Activate() {
        gameObject.SetActive(false);
    }
    public override void Deactivate() {
        gameObject.SetActive(true);
    }
}
