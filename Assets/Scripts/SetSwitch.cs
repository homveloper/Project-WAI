using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SetSwitch : MonoBehaviourPun
{
    // Start is called before the first frame update
    public GameObject swichFlash;
    public GameObject moveWell;
    public GameObject hiddenGate;
    public AudioSource wallSound;
    public Transform temple;
    public GameObject info;
    Vector3 tmp;

    int cnt = 1;
    void Start()
    {
        swichFlash.SetActive(false);
        tmp = info.transform.localScale;
        info.transform.localScale = new Vector3(0,0,0);
    }
    // Start is called before the first frame update
     private void OnTriggerEnter(Collider other)
    {
        /*if (other.GetComponent<PhotonView>().IsMine == false)
            return;*/
        if (other.GetComponent<PhotonView>() == null || other.GetComponent<PhotonView>().IsMine == false)
            return;
        if(cnt == 1 || swichFlash.active == false)
        {
            info.transform.localScale = tmp;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            cnt--;
            swichFlash.SetActive(true);

            if(moveWell == null)
                return;
            else
            {
                moveWell.SetActive(false);
                wallSound.Play();
            }
            
            if(hiddenGate != null)
                hiddenGate.SetActive(true);
        }
        // Debug.Log(other.name + "감지 시작!");
    }

    // Collider 컴포넌트의 is Trigger가 true인 상태로 충돌중일 때

    private void OnTriggerEixt(Collider other)
    {
       info.transform.localScale = new Vector3(0,0,0);
    }
}
