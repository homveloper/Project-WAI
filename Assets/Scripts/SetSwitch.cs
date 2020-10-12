using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSwitch : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject swichFlash;
    void Start()
    {
        swichFlash.SetActive(false);
    }
    // Start is called before the first frame update
     private void OnTriggerEnter(Collider other)
    {
        // Debug.Log(other.name + "감지 시작!");
        swichFlash.SetActive(true);
    }

    // Collider 컴포넌트의 is Trigger가 true인 상태로 충돌중일 때

    private void OnTriggerStay(Collider other)
    {
       
    }
}
