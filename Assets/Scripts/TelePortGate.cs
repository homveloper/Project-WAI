using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TelePortGate : MonoBehaviourPun
{
    public Transform temple;
    public GameObject info;
    Vector3 tmp;

   
    void Start()
    {
        tmp = info.transform.localScale;
        info.transform.localScale = new Vector3(0,0,0);
    }
    
    void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<PhotonView>() == null || other.GetComponent<PhotonView>().IsMine == false)
            return;

        info.transform.localScale = tmp;

        if (Input.GetKeyDown(KeyCode.E))
        {
            GameManager.GetInstance().mPlayer.GetComponent<Player>().SetMove(false);
            GameManager.GetInstance().GetComponent<FadeController>().OnFadeOut();
            Invoke("OnStartWarp", 1.0f);
        }
    }

    void OnTriggerExit(Collider other)
    {
        info.transform.localScale = new Vector3(0,0,0);
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
