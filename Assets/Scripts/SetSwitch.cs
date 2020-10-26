using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSwitch : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject swichFlash;
    public GameObject moveWell;
    public GameObject hiddenGate;
    void Start()
    {
        swichFlash.SetActive(false);
    }
    // Start is called before the first frame update
     private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PhotonView>().IsMine == false)
            return;

        // Debug.Log(other.name + "감지 시작!");
        swichFlash.SetActive(true);
        if(moveWell == null)
            return;
        else
            moveWell.SetActive(false);
            
        if(hiddenGate != null)
            hiddenGate.SetActive(true);
    }

    // Collider 컴포넌트의 is Trigger가 true인 상태로 충돌중일 때

    private void OnTriggerStay(Collider other)
    {
       
    }
}
