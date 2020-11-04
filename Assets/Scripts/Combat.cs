using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;



[RequireComponent(typeof(Player))]
public class Combat : MonoBehaviourPun
{
    public float attackSpeed{get; set;} = 1f;
    private float attackCooldown = 0f;
    Player myStats;

    public delegate void OnAttack();
    public OnAttack OnAttackCallback; 

    // Start is called before the first frame update
    void Start()
    {
        myStats = GetComponent<Player>();
    }

    void Update(){
        attackCooldown -= Time.deltaTime;

        if(Input.GetButtonDown("Attack") && attackCooldown <= 0f){
            OnAttackCallback.Invoke();
            attackCooldown = 1f / attackSpeed;
        }
    }

    void OnTriggerStay(Collider other){

        if(other.gameObject != gameObject){
            Player targetPlayer = other.GetComponent<Player>();

            if(Input.GetButtonDown("Attack") && targetPlayer != null ){
                Attack(targetPlayer);
            }
        }

 

    }
    public void Attack(Player targetPlayer){
        if (PhotonNetwork.IsConnected)
            if (!photonView.IsMine)
                return;

        if(attackCooldown <= 0f){
            OnAttackCallback.Invoke();

            // photonView.RPC("TakeDamage",RpcTarget.AllBuffered,myStats.damage);
            // targetPlayer.TakeDamage(myStats.damage);
            print("Attack " + targetPlayer.transform.name + " HP : " + targetPlayer.GetHP());
            attackCooldown = 1f / attackSpeed;
        }
    }


}
