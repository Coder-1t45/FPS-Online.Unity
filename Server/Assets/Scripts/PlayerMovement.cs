using RiptideNetworking;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerAnim : ushort
{
    idle = 1,
    walking,
    running,
    crouching,
    chrouchwalking,
    swiming,
    sliding
}
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Player player;
    private PlayerWeapon weapon => GetComponent<PlayerWeapon>();

    private bool[] inputs;


    public Rigidbody rb => GetComponent<Rigidbody>();
    public CapsuleCollider capsule => GetComponent<CapsuleCollider>();

    //Movement
    public float moveSpeed = 1500;
    public float maxSpeed = 10;
    private Vector3 camForward;
    public float counterMovement = 0.175f;
    private float threshold = 0.01f;
    public float maxSlopeAngle = 10f;

    //Crouch & Slide
    public float slideForce = 400;
    public float slideCounterMovement = 0.2f;

    //Jumping
    public LayerMask whatIsGround;
    public float jumpForce = 250f;
    private float jumpCounter = 2f;

    //Input
    public float x, y;
    public bool jumping, running, crouching, sliding, grounded, inwater;
    private bool was_sliding, already_slide, was_inwater;
    private float playerHeight;

    //WallRun
    private Vector3 currWall;
    private Vector3 WallForward;

    private PlayerAnim animState
    {
        get
        {
            return sliding? PlayerAnim.sliding:
                inwater? PlayerAnim.swiming:
                crouching && (x != 0 || y != 0)? PlayerAnim.chrouchwalking:
                crouching? PlayerAnim.crouching:
                running && (x != 0 || y != 0) ? PlayerAnim.running:
                x != 0 || y != 0 ? PlayerAnim.walking : PlayerAnim.idle;
        }
    }


    //1 Right, -1 Left
    public int wallside
    {
        get
        {
            Vector3 c = currWall;
            int a = (c.x > 1 || c.z > 1) ? 1 : (c.x < 1 || c.z < 1) ? -1 : 0;
            return a * AngleSide();
        }
    }
    
    private void Awake()
    {
        playerHeight = capsule.height;
    }
    private void Start()
    {
        inputs = new bool[7];
    }
    private void Update()
    {
        //Down
        if (!was_sliding && sliding)
        {
            already_slide = true;
            StartSlide();
        }

        if (!was_inwater && inwater)
        {
            var vel = rb.velocity;
            rb.velocity = new Vector3(vel.x, vel.y / 3, vel.z);
        }

        bool wasWall = wallrunning;

        if (wallrunning && !wasWall && !disableWallrunning)
        {
            rb.AddForce(Vector2.up * jumpForce * 1.3f);

            //If jumping while falling, reset y velocity.
            Vector3 vel = rb.velocity;
            if (rb.velocity.y < 0.5f)
                rb.velocity = new Vector3(vel.x, 0, vel.z);
            else if (rb.velocity.y > 0)
                rb.velocity = new Vector3(vel.x, vel.y / 2, vel.z);
        }

    }
    private bool was_slope = false;
    private void LateUpdate()
    {
        was_sliding = sliding;
        if (!was_sliding)
            already_slide = false;

        was_slope = slope;
        was_inwater = inwater;
    }
    
    private bool _canFly;
    private void FixedUpdate()
    {
        SendMovement();
        GroundCheck();
        Movement();
        Vault();

        //Get Timers:
        CalculateTimers();

        //Can Fly and Jump Counter handeling..
        _canFly = !grounded && !wallrunning && jumpCounter > 0 && _ungroundedTimer > .2f;
        if (_groundedTime > .2f || _wallrunningTime > .2f) jumpCounter += Time.deltaTime * 1.5f;

        jumpCounter = Mathf.Clamp(jumpCounter, 0, 2);
        
        //sliding
        sliding = crouching && running && (_crouchingTime <= _runningTime) && grounded && !already_slide;


        //Handle collider height changing..
        capsule.height = sliding ? playerHeight / 4 : crouching ? playerHeight / 2 : playerHeight;
    }
    private float _groundedTime;
    private float _wallrunningTime;
    private float _runningTime;
    private float _crouchingTime;
    public void CalculateTimers()
    {
        if (grounded) _ungroundedTimer = 0;
        else _ungroundedTimer += Time.deltaTime;

        if (!grounded) _groundedTime = 0;
        else _groundedTime += Time.deltaTime;

        if (!wallrunning) _wallrunningTime = 0;
        else _wallrunningTime += Time.deltaTime;

        if (!running) _runningTime = 0;
        else _runningTime += Time.deltaTime;

        if (!crouching) _crouchingTime = 0;
        else _crouchingTime += Time.deltaTime;
    }
    public void SetInput(bool[] inputs, Vector3 forward, Vector3 camForward)
    {
        int x = 0; int y = 0;

        y += inputs[0] ? 1 : inputs[1] ? -1 : 0;
        x += inputs[3] ? 1 : inputs[2] ? -1 : 0;

        transform.forward = forward;
        crouching = inputs[6];

        this.x = x; this.y = y;

        jumping = inputs[4];
        running = inputs[5];

        maxSpeed = inwater? 7.5f : running ? 15 : crouching ? 7 : 10;
        this.camForward = camForward;
    }
    private void SendMovement()
    {
        Message message = Message.Create(MessageSendMode.unreliable, ServerToClientId.playerMovement);

        message.AddUShort(player.Id);
        message.AddInt(player.health);
        message.AddVector3(camForward);

        message.AddUShort((ushort)this.animState);

        message.AddVector3(transform.position);
        message.AddVector3(transform.forward);

        message.AddBool(wallrunning && !disableWallrunning);
        message.AddInt(wallside);

        message.AddFloat(jumpCounter);
        message.AddBool(sliding);


        NetworkManager.Singleton.Server.SendToAll(message);
    }

    public Vector3 normalVector;
    private int AngleSide()
    {
        float a = WallAngle(transform.forward, currWall);
        return a > 0 ? 1 : a < 0 ? -1 : 0;
    }

    private void GroundCheck()
    {
        bool wasGrounded = grounded;
        grounded = false;

        float height = this.GetComponent<CapsuleCollider>().height * transform.localScale.y;
        var checking_position = Vector3.up * -height / 2;

        Collider[] colliders = Physics.OverlapSphere(transform.position + checking_position, .25f, whatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                grounded = true;
            }
        }

        if (!wasGrounded && grounded && sliding)
            rb.AddForce(transform.forward * slideForce);

    }
    private void OnDrawGizmos()
    {
        float height = this.GetComponent<CapsuleCollider>().height * transform.localScale.y;
        Gizmos.DrawSphere(transform.position + Vector3.up * -height / 2, .25f);

        /* WallRunnnig
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + currWall * 3f);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + WallForward * 3f);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 3f);
        */
    }

    private void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!grounded || jumping) return;

        //Slow down sliding
        /*if (crouching && grounded)
        {
            rb.AddForce(moveSpeed * Time.deltaTime * -rb.velocity.normalized * slideCounterMovement);
            return;
        }*/

        //Counter movement
        if (Math.Abs(mag.x) > threshold && Math.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0))
        {
            rb.AddForce(moveSpeed * transform.right * Time.deltaTime * -mag.x * counterMovement);
        }
        if (Math.Abs(mag.y) > threshold && Math.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0))
        {
            rb.AddForce(moveSpeed * transform.forward * Time.deltaTime * -mag.y * counterMovement);
        }


        /*//Limit diagonal running. This will also cause a full stop if sliding fast and un-crouching, so not optimal.
        if (Mathf.Sqrt((Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2))) > maxSpeed)
        {
            float fallspeed = rb.velocity.y;
            Vector3 n = rb.velocity.normalized * maxSpeed;
            rb.velocity = new Vector3(n.x, fallspeed, n.z);
        }*/
    }

    private void Movement()
    {
        //Extra gravity
        rb.AddForce(Vector3.down * Time.deltaTime * 10);

        //Find actual velocity relative to where player is looking
        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;

        //Counteract sliding and sloppy movement
        CounterMovement(x, y, mag);

        //If holding jump && ready to jump, then jump
        if (jumping) Jump();

        //Set max speed
        float maxSpeed = this.maxSpeed;

        //If sliding down a ramp, add force down so player stays grounded and also builds speed
        if (sliding && grounded)
        {
            rb.AddForce(Vector3.down * Time.deltaTime * 3000);
            return;
        }

        //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
        if (x > 0 && xMag > maxSpeed) x = 0;
        if (x < 0 && xMag < -maxSpeed) x = 0;
        if (y > 0 && yMag > maxSpeed) y = 0;
        if (y < 0 && yMag < -maxSpeed) y = 0;

        //Some multipliers
        float multiplier = 1f, multiplierV = 1f;

        // Movement in air
        if (!grounded)
        {
            multiplier = 0.5f;
            multiplierV = 0.5f;
        }
        if (inwater)
        {
            multiplier = 0.6f;
            multiplierV = 0.6f;
            rb.AddForce(Mathf.Abs(Physics.gravity.y) * .9f * rb.mass * Vector3.up);
        }
        if (crouching)
        {
            rb.AddForce(Physics.gravity.y * .5f * rb.mass * Vector3.up);
        }

        // if wallrunning Aplly wall forces to move player
        if (wallrunning && !disableWallrunning)
        {
            //Disable half gravitation
            rb.AddForce(Vector3.up * 14f);

            //Apply forces to move player
            rb.AddForce(WallForward * moveSpeed * 1.55f * Time.deltaTime * multiplier * multiplierV);
            return; //Disable the rest of the function
        }

        //Apply forces to move player
        rb.AddForce(transform.forward * y * moveSpeed * Time.deltaTime * multiplier * multiplierV);
        rb.AddForce(transform.right * x * moveSpeed * Time.deltaTime * multiplier);
        
        //Slope movment:
        
        if (slope)
        {
            Vector3 forward = SlopeForward(transform.forward, normalVector);
            rb.AddForce(forward * y * moveSpeed * Time.deltaTime * multiplier * multiplierV);

            rb.useGravity = false;
        }
        else
        {
            rb.useGravity = true;
        }


    }

    /// <summary>
    /// Find the velocity relative to where the player is looking
    /// Useful for vectors calculations regarding movement and limiting movement
    /// </summary>
    /// <returns></returns>
    public Vector2 FindVelRelativeToLook()
    {
        float lookAngle = transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitue = rb.velocity.magnitude;
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
    }
    
    private void Jump()
    {
        if (inwater)
        {
            Vector3 vel = rb.velocity;
            rb.velocity = new Vector3(vel.x, 3f, vel.z);

            return;
        }
        if (wallrunning && !disableWallrunning)
        {
            //Add jump forces
            rb.AddForce(Vector2.up * jumpForce * 1.5f);

            //Add walljump forces
            rb.AddForce(currWall * jumpForce * 1f);

            //If jumping while falling, reset y velocity.
            Vector3 ve = rb.velocity;
            if (rb.velocity.y < 0.5f)
                rb.velocity = new Vector3(ve.x, 0, ve.z);
            else if (rb.velocity.y > 0)
                rb.velocity = new Vector3(ve.x, ve.y / 2, ve.z);

            return;
        }
        if (grounded)
        {
            jumpCounter -= .4f;
            //Add jump forces
            rb.AddForce(Vector2.up * jumpForce * 1.5f);

            //If jumping while falling, reset y velocity.
            Vector3 vel = rb.velocity;
            if (rb.velocity.y < 0.5f)
                rb.velocity = new Vector3(vel.x, 0, vel.z);
            else if (rb.velocity.y > 0)
                rb.velocity = new Vector3(vel.x, vel.y / 2, vel.z);

        }

        if (_canFly)
        {
            jumpCounter -= Time.deltaTime / 1.5f;
            Vector3 vel = rb.velocity;

            rb.velocity = new Vector3(vel.x, 7.5f, vel.z);
        }
    }

    private void StartSlide()
    {
        if ((new Vector2(rb.velocity.x, rb.velocity.z)).magnitude > 0.5f)
        {
            if (grounded)
            {
                rb.AddForce(transform.forward * slideForce);
            }
        }
    }


    //WallRun
    public bool wallrunning;
    private float _ungroundedTimer;

    private bool IsFloor(Vector3 n)
    {
        float angle = Vector3.Angle(Vector3.up, n);
        return angle < maxSlopeAngle;
    }private bool IsSlope(Vector3 n)
    {
        float angle = Vector3.Angle(Vector3.up, n);
        return angle > maxSlopeAngle && angle < 70;
    }
    private bool IsWall(Vector3 n)
    {
        return Mathf.Abs(Vector3.Dot(n, Vector3.up)) < 0.1f;
    }
    private bool slope;
    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.gameObject.GetComponent<Rigidbody>() == null)
        {
            normalVector = collision.GetContact(0).normal;
            slope = IsSlope(normalVector);

            if (Mathf.Abs(Vector3.Dot(collision.GetContact(0).normal, Vector3.up)) < 0.1f && !grounded)
            {
                currWall = normalVector;
                wallrunning = true;
                //First getting 2 angles, one forward one backwords
                float angle = WallAngle(transform.forward, currWall);
                float condition = 0; //To Handle right and left situations...

                //after that calculate the vector troughout which angle is shorter:
                WallForward = angle < 20 && angle > -20 ? Vector3.zero : angle < -160 || angle > 160 ? Vector3.zero :
                    angle > condition ? Quaternion.AngleAxis(270, Vector3.up) * currWall :
                    angle < condition ? Quaternion.AngleAxis(90, Vector3.up) * currWall : Vector3.zero;

            }
            else
            {
                currWall = Vector3.down;
            }
        }
    }
    private bool disableWallrunning;
    public void Vault()
    {
        disableWallrunning = false;
        if (IsWall(normalVector))
        {
            Vector3 dir = transform.forward;
            Vector3 maxVaultPos = transform.position + Vector3.up * 1f;

            if (!Physics.Raycast(transform.position, dir, 2f, whatIsGround))
                return;

            if (Physics.Raycast(maxVaultPos, dir, 2f, whatIsGround))
                return;

            Vector3 hoverPos= maxVaultPos + dir * 2;
            RaycastHit hit;

            if (!Physics.Raycast(hoverPos, Vector3.down, out hit, 3, whatIsGround))
                return;

            disableWallrunning = true;

            //Disabling Y Velocity and applying new forces..
            var vel = rb.velocity;
            //if (rb.velocity.y < 0) 
                rb.velocity = new Vector3(vel.x, 15, vel.z);

            rb.AddForce(transform.forward * 100);
            //rb.AddForce(transform.up * jumpForce *0.85f);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        wallrunning = false;
        slope = false;
        normalVector = currWall = Vector3.down;
    }
    float WallAngle(Vector3 forward, Vector3 normal)
    {
        float b = Mathf.Atan2(normal.z, normal.x) * 180 / Mathf.PI;

        normal = Quaternion.AngleAxis(b, Vector3.up) * normal;
        forward = Quaternion.AngleAxis(b, Vector3.up) * forward;

        float a = Mathf.Atan2(forward.z, forward.x) * 180 / Mathf.PI;
        a = a > 180 ? -(360 - a) : a;

        return a;
    }
    Vector3 SlopeForward(Vector3 forward, Vector3 normal)
    {
        Vector3 vector = Vector3.ProjectOnPlane(forward, normal).normalized;
        return vector;
    }
    
}
