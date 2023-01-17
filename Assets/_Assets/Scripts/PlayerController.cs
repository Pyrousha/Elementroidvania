using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private BoxCollider2D col;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator anim;
    private string currentState;

    //Animation states
    const string PLAYER_IDLE = "Player_Idle";
    const string PLAYER_JUMP_UP = "Player_Jump_Up";
    const string PLAYER_JUMP_DOWN = "Player_Jump_Down";

    [Header("Parameters")]
    [SerializeField] private float accelSpeed;
    [SerializeField] private float frictionSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float jumpPower;
    [Space(5)]
    [SerializeField] private float gravUp;
    [SerializeField] private float gravDown;
    [SerializeField] private float spaceReleaseGravMult;
    [Space(5)]
    [SerializeField] private LayerMask terrainLayer;

    private float xSpeed = 0;
    private bool grounded = false;
    private float lastTimePressedJump = -100.0f;
    private float lastTimeGrounded = -100.0f;
    private readonly float epsilon = 0.05f;
    private readonly float jumpBuffer = 0.1f;
    private readonly float coyoteTime = 0.1f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Get sign of current speed
        float sign = 1;
        if (xSpeed < 0)
            sign = -1;

        //Accelerate
        float inputX = InputHandler.Instance.Direction.x;
        if (inputX < -epsilon) //Pressing left
        {
            if (xSpeed > -maxSpeed)
            {
                //Can still accelerate to the left (but do not exceed max)
                xSpeed = Mathf.Max(-maxSpeed, xSpeed + accelSpeed * inputX * Time.deltaTime);
            }
        }
        else
        {
            if (inputX > epsilon) //Pressing right
            {
                if (xSpeed < maxSpeed)
                {
                    //Can still accelerate to the right (but do not exceed max)
                    xSpeed = Mathf.Min(maxSpeed, xSpeed + accelSpeed * inputX * Time.deltaTime);
                }
            }
            else //pressing nothing
            {
                //Not pressing anything, subtract friction and update speed
                float newSpeedMagnitude = Mathf.Max(0, Mathf.Abs(xSpeed) - frictionSpeed * Time.deltaTime);
                xSpeed = newSpeedMagnitude * sign;
            }
        }

        rb.velocity = new Vector2(xSpeed, rb.velocity.y);

        //Sprite flipping
        if (xSpeed < 0)
            spriteRenderer.flipX = true;
        else
        {
            if (xSpeed > 0)
                spriteRenderer.flipX = false;
        }

        //Ground checking
        bool lastGrounded = grounded;
        grounded = Physics2D.BoxCast(col.bounds.center, col.bounds.size * 0.99f, 0f, Vector2.down, 0.2f, terrainLayer);

        //Get last time grounded
        if ((lastGrounded == true) && (grounded == false))
            lastTimeGrounded = Time.time;

        //Jump - grounded
        if (InputHandler.Instance.Jump.down)
        {
            if (grounded || (Time.time - lastTimeGrounded <= coyoteTime))
                Jump();
            else
                lastTimePressedJump = Time.time;
        }
        //Jump - buffered
        if ((lastGrounded == false) && (grounded == true))
        {
            if (Time.time - lastTimePressedJump <= jumpBuffer)
                Jump();
        }


        //Space release gravity
        if (InputHandler.Instance.Jump.released && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * spaceReleaseGravMult);
        }

        //Gravity
        if (InputHandler.Instance.Jump.held && rb.velocity.y > 0)
            rb.velocity -= new Vector2(0, gravUp * Time.deltaTime);
        else
            rb.velocity -= new Vector2(0, gravDown * Time.deltaTime);


        //Set animation states
        if (grounded)
        {
            ChangeAnimationState(PLAYER_IDLE);
        }
        else
        {
            if (rb.velocity.y >= 0)
                ChangeAnimationState(PLAYER_JUMP_UP);
            else
                ChangeAnimationState(PLAYER_JUMP_DOWN);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpPower);
    }

    private void ChangeAnimationState(string _newState)
    {
        //Stop same animation from interrupting itself
        if (currentState == _newState)
            return;

        //Play new animation
        anim.Play(_newState);

        //Update current anim state var
        currentState = _newState;
    }
}
