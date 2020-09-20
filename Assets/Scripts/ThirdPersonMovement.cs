﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{

    public CharacterController controller;

    public float speed = 6f;
    public float runSpeedRate = 1.5f;
    public float turnSmoothTime = 0.1f;
    public float turnSmoothVelocity;
    public float gravity = -9.81f;

    // public float jumpHeight = 3f;

    // public Transform groundCheck;
    // public float groundDistance = 0.4f;
    // public LayerMask groundMask;

    // public bool isGrounded;
    // Vector3 velocity;

    // Update is called once per frame
    void Update()
    {
        // isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // if(isGrounded && velocity.y < 0){
        //     velocity.y = -2f;
        // }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
    
        bool isRun = Input.GetButton("Run");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        // if is pressed either horizontal and vertical, diagnoal(대각선) is calculated root 2 because x and z is 1
        // so diagnoal's speed is faster than others

        if(direction.magnitude >= 0.1f){

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            
            transform.rotation = Quaternion.Euler(0f,angle,0f);
            controller.Move(direction * speed * (isRun ? runSpeedRate : 1f) * Time.deltaTime);
        }

        // bool isJump = Input.GetButtonDown("Jump");
        // animator.SetBool("isJump",isJump);

        // if(isJump && isGrounded){
        //     velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        // }

        // velocity.y += gravity * Time.deltaTime;
        // controller.Move(velocity);
    }
}
