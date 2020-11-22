using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


[RequireComponent(typeof(Player))]
[System.Serializable]
public class Combat : MonoBehaviourPunCallbacks
{
    Player myPlayer;
    Player targetPlayer;
    public PlayerAnimation researcherAnimation;
    public AlienAnimation alienAnimation;

    public float attackSpeed = 1.0f; // 1초 당 타격횟수
    private float cooldown = 0.0f; // 공격 쿨타임

    [SerializeField]
    private float delayTime = 1.5f;   // 공격 후 휴면시간

    private bool inTrigger;

    public delegate void OnAttack();
    public OnAttack OnAttackCallback;

    public List<AudioSource> attackSounds;

    // Transform alienRightHand;

    bool isPunch;

    void Start()
    {
        // alienRightHand = TransformExtention.FirstOrDefault(transform.Find("Alien"),x => x.name == "mixamorig:RightHand");

        attackSounds.ForEach(x => x.Pause());

        myPlayer = GetComponent<Player>();
        researcherAnimation.animator.SetFloat("attackSpeed", attackSpeed);
        alienAnimation.animator.SetFloat("attackSpeed", attackSpeed);

        if (Inventory.instance != null)
            Inventory.instance.onItemChangedCallback += UpdateDamage;
    }

    void Update()
    {
        if(PhotonNetwork.IsConnected)
            if (!photonView.IsMine)
                return;

        cooldown -= Time.deltaTime;

        if (!myPlayer.IsControllable())
            return;

        if (Input.GetButtonDown("Attack"))
        {
            myPlayer.SetMove(false);
            StartCoroutine(IsPlaying());
            Attack(targetPlayer != null ? targetPlayer : null);
        }
    }

    IEnumerator IsPlaying()
    {
        float calibrationTime = 0.5f;
        yield return new WaitForSeconds(calibrationTime);
        
        if(!myPlayer.IsAlienObject()){
            while (true)
            {

                bool isPunch = researcherAnimation.AnimatorIsPlaying("Punch");
                bool isSword = researcherAnimation.AnimatorIsPlaying("Stable Sword Outward Slash");

                if (!isPunch && !isSword)
                {
                    myPlayer.SetMove(true);
                    break;
                }

                yield return null;  //1프레임 마다 체크합니다.
            }
        }else{

            // //애니메이션 실행 동안만 생성되는 무기
            // GameObject club = PhotonNetwork.Instantiate("Item/Club", alienRightHand.position, Quaternion.identity);
            // club.transform.SetParent(alienRightHand);
            // club.transform.localPosition = Vector3.zero;
            // club.transform.localRotation = Quaternion.identity;

            while (true)
            {
                bool isAttack = alienAnimation.AnimatorIsPlaying("Attack");

                if (!isAttack)
                {
                    // foreach(Transform child in alienRightHand){
                    //     photonView.RPC("DestoryClub", RpcTarget.AllBuffered, photonView.OwnerActorNr);
                    // }

                    myPlayer.SetMove(true);
                    break;
                }

                yield return null;  //1프레임 마다 체크합니다.
            }
        }
    }

    IEnumerator WaitUntilTime(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        myPlayer.SetMove(true);
    }

    public void UpdateDamage()
    {
        if (!myPlayer.IsAlienObject())
        {
            float totalDamage = Player.RESEARCHER_DAMAGE;

            if (Inventory.instance != null)
            {
                foreach (Item item in Inventory.instance.items)
                {
                    if (item is InteractableItem)
                    {
                        if (((InteractableItem)item).Itemtype == Itemtype.WEAPHONE)
                        {
                            totalDamage += ((InteractableItem)item).DamageModifier;
                        }
                    }
                }
            }

            myPlayer.damage = totalDamage;
        }
        else
        {
            myPlayer.damage = Player.ALIEN_DAMAGE;
        }

    }

    void OnTriggerStay(Collider other)
    {

        if (!photonView.IsMine)
            return;
        if (other.gameObject == gameObject || !other.CompareTag("HitBox"))
            return;

        inTrigger = true;
        targetPlayer = other.GetComponentInParent<Player>();
    }

    void OnTriggerExit(Collider other)
    {
        if (!photonView.IsMine)
            return;

        if (other.gameObject == gameObject || !other.CompareTag("HitBox"))
            return;

        inTrigger = false;
        targetPlayer = null;
    }
    public void SetAck()
    {
         photonView.RPC("CombatSound", RpcTarget.AllBuffered, photonView.OwnerActorNr);
    }

    [PunRPC]
    public void CombatSound(int actorNumber)
    {
        if (photonView.OwnerActorNr != actorNumber)
            return;

        attackSounds[Random.Range(0, attackSounds.Count)].Play();
    }

    public void Attack(Player target)
    {
        if (cooldown > 0.0f)
            return;

        SetAck();

        if (target != null)
            target.SetHit(myPlayer.damage);

        OnAttackCallback.Invoke();
        cooldown = 1f / attackSpeed;
    }

    // [PunRPC]
    // void DestoryClub(int actorNumber){
    //     GameObject[] weapones = GameObject.FindGameObjectsWithTag("Club");

    //     foreach(GameObject weapone in weapones){
    //         if (photonView.OwnerActorNr == actorNumber){
    //             Destroy(weapone);
    //             break;
    //         }
    //     }
    // }
}
