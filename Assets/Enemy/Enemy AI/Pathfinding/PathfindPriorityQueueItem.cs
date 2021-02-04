using System;
using UnityEngine;

public class PathfindPriorityQueueItem : IComparable<object>
{
    public PathfindNode node;
    public PathfindNode prev;
    public float distanceToDest;
    public float pathDistanceFromOrigin;

    public int CompareTo(object obj)
    {
        if (obj == null) return 1;

        PathfindPriorityQueueItem other = obj as PathfindPriorityQueueItem;
        if (other != null)
        {
            float thisHeuristic = distanceToDest + pathDistanceFromOrigin;
            float otherHeuristic = other.distanceToDest + other.pathDistanceFromOrigin;
            return thisHeuristic.CompareTo(otherHeuristic);
        }
        else
        {
            throw new ArgumentException("Object is not a PathfindPriorityQueueItem");
        }
    }

}
