using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PathfindNode : MonoBehaviour
{
    public List<PathfindNode> nodes;
    public Dictionary<PathfindNode, PathfindEdge> edges;

    void Start()
    {
        edges = new Dictionary<PathfindNode, PathfindEdge>();
        for (int i = 0; i < nodes.Count; i++)
        {
            PathfindNode node = nodes[i];
            float dist = Vector2.Distance(transform.position, node.transform.position);

            PathfindEdge edge = new PathfindEdge(dist);
            edges.Add(node, edge);
        }
    }


    void OnDrawGizmosSelected()
    {
        if (nodes != null)
        {
            foreach (PathfindNode node in nodes)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, node.transform.position);
            }
        }
    }
}
