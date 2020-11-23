using Photon.Pun;
using UnityEngine;
using System.Collections;

public class AnimationController : MonoBehaviourPunCallbacks{

    [SerializeField]
    public Animator animator;

    void Start(){
        GetComponentInParent<Combat>().OnAttackCallback += Attack;
        GetComponentInParent<Player>().onTakeDamageCallback+= TakeDamage;
    }

    void Update()
    {
        if (PhotonNetwork.IsConnected)
            if (!photonView.IsMine)
                return;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        bool hasHorizontalInput = !Mathf.Approximately(horizontal,0f);
        bool hasVeritcalInput = !Mathf.Approximately(vertical,0f);

        bool isRun = hasHorizontalInput || hasVeritcalInput;

        animator.SetBool("isRun",isRun);
    }

     public void StopAnimation(){
        gameObject.GetComponent<Animator>().enabled = false;
    }

    public void PlayAinmation(){
        gameObject.GetComponent<Animator>().enabled = true;
    }

    public void Dead(){
        if(PhotonNetwork.IsConnected)
            if (!photonView.IsMine)
                return;

        animator.SetTrigger("dead");
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

    public bool  AnimatorIsPlaying(){
      return animator.GetCurrentAnimatorStateInfo(0).length >
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    public bool AnimatorIsPlaying(string stateName){
        return AnimatorIsPlaying() && animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }

    public IEnumerator WaitUntilAnimationFinished(string stateName){
        yield return new WaitForSeconds (animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
    }
}