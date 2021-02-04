using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public Transform graphNodesWrapper;
    private List<PathfindNode> graph;

    void Start()
    {
        graph = new List<PathfindNode>();
        GameObject[] graphNodes = GameObject.FindGameObjectsWithTag("GraphNode");

        foreach (Transform child in graphNodesWrapper)
        {
            if (child.tag == "GraphNode")
            {
                graph.Add(child.GetComponent<PathfindNode>());
            }
        }

        Transform playerTransform = GameObject.FindGameObjectsWithTag("Player")[0].transform;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            foreach (Transform child in transform)
            {
                if (child.tag == "Patrols")
                {
                    child.GetComponent<EnemyAI>().TransitionToPatrolState();
                }
            }
        }
    }

    public List<PathfindNode> GetGraphNodes()
    {
        return graph;
    }

    // On Scene Start, Load the graph
    public List<PathfindNode> FindPath(PathfindNode origin, PathfindNode dest)
    {
        List<PathfindNode> path = new List<PathfindNode>();

        PriorityQueue<PathfindPriorityQueueItem> priority = 
            new PriorityQueue<PathfindPriorityQueueItem>();
        List<PathfindPriorityQueueItem> unseen = new List<PathfindPriorityQueueItem>();
        List<PathfindPriorityQueueItem> seen = new List<PathfindPriorityQueueItem>();

        InitializeUnseenList(unseen, origin, dest, priority);

        PathfindPriorityQueueItem item;
        while (priority.Count() > 0 && (item = priority.Dequeue()).node != null)
        {
            seen.Add(item);
            unseen.Remove(item);
            foreach (PathfindNode adj in item.node.nodes)
            {
                PathfindPriorityQueueItem enqueuedItem = FindInList(adj, unseen);
                float segmentDistance = item.node.edges[adj].distance;
                if (enqueuedItem != null)
                {
                    if (segmentDistance >= 0)
                    {
                        if (priority.Contains(enqueuedItem))
                        {
                            float pathWeight = segmentDistance + item.pathDistanceFromOrigin;
                            if (pathWeight < enqueuedItem.pathDistanceFromOrigin)
                            {
                                enqueuedItem.pathDistanceFromOrigin = pathWeight;
                                enqueuedItem.prev = item.node;
                                priority.Sort();
                            }
                        }
                        else
                        {
                            enqueuedItem.prev = item.node;
                            enqueuedItem.pathDistanceFromOrigin = segmentDistance + item.pathDistanceFromOrigin;
                            priority.Enqueue(enqueuedItem);
                        }
                    }
                }
            }

            if (item.node == dest)
            {
                PathfindPriorityQueueItem current = item;
                while (current != null)
                {
                    path.Add(current.node);
                    current = FindInList(current.prev, seen);
                }
                break;
            }
        }
        path.Reverse();
        return path;
    }

    private PathfindPriorityQueueItem FindInList(PathfindNode n, List<PathfindPriorityQueueItem> list)
    {
        if (n == null)
        {
            return null;
        }
        return list.Find(c => (c.node == n));
    }

    private void InitializeUnseenList(List<PathfindPriorityQueueItem> unseen, 
        PathfindNode origin, 
        PathfindNode dest,
        PriorityQueue<PathfindPriorityQueueItem> priority)
    {
        foreach (PathfindNode node in graph)
        {
            PathfindPriorityQueueItem item = new PathfindPriorityQueueItem();
            item.node = node;
            item.prev = null;
            float dist = Vector2.Distance(node.transform.position, dest.transform.position);
            item.distanceToDest = dist;
            item.pathDistanceFromOrigin = 99999999;                 // basically infinity

            if (node == origin)
            {
                item.pathDistanceFromOrigin = 0;
                priority.Enqueue(item);
            }
            unseen.Add(item);
        }
    }
}
