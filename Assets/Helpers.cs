using System.Collections.Generic;
using UnityEngine;

public static class Helpers
{
    public static GameObject GetClosestObject(List<GameObject> objects, Vector2 position)
    {
        GameObject closestObject = null;
        float minDist = Mathf.Infinity;
        foreach (var obj in objects)
        {
            float dist = Vector3.Distance(position, obj.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closestObject = obj;
            }
        }

        return closestObject;
    }
} 