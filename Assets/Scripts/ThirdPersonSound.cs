using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonSound : MonoBehaviourPunCallbacks
{
    public AudioSource walkSound;
    public AudioSource runSound;
    public ParticleSystem dust1;
    public ParticleSystem dust2;

    int cnt = 1;
    bool isDie = false;
    void Start()
    {
        dust1.Stop();
        dust2.Stop();
        walkSound.Pause();
        runSound.Pause();
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


        SetSound(isWalk , isRun);
    }
    void LateUpdate()
    {
        if(cnt == 1)
        {
            dust1.Play();
            dust2.Play();
            cnt--;
        }
    }
        
    public void SetSound(bool isWalk , bool isRun)
    {
        photonView.RPC("MoveSound", RpcTarget.AllBuffered, photonView.OwnerActorNr,isWalk,isRun);
    }

    [PunRPC]
    public void MoveSound(int actorNumber,bool isWalk , bool isRun)
    {
        if (photonView.OwnerActorNr != actorNumber)
            return;

        if(isRun)
            isWalk = false;

        if(isRun && !runSound.isPlaying)
        {
            walkSound.Pause();
            runSound.Play();
            
        }
        else if (isWalk && !walkSound.isPlaying)
        {
            runSound.Pause();
            walkSound.Play();
        }
        else if(!isRun && runSound.isPlaying)
        {
            runSound.Pause();
        }
        else if(!isWalk && walkSound.isPlaying)
        {
            walkSound.Pause();
        }

        // Debug.Log(isWalk +"   "+dust1.isStopped);

        // if((isWalk == true && dust1.isStopped == true) ||(isRun == true && dust1.isStopped == true))
        // {
        //     dust1.Play();
        //     dust2.Play();
        // }
        // else if((isWalk == false && dust1.isStopped == false) && (isRun == false && dust1.isStopped == false))
        // {
        //     dust1.Stop();
        //     dust2.Stop();
        // }
    }
}