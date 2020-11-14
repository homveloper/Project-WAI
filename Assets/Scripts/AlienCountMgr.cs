using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AlienCountMgr : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update

    [SerializeField]
    private BGMSelect bgmMgr;
    public int chgCode;
    public int orginCode;

    public bool isAlienInTrigger;
    void Awake()
    {
        if (PhotonNetwork.IsConnected)
            if (!photonView.IsMine)
                return;
        Debug.Log("star in");
        bgmMgr = Camera.main.GetComponent<BGMSelect>();
    }

    void Update(){

        if(isAlienInTrigger){
            orginCode = bgmMgr.stat;
            bgmMgr.stat = chgCode;
        }else{
            bgmMgr.stat = orginCode;
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (PhotonNetwork.IsConnected)
            if (!photonView.IsMine)
                return;

        if (other.GetComponent<PhotonView>() == null || other.GetComponent<PhotonView>().IsMine == true)
            return;

        if(other.gameObject.tag == "Player" && other.GetComponent<Player>().IsAlienObject())
        {
            isAlienInTrigger = true;
        }
        
    }
    void OnTriggerExit(Collider other)
    {
         if (PhotonNetwork.IsConnected)
            if (!photonView.IsMine)
                return;

        if (other.GetComponent<PhotonView>() == null || other.GetComponent<PhotonView>().IsMine == true)
            return;

        isAlienInTrigger = false;
    }
}
