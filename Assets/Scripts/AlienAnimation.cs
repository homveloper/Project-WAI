﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienAnimation : MonoBehaviour
{
    // Start is called before the first frame update

    Animator animator;
    void Awake() {
        animator = GetComponentInChildren<Animator>();    
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        bool hasHorizontalInput = !Mathf.Approximately(horizontal,0f);
        bool hasVeritcalInput = !Mathf.Approximately(vertical,0f);

        bool isRun = hasHorizontalInput || hasVeritcalInput;

        animator.SetBool("isRun",isRun);
    }
}
