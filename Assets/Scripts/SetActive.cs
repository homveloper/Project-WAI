using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class SetActive : MonoBehaviourPun
{
    public List<Item> items;
    
    public GameObject info;
    Vector3 tmp;
    bool isCalled = false;

    private bool inTrigger;

    void Start()
    {
        tmp = info.transform.localScale;
        info.transform.localScale = new Vector3(0,0,0);
    }

    void Update(){
        if (Input.GetButtonDown("Interact") && inTrigger)
        {
            PickUp();
        }
    }

    // Start is called before the first frame update
     private void OnTriggerEnter(Collider other)
    {
        // Debug.Log(other.name + "감지 시작!");
        
    }

    /*
        중요!!
        OnTriggerStay는 FixedUpdate 방식이므로 여러번 호출 될 수 있습니다.
        한번만 호출하기 위해 bool 변수로 상태를 설정하고, Udpate 함수에서 해당 기능을 호출하도록 합니다.
    */
    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<PhotonView>() == null || other.GetComponent<PhotonView>().IsMine == false)
            return;

        info.transform.localScale = tmp;

        bool isResearcher = false;
        if(other.GetComponent<Player>() != null)
            isResearcher = !other.GetComponent<Player>().IsAlienObject();

        if(other.gameObject.tag == "Player" && isResearcher)
            inTrigger = true;

    }

    // Collider 컴포넌트의 is Trigger가 true인 상태로 충돌이 끝났을 때

    private void OnTriggerExit(Collider other)
    {
        info.transform.localScale = new Vector3(0,0,0);
        inTrigger = false;
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
