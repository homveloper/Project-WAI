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

    void Awake()
    {
        if (PhotonNetwork.IsConnected)
            if (!photonView.IsMine)
                return;
        bgmMgr = Camera.main.GetComponent<BGMSelect>();
    }

    void Update(){

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
            if (chgCode != bgmMgr.stat)
            {
                orginCode = bgmMgr.stat;
                bgmMgr.stat = chgCode;
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
         if (PhotonNetwork.IsConnected)
            if (!photonView.IsMine)
                return;

        if (other.GetComponent<PhotonView>() == null || other.GetComponent<PhotonView>().IsMine == true)
            return;

        if (other.gameObject.tag == "Player" && other.GetComponent<Player>().IsAlienObject())
        {
            bgmMgr.stat = orginCode;
        }
    }
}
