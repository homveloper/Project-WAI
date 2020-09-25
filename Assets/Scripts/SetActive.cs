using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActive : MonoBehaviour
{
    public GameObject info;
    void Start()
    {
        info.SetActive(false);
    }
    // Start is called before the first frame update
     private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name + "감지 시작!");
        info.SetActive(true);
    }


    // Collider 컴포넌트의 is Trigger가 true인 상태로 충돌중일 때

    private void OnTriggerStay(Collider other)

    {
        Debug.Log(other.name + "감지 중!");
        info.SetActive(true);

    }



    // Collider 컴포넌트의 is Trigger가 true인 상태로 충돌이 끝났을 때

    private void OnTriggerExit(Collider other)

    {
        Debug.Log(other.name + "감지 끝!");
        info.SetActive(false);
    }
}
