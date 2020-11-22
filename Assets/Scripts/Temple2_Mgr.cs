using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Temple2_Mgr : MonoBehaviourPun
{
    public GameObject jewelry;
    public int cnt = 3; //3개 패널에서 스테이지 클리어 시 유적 클리어.
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(cnt == 0)
        {
            photonView.RPC("OnActiveJewelry", RpcTarget.AllBuffered);
            
        }
    }

    [PunRPC]
    void OnActiveJewelry()
    {
        jewelry.SetActive(true);
        cnt = 1;
    }
}
