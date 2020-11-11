using Photon.Pun;
using UnityEngine;

public class PlayerAnimation : MonoBehaviourPunCallbacks
{
    [SerializeField]
    public Animator animator;

    [SerializeField]
    private bool isWalk;
    [SerializeField]
    private bool isRun;

    public bool IsWalk{
        get =>isWalk;
    }

    public bool IsRun{
        get =>isRun;
    }

    void SetPlayerDead(){
        animator.SetTrigger("dead");
    }

    void Start(){
        GetComponentInParent<Combat>().OnAttackCallback += Attack;
        GetComponentInParent<Player>().onTakeDamageCallback+= TakeDamage;
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

        isWalk = hasHorizontalInput || hasVeritcalInput;
        isRun = Input.GetButton("Run") && isWalk;

        animator.SetBool("isWalk",isWalk);
        animator.SetBool("isRun",isRun);

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
        animator.SetTrigger("hit");
        animator.SetFloat("hitStatus",hitStatus);
    }

    void Attack(){
        if(PhotonNetwork.IsConnected)
            if (!photonView.IsMine)
                return;

        animator.SetTrigger("attack");
    }

    void OnCasting(){
        if(PhotonNetwork.IsConnected)
            if (!photonView.IsMine)
                return;

        animator.SetTrigger("onCasting");
    }

    void EndAnimation(){
        if(PhotonNetwork.IsConnected)
            if (!photonView.IsMine)
                return;
                
        animator.SetTrigger("end");
    }
    
    bool AnimatorIsPlaying(){
     return animator.GetCurrentAnimatorStateInfo(0).length >
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    public bool AnimatorIsPlaying(string stateName){
        return AnimatorIsPlaying() && animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }

    public AnimatorStateInfo PrintAnimatorIsPlaying(){
        return animator.GetCurrentAnimatorStateInfo(0);
    }
}
