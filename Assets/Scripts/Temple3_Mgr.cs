using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Temple3_Mgr : MonoBehaviourPun
{
    // Start is called before the first frame update
    public int cnt = 2;
    public GameObject stone;

    void Start()
    {
        stone.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(cnt == 0)
        {
            photonView.RPC("OnLightUp", RpcTarget.AllBuffered);
            cnt =2;
        }
    }

    [PunRPC]
    public void OnLightUp()
    {
        stone.SetActive(true);
    }
}
