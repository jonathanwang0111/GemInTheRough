using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class VisionCone : MonoBehaviour
{
    public Transform startingPoint;

    // Variables for small vision cone
    public float coneMaxAngleOffset;
    public float coneMaxDistance;
    public int coneNumRaysEachSide;
    private float coneAngleOffsetIncrementAmount;
    private float coneDistanceOffsetAmount;
    private float coneCurrentOffset;
    private float coneCurrentDistance;

    // Variables for both
    public LayerMask layer;
    private float addedTwist;
    private bool facingRight;

    private bool inCone;

    void Start()
    {
        // take out facing right, and broadcast it from Enemy AI
        facingRight = false;
        addedTwist = startingPoint.rotation.eulerAngles.z * (Mathf.PI / 180.0f);
        inCone = false;

        coneAngleOffsetIncrementAmount = (coneMaxAngleOffset * (Mathf.PI / 180.0f)) / coneNumRaysEachSide;
        coneDistanceOffsetAmount = (coneMaxDistance / (coneNumRaysEachSide + 1));
    }

    // Update is called once per frame
    void Update()
    {
        coneCurrentOffset = 0;
        coneCurrentDistance = coneMaxDistance;
        CastRays();
    }

    private void CastRays()
    {
        bool hit = false;
        for (int i = 0; i < (coneNumRaysEachSide + 1); i++)
        {
            Vector2 rayTop, rayBottom;
            if (facingRight)
            {
                rayTop = new Vector2(Mathf.Cos(coneCurrentOffset) + Mathf.Cos(addedTwist), Mathf.Sin(coneCurrentOffset) + Mathf.Sin(addedTwist));
                rayBottom = new Vector2(Mathf.Cos(coneCurrentOffset) + Mathf.Cos(addedTwist), -Mathf.Sin(coneCurrentOffset) + Mathf.Sin(addedTwist));
            }
            else
            {
                rayTop = new Vector2(-Mathf.Cos(coneCurrentOffset) - Mathf.Cos(addedTwist), Mathf.Sin(coneCurrentOffset) + Mathf.Sin(addedTwist));
                rayBottom = new Vector2(-Mathf.Cos(coneCurrentOffset) - Mathf.Cos(addedTwist), -Mathf.Sin(coneCurrentOffset) + Mathf.Sin(addedTwist));
            }

            rayTop.Normalize();
            rayBottom.Normalize();

            UnityEngine.Debug.DrawRay(startingPoint.position, coneCurrentDistance * rayTop, Color.green);
            UnityEngine.Debug.DrawRay(startingPoint.position, coneCurrentDistance * rayBottom, Color.green);
            RaycastHit2D hitTop = Physics2D.Raycast(startingPoint.position, rayTop, coneCurrentDistance, layer);
            RaycastHit2D hitBottom = Physics2D.Raycast(startingPoint.position, rayBottom, coneCurrentDistance, layer);
            if (hitTop.collider != null && hitTop.collider.tag == "Player")
            {
                hit = true;
            }
            else if (hitBottom.collider != null && hitBottom.collider.tag == "Player")
            {
                hit = true;
            }

            coneCurrentOffset += coneAngleOffsetIncrementAmount;
            coneCurrentDistance -= coneDistanceOffsetAmount;
        }
        inCone = hit;
    }

    public void SetFacingDirection(bool faceRight)
    {
        // Use of BroadcastMessage to set
        facingRight = faceRight;
    }


    public bool InVisionCone()
    {
        return inCone;
    }

}
