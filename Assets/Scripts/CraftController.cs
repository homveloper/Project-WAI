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

    bool isActive;

    void Start()
    {
        craftObj.SetActive(false);

        for (int i = 0; i <= 5; i++)
        {
            GameObject itemObject = craftObj.transform.Find("UI_CraftWindow_Item_" + i).gameObject;

            if (i >= item.Count)
            {
                itemObject.transform.localScale = new Vector3(0, 0, 0);
                continue;
            }

            itemObject.transform.localScale = new Vector3(1, 1, 1);
            itemObject.transform.Find("UI_CraftWindow_Image").GetComponent<Image>().sprite = item[i].icon;
            itemObject.transform.Find("UI_CraftWindow_Name").GetComponent<Text>().text = item[i].name;
            itemObject.transform.Find("UI_CraftWindow_Desc").GetComponent<Text>().text = item[i].description;

            GameObject meterialWood = itemObject.transform.Find("UI_CraftWindow_Meterial_0").gameObject;
            if (item[i].meterialWood > 0)
            {
                meterialWood.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                meterialWood.transform.Find("UI_CraftWindow_Meterial_Text").GetComponent<Text>().text = item[i].meterialWood.ToString();
            }
            else
            {
                meterialWood.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
            }

            GameObject meterialIron = itemObject.transform.Find("UI_CraftWindow_Meterial_1").gameObject;
            if (item[i].meterialIron > 0)
            {
                meterialIron.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                meterialIron.transform.Find("UI_CraftWindow_Meterial_Text").GetComponent<Text>().text = item[i].meterialIron.ToString();
            }
            else
            {
                meterialIron.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
            }

            GameObject meterialPart = itemObject.transform.Find("UI_CraftWindow_Meterial_2").gameObject;
            if (item[i].meterialPart > 0)
            {
                meterialPart.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                meterialPart.transform.Find("UI_CraftWindow_Meterial_Text").GetComponent<Text>().text = item[i].meterialPart.ToString();
            }
            else
            {
                meterialPart.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
            }
        }
    }
    public void Update()
    {
        if (!isActive)
            SetSwitchCraft(false);
        else if (isActive && Input.GetKeyDown(KeyCode.C))
            SetSwitchCraft();
    }
    public void SetSwitchCraft() // 크래프팅 창 출력 (스위칭)
    {
        SetSwitchCraft(!craftObj.activeSelf);
    }
    public void SetSwitchCraft(bool val) // 크래프팅 창 출력 (매뉴얼)
    {
        craftObj.SetActive(val);
    }
    public void OnCraft(int num)
    {
        Player player = GameManager.GetInstance().mPlayer.GetComponent<Player>();

        // 모든 재료가 있음
        if ((player.GetWood() >= item[num].meterialWood) && (player.GetIron() >= item[num].meterialIron) && (player.GetPart() >= item[num].meterialPart))
        {
            if (Inventory.instance.Add(item[num]) == true)
            {
                GameManager.GetInstance().GetComponent<MiniAlertController>().OnEnableAlert("제작 성공", item[num].name + "의 제작이 성공적입니다!");
                player.SetWood(player.GetWood() - item[num].meterialWood);
                player.SetIron(player.GetIron() - item[num].meterialIron);
                player.SetPart(player.GetPart() - item[num].meterialPart);
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
    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player") || !other.GetComponent<PhotonView>().IsMine)
            return;

        isActive = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player") || !other.GetComponent<PhotonView>().IsMine)
            return;

        isActive = false;
    }
}