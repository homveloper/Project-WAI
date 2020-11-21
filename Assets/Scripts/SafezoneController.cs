﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SafezoneController : MonoBehaviour
{
    List<GameObject> survivorList;

    void Start()
    {
        // ** 세이프존 컨트롤러는 꼭 시작 시에 Active 상태가 꺼져있어야 한다.
        survivorList = GameManager.GetInstance().survivorList;
    }

    // ---------------------------------------------------------------------------------------------------
    // # 트리거 메소드
    // ---------------------------------------------------------------------------------------------------
    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        GameObject player = other.gameObject;
        bool valid = true;

        if (other.GetComponent<Player>().IsDead())
            valid = false;
        //else if (other.GetComponent<Player>().IsAlienPlayer())
        //    valid = false;

        if (!survivorList.Contains(player) && valid)
            survivorList.Add(player);
        else if (survivorList.Contains(player) && !valid)
            survivorList.Remove(player);
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        GameObject player = other.gameObject;

        if (survivorList.Contains(player))
            survivorList.Remove(player);
    }
}
