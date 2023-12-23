using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerControl : MonoBehaviour
{
    private Rigidbody2D rBody;
    private Rigidbody2D otherBody;
    private HingeJoint2D ropeHinge;
    public GameObject OtherPlayer;
    public float MovementSpeed;
    public float Acceleration;
    public float JumpForce;
    public float ThrowForce;
    private float actualSpeed;
    private float desiredSpeed;
    public bool throwing;
    public bool grounded;
    public bool looksRight;

    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        rBody = gameObject.GetComponent<Rigidbody2D>();
        otherBody = OtherPlayer.GetComponent<Rigidbody2D>();
        ropeHinge = gameObject.GetComponent<HingeJoint2D>();
        grounded = true;
        looksRight = true;
        throwing = false;
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (desiredSpeed != 0)
        {
            if (rBody.velocity.x < desiredSpeed && math.abs(rBody.velocity.x - desiredSpeed) > 0.1)
            {
                Vector2 force = new Vector2(1, 0);
                rBody.AddForce(force * Time.fixedDeltaTime * Acceleration, ForceMode2D.Force);
                looksRight = true;
                transform.SetPositionAndRotation(transform.position, Quaternion.Euler(0,0,0));
            }
            else if (rBody.velocity.x > desiredSpeed && math.abs(rBody.velocity.x - desiredSpeed) > 0.1)
            {
                Vector2 force = new Vector2(-1, 0);
                rBody.AddForce(force * Time.fixedDeltaTime * Acceleration, ForceMode2D.Force);
                looksRight = false;
                transform.SetPositionAndRotation(transform.position, Quaternion.Euler(0,180,0));
            }
        }
        
        // grounded = Physics2D.Raycast(transform.position, Vector2.down, 2f, LayerMask.GetMask("Ground"));
        grounded = Physics2D.BoxCast(transform.position, new Vector2(1,0.01f),0,Vector2.down, 2f, LayerMask.GetMask("Ground"));
        if (grounded)
        {
            rBody.sharedMaterial.friction = 0.6f;
        }
        else
        {
            rBody.sharedMaterial.friction = 0;
        }
        
        if (throwing)
        {
            Hold();
        }

    }
    
    // Update is called once per frame
    void Update()
    {
        animator.SetBool("grounded", grounded);
        animator.SetFloat("movementSpeed", math.abs(rBody.velocity.x));
    }
    

    public void OnMove(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<Vector2>();
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
            rBody.AddForce(Vector2.up*JumpForce, ForceMode2D.Impulse);
        }
    }

    public void OnThrow(InputAction.CallbackContext context)
    {
        if (throwing == false && context.performed)
        {
            bool hit;
            if (looksRight)
            {
                hit = Physics2D.Raycast(transform.position, Vector2.right, 1.0f, LayerMask.GetMask(LayerMask.LayerToName(OtherPlayer.layer)));
                if (hit)
                {
                    OtherPlayer.GetComponent<PlayerInput>().DeactivateInput();
                    OtherPlayer.GetComponent<HingeJoint2D>().enabled = false;
                    throwing = true;
                }
            }
            else
            {
                hit = Physics2D.Raycast(transform.position, Vector2.left, 1.0f, LayerMask.GetMask(LayerMask.LayerToName(OtherPlayer.layer)));
                if (hit)
                {
                    OtherPlayer.GetComponent<PlayerInput>().DeactivateInput();
                    OtherPlayer.GetComponent<HingeJoint2D>().enabled = false;
                    throwing = true;
                }
            }

           
        } else if (throwing == true && context.performed)
        {
            if (looksRight)
            {
                otherBody.AddForce(new Vector2(0.3f ,0.5f).normalized*ThrowForce, ForceMode2D.Impulse);
                throwing = false;
                OtherPlayer.GetComponent<PlayerInput>().ActivateInput();
                OtherPlayer.GetComponent<HingeJoint2D>().enabled = true;
            }
            else
            {
                otherBody.AddForce(new Vector2(-0.3f ,0.5f).normalized*ThrowForce, ForceMode2D.Impulse);
                throwing = false;
                OtherPlayer.GetComponent<PlayerInput>().ActivateInput();
                OtherPlayer.GetComponent<HingeJoint2D>().enabled = true;
            }
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

    private void Hold()
    {
        if (looksRight)
        {
            otherBody.velocity = Vector2.zero;
            otherBody.position = rBody.position + new Vector2(0.3f ,1f);
        }
        else
        {
            otherBody.velocity = Vector2.zero;
            otherBody.position = rBody.position + new Vector2(-0.3f ,1f);
        }
    }
}
