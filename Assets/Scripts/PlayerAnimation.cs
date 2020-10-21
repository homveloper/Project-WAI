using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update    Animator animator;

    [SerializeField]
    public Animator animator;
    public Player player;
    
    void Awake() 
    {
        animator = GetComponentInChildren<Animator>();
        player = GetComponent<Player>();

        player.onPlayerDeadCallback += SetPlayerDead;
    }

    void SetPlayerDead(){
        animator.SetTrigger("dead");
    }

    // Update is called once per frame
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

        animator.SetBool("isWalk",isWalk);
        animator.SetBool("isRun",isRun);
    }
    
}
