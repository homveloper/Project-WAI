using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ItemPickup : MonoBehaviour
{
    public Item item;

    private void OnTriggerEnter(Collider other) {
    
    }

    private void OnTriggerStay(Collider other) {
        if(other.tag == "Player" && Input.GetButtonDown("Interact")){
            PickUp();
        }
    }
    private void OnTriggerExit(Collider other) {

    }

    void PickUp(){
        bool wasPickedUp = Inventory.instance.Add(item);

        if(wasPickedUp){
            Debug.Log(item.name + "을 주웠습니다.");
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
