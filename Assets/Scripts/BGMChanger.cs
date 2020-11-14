using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BGMChanger : MonoBehaviourPunCallbacks
{
    public GameObject bgmMgr;
    public int chgCode;
    public int orginCode;
    int cnt = 1;
    // Start is called before the first frame update
    void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<PhotonView>() == null || other.GetComponent<PhotonView>().IsMine == false)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            //orginCode = bgmMgr.GetComponent<BGMSelect>().stat;
            bgmMgr.GetComponent<BGMSelect>().stat = chgCode;
            // cnt--;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PhotonView>() == null || other.GetComponent<PhotonView>().IsMine == false)
            return;
            
    }
}
