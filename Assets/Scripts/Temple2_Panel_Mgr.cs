﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Temple2_Panel_Mgr : MonoBehaviourPun
{
    public GameObject stage;
    public GameObject mgr;
    public GameObject door;

    public int tmp = 1;
    // Start is called before the first frame update
    void Start()
    {
        door.SetActive(false);
    }
    void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<PhotonView>() == null || other.GetComponent<PhotonView>().IsMine == false)
            return;

        if (other.tag != "Player")
            return;

        if (Input.GetKeyDown(KeyCode.E) && tmp == 1)
        {
            GameManager.GetInstance().mPlayer.GetComponent<Player>().SetMove(false);
            GameManager.GetInstance().GetComponent<FadeController>().OnFadeOut();
            StartCoroutine(this.OnStartMinigame());
        }
    }

    IEnumerator OnStartMinigame()
    {
        yield return new WaitForSeconds(1);
        stage.SetActive(true);
        MiniGame_enemy[] elist = stage.transform.Find("Enemies").GetComponentsInChildren<MiniGame_enemy>();

        foreach (MiniGame_enemy e in elist)
        {
            e.GetComponent<Animator>().SetInteger("EnemyType", (int)e.type);
        }
        GameInterfaceManager.GetInstance().OnSwitchHide(true);
        photonView.RPC("OnActiveDoor", RpcTarget.AllBuffered);
        GameManager.GetInstance().GetComponent<FadeController>().OnFadeIn();
    }
    // Update is called once per frame
    void Update()
    {
        if (tmp == 0)
        {
            mgr.GetComponent<Temple2_Mgr>().cnt--;
            tmp = 2;
        }
    }

    [PunRPC]
    public void OnActiveDoor()
    {
        door.SetActive(true);
    }

    [PunRPC]
    public void OnDisableDoor()
    {
        door.SetActive(false);
    }
}
