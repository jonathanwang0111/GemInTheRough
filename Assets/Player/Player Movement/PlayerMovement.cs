using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Transform startingPoint;
    private Transform currentCheckpoint;
    public PauseMenuInformation pauseMenu;

    // Immobile Variables
    private bool immobile;
    public float immobileTime;
    private float timeSpentImmobile;

    // Horizontal Movement
    public float groundSpeed;
    public float airSpeed;
    public float accelerationTime;
    public float decelerationTime;
    private int oldDirection;
    private int decelerationDirection;
    private float rawHorizontalValue;
    private bool facingRight;
    private float timePassedHorizontal;

    // Vertical Movement
    public float jumpPower;
    public float additionalDownwardsForce;
    public Transform groundCheck;
    public float wallSlideFactor;
    public float wallJumpPower;
    public Vector2 wallJumpDirection;
    public Transform wallCheck;
    public LayerMask groundLayers;
    public float jumpBuffer;
    public float coyoteTime;
    private float coyoteCounter;
    private float jumpBufferPassed;
    private bool jumpPressed;
    private bool jumpTriggered;
    private bool midJump;
    private bool wasGrounded;
    // Animation related
    public Animator anim;
    private Rigidbody2D rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        facingRight = true;

        timePassedHorizontal = 0;
        oldDirection = 0;
        decelerationDirection = 0;

        wallJumpDirection.Normalize();

        jumpPressed = false;
        midJump = true;
        wasGrounded = true;
        jumpBufferPassed = -1;
        immobile = false;
        timeSpentImmobile = 0;

        if (GlobalData.GetCheckpoint()[0] != 0 ||
            GlobalData.GetCheckpoint()[1] != 0 ||
            GlobalData.GetCheckpoint()[2] != 0)
        {
            startingPoint.position = new Vector3(
                GlobalData.GetCheckpoint()[0],
                GlobalData.GetCheckpoint()[1],
                GlobalData.GetCheckpoint()[2]
            );
        }

        currentCheckpoint = startingPoint;
        transform.position = startingPoint.position;
    }

    // Update is called once per frame
    void Update()
    {
        bool isGrounded = CheckGrounded();
        bool isWalled = CheckWalled();
        if (!isGrounded && rigidBody.velocity.y < 0 && !isWalled)
        {
            anim.SetInteger("VerticalState", -1);
        }
        else if (isGrounded)
        {
            anim.SetInteger("VerticalState", 0);
        }
        wasGrounded = isGrounded;
        rawHorizontalValue = Input.GetAxisRaw("Horizontal");

        if (Input.GetButton("Jump"))
        {
            jumpPressed = true;
        }
        else
        {
            jumpPressed = false;
        }

        if (isGrounded)
        {
            coyoteCounter = coyoteTime;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferPassed = jumpBuffer;
        }
        else
        {
            jumpBufferPassed -= Time.deltaTime;
        }
        if (immobile)
        {
            if (timeSpentImmobile > immobileTime)
            {
                immobile = false;
            }
            else
            {
                timeSpentImmobile += Time.deltaTime;
            }
        }
    }

    void FixedUpdate()
    {
        if (immobile)
        {
            return;
        }

        bool isWalled = CheckWalled();
        float horizontalVelocity = 0.0f;
        timePassedHorizontal += Time.deltaTime;
        float topSpeed = groundSpeed;
        if (!wasGrounded)
        {
            topSpeed = airSpeed;
        }

        // Determine the direction you want to go from now on
        int newDirection = 0;
        if (rawHorizontalValue > 0.1)
        {
            newDirection = 1;
            if (!facingRight)
            {
                Flip();
            }
        }
        else if (rawHorizontalValue < -0.1)
        {
            newDirection = -1;
            if (facingRight)
            {
                Flip();
            }
        }

        bool allowForAnimationChange = true;
        // Determine behavior based on new/old directions
        if (oldDirection == newDirection)
        {
            if (oldDirection == 0)
            {
                if (timePassedHorizontal < decelerationTime)
                {
                    horizontalVelocity = decelerationDirection * topSpeed * (decelerationTime - timePassedHorizontal) / decelerationTime;
                }
                else
                {
                    decelerationDirection = 0;
                }
            }
            else if (timePassedHorizontal > accelerationTime)
            {
                horizontalVelocity = newDirection * topSpeed;
            }
            else
            {
                horizontalVelocity = newDirection * topSpeed * timePassedHorizontal / accelerationTime;
            }
        }
        else
        {
            timePassedHorizontal = 0;
            if (oldDirection == 0)
            {
                horizontalVelocity = 0;
            }
            else if (newDirection != 0)
            {
                horizontalVelocity = 0;
            }
            else
            {
                decelerationDirection = oldDirection;
                horizontalVelocity = newDirection * topSpeed;
            }
            if (timePassedHorizontal < accelerationTime)
            {
                allowForAnimationChange = false;
            }
            oldDirection = newDirection;
        }


        if ((!jumpPressed || rigidBody.velocity.y < 0))
        {
            rigidBody.AddForce(-additionalDownwardsForce * Vector2.up);
        }

        if ((isWalled && rigidBody.velocity.y < 0))
        {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, wallSlideFactor * rigidBody.velocity.y);
        }

        if (allowForAnimationChange)
        {
            anim.SetFloat("HorizontalSpeed", Mathf.Abs(horizontalVelocity));
        }

        Vector3 targetVelocity = new Vector2(horizontalVelocity * Time.deltaTime * 10f, rigidBody.velocity.y);
        Vector3 zeroVector = new Vector3(0f, 0f, 0f);
        rigidBody.velocity = targetVelocity;

        if ((jumpBufferPassed >= 0) && ((CheckGrounded() && rigidBody.velocity.y == 0) || (!midJump && coyoteCounter >= 0))) // or hang time
        {
            anim.SetInteger("VerticalState", 1);
            AddForceForJump(Vector2.up, jumpPower);
        }
        else if ((jumpBufferPassed >= 0) && isWalled)
        {
            Vector2 dir = wallJumpDirection;
            if (facingRight)
            {
                dir = new Vector2(-wallJumpDirection.x, wallJumpDirection.y);
            }
            AddForceForJump(dir, wallJumpPower);
            anim.SetInteger("VerticalState", 1);
            timeSpentImmobile = 0;
            immobile = true;
            Flip();

        }
    }

    // Called after the prep animation ends
    public void AddForceForJump(Vector2 forceDirection, float force)
    {
        // Add force
        rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);
        rigidBody.AddForce(force * forceDirection, ForceMode2D.Impulse);
        jumpBufferPassed = 0;
        midJump = true;
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private bool CheckGrounded()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(groundCheck.position, new Vector2(0.4f, 0.1f), 0, groundLayers);
        if (colliders.Length > 0)
        {
            midJump = false;
            return true;
        }

        return false;
    }

    private bool CheckWalled()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(wallCheck.position, new Vector2(0.1f, 0.8f), 0, groundLayers);
        if (colliders.Length > 0)
        {
            return true;
        }

        return false;
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(groundCheck.position, new Vector3(0.4f, 0.1f, 0f));
        Gizmos.DrawWireCube(wallCheck.position, new Vector3(0.1f, 0.8f, 0f));

    }

    public bool IsFacingRight()
    {
        return facingRight;
    }

    public void SetImmobileState(bool immobileTrue)
    {
        immobile = immobileTrue;
    }

    public void Reset()
    {
        transform.position = currentCheckpoint.position;
    }

    public void SetCheckpoint(Transform checkpt, int id)
    {
        currentCheckpoint.position = checkpt.position;
        float[] pt = new float[3];
        pt[0] = checkpt.position.x;
        pt[1] = checkpt.position.y;
        pt[2] = checkpt.position.z;
        pauseMenu.SetCheckpoint(pt, id);
    }
}


