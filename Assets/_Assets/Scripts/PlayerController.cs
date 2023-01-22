using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    [Header("External References")]
    [SerializeField] private GameObject afterimagePrefab;

    [Header("Self-References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private BoxCollider2D col;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator anim;
    [Space(5)]
    [SerializeField] private GameObject feetVFX_left;
    [SerializeField] private GameObject feetVFX_right;

    private PlayerAnimStateEnum currentAnimation;

    //Animation states
    enum PlayerAnimStateEnum
    {
        Player_Idle,
        Player_Jump_Up,
        Player_Jump_Down,
        Player_Drill_Right,
        Player_Drill_Down
    }

    enum DrillStateEnum
    {
        Charged,
        Drilling,
        Cooldown
    }
    private DrillStateEnum drillState;

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
    [SerializeField] private float drillPower;
    [SerializeField] private float drillDuration;
    [Space(5)]
    [SerializeField] private LayerMask terrainLayer;

    private float xSpeed = 0;
    private bool grounded = false;
    private bool useGravity = true;
    private float lastTimePressedJump = -100.0f;
    private float lastTimeGrounded = -100.0f;
    private float drillTimer;
    private const float epsilon = 0.05f;
    private const float jumpBuffer = 0.1f;
    private const float coyoteTime = 0.1f;

    #region Afterimage
    private const float secsPerAfterimage = 0.01f;
    private float lastTimeSpawnedAfterimage = -100.0f;
    #endregion


    /// <summary>
    /// Returns if the player is currently able to move (not attacking, dashing, stunned, etc.)
    /// </summary>
    /// <returns></returns>
    private bool CanMove =>
    (
        drillState != DrillStateEnum.Drilling
    );

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (CanMove)
        {
            xSpeed = rb.velocity.x;

            //Accelerate + Friction
            float inputX = InputHandler.Instance.Direction.x;
            if (inputX < -epsilon) //Pressing left
            {
                if (xSpeed > -maxSpeed)
                {
                    //Can still accelerate to the left (but do not exceed max)
                    xSpeed = Mathf.Max(-maxSpeed, xSpeed + accelSpeed * inputX * Time.deltaTime);
                }
                else
                {
                    //Apply friction
                    xSpeed = Mathf.Min(xSpeed + frictionSpeed * Time.deltaTime, -maxSpeed);
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
                    else
                    {
                        //Apply friction
                        xSpeed = Mathf.Max(xSpeed - frictionSpeed * Time.deltaTime, maxSpeed);
                    }
                }
                else //pressing nothing
                {
                    //Get sign of current speed
                    float sign = 1;
                    if (xSpeed < 0)
                        sign = -1;

                    //Not pressing anything, subtract friction and update speed
                    float newSpeedMagnitude = Mathf.Max(0, Mathf.Abs(xSpeed) - frictionSpeed * Time.deltaTime);
                    xSpeed = newSpeedMagnitude * sign;
                }
            }

            //Set velocity
            rb.velocity = new Vector2(xSpeed, rb.velocity.y);
        }


        //Sprite flipping + spark vfx
        bool leftVFXActive = false;
        bool rightVFXActive = false;
        if (rb.velocity.x < -epsilon)
        {
            spriteRenderer.flipX = true;
            leftVFXActive = true;
        }
        else
        {
            if (rb.velocity.x > epsilon)
            {
                spriteRenderer.flipX = false;
                rightVFXActive = true;
            }
        }


        //Ground checking
        bool lastGrounded = grounded;
        grounded = Physics2D.BoxCast(col.bounds.center, col.bounds.size * 0.99f, 0f, Vector2.down, 0.2f, terrainLayer);

        //Set spark vfx
        feetVFX_left.SetActive(leftVFXActive && grounded);
        feetVFX_right.SetActive(rightVFXActive && grounded);


        //Get last time grounded
        if ((lastGrounded == true) && (grounded == false))
            lastTimeGrounded = Time.time;

        //Jump - grounded
        if (InputHandler.Instance.Jump.down)
        {
            if (CanMove && (grounded || (Time.time - lastTimeGrounded <= coyoteTime)))
                Jump();
            else
                lastTimePressedJump = Time.time;
        }
        //Jump - buffered
        if (CanMove && (grounded == true))
        {
            if (Time.time - lastTimePressedJump <= jumpBuffer)
                Jump();
        }


        #region Change Drill Dash state
        switch (drillState)
        {
            case DrillStateEnum.Charged:
                {
                    if (InputHandler.Instance.Drill.down)
                    {
                        drillState = DrillStateEnum.Drilling;

                        drillTimer = drillDuration;
                        useGravity = false;

                        if (InputHandler.Instance.Direction.y < -0.6f)
                        {
                            //Drill down
                            rb.velocity = new Vector2(0, -drillPower);

                            ChangeAnimationState(PlayerAnimStateEnum.Player_Drill_Down);
                        }
                        else
                        {
                            //Drill horizontal
                            float inputX = InputHandler.Instance.Direction.x;

                            //holding a direction, dash in that direction
                            if (Mathf.Abs(inputX) >= epsilon)
                            {
                                if (inputX > 0) //Dash right
                                    rb.velocity = new Vector2(drillPower, 0);
                                else //Dash left
                                    rb.velocity = new Vector2(-drillPower, 0);
                            }
                            else
                            {
                                //not holding a direction, dash in direction player is facing
                                if (!spriteRenderer.flipX) //dash right
                                    rb.velocity = new Vector2(drillPower, 0);
                                else //Dash left
                                    rb.velocity = new Vector2(-drillPower, 0);
                            }

                            ChangeAnimationState(PlayerAnimStateEnum.Player_Drill_Right);
                        }
                    }
                    break;
                }
            case DrillStateEnum.Drilling:
                {
                    if (Time.time - lastTimeSpawnedAfterimage >= secsPerAfterimage)
                    {
                        Afterimage.SpawnAfterimage(afterimagePrefab, transform.position, spriteRenderer);
                        lastTimeSpawnedAfterimage = Time.time;
                    }

                    drillTimer -= Time.deltaTime;
                    if (drillTimer <= 0)
                    {
                        //End drill
                        useGravity = true;
                        drillState = DrillStateEnum.Cooldown;
                    }
                    break;
                }
            case DrillStateEnum.Cooldown:
                {
                    if (grounded)
                        drillState = DrillStateEnum.Charged;
                    break;
                }
        }
        #endregion


        //Gravity
        if (useGravity)
        {
            if (InputHandler.Instance.Jump.held && rb.velocity.y > 0)
                rb.velocity -= new Vector2(0, gravUp * Time.deltaTime);
            else
                rb.velocity -= new Vector2(0, gravDown * Time.deltaTime);
        }

        //Space release gravity
        if (InputHandler.Instance.Jump.released && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * spaceReleaseGravMult);
        }


        //Set animation states
        if (drillState != DrillStateEnum.Drilling)
        {
            if (grounded)
            {
                ChangeAnimationState(PlayerAnimStateEnum.Player_Idle);
            }
            else
            {
                if (rb.velocity.y >= 0)
                    ChangeAnimationState(PlayerAnimStateEnum.Player_Jump_Up);
                else
                    ChangeAnimationState(PlayerAnimStateEnum.Player_Jump_Down);
            }
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpPower);
    }

    private void SpawnAfterimage()
    {
        lastTimeSpawnedAfterimage = Time.time;
    }

    private void ChangeAnimationState(PlayerAnimStateEnum _newState)
    {
        //Stop same animation from interrupting itself
        if (currentAnimation == _newState)
            return;

        //Play new animation
        anim.Play(_newState.ToString());

        //Update current anim state var
        currentAnimation = _newState;
    }
}
