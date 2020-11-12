using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


[RequireComponent(typeof(Player))]
[System.Serializable]
public class Combat : MonoBehaviourPun
{
    Player player;
    public PlayerAnimation researcherAnimation;
    public AlienAnimation alienAnimation;

    public float attackSpeed = 1.0f; // 1초 당 타격횟수
    private float cooldown = 0.0f; // 공격 쿨타임

    public delegate void OnAttack();
    public OnAttack OnAttackCallback; 

    public AudioSource playerAck1;

    public AudioSource playerAck2;

    int cnt = 0;

    void Start()
    {
        playerAck1.Pause();
        playerAck2.Pause();
        player = GetComponent<Player>();
        researcherAnimation.animator.SetFloat("attackSpeed", attackSpeed);
        alienAnimation.animator.SetFloat("attackSpeed", attackSpeed);

        if(Inventory.instance != null)
            Inventory.instance.onItemChangedCallback += UpdateDamage;
        
    }

    void Update(){
        cooldown -= Time.deltaTime;

        if (Input.GetButtonDown("Attack")){
            if(cnt == 0)
            {
                playerAck1.Play();
                cnt++;
            }
            else
            {
                playerAck2.Play();
                cnt--;
            }
            Attack(null);
        }
    }

    void UpdateDamage(){
        float totalDamage = Player.RESEARCHER_DAMAGE;

        if(Inventory.instance != null){     
            foreach(Item item  in Inventory.instance.items){
                if(item is InteractableItem){
                    if(((InteractableItem)item).Itemtype == Itemtype.WEAPHONE){
                        totalDamage += ((InteractableItem)item).DamageModifier;
                    }
                }
            }
        }

        player.damage = totalDamage;
    }

    void OnTriggerStay(Collider other){

        if(!photonView.IsMine)
            return;

        if (other.gameObject == gameObject || !other.CompareTag("Player"))
            return;

        if (Input.GetButtonDown("Attack")){

            if(player.IsAlienObject()){
                player.SetMove(false);
                Attack(other.GetComponent<Player>());

                player.SetMove(true);
            }
            else{
                Attack(other.GetComponent<Player>());
            }
        }
    }

    public void Attack(Player target)
    {
        if (cooldown > 0.0f)
            return;

        if (target != null)
            target.SetHit(player.damage);

        OnAttackCallback.Invoke();
        cooldown = 1f / attackSpeed;
    }


}
