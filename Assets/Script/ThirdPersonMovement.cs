using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 6f;
    public float runSpeedRate = 1.5f;
    public float turnSmoothTime = 0.1f;

    Animator animator;
    void Awake() {
        animator = GetComponentInChildren<Animator>();    
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        bool hasHorizontalInput = !Mathf.Approximately(horizontal,0f);
        bool hasVeritcalInput = !Mathf.Approximately(vertical,0f);

        bool isRun = Input.GetButton("Run");
        bool isWalk = hasHorizontalInput || hasVeritcalInput;


        animator.SetBool("isWalk",isWalk);
        animator.SetBool("isRun",isRun);
    
        Debug.Log(isRun);


        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        // if is pressed either horizontal and vertical, diagnoal(대각선) is calculated root 2 because x and z is 1
        // so diagnoal's speed is faster than others


        if(direction.magnitude >= 0.1f){
            transform.position += direction * speed * (isRun ? runSpeedRate : 1f) * Time.deltaTime;

            transform.LookAt(transform.position + direction);
        }
    }
}
