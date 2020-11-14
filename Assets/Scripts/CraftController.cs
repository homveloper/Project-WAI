using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using UnityEngine.UI;
using System.Linq;
using System;
using System.Linq.Expressions;

public class CraftController : MonoBehaviourPunCallbacks
{
    public GameObject craftObj;

    void Start()
    {
        craftObj.SetActive(false);
        //Inventory.instance.Add();
    }
    void Update()
    {
        
    }
    public void SetSwitchCraft() // 크래프팅 창 출력 (스위칭)
    {
        SetSwitchCraft(!craftObj.activeSelf);
    }
    public void SetSwitchCraft(bool val) // 크래프팅 창 출력 (매뉴얼)
    {
        GameManager.GetInstance().mPlayer.GetComponent<Player>().SetMove(!val);
        craftObj.SetActive(val);
    }
    // ---------------------------------------------------------------------------------------------------
    // # 트리거 메소드
    // ---------------------------------------------------------------------------------------------------
    void OnTriggerStay(Collider other) // 상호작용
    {
        if (!other.CompareTag("Player") || other.GetComponent<PhotonView>().IsMine == false)
            return;

        if (Input.GetKeyDown(KeyCode.C))
            SetSwitchCraft();
    }
}