using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Photon.Pun;

public class Player : MonoBehaviourPunCallbacks
{
    private float statHp = 0.0f;
    private float statHpMax = 0.0f;
    private float statO2 = 0.0f;
    private float statO2Max = 0.0f;

    public float hpModifier = 5.0f;
    public float o2Modifer = 1.0f;

    private int meterialWood = 0;
    private int meterialIron = 0;
    private int meterialPart = 0;

    public GameObject flashlight;

    UI_Inventory uI_Inventory;

    void Start()
    {
        print("Start Player");

        SetHPMax(100);
        SetO2Max(100);

        SetHP(100);
        SetO2(100);

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

        // 산소 차감
        SetO2(GetO2() - Time.deltaTime * o2Modifer);

        // 체력 차감
        if (GetO2() <= 0)
            SetHP(GetHP() - Time.deltaTime * hpModifier);

        // 플래시라이트 (F)
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (flashlight.activeSelf == true) flashlight.SetActive(false);
            else flashlight.SetActive(true);

            photonView.RPC("OnFlash", RpcTarget.AllBuffered, photonView.OwnerActorNr, flashlight.activeSelf);
        }

        // 체력 부족으로 사망
        if (GetHP() <= 0)
        {
            PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
        }
    }

    // 캐릭터 이동가능 여부 설정 함수
    public void SetMove(bool val)
    {
        GetComponent<PlayerAnimation>().enabled = val;
        GetComponent<ThirdPersonMovement>().controllable = val;
        GetComponent<ThirdPersonSound>().enabled = val;
    }

    [PunRPC]
    public void OnFlash(int actorNumber, bool val) // RPC로 플래시라이트 사용을 알림
    {
        if (photonView.OwnerActorNr == actorNumber)
            flashlight.SetActive(val);
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
}
