using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class SetActive : MonoBehaviourPun
{
    public List<Item> items;

    bool isCalled = false;

    void Start()
    {
    }
    // Start is called before the first frame update
     private void OnTriggerEnter(Collider other)
    {
        // Debug.Log(other.name + "감지 시작!");
        
    }

    // Collider 컴포넌트의 is Trigger가 true인 상태로 충돌중일 때

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<PhotonView>() == null || other.GetComponent<PhotonView>().IsMine == false)
            return;

        if (other.tag == "Player" && Input.GetButtonDown("Interact"))
        {
            PickUp();
        }
    }

    // Collider 컴포넌트의 is Trigger가 true인 상태로 충돌이 끝났을 때

    private void OnTriggerExit(Collider other)
    {
    }

    void PickUp()
    {
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
