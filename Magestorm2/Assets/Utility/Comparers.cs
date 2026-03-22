using System.Collections.Generic;

using UnityEngine;

public class GameObjectDistanceComparer : IComparer<GameObject>
{
    public GameObjectDistanceComparer(Vector3 referencePosition)
    {
        ReferencePosition = referencePosition;
    }

    public Vector3 ReferencePosition
    {
        get; set;
    }
    public int Compare(GameObject x, GameObject y)
    {
        float distanceX = Vector3.Distance(ReferencePosition, x.transform.position);
        float distanceY = Vector3.Distance(ReferencePosition, y.transform.position);
        return distanceX.CompareTo(distanceY);
    }
}

