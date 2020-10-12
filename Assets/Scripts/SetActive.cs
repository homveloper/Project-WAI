using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class SetActive : MonoBehaviour
{
    public List<Item> items;

    bool isCalled = false;

    public GameObject info;
    void Start()
    {
        info.SetActive(false);
    }
    // Start is called before the first frame update
     private void OnTriggerEnter(Collider other)
    {
        // Debug.Log(other.name + "감지 시작!");
        info.SetActive(true);
    }

    // Collider 컴포넌트의 is Trigger가 true인 상태로 충돌중일 때

    private void OnTriggerStay(Collider other)
    {
        // Debug.Log(other.name + "감지 중!");
        if(other.tag == "Player" && Input.GetButtonDown("Interact")){
            PickUp();
            print(other);
        }
    }

    // Collider 컴포넌트의 is Trigger가 true인 상태로 충돌이 끝났을 때

    private void OnTriggerExit(Collider other)
    {
        // Debug.Log(other.name + "감지 끝!");
        info.SetActive(false);
    }

    void PickUp(){
        Item randomItem = items[Random.Range(0,items.Count)];
        bool wasPickedUp = Inventory.instance.Add(randomItem);

        if(wasPickedUp){
            Debug.Log(randomItem.name + "을 주웠습니다.");
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
