using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System;

public class EnemyAI : MonoBehaviour
{
    // Patrol State Variables
    public Transform starting;
    public float patrolFlipTime;
    public VisionCone visionCone;
    private float timeSinceFlip;

    // Chase State Variables
    public float chaseSpeed;
    public float nodeRadiusThreshold;
    public Rigidbody2D playerRB;
    private Transform nextWayPoint;
    private int indexInPath;
    private List<PathfindNode> path;
    private PathfindNode oldDestination;
    public float maxDistance;

    // Other
    public LayerMask groundLayers;
    public Transform PlayerTransform;
    public Pathfinder pathfinder;
    private Rigidbody2D rb;
    private bool facingRight;
    private Animator anim;                   // state: Patrol --> 0, Gain Alert --> 1, Alert --> 2


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        facingRight = false;
        anim = GetComponent<Animator>();
        path = new List<PathfindNode>();
        indexInPath = 0;
        nextWayPoint = PlayerTransform;
        oldDestination = null;
        timeSinceFlip = 0;
    }

    public void PatrolState()
    {
        if (timeSinceFlip < patrolFlipTime)
        {
            timeSinceFlip += Time.deltaTime;
        }
        else
        {
            Flip();
            timeSinceFlip = 0;
        }
        if (visionCone.InVisionCone())
        {
            anim.SetBool("Chase", true);
        }
    }

    public void ChaseState()
    {
        if (Vector2.Distance(transform.position, PlayerTransform.transform.position) > maxDistance)
        {
            anim.SetBool("Chase", false);
        }

        PathfindNode dest = FindNearestGraphNode(PlayerTransform);
        PathfindNode origin = FindNearestGraphNode(this.transform);

        if (dest != null && origin != null && dest != oldDestination)
        {
            // Run A star algorithm and skip any unessary path nodes
            path = pathfinder.FindPath(origin, dest);

            // Skip the first one if unnecessary
            if (path.Count > 1)
            {
                int horizontalDirection = 0;
                if (path[1].transform.position.x - path[0].transform.position.x > nodeRadiusThreshold)
                {
                    horizontalDirection = 1;
                }
                else if (path[1].transform.position.x - path[0].transform.position.x < -nodeRadiusThreshold)
                {
                    horizontalDirection = -1;
                }

                int verticalDirection = 0;
                if (path[1].transform.position.y - path[0].transform.position.y > nodeRadiusThreshold)
                {
                    verticalDirection = 1;
                }
                else if (path[1].transform.position.y - path[0].transform.position.y < -nodeRadiusThreshold)
                {
                    verticalDirection = -1;
                }

                int enemyToRight = 0;
                if (transform.position.x - path[0].transform.position.x > nodeRadiusThreshold)
                {
                    enemyToRight = 1;
                }
                else if (transform.position.x - path[0].transform.position.x < -nodeRadiusThreshold)
                {
                    enemyToRight = -1;
                }

                int enemyAbove = 0;
                if (transform.position.y - path[0].transform.position.y > nodeRadiusThreshold)
                {
                    enemyAbove = 1;
                }
                else if (transform.position.y - path[0].transform.position.y < -nodeRadiusThreshold)
                {
                    enemyAbove = -1;
                }

                if ((horizontalDirection == enemyToRight || horizontalDirection == 0) && (verticalDirection == enemyAbove || verticalDirection == 0))
                {
                    indexInPath++;
                }
            }

            if (indexInPath < path.Count)
            {
                nextWayPoint = path[indexInPath].transform;
            }
            indexInPath = 0;
            oldDestination = dest;
        }

        // Determine what point the player is trying to get to
        if (indexInPath >= path.Count)
        {
            // Chase Player
            nextWayPoint = PlayerTransform;
        }
        else
        {
            nextWayPoint = path[indexInPath].transform;

            // If you get to the next waypoint and pass it, increment index
            if (Vector2.Distance(transform.position, nextWayPoint.position) < nodeRadiusThreshold)
            {
                indexInPath++;

                if (indexInPath < path.Count)
                {
                    nextWayPoint = path[indexInPath].transform;
                }
            }
        }

        if (nextWayPoint != null)
        {
            if (Vector2.Distance(transform.position, PlayerTransform.position) < 0.1f * nodeRadiusThreshold)
            {
                rb.velocity = new Vector2(0f, 0f);
            }
            else
            {
                Vector2 dir = nextWayPoint.position - transform.position;
                dir.Normalize();
                rb.velocity = new Vector2(dir.x * chaseSpeed * Time.deltaTime, dir.y * chaseSpeed * Time.deltaTime);
                if ((nextWayPoint.position.x > transform.position.x && !facingRight) ||
                    (nextWayPoint.position.x < transform.position.x && facingRight))
                {
                    Flip();
                }
            }
        }
    }

    private PathfindNode FindNearestGraphNode(Transform transform)
    {
        if (pathfinder.GetGraphNodes().Count == 0)
        {
            return null;
        }

        List<PathfindNode> possibles = pathfinder.GetGraphNodes();

        if (possibles.Count != 0)
        {
            PathfindNode closest = possibles[0];
            float closestDistance = Vector2.Distance(closest.transform.position, transform.position);
            foreach (PathfindNode node in possibles)
            {
                float distance = Vector2.Distance(node.transform.position, transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = node;
                }
            }
            return closest;
        }

        return null;
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
        visionCone.SetFacingDirection(facingRight);
    }

    public void Reset()
    {
        transform.position = starting.position;
        rb.velocity = new Vector2(0f, 0f);
        anim.SetBool("Chase", false);
        indexInPath = 0;
        path.Clear();
    }

    public void TransitionToPatrolState()
    {
        anim.SetInteger("State", 0);
    }

    void OnDrawGizmosSelected()
    {
        if (path != null)
        {
            foreach (PathfindNode t in path)
            {
                Gizmos.DrawSphere(t.transform.position, nodeRadiusThreshold);
            }
        }
    }

}


