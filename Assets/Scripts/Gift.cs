using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Gift : MonoBehaviourPun
{
    public GameObject gift; 
    int cnt = 1;
    // Start is called before the first frame update
    void Start()
    {
        gift.SetActive(false);
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag != "HitBox")
            return ;
        
        if(cnt == 1)
            photonView.RPC("OnGiftPop", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void OnGiftPop()
    {
        gift.SetActive(true);
        cnt--;
    }
}
