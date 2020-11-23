using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Temple1_2_Switch : MonoBehaviourPun
{
    // Start is called before the first frame update
     public GameObject swichFlash;
     public GameObject mgr;

     int tmp = 1;
    void Start()
    {
        swichFlash.SetActive(false);
    }
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PhotonView>() == null || other.GetComponent<PhotonView>().IsMine == false)
            return;

        if(other.gameObject.tag != "HitBox")
            return ;
            
        photonView.RPC("OnLightUp", RpcTarget.AllBuffered);

        if(tmp == 1)
        {
            photonView.RPC("OnCntDown", RpcTarget.AllBuffered);
            tmp--;
        }
    }

    // Collider 컴포넌트의 is Trigger가 true인 상태로 충돌중일 때

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<PhotonView>() == null || other.GetComponent<PhotonView>().IsMine == false)
            return;

        other.GetComponentInParent<Player>().statO2++;
    }

    [PunRPC]
    public void OnCntDown()
    {
        swichFlash.SetActive(true);
        mgr.GetComponent<Temple1_2_Mgr>().cnt--;
    }

    [PunRPC]
    public void OnLightUp()
    {
        swichFlash.SetActive(true);
    }
}
