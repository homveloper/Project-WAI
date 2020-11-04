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

        // bool pushStanding = Input.GetKeyDown(KeyCode.E);

        // if(pushStanding){
        //     pushStand = pushStand ? false : true;
        //     thirdPersonMovement.speed *= 0.5f;
        //     animator.SetTrigger("pushStanding");
        //     animator.SetBool("pushStand",pushStand);
        // }
    }

    void TakeDamage(){
         if(PhotonNetwork.IsConnected)
            if (!photonView.IsMine)
                return;


        float hitStatus = Random.Range(0f,3f);
        print(hitStatus);
        animator.SetTrigger("hit");
        animator.SetFloat("hitStatus",hitStatus);
    }

    void Attack(){
        if(PhotonNetwork.IsConnected)
            if (!photonView.IsMine)
                return;

        animator.SetTrigger("attack");
    }
    
}
