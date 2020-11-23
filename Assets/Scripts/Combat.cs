using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


[System.Serializable]
public class Combat : MonoBehaviourPunCallbacks
{
    Player myPlayer;
    Player targetPlayer;

    public ResearcherAnimation researcherAnimation;
    public AlienAnimation alienAnimation;

    public float researcherAttackSpeed = 1.0f; // 1초 당 타격횟수
    public float alienAttackSpeed = 1.0f;

    [SerializeField]
    private float alienFirstDelayTime = 1.0f;
    [SerializeField]
    private float alienSecondDelayTime = 0.3f;

    [SerializeField]
    private float alienEndDelayTime = 0.4f;


    private float cooldown = 0.0f; // 공격 쿨타임

    [SerializeField]
    private float delayTime = 1.5f;   // 공격 후 휴면시간

    private bool inTrigger;

    public delegate void OnAttack();
    public OnAttack OnAttackCallback;

    public List<AudioSource> attackSounds;

    public Collider box1; // 인간
    public Collider box2; // 외계인
    // Transform alienRightHand;

    bool isPunch;

    void Start()
    {
        // alienRightHand = TransformExtention.FirstOrDefault(transform.Find("Alien"),x => x.name == "mixamorig:RightHand");
        box1.enabled = true;
        box2.enabled = false;
        attackSounds.ForEach(x => x.Pause());

        myPlayer = GetComponent<Player>();
        researcherAnimation.animator.SetFloat("attackSpeed", researcherAttackSpeed);
        alienAnimation.animator.SetFloat("attackSpeed", alienAttackSpeed);

        if (Inventory.instance != null)
            Inventory.instance.onItemChangedCallback += UpdateDamage;
    }

    void Update()
    {
        if (PhotonNetwork.IsConnected)
            if (!photonView.IsMine)
                return;

        if (myPlayer.IsAlienObject() && !box2.enabled)
        {
            box1.enabled = false;
            box2.enabled = true;
        }
        else if (!myPlayer.IsAlienObject() && box2.enabled)
        {
            box1.enabled = true;
            box2.enabled = false;
        }

        cooldown -= Time.deltaTime;

        if (!myPlayer.IsControllable())
            return;

        if (Input.GetButtonDown("Attack"))
        {
            StartCoroutine(Playing());
        }
    }

    IEnumerator Playing()
    {
        // 연구원
        if (!myPlayer.IsAlienObject())
        {
            OnAttackCallback.Invoke();

            if (Inventory.instance.HasWeaphone)
            {
                yield return new WaitForSeconds(0.18f);
            }
            else
            {
                yield return new WaitForSeconds(0.13f);
            }

            Attack(targetPlayer != null ? targetPlayer : null);

            /*
                while(attack animation is playing)
                    if attack is punch
                        in Cross Punch Animation Clip set EventHandler SetMove(false)

                    if attack is knife
                        in Stable Sword Outward Slash Aniatmion Clip set EventHandler SetMove(false)

                EventHandler's SetMove(true)
            */

            // 외계인
        }
        else
        {
            OnAttackCallback.Invoke();
            yield return new WaitForSeconds(alienFirstDelayTime);
            myPlayer.SetMove(false);

            yield return new WaitForSeconds(alienSecondDelayTime);
            Attack(targetPlayer != null ? targetPlayer : null);

            yield return new WaitForSeconds(alienEndDelayTime);
            myPlayer.SetMove(true);
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

        if (!other.CompareTag("HitBox") || other.transform.parent.gameObject == gameObject)
            return;

        inTrigger = true;
        targetPlayer = other.GetComponentInParent<Player>();
    }

    void OnTriggerExit(Collider other)
    {
        if (!photonView.IsMine)
            return;

        if (!other.CompareTag("HitBox") || other.transform.parent.gameObject == gameObject)
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


        if (!myPlayer.IsAlienObject())
            cooldown = 1f / researcherAttackSpeed;
        else
            cooldown = 1f / alienAttackSpeed;

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
