using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Parameters")]
    [SerializeField] private float accelSpeed;
    [SerializeField] private float frictionSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float jumpPower;

    private float xSpeed = 0;

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

        //Subtract friction and update speed
        float newSpeedMagnitude = Mathf.Max(0, Mathf.Abs(xSpeed) - frictionSpeed * Time.deltaTime);
        xSpeed = newSpeedMagnitude * sign;


        //Accelerate
        float inputX = InputHandler.Instance.Direction.x;
        if (inputX < 0)
        {
            if (xSpeed > -maxSpeed)
            {
                //Can still accelerate to the left (but do not exceed max)
                xSpeed = Mathf.Max(-maxSpeed, xSpeed + accelSpeed * inputX * Time.deltaTime);
            }
        }
        else
        {
            if (inputX > 0)
            {
                if (xSpeed < maxSpeed)
                {
                    //Can still accelerate to the right (but do not exceed max)
                    xSpeed = Mathf.Min(maxSpeed, xSpeed + accelSpeed * inputX * Time.deltaTime);
                }
            }
        }


        //Jump
        if (InputHandler.Instance.Jump.down)
        {
            Debug.Log("jump");
            rb.velocity = new Vector2(xSpeed, jumpPower);
        }
        else
            rb.velocity = new Vector2(xSpeed, rb.velocity.y);
    }
}
