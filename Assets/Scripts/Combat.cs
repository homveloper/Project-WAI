using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


[RequireComponent(typeof(Player))]
public class Combat : MonoBehaviourPun
{
    Player player;
    private float speed {get; set;} = 1.0f; // 공격 속도
    private float cooldown = 0.0f; // 공격 쿨타임

    public delegate void OnAttack();
    public OnAttack OnAttackCallback; 

    void Start()
    {
        player = GetComponent<Player>();
    }

    void Update(){
        cooldown -= Time.deltaTime;

        if (Input.GetButtonDown("Attack"))
            Attack(null);
    }

    void OnTriggerStay(Collider other){

        if(!photonView.IsMine)
            return;

        if (other.gameObject == gameObject || !other.CompareTag("Player"))
            return;

        if (Input.GetButtonDown("Attack"))
            Attack(other.GetComponent<Player>());
    }

    public void Attack(Player target)
    {
        if (cooldown > 0.0f)
            return;

        if (target != null)
            target.SetHit(player.damage);

        OnAttackCallback.Invoke();
        cooldown = 1f / speed;
    }
}
