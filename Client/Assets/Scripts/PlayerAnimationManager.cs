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

public class PlayerAnimationManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float playerMoveSpeed;

    public bool IsLocal
    {
        get
        {
            return GetComponent<Player>().IsLocal;
        }
    }

    private Vector3 lastPosition;
    private Vector3 velocity;

    public PlayerAnim state;

    //Verihables:
    private float movement;
    private float crouchMovement;
    private bool crouching, was_crouching;
    private bool sliding, was_sliding;
    private bool swiming, was_swiming;

    private bool IsMoving { get
        {
            return new Vector3(velocity.x, 0, velocity.z).magnitude > 0;
        } 
    }

    public void Shoot()
    {
        animator.Play("GShoot", -1,0);
    }

    public void StopAnimation()
    {
        animator.enabled = false;
        animator.enabled = true;
    }
    public void Event()
    {
        StopAnimation();
    }
    private void Update()
    {
        velocity = (transform.position - lastPosition) / Time.deltaTime;
        sliding = swiming = false;
        animator.SetLayerWeight(1, 0.8f);
        switch (state)
        {

            case PlayerAnim.idle:
                movement = Mathf.Lerp(movement, 0, Time.deltaTime * 8f);
                animator.Play("Movement");
                crouching = false;
                break;
            case PlayerAnim.walking:
                movement = Mathf.Lerp(movement, 0.5f, Time.deltaTime * 8f);
                animator.Play("Movement");
                crouching = false;
                break;
            case PlayerAnim.running:
                movement = Mathf.Lerp(movement, 1f, Time.deltaTime * 8f);
                animator.Play("Movement");
                crouching = false;
                break;
            case PlayerAnim.crouching:
                crouchMovement = Mathf.Lerp(crouchMovement, 0, Time.deltaTime * 8f);
                animator.Play("Crouch Movement");
                crouching = true;
                break;
            case PlayerAnim.chrouchwalking:
                crouchMovement = Mathf.Lerp(crouchMovement, 1, Time.deltaTime * 8f);
                animator.Play("Crouch Movement");
                crouching = true;
                break;
            case PlayerAnim.swiming:
                crouching = false;
                animator.Play("Swim");
                animator.SetLayerWeight(1, 0f);
                swiming = true;
                break;
            case PlayerAnim.sliding:
                crouching = false;
                animator.Play("Slide");
                animator.SetLayerWeight(1, 0f);
                sliding = true;
                break;
            default:
                crouching = false;
                break;

        }
        if (!crouching && crouching)
        {
            StopAnimation();
            animator.Play("Crouch Movement");
        }

        if (!was_sliding && sliding)
        {
            StopAnimation();
            animator.Play("Slide");
        }
        if (!was_swiming && swiming)
        {
            StopAnimation();
            animator.Play("Swim");
        }
        animator.SetFloat("Movement", movement);
        animator.SetFloat("CrouchMovement", crouchMovement);
    }
    private void LateUpdate()
    {
        lastPosition = transform.position;
        was_crouching = crouching;
        was_sliding = sliding;
        was_swiming = swiming;
    }
    
}
