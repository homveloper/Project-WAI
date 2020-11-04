﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


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


        if (Input.GetKeyDown(KeyCode.R))
            SetAttack();
    }

    void OnTriggerStay(Collider other){

        if(!photonView.IsMine)
            return;

        if(other.gameObject != gameObject && other.tag == "Player"){
            Player targetStat = other.GetComponent<Player>();

            if(Input.GetButtonDown("Attack") && targetStat != null ){
                Photon.Realtime.Player targetPlayer = other.gameObject.GetComponent<PhotonView>().Owner;
                print(other);
                print(targetPlayer);
                Attack(targetStat, other.gameObject.GetComponent<PhotonView>().Owner.ActorNumber);
            }
        }

    }

    public void Attack(Player targetStat, int actorNumber)
    {
        if (PhotonNetwork.IsConnected)
            if (!photonView.IsMine)
                return;

        if(attackCooldown <= 0f){
            OnAttackCallback.Invoke();
            photonView.RPC("TakeDamage", RpcTarget.All, actorNumber, myStats.damage);
            // targetPlayer.TakeDamage(myStats.damage);
            print("Attack " + targetStat.transform.name + " HP : " + targetStat.GetHP());
            attackCooldown = 1f / attackSpeed;
        }
    }

    public void SetAttack()
    {
        photonView.RPC("Damaged", RpcTarget.All, photonView.OwnerActorNr);
    }


}