using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ItemPickup : MonoBehaviourPun
{
    public ConsumableItem item;

    private void OnTriggerEnter(Collider other) {
    
    }

    private void OnTriggerStay(Collider other) {

        if (other.GetComponent<PhotonView>() == null || other.GetComponent<PhotonView>().IsMine == false)
            return;

        if (other.tag == "Player" && Input.GetButtonDown("Interact")){
            PickUp();
        }
    }
    private void OnTriggerExit(Collider other) {

    }

    void PickUp(){
        bool wasPickedUp = Inventory.instance.Add(item);

        if(wasPickedUp){
            Debug.Log(item.name + "을 주웠습니다.");
            photonView.RPC("DestroyItem", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void DestroyItem() => Destroy(gameObject);
}
