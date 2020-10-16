﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonSound : MonoBehaviourPunCallbacks
{
    public AudioSource walkSound;
    public AudioSource runSound;
    public ParticleSystem dust1;
    public ParticleSystem dust2;

    void Start()
    {
        dust1.Stop();
        dust2.Stop();
    }
    
    void Update()
    {
        if(PhotonNetwork.IsConnected)
            if (!photonView.IsMine)
                return;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        bool hasHorizontalInput = !Mathf.Approximately(horizontal,0f);

        bool hasVeritcalInput = !Mathf.Approximately(vertical,0f);

        bool isWalk = hasHorizontalInput || hasVeritcalInput;
        bool isRun = Input.GetButton("Run") && isWalk;

        if(isRun && !runSound.isPlaying)
        {
            runSound.Play();
          
        }
        else if (isWalk && !walkSound.isPlaying)
        {
            walkSound.Play();
        }
        else
        {
            runSound.Pause();
            walkSound.Pause();
        }

        if(!isWalk)
        {
            dust1.Play();
            dust2.Play();
        }
        else
        {
            if(!dust1.isPlaying && !dust2.isPlaying)
            {
                dust1.Stop();
                dust2.Stop();
            }
        }
    }
}