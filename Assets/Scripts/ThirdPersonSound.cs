using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonSound : MonoBehaviourPunCallbacks
{
    public AudioSource walkSound;
    public AudioSource runSound;
    public AudioSource alienSound;
    public ParticleSystem dust1;
    public ParticleSystem dust2;

    public GameObject playStat;

    public bool isDead=true;

    int cnt = 1;
    bool isDie = false;
    int frame = 0;

    void Awake()
    {
        dust1.Stop();
        dust2.Stop();
    }
    void Start()
    {
        walkSound.Pause();
        runSound.Pause();
        alienSound.Pause();
        dust1.Play();
        dust2.Play();
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

        bool isA = playStat.GetComponent<Player>().IsAlienObject();
        
        if(isA)
            SetSoundAlien(isWalk , isRun);
        else
            SetSound(isWalk , isRun);
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
        if(isDead == false)
        {
            walkSound.Pause();
            runSound.Pause();

            return ;
        }
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
    }
    public void SetSoundAlien(bool isWalk , bool isRun)
    {
        photonView.RPC("MoveSoundAlien", RpcTarget.AllBuffered, photonView.OwnerActorNr,isWalk,isRun);
    }

    [PunRPC]
    public void MoveSoundAlien(int actorNumber,bool isWalk , bool isRun)
    {
        if (photonView.OwnerActorNr != actorNumber)
            return;

        if(isDead == false)
        {
            alienSound.Pause();

            return ;
        }
        bool isAlienWalk = isWalk || isRun;
        
        if(isAlienWalk && !alienSound.isPlaying)
        {
            alienSound.Play();
        }
        else if(!isAlienWalk && alienSound.isPlaying)
        {
            alienSound.Pause();
        }
    }
}