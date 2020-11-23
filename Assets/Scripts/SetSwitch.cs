using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class SetSwitch : MonoBehaviourPun
{
    // Start is called before the first frame update
    public GameObject swichFlash;
    public GameObject moveWall;
    public GameObject hiddenGate;
    public AudioSource wallSound;
    
    int cnt = 1;
    void Start()
    {
        swichFlash.SetActive(false);
    }
    // Start is called before the first frame update
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag != "HitBox")
            return ;
            
        if (cnt == 1)
        {
            photonView.RPC("OnSetSwitch", RpcTarget.AllBuffered);
        }
    }
    void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag != "HitBox")
            return ;
        
        other.GetComponentInParent<Player>().statO2++;
    }

    void OnTriggerExit(Collider other)
    {
        
    }

    [PunRPC]
    public void OnSetSwitch()
    {
        cnt--;
        swichFlash.SetActive(true);

        moveWall.SetActive(false);
        wallSound.Play();

        if (hiddenGate != null)
            hiddenGate.SetActive(true);
    }
}
