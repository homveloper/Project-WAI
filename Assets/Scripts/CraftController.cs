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
    public List<Item> item;

    void Start()
    {
        craftObj.SetActive(false);
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
    public void OnCraft(Item item)
    {
        Player player = GameManager.GetInstance().mPlayer.GetComponent<Player>();

        // 모든 재료가 있음
        if ((player.GetWood() >= item.meterialWood) && (player.GetIron() >= item.meterialIron) && (player.GetPart() >= item.meterialPart))
        {
            if (Inventory.instance.Add(item) == true)
            {
                GameManager.GetInstance().GetComponent<MiniAlertController>().OnEnableAlert("제작 성공", item.name + "의 제작이 성공적입니다!");
                player.SetWood(player.GetWood() - item.meterialWood);
                player.SetIron(player.GetIron() - item.meterialIron);
                player.SetPart(player.GetPart() - item.meterialPart);
            }
            else
            {
                GameManager.GetInstance().GetComponent<MiniAlertController>().OnEnableAlert("제작 실패", "인벤토리 공간이 부족합니다.");
            }
            
        }
        // 재료 일부가 부족
        else
        {
            GameManager.GetInstance().GetComponent<MiniAlertController>().OnEnableAlert("제작 실패", "재료가 부족합니다.");
        }

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