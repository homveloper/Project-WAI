using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RepairController : MonoBehaviourPunCallbacks
{
    public GameObject buttonHint;
    public Item requiredItem;

    public int currentCount;
    public int maxCount;

    public bool isActive;

    void Start()
    {
        
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && isActive)
            Repair();
    }
    public void Repair()
    {
        if (!Inventory.instance.items.Contains(requiredItem))
        {
            GameManager.GetInstance().GetComponent<MiniAlertController>().OnEnableAlert("???", "필요한 아이템이 없습니다.");
            return;
        }

        Inventory.instance.Remove(requiredItem);
        currentCount++;

        if (currentCount > maxCount)
            currentCount = maxCount;
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
