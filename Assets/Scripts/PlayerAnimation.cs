using Photon.Pun;
using UnityEngine;

public class PlayerAnimation : MonoBehaviourPunCallbacks
{
    [SerializeField]
    public Animator animator;

    void SetPlayerDead(){
        animator.SetTrigger("dead");
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

        animator.SetBool("isWalk",isWalk);
        animator.SetBool("isRun",isRun);
    }
    
}
