using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Photon.Pun;
using System;

public class Player : MonoBehaviourPunCallbacks
{
    // 오브젝트
    public GameObject researcher;
    public GameObject alien;
    public GameObject flashlight;

    // 스텟 증감치
    public float modHp;
    public float modHpAlienHeal;
    public float modO2;
    public float modO2Run;
    public float modBt;
    public float modBtRecharge;
    public float damage { set; get; } = 5f;

    // 연구원 스텟
    public float statHp;
    public float statHpMax;
    public float statO2;
    public float statO2Max;
    public float statBt;
    public float statBtMax;

    // 외계인 스텟
    public float statHpAlien;
    public float statHpMaxAlien;
    
    // 재료
    public int meterialWood;
    public int meterialIron;
    public int meterialPart;

    // 기타
    private PlayerColorPalette colorPalette;
    private UI_Inventory uI_Inventory;
    public delegate void OnTakeDamage();
    public OnTakeDamage onTakeDamageCallback;

    private void Awake()
    {
        colorPalette = Instantiate(Resources.Load<PlayerColorPalette>("PlayerColorPalette"));
    }

    void Start()
    {
        alien.SetActive(false);
        flashlight.SetActive(false);

        if (PhotonNetwork.IsConnected)
            if (photonView.IsMine)
                return;

        uI_Inventory = GameObject.Find("UI_Inventory").GetComponent<UI_Inventory>();
        uI_Inventory.UpdateInventory();
    }

    void Update()
    {
        if (PhotonNetwork.IsConnected)
            if (!photonView.IsMine)
                return;

        // 플래시라이트 (F)
        if (Input.GetKeyDown(KeyCode.F))
            SetFlash();

        // 변신 해제 (X)
        if (Input.GetKeyDown(KeyCode.X))
            SetTransform(false);

        // 체력 차감
        if (GetO2() <= 0)
            SetHP(GetHP() - Time.deltaTime * modHp);

        // 산소 차감
        if (GetComponent<ThirdPersonMovement>().IsRun() == true)
            SetO2(GetO2() - Time.deltaTime * modO2Run);
        else
            SetO2(GetO2() - Time.deltaTime * modO2);

        // 배터리 차감
        if (IsFlash() == true)
            SetBt(GetBt() - Time.deltaTime * modBt);

        // 체력 회복 (외계인)
        if (IsAlienObject() == true)
            SetHP(GetHP(true) + Time.deltaTime * modHpAlienHeal, true);

        // 배터리 회복
        if (IsFlash() == false)
            SetBt(GetBt() + Time.deltaTime * modBtRecharge);

        // 체력 부족
        if (GetHP() <= 0 && IsDead() == false)
            if (IsAlienPlayer() == true && IsAlienObject() == false)
                SetTransform(false);
            else
                SetDead();

        // 배터리 부족
        if (IsFlash() == true && GetBt() <= 0)
            SetFlash(false);

        //if(Input.GetKeyDown(KeyCode.Y)){
            //TakeDamage(,10);
        //}
    }

    // ---------------------------------------------------------------------------------------------------
    // # 포톤 메시지 메소드
    // ---------------------------------------------------------------------------------------------------
    [PunRPC]
    public void OnDead(int actorNumber) // 사망
    {
        if (photonView.OwnerActorNr != actorNumber)
            return;

        SetMove(false);
        SetFlash(false);
        researcher.GetComponent<PlayerAnimation>().animator.SetTrigger("dead");
    }
    [PunRPC]
    public void OnTransform(int actorNumber, bool val)
    {
        if (photonView.OwnerActorNr != actorNumber)
            return;

        SetMove(true);
        SetFlash(false);
        researcher.SetActive(val);
        alien.SetActive(!val);
        GetComponent<ThirdPersonMovement>().alien = !val;
    }
    [PunRPC]
    public void OnFlash(int actorNumber, bool val) // 플래시라이트
    {
        if (photonView.OwnerActorNr != actorNumber)
            return;

        if (val == true)
            SetBt(GetBt() - 10);
        
        flashlight.SetActive(val);
    }
    [PunRPC]
    public void TakeDamage(Photon.Realtime.Player targetPlayer, float damage)
    {
        Debug.Log("asdfasdfasdf");
        if (photonView.Owner != targetPlayer)
        {
            onTakeDamageCallback.Invoke();
            return;
        }

        SetHP(GetHP() - damage);

        onTakeDamageCallback.Invoke();

        Debug.Log(transform.name + " takes " + damage + " damage.");

        if (statHp <= 0f)
        {
            Debug.Log("dead");
            SetDead();
        }
    }
    // ---------------------------------------------------------------------------------------------------
    // # GET 메소드
    // ---------------------------------------------------------------------------------------------------
    public float GetHP() // 체력 (겉보기 현재 수치)
    {
        if (IsAlienObject() == true) return this.statHpAlien;
        else return this.statHp;
    }
    public float GetHP(bool statAlien) // 체력 (지정 현재 수치)
    {
        if (statAlien == true) return this.statHpAlien;
        else return this.statHp;
    }
    public float GetHPMax() // 체력 (겉보기 최대 수치)
    {
        if (IsAlienObject() == true) return this.statHpMaxAlien;
        else return this.statHpMax;
    }
    public float GetHPMax(bool statAlien) // 체력 (지정 최대 수치)
    {
        if (statAlien == true) return this.statHpMaxAlien;
        else return this.statHpMax;
    }
    public float GetO2() // 산소 (현재 수치)
    {
        return this.statO2;
    }
    public float GetO2Max() // 산소 (최대치)
    {
        return this.statO2Max;
    }
    public float GetBt() // 배터리 (현재 수치)
    {
        return this.statBt;
    }
    public float GetBtMax() // 배터리 (최대치)
    {
        return this.statBtMax;
    }
    public int GetWood() // 재료 (나무)
    {
        return this.meterialWood;
    }
    public int GetIron() // 재료 (철)
    {
        return this.meterialIron;
    }
    public int GetPart() // 재료 (부품)
    {
        return this.meterialPart;
    }
    public bool IsAlienPlayer() // 외계인 역할 여부
    {
        ExitGames.Client.Photon.Hashtable prop = photonView.Owner.CustomProperties;

        if (prop.ContainsKey("isAlien") == true && (bool)prop["isAlien"] == true) return true;
        else return false;
    }
    public bool IsAlienObject() // 외계인 상태 여부
    {
       return alien.activeSelf;
    }
    public bool IsDead() // 사망 여부
    {
        if (researcher.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Dying Backwards") == true)
            return true;
        else
            return false;
    }
    public bool IsControllable() // 조작 가능 여부
    {
        return GetComponent<ThirdPersonMovement>().controllable;
    }
    public bool IsFlash() // 라이트 사용 여부
    {
        return flashlight.activeSelf;
    }
    // ---------------------------------------------------------------------------------------------------
    // # SET 메소드
    // ---------------------------------------------------------------------------------------------------
    public void SetHP(float hp) // 체력 (겉보기 현재 수치)
    {
        if (IsAlienObject() == true)
        {
            this.statHpAlien = hp;

            if (this.statHpAlien > this.statHpMaxAlien) this.statHpAlien = this.statHpMaxAlien;
            if (this.statHpAlien < 0) this.statHpAlien = 0;
        }
        else
        {
            this.statHp = hp;

            if (this.statHp > this.statHpMax) this.statHp = this.statHpMax;
            if (this.statHp < 0) this.statHp = 0;
        }
    }
    public void SetHP(float hp, bool statAlien) // 체력 (지정 현재 수치)
    {
        if (statAlien == true)
        {
            this.statHpAlien = hp;

            if (this.statHpAlien > this.statHpMaxAlien) this.statHpAlien = this.statHpMaxAlien;
            if (this.statHpAlien < 0) this.statHpAlien = 0;
        }
        else
        {
            this.statHp = hp;

            if (this.statHp > this.statHpMax) this.statHp = this.statHpMax;
            if (this.statHp < 0) this.statHp = 0;
        }
    }
    public void SetHPMax(float hpmax) // 체력 (겉보기 최대 수치)
    {
        if (IsAlienObject() == true)
        {
            this.statHpMaxAlien = hpmax;

            if (this.statHpAlien > this.statHpMaxAlien) this.statHpAlien = this.statHpMaxAlien;
            if (this.statHpAlien < 0) this.statHpAlien = 0;
        }
        else
        {
            this.statHp = hpmax;

            if (this.statHp > this.statHpMax) this.statHp = this.statHpMax;
            if (this.statHp < 0) this.statHp = 0;
        }
    }
    public void SetHPMax(float hpmax, bool statAlien) // 체력 (지정 최대 수치)
    {
        if (statAlien == true)
        {
            this.statHpMaxAlien = hpmax;

            if (this.statHpAlien > this.statHpMaxAlien) this.statHpAlien = this.statHpMaxAlien;
            if (this.statHpAlien < 0) this.statHpAlien = 0;
        }
        else
        {
            this.statHp = hpmax;

            if (this.statHp > this.statHpMax) this.statHp = this.statHpMax;
            if (this.statHp < 0) this.statHp = 0;
        }
    }
    public void SetO2(float o2) // 산소 (현재 수치)
    {
        this.statO2 = o2;

        if (this.statO2 > this.statO2Max) this.statO2 = this.statO2Max;
        if (this.statO2 < 0) this.statO2 = 0;
    }
    public void SetO2Max(float o2max) // 산소 (최대치)
    {
        this.statO2Max = o2max;

        if (this.statO2 > this.statO2Max) this.statO2 = this.statO2Max;
        if (this.statO2 < 0) this.statO2 = 0;
    }
    public void SetBt(float bt) // 배터리 (현재 수치)
    {
        this.statBt = bt;

        if (this.statBt > this.statBtMax) this.statBt = this.statBtMax;
        if (this.statBt < 0) this.statBt = 0;
    }
    public void SetBtMax(float btmax) // 배터리 (최대치)
    {
        this.statBtMax = btmax;

        if (this.statBt > this.statBtMax) this.statBt = this.statBtMax;
        if (this.statBt < 0) this.statBt = 0;
    }
    public void SetWood(int wood) // 재료 (나무)
    {
        this.meterialWood = wood;
    }
    public void SetIron(int iron) // 재료 (철)
    {
        this.meterialIron = iron;
    }
    public void SetPart(int part) // 재료 (부품)
    {
        this.meterialPart = part;
    }
    public void SetMove(bool val) // 조작 설정
    {
        researcher.GetComponent<PlayerAnimation>().enabled = val;
        alien.GetComponent<AlienAnimation>().enabled = val;
        GetComponent<ThirdPersonMovement>().controllable = val;
        GetComponent<ThirdPersonSound>().enabled = val;
    }
    public void SetDead() // 사망 설정
    {
        Inventory.instance.DropAll();
        photonView.RPC("OnDead", RpcTarget.AllBuffered, photonView.OwnerActorNr);
    }
    public void SetTransform() // 변신 설정 (스위칭)
    {
        if (IsAlienPlayer() == false)
            return;

        photonView.RPC("OnTransform", RpcTarget.AllBuffered, photonView.OwnerActorNr, !alien.activeSelf);
    }
    public void SetTransform(bool val) // 변신 설정 (매뉴얼)
    {
        if (IsAlienPlayer() == false)
            return;

        photonView.RPC("OnTransform", RpcTarget.AllBuffered, photonView.OwnerActorNr, val);
    }
    public void SetFlash() // 라이트 설정 (스위칭)
    {
        if (IsAlienObject() == true)
            return;

        if (flashlight.activeSelf == false && GetBt() <= 10)
            return;

        photonView.RPC("OnFlash", RpcTarget.AllBuffered, photonView.OwnerActorNr, !flashlight.activeSelf);
    }
    public void SetFlash(bool val) // 라이트 설정 (매뉴얼)
    {
        if (IsAlienObject() == true)
            return;

        if (val == true && GetBt() <= 10)
            return;

        photonView.RPC("OnFlash", RpcTarget.AllBuffered, photonView.OwnerActorNr, val);
    }
    // ---------------------------------------------------------------------------------------------------
    // # 콜백 메소드
    // ---------------------------------------------------------------------------------------------------
    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        if (targetPlayer != photonView.Owner)
            return;

        if (changedProps.ContainsKey("color") == false)
            return;

        researcher.transform.Find("body").GetComponent<SkinnedMeshRenderer>().material.SetColor("_MainColor", colorPalette.colors[(int)changedProps["color"]]);
        researcher.transform.Find("head").gameObject.GetComponent<SkinnedMeshRenderer>().material.SetColor("_MainColor", colorPalette.colors[(int)changedProps["color"]]);
    }
}
