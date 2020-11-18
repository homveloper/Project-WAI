using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SubmitController : MonoBehaviourPunCallbacks
{
    public GameObject buttonHint;

    public int phase;
    public Item repairItem;
    public int repairCurrentCount;
    public int repairMaxCount;
    public Item crystalItem;
    public int crystalCurrentCount;
    public int crystalMaxCount;

    public bool isActive;

    void Start()
    {
        
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && isActive)
            Submit();
    }
    public void Submit()
    {
        Item item;

        if (phase == 1)
            item = repairItem;
        else if (phase == 2)
            item = crystalItem;
        else
            return;

        if (!Inventory.instance.items.Contains(item))
        {
            GameManager.GetInstance().GetComponent<MiniAlertController>().OnEnableAlert("아이템 부족", "필요한 아이템이 없습니다.");
            return;
        }
        
        Inventory.instance.Remove(item);
        photonView.RPC("OnSubmit", RpcTarget.AllBuffered, GameManager.GetInstance().mPlayer.GetComponent<Player>().GetNickname(), phase);
    }
    // ---------------------------------------------------------------------------------------------------
    // # 네트워크 메소드
    // ---------------------------------------------------------------------------------------------------
    [PunRPC]
    public void OnSubmit(string nickname, int phase)
    {
        if (phase == 1)
        {
            repairCurrentCount++;

            if (repairCurrentCount < repairMaxCount)
                GameManager.GetInstance().GetComponent<MiniAlertController>().OnEnableAlert("수리 진행중", nickname + "(이)가 우주선을 수리했습니다.");

            if (PhotonNetwork.IsMasterClient && repairCurrentCount >= repairMaxCount)
                photonView.RPC("OnSubmitFinish", RpcTarget.AllBuffered, phase);
        }
        else if (phase == 2)
        {
            crystalCurrentCount++;

            if (crystalCurrentCount < crystalMaxCount)
                GameManager.GetInstance().GetComponent<MiniAlertController>().OnEnableAlert("보석 적재중", nickname + "(이)가 우주선에 보석을 적재했습니다.");

            if (PhotonNetwork.IsMasterClient && crystalCurrentCount >= crystalMaxCount)
                photonView.RPC("OnSubmitFinish", RpcTarget.AllBuffered, phase);
        }
    }
    [PunRPC]
    public void OnSubmitFinish(int phase)
    {
        if (phase == 1)
        {
            phase = 2;
            GameManager.GetInstance().GetComponent<MiniAlertController>().OnEnableAlert("수리 완료", "수리가 완료되었습니다.\n이제, 보석을 적재할 수 있습니다.");
        }
        else if (phase == 2)
        {
            phase = 3;
            GameManager.GetInstance().GetComponent<MiniAlertController>().OnEnableAlert("적재 완료", "적재가 완료되었습니다.\n이제, 우주선에 탈 수 있습니다.");
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
