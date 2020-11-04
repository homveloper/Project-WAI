using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Photon.Pun;

public class Player : MonoBehaviourPunCallbacks
{
    public GameObject researcher;
    public GameObject alien;

    private float statHp = 0.0f;
    private float statHpMax = 0.0f;
    private float statO2 = 0.0f;
    private float statO2Max = 0.0f;
    private float statBt = 0.0f;
    private float statBtMax = 0.0f;

    public float hpModifier;
    public float o2Modifier;
    public float btModifierRecharge;
    public float btModifierUse;

    private int meterialWood = 0;
    private int meterialIron = 0;
    private int meterialPart = 0;

    private bool dead = false;

    public GameObject flashlight;

    UI_Inventory uI_Inventory;

    void Start()
    {

        SetHPMax(100);
        SetO2Max(100);
        SetBtMax(100);

        SetHP(100);
        SetO2(100);
        SetBt(100);

        SetWood(0);
        SetIron(0);
        SetPart(0);

        if (PhotonNetwork.IsConnected)
        {
            if (photonView.IsMine)
            {
                uI_Inventory = GameObject.Find("UI_Inventory").GetComponent<UI_Inventory>();
                uI_Inventory.UpdateInventory();
            }
        }

        flashlight.SetActive(false);

    }

    void Update()
    {
        if (PhotonNetwork.IsConnected)
            if (!photonView.IsMine)
                return;

        // 플래시라이트 (F)
        if (Input.GetKeyDown(KeyCode.F))
        {
            SetFlash();
        }

        // 플래시라이트 (F)
        if (Input.GetKeyDown(KeyCode.X))
        {
            if(alien.activeSelf == true)
            {
                alien.SetActive(false);
                researcher.SetActive(true);
            }
            else
            {
                alien.SetActive(true);
                researcher.SetActive(false);
            }
        }

        // 디버그 중에는 스텟 차감 및 사망 발생하지 않음
        if (GameManager.DEBUG_GAME == false)
            return;

        // 산소 차감
        SetO2(GetO2() - Time.deltaTime * o2Modifier);

        // 라이트 회복/차감
        if (IsFlash() == false)
        {
            SetBt(GetBt() + Time.deltaTime * btModifierRecharge);
        }
        else if (IsFlash() == true)
        {
            SetBt(GetBt() - Time.deltaTime * btModifierUse);
        }

        // 배터리 부족으로 라이트 꺼짐
        if (IsFlash() == true && GetBt() <= 0)
            SetFlash(false);

        // 체력 차감
        if (GetO2() <= 0){
            SetHP(GetHP() - Time.deltaTime * hpModifier);
        }

        // 체력 부족으로 사망
        if (GetHP() <= 0 && dead == false) 
        {
            Debug.Log("dead");
            SetDead();
        }
    }

    [PunRPC]
    public void OnFlash(int actorNumber, bool val) // RPC로 플래시라이트 사용을 알림
    {
        if (photonView.OwnerActorNr != actorNumber)
            return;
        
        flashlight.SetActive(val);
    }

    [PunRPC]
    public void OnDead(int actorNumber) // RPC로 캐릭터 사망을 알림
    {
        if (photonView.OwnerActorNr != actorNumber)
            return;

        dead = true;
        SetMove(false);
        
        GetComponent<PlayerAnimation>().animator.SetTrigger("dead");
    }

    // 이하 Get / Set 메소드 --------------------------------------
    public float GetHP()
    {
        return this.statHp;
    }

    public float GetHPMax()
    {
        return this.statHpMax;
    }

    public float GetO2()
    {
        return this.statO2;
    }

    public float GetO2Max()
    {
        return this.statO2Max;
    }

    public float GetBt()
    {
        return this.statBt;
    }

    public float GetBtMax()
    {
        return this.statBtMax;
    }

    public int GetWood()
    {
        return this.meterialWood;
    }

    public int GetIron()
    {
        return this.meterialIron;
    }

    public int GetPart()
    {
        return this.meterialPart;
    }

    public void SetHP(float hp)
    {
        this.statHp = hp;

        if (this.statHp > this.statHpMax) this.statHp = this.statHpMax;
        if (this.statHp < 0) this.statHp = 0;
    }

    public void SetHPMax(float hpmax)
    {
        this.statHpMax = hpmax;

        if (this.statHp > this.statHpMax) this.statHp = this.statHpMax;
        if (this.statHp < 0) this.statHp = 0;
    }

    public void SetO2(float o2)
    {
        this.statO2 = o2;

        if (this.statO2 > this.statO2Max) this.statO2 = this.statO2Max;
        if (this.statO2 < 0) this.statO2 = 0;
    }

    public void SetO2Max(float o2max)
    {
        this.statO2Max = o2max;

        if (this.statO2 > this.statO2Max) this.statO2 = this.statO2Max;
        if (this.statO2 < 0) this.statO2 = 0;
    }

    public void SetBt(float bt)
    {
        this.statBt = bt;

        if (this.statBt > this.statBtMax) this.statBt = this.statBtMax;
        if (this.statBt < 0) this.statBt = 0;
    }

    public void SetBtMax(float btmax)
    {
        this.statBtMax = btmax;

        if (this.statBt > this.statBtMax) this.statBt = this.statBtMax;
        if (this.statBt < 0) this.statBt = 0;
    }

    public void SetWood(int wood)
    {
        this.meterialWood = wood;
    }

    public void SetIron(int iron)
    {
        this.meterialIron = iron;
    }

    public void SetPart(int part)
    {
        this.meterialPart = part;
    }

    public bool IsDead() // 캐릭터 사망 여부
    {
        return this.dead;
    }

    public void SetDead() // 캐릭터 사망 설정
    {
        dead = true;
        SetMove(false);
        GetComponent<PlayerAnimation>().animator.SetTrigger("dead");

        Inventory.instance.DropAll();
        photonView.RPC("OnDead", RpcTarget.AllBuffered, photonView.OwnerActorNr);
    }

    public bool IsMove() // 캐릭터 이동 여부
    {
        return GetComponent<ThirdPersonMovement>().controllable;
    }

    public void SetMove(bool val) // 캐릭터 이동 설정
    {
        researcher.GetComponent<PlayerAnimation>().enabled = val;
        alien.GetComponent<AlienAnimation>().enabled = val;
        GetComponent<ThirdPersonMovement>().controllable = val;
        GetComponent<ThirdPersonSound>().enabled = val;
    }

    public bool IsFlash() // 캐릭터 라이트 사용 여부
    {
        return flashlight.activeSelf;
    }

    public void SetFlash() // 캐릭터 라이트 설정 (스위칭)
    {
        if (flashlight.activeSelf == false && GetBt() <= 0) // 배터리가 없으면 라이트를 켤 수 없음
            return;

        if (flashlight.activeSelf == true) flashlight.SetActive(false);
        else flashlight.SetActive(true);

        photonView.RPC("OnFlash", RpcTarget.AllBuffered, photonView.OwnerActorNr, flashlight.activeSelf);
    }

    public void SetFlash(bool val) // 캐릭터 라이트 설정 (매뉴얼)
    {
        if (val == true && GetBt() <= 0) // 배터리가 없으면 라이트를 켤 수 없음
            return;

        flashlight.SetActive(val);

        photonView.RPC("OnFlash", RpcTarget.AllBuffered, photonView.OwnerActorNr, flashlight.activeSelf);
    }
}
