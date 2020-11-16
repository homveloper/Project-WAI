using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class InteractableBox : Interactable
{
    public List<Item> items;
    
    Vector3 tmp;

    // override void Start()
    // {
    //     tmp = info.transform.localScale;
    //     info.transform.localScale = new Vector3(0,0,0);
    // }

    public override void Interact(){
        Item randomItem = items[Random.Range(0,items.Count)];
        //Item randomItem = items[1];
        bool wasPickedUp = Inventory.instance.Add(randomItem);

        if(wasPickedUp)
        {
            Debug.Log(randomItem.name + "을 주웠습니다.");
            photonView.RPC("DestroyItem", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void DestroyItem() => Destroy(gameObject);
}
