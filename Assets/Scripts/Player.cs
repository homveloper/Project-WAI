using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Photon.Pun;
using System;
using System.ComponentModel.Design.Serialization;

public class Player : MonoBehaviourPunCallbacks
{
    private float REFRESH_TIME = 1.0f;

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

    public const float RESEARCHER_DAMAGE = 10f;
    public const float ALIEN_DAMAGE = 60f;
    public float damage { set; get; } = RESEARCHER_DAMAGE;

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
        alien.transform.localScale = new Vector3(0, 0, 0);
        flashlight.SetActive(false);

        if (PhotonNetwork.IsConnected)
            if (!photonView.IsMine)
                return;

        uI_Inventory = GameObject.Find("UI_Inventory").GetComponent<UI_Inventory>();
        uI_Inventory.UpdateInventory();

        StartCoroutine(OnRefresh());
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
        if (GetO2() <= 0 && !IsAlienObject())
            SetHP(GetHP() - Time.deltaTime * modHp);

        // 산소 차감
        if (GetComponent<ThirdPersonMovement>().IsRun() == true && !IsAlienObject())
            SetO2(GetO2() - Time.deltaTime * modO2Run);
        else
            SetO2(GetO2() - Time.deltaTime * modO2);

        // 배터리 차감
        if (IsFlash() == true)
            SetBt(GetBt() - Time.deltaTime * modBt);

        // 체력 부족
        if (GetHP() <= 0 && !IsDead())
            if (IsAlienPlayer() == true && IsAlienObject() == false)
                SetTransform(false);
            else
                SetDead();

        // 배터리 부족
        if (GetBt() <= 0 && IsFlash())
            SetFlash(false);

        // 체력 회복 (외계인)
        if (IsAlienObject() && !IsDead())
            SetHP(GetHP(true) + Time.deltaTime * modHpAlienHeal, true);

        // 배터리 회복
        if (!IsFlash() && !IsDead())
            SetBt(GetBt() + Time.deltaTime * modBtRecharge);
    }
    IEnumerator OnRefresh()
    {
        while (true)
        {
            Refresh();

            yield return new WaitForSeconds(REFRESH_TIME);
        }
    }
    public void Refresh()
    {
        photonView.RPC("OnRefreshMeterial", RpcTarget.OthersBuffered, photonView.OwnerActorNr, GetWood(), GetIron(), GetPart());
    }
    // ---------------------------------------------------------------------------------------------------
    // # 포톤 메시지 메소드
    // ---------------------------------------------------------------------------------------------------
    [PunRPC]
    public void OnDead(int actorNumber) // 사망
    {
        if (photonView.OwnerActorNr != actorNumber)
            return;

        if (IsDead())
            return;

        if (IsAlienPlayer())
            alien.GetComponent<AlienAnimation>().animator.SetTrigger("dead");
        else if (!IsAlienPlayer())
            researcher.GetComponent<PlayerAnimation>().animator.SetTrigger("dead");

        SetMove(false);
        SetFlash(false);

        if (photonView.IsMine)
        {
            Inventory.instance.DropAll();

            ExitGames.Client.Photon.Hashtable myProp = photonView.Owner.CustomProperties;
            myProp.Remove("fakeNick");
            myProp.Remove("fakeColor");
            photonView.Owner.SetCustomProperties(myProp);
        }
    }
    [PunRPC]
    public void OnTransform(int actorNumber, bool val)
    {
        if (photonView.OwnerActorNr != actorNumber)
            return;

        SetMove(true);
        SetFlash(false);
        researcher.transform.localScale = val ? new Vector3(1, 1, 1) : new Vector3(0, 0, 0);
        alien.transform.localScale = val ? new Vector3(0, 0, 0) : new Vector3(1, 1, 1);

        if (val == false)
        {
            damage = ALIEN_DAMAGE;
            SetO2(0);
            SetBt(0);
        }
        else
        {
            damage = RESEARCHER_DAMAGE;
            SetO2(GetO2Max());
            SetBt(GetBtMax());
        }

        GetComponent<ThirdPersonMovement>().alien = !val;

        if (photonView.IsMine && val == false)
        {
            Inventory.instance.DropAll();

            ExitGames.Client.Photon.Hashtable myProp = photonView.Owner.CustomProperties;
            myProp.Remove("fakeNick");
            myProp.Remove("fakeColor");
            photonView.Owner.SetCustomProperties(myProp);
        }
    }
    [PunRPC]
    public void OnFlash(int actorNumber, bool val) // 플래시라이트
    {
        if (photonView.OwnerActorNr != actorNumber)
            return;

        if (val == true)
            SetBt(GetBt() - 5);
        
        flashlight.SetActive(val);
    }
    [PunRPC]
    public void OnHit(int actorNumber, float damage) // 피격
    {
        if (photonView.OwnerActorNr != actorNumber)
            return;

        SetHP(GetHP() - damage);
        onTakeDamageCallback.Invoke();

        if (photonView.IsMine && GameInterfaceManager.GetInstance().IsChating())
            GameInterfaceManager.GetInstance().OnSwitchChat(false);
    }
    [PunRPC]
    public void OnTakeOver(int actorNumber) // 시체 소멸
    {
        if (photonView.OwnerActorNr != actorNumber)
            return;

        researcher.transform.localScale = new Vector3(0, 0, 0);
        alien.transform.localScale = new Vector3(0, 0, 0);
    }
    [PunRPC]
    public void OnRefreshMeterial(int actorNumber, int wood, int iron, int part)
    {
        if (photonView.OwnerActorNr != actorNumber)
            return;

        SetWood(wood);
        SetIron(iron);
        SetPart(part);
    }
    [PunRPC]
    public void OnTransformMeterial(int actorNumber, int wood, int iron, int part)
    {
        if (photonView.OwnerActorNr != actorNumber)
            return;

        SetWood(GetWood() + wood);
        SetIron(GetIron() + iron);
        SetPart(GetPart() + part);
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
    public string GetNickname()
    {
        return GetNickname(false);
    }
    public string GetNickname(bool original)
    {
        ExitGames.Client.Photon.Hashtable prop = photonView.Owner.CustomProperties;

        if (original)
            return photonView.Owner.NickName;
        else if (prop.ContainsKey("isAlien") == true && prop.ContainsKey("fakeNick") == true)
            return (string)prop["fakeNick"];
        else
            return photonView.Owner.NickName;
    }
    public Color GetColor()
    {
        return GetColor(false);
    }
    public Color GetColor(bool original)
    {
        ExitGames.Client.Photon.Hashtable prop = photonView.Owner.CustomProperties;

        if (original)
            return colorPalette.colors[(int)prop["color"]];
        else if (prop.ContainsKey("isAlien") == true && prop.ContainsKey("fakeColor") == true)
            return colorPalette.colors[(int)prop["fakeColor"]];
        else
            return colorPalette.colors[(int)prop["color"]];

    }
    public bool IsAlienPlayer() // 외계인 역할 여부
    {
        ExitGames.Client.Photon.Hashtable prop = photonView.Owner.CustomProperties;

        if (prop.ContainsKey("isAlien") == true && (bool)prop["isAlien"] == true) return true;
        else return false;
    }
    public bool IsAlienObject() // 외계인 상태 여부
    {
       return alien.transform.localScale == new Vector3(1, 1, 1);
    }
    public bool IsDead() // 사망 여부
    {
        if (IsAlienPlayer() && alien.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Dying Backwards"))
            return true;
        if (!IsAlienPlayer() && researcher.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Dying Backwards"))
            return true;
        
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
            // 리펙토링 코드
            // this.statHp = Mathf.Clamp(statHp, 0,statHpMax);

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
        if (this.meterialWood < 0) this.meterialWood = 0;
    }
    public void SetIron(int iron) // 재료 (철)
    {
        this.meterialIron = iron;
        if (this.meterialIron < 0) this.meterialIron = 0;
    }
    public void SetPart(int part) // 재료 (부품)
    {
        this.meterialPart = part;
        if (this.meterialPart < 0) this.meterialPart = 0;
    }
    public void SetMove(bool val) // 조작 설정
    {
        //researcher.GetComponent<PlayerAnimation>().enabled = val;
        //alien.GetComponent<AlienAnimation>().enabled = val;
        GetComponent<ThirdPersonMovement>().controllable = val;
        GetComponent<ThirdPersonSound>().enabled = val;
    }
    public void SetDead() // 사망 설정
    {
        photonView.RPC("OnDead", RpcTarget.AllBuffered, photonView.OwnerActorNr);
    }
    public void SetTransform() // 변신 설정 (스위칭)
    {
        SetTransform(alien.transform.localScale == new Vector3(1, 1, 1));
    }
    public void SetTransform(bool val) // 변신 설정 (매뉴얼)
    {
        if (IsAlienPlayer() == false)
            return;

        photonView.RPC("OnTransform", RpcTarget.AllBuffered, photonView.OwnerActorNr, val);
    }
    public void SetFlash() // 라이트 설정 (스위칭)
    {
        SetFlash(!flashlight.activeSelf);
    }
    public void SetFlash(bool val) // 라이트 설정 (매뉴얼)
    {
        if (IsAlienObject() == true)
            return;

        if (val == true && GetBt() <= 10)
            return;

        photonView.RPC("OnFlash", RpcTarget.AllBuffered, photonView.OwnerActorNr, val);
    }
    public void SetHit(float damage) // 피격 설정
    {
        photonView.RPC("OnHit", RpcTarget.AllBuffered, photonView.OwnerActorNr, damage);
    }
    public void SetRooting(Player target) // 사망 연구원 루팅
    {
        SetMove(false);

        // 프로퍼티 처리
        ExitGames.Client.Photon.Hashtable myProp = photonView.Owner.CustomProperties;
        ExitGames.Client.Photon.Hashtable targetProp = target.photonView.Owner.CustomProperties;
        myProp["fakeNick"] = target.photonView.Owner.NickName;
        myProp["fakeColor"] = targetProp["color"];
        photonView.Owner.SetCustomProperties(myProp);

        // 변신
        SetTransform(true);

        // 스텟 회복
        SetHP(GetHPMax());
        SetO2(GetO2Max());
        SetBt(GetBtMax());

        // 사망 연구원을 소멸 처리
        target.SetTakeOver();

        SetMove(true);
    }
    public void SetTakeOver() // 시체 소멸
    {
        photonView.RPC("OnTakeOver", RpcTarget.AllBuffered, photonView.OwnerActorNr);
    }
    public void SetTransformMeterial(int wood, int iron, int part)
    {
        photonView.RPC("OnTransformMeterial", RpcTarget.AllBuffered, photonView.OwnerActorNr, wood, iron, part);
    }
    // ---------------------------------------------------------------------------------------------------
    // # 트리거 메소드
    // ---------------------------------------------------------------------------------------------------
    // 외계인의 사망 연구원 루팅을 위한 트리거 메소드
    void OnTriggerStay(Collider other)
    {
        if (!photonView.IsMine)
            return;

        if (IsAlienPlayer() == false)
            return;

        if (IsAlienObject() == false)
            return;

        if (other.gameObject == gameObject || !other.CompareTag("Player"))
            return;

        // 루팅 (R)
        if (Input.GetKeyDown(KeyCode.R))
            SetRooting(other.GetComponent<Player>());
    }
    // ---------------------------------------------------------------------------------------------------
    // # 콜백 메소드
    // ---------------------------------------------------------------------------------------------------
    // 연구원 색상 갱신을 위한 프로퍼티 갱신 콜백 메소드
    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        if (targetPlayer != photonView.Owner)
            return;

        if (changedProps.ContainsKey("color") == false)
            return;

        if (changedProps.ContainsKey("fakeColor") == true)
        {
            researcher.transform.Find("body").GetComponent<SkinnedMeshRenderer>().material.SetColor("_MainColor", colorPalette.colors[(int)changedProps["fakeColor"]]);
            researcher.transform.Find("head").gameObject.GetComponent<SkinnedMeshRenderer>().material.SetColor("_MainColor", colorPalette.colors[(int)changedProps["fakeColor"]]);
        }
        else
        {
            researcher.transform.Find("body").GetComponent<SkinnedMeshRenderer>().material.SetColor("_MainColor", colorPalette.colors[(int)changedProps["color"]]);
            researcher.transform.Find("head").gameObject.GetComponent<SkinnedMeshRenderer>().material.SetColor("_MainColor", colorPalette.colors[(int)changedProps["color"]]);
        }
    }
}
