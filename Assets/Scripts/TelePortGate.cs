using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelePortGate : MonoBehaviourPun
{
    public Transform temple;
   
    
    void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<PhotonView>() == null || other.GetComponent<PhotonView>().IsMine == false)
            return;

        if(other.tag != "Player")
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            GameManager.GetInstance().mPlayer.GetComponent<Player>().SetMove(false);
            GameManager.GetInstance().GetComponent<FadeController>().OnFadeOut();
            Invoke("OnStartWarp", 1.0f);
        }
    }

    void OnTriggerExit(Collider other)
    {
    }
    void OnStartWarp()
    {
        GameManager.GetInstance().mPlayer.transform.position = temple.transform.position;
        Invoke("OnFinishWarp", 1.0f);
    }

    void OnFinishWarp()
    {
        GameManager.GetInstance().GetComponent<FadeController>().OnFadeIn();
        GameManager.GetInstance().mPlayer.GetComponent<Player>().SetMove(true);
    }
}
