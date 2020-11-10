using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ItemPickup : MonoBehaviourPun
{
    public Item item;

    private bool inTrigger;

    void Update(){
        if (Input.GetButtonDown("Interact") && inTrigger)
        {
            PickUp();
        }
    }

    /*
        중요!!
        OnTriggerStay는 FixedUpdate 방식이므로 여러번 호출 될 수 있습니다.
        한번만 호출하기 위해 bool 변수로 상태를 설정하고, Udpate 함수에서 해당 기능을 호출하도록 합니다.
    */
    private void OnTriggerStay(Collider other) {

        if (other.GetComponent<PhotonView>() == null || other.GetComponent<PhotonView>().IsMine == false)
            return;


        bool isResearcher = false;
        if(other.GetComponent<Player>() != null)
            isResearcher = !other.GetComponent<Player>().IsAlienObject();

        if(other.gameObject.tag == "Player" && isResearcher)
            inTrigger = true;
    }
    private void OnTriggerExit(Collider other) {
        inTrigger = false;
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
