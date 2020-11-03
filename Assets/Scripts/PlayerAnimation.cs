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
    public Combat combat;

    public ThirdPersonMovement thirdPersonMovement;
    private bool pushStand;
    
    void Awake() 
    {
        animator = GetComponentInChildren<Animator>();
        player = GetComponent<Player>();
        combat = GetComponent<Combat>();

        player.onTakeDamageCallback +=  TakeDamage;
        combat.OnAttackCallback += Attack;
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
        bool isRun = isWalk && Input.GetButton("Run");

        animator.SetBool("isWalk",isWalk == true ? true : false);
        animator.SetBool("isRun",isRun == true ? true : false);

        bool pushStanding = Input.GetKeyDown(KeyCode.E);
        if(pushStanding){
            pushStand = pushStand ? false : true;
            thirdPersonMovement.speed *= 0.5f;
            animator.SetTrigger("pushStanding");
            animator.SetBool("pushStand",pushStand);
        }
    }

    void TakeDamage(){
        if(PhotonNetwork.IsConnected)
            if (!photonView.IsMine)
                return;

        float hitStatus = Random.Range(0f,3f);
        print(hitStatus);
        animator.SetFloat("hitStatus",hitStatus);
        animator.SetTrigger("hit");
    }

    void Attack(){
        if(PhotonNetwork.IsConnected)
            if (!photonView.IsMine)
                return;

        animator.SetTrigger("attack");
    }
}
