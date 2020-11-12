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

    public void OnCasting(){
        if(PhotonNetwork.IsConnected)
            if (!photonView.IsMine)
                return;

        animator.SetTrigger("casting");
    }

    public void EndAnimation(){
        if(PhotonNetwork.IsConnected)
            if (!photonView.IsMine)
                return;
                
        animator.SetTrigger("end");
    }
    
    bool AnimatorIsPlaying(int layer = 0){
        return animator.GetCurrentAnimatorStateInfo(layer).normalizedTime < 1.0f;
    }

    public bool AnimatorIsPlaying( string stateName,int layer = 0){
        Debug.Log("AnimatorIsPlaying : " + AnimatorIsPlaying(layer));
        Debug.Log("current Animation " + stateName + " is " + animator.GetCurrentAnimatorStateInfo(layer).IsName(stateName) + " : " + animator.GetCurrentAnimatorClipInfo(layer)[0].clip.name);
        return AnimatorIsPlaying(layer) && animator.GetCurrentAnimatorStateInfo(layer).IsName(stateName);
    }
}
