using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelePortGate : MonoBehaviour
{
    public Transform temple;
    public GameObject info;

    void Start()
    {
        info.SetActive(false);
    }
    
    void OnTriggerStay(Collider other)
    {
        info.SetActive(true);
        if(Input.GetKeyDown(KeyCode.E))
        {
            GameManager.GetInstance().mPlayer.GetComponent<Player>().SetMove(false);
            GameManager.GetInstance().GetComponent<FadeController>().OnFadeOut();
            Invoke("OnStartWarp", 1.0f);
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Debug.Log(other.name + "감지 끝!");
        info.SetActive(false);
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
