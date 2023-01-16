using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private BoxCollider2D col;

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
    private float epsilon = 0.05f;

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

        //Ground checking
        grounded = Physics2D.BoxCast(col.bounds.center, col.bounds.size * 0.95f, 0f, Vector2.down, 0.2f, terrainLayer);


        //Jump
        if (InputHandler.Instance.Jump.down && grounded)
        {
            rb.velocity = new Vector2(xSpeed, jumpPower);
        }
        else
            rb.velocity = new Vector2(xSpeed, rb.velocity.y);


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
    }
}
