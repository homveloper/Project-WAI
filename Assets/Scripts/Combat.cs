using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


[RequireComponent(typeof(Player))]
[System.Serializable]
public class Combat : MonoBehaviourPun
{
    Player myPlayer;
    Player targetPlayer;
    public PlayerAnimation researcherAnimation;
    public AlienAnimation alienAnimation;

    public float attackSpeed = 1.0f; // 1초 당 타격횟수
    private float cooldown = 0.0f; // 공격 쿨타임

    [SerializeField]
    private float delayTime = 1f;   // 공격 후 휴면시간

    private bool inTrigger;


    public delegate void OnAttack();
    public OnAttack OnAttackCallback;

    public List<AudioSource> attackSounds;

    bool isPunch;

    void Start()
    {
        attackSounds.ForEach(x => x.Pause());

        myPlayer = GetComponent<Player>();
        researcherAnimation.animator.SetFloat("attackSpeed", attackSpeed);
        alienAnimation.animator.SetFloat("attackSpeed", attackSpeed);

        if(Inventory.instance != null)
            Inventory.instance.onItemChangedCallback += UpdateDamage;
    }

    void Update(){
        cooldown -= Time.deltaTime;

        if(Input.GetButtonDown("Attack")){
            Attack(targetPlayer != null ? targetPlayer : null);
            myPlayer.SetMove(false);
            StartCoroutine(IsPlaying());
        }
    }
    
    IEnumerator IsPlaying(){

        float calibrationTime = 0.5f;
        yield return new WaitForSeconds(calibrationTime);

        while(true){

            bool isPunch = researcherAnimation.AnimatorIsPlaying("Punch");

            if(!isPunch){
                myPlayer.SetMove(true);
                break;
            }

            yield return null;  //1프레임 마다 체크합니다.
        }
    }

    IEnumerator WaitUntilTime(float delayTime){
        yield return new WaitForSeconds(delayTime);
        myPlayer.SetMove(true);
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

        myPlayer.damage = totalDamage;
    }

    void OnTriggerStay(Collider other){

        if(!photonView.IsMine)
            return;
        if (other.gameObject == gameObject || !other.CompareTag("Player"))
            return;
        
        inTrigger = true;
        targetPlayer = other.GetComponent<Player>();
    }

    void OnTriggerExit(Collider other){
        if(!photonView.IsMine)
            return;

        if (other.gameObject == gameObject || !other.CompareTag("Player"))
            return;

        inTrigger = false;
        targetPlayer = null;
    }

    public void Attack(Player target)
    {
        if (cooldown > 0.0f)
            return;

        attackSounds[Random.Range(0,attackSounds.Count)].Play();

        if (target != null)
            target.SetHit(myPlayer.damage);

        OnAttackCallback.Invoke();
        cooldown = 1f / attackSpeed;
    }


}
