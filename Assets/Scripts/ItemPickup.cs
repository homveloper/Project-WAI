using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ItemPickup : Interactable
{
    public Item item;

    // public Interactable interactable;

    public override void Interact(){
        Debug.Log("Interacting with " + transform.name);

        bool wasPickedUp = Inventory.instance.Add(item);

        if(wasPickedUp){
            Debug.Log(item.name + "을 주웠습니다.");
            photonView.RPC("DestroyItem", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void DestroyItem() => Destroy(gameObject);
}
