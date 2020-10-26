using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftCtr : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject window;
    public GameObject info;

    void Start()
    {
        window.SetActive(false);
        info.SetActive(false);
    }
    // Start is called before the first frame update

    // Collider 컴포넌트의 is Trigger가 true인 상태로 충돌중일 때

    private void OnTriggerStay(Collider other)
    {
       window.SetActive(true);
        info.SetActive(true);
    }
    void OnTriggerExit(Collider other)
    {
        window.SetActive(false);
        info.SetActive(false);
    }
}
