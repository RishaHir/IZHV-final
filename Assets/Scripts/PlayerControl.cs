using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{

    private Rigidbody2D rBody;
    private HingeJoint2D ropeHinge;
    public float MovementSpeed;
    public float Acceleration;
    public float JumpForce;
    private float actualSpeed;
    private float desiredSpeed;
    private bool grounded;

    // Start is called before the first frame update
    void Start()
    {
        rBody = gameObject.GetComponent<Rigidbody2D>();
        ropeHinge = gameObject.GetComponent<HingeJoint2D>();
        grounded = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (rBody.velocity.x < desiredSpeed && math.abs(rBody.velocity.x - desiredSpeed) > 0.1)
        {
            Vector2 force = new Vector2(1, 0);
            rBody.AddForce(force*Time.deltaTime*Acceleration);
        }
        else if (rBody.velocity.x > desiredSpeed && math.abs(rBody.velocity.x - desiredSpeed) > 0.1)
        {
            Vector2 force = new Vector2(-1, 0);
            rBody.AddForce(force*Time.deltaTime*Acceleration);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<Vector2>();
        if (grounded)
        {
            if (value.x > 0)
            {
                desiredSpeed = MovementSpeed;
            }
            else if (value.x < 0)
            {
                desiredSpeed = -MovementSpeed;
            }
            else
            {
                desiredSpeed = 0;
            }
        }
        
        if (value.y < 0)
        {
            ClimbDown();    
        } else if (value.y > 0)
        {
            ClimbUp();
        }
    }
    
    public void OnJump(InputAction.CallbackContext context)
    {
        if (grounded && context.performed)
        {
            rBody.AddForce(new Vector2(0,JumpForce));
        }
    }

    private void ClimbUp()
    {
        Rigidbody2D nextSegment = ropeHinge.connectedBody.GetComponentInParent<Rope>().left.GetComponent<Rigidbody2D>();
        ropeHinge.connectedBody = nextSegment;
        
    }

    private void ClimbDown()
    {
        Rigidbody2D nextSegment = ropeHinge.connectedBody.GetComponentInParent<Rope>().right.GetComponent<Rigidbody2D>();
        ropeHinge.connectedBody = nextSegment;
    }
}
