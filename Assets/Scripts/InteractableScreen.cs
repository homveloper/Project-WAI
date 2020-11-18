using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Photon.Pun;

public class InteractableScreen : MonoBehaviour
{
    [SerializeField]
    List<Interaction> interactions;

    [SerializeField]
    GameObject buttonHint;

    GameObject buttonHintLayout;

    bool inTrigger;
    bool hasInteracted = false;

    public virtual void Interact(KeyCode _KeyCode){
        //To do Code you want
    }

    public virtual void Initialize(){
        // To do Code initialize
    }

    void Start(){

        buttonHintLayout =  GameObject.Instantiate(Resources.Load("UI/UI_ButtonHintLayout") as GameObject);

        foreach(Interaction interaction in interactions){
            GameObject newButtonHint = GameObject.Instantiate(buttonHint,buttonHintLayout.transform.Find("UI_Horizontal_Buttons"),false);
            newButtonHint.transform.Find("Background").Find("Key").GetComponent<Text>().text = interaction.Key;
            newButtonHint.transform.Find("Background").GetComponent<Image>().color = interaction._Color;
            newButtonHint.transform.Find("Description").GetComponent<Text>().text = interaction.Description;
        }

        Initialize();
    }
    void Update(){

        // if (PhotonNetwork.IsConnected)
        //     if (!photonView.IsMine)
        //         return;

        OnButtonHint(inTrigger);

        if(inTrigger){
            foreach(Interaction interaction in interactions){
                if (Input.GetKeyDown(interaction._KeyCode))
                {
                    Interact(interaction._KeyCode);
                }
            }
        }
    }

    /*
        중요!!
        OnTriggerStay는 FixedUpdate 방식이므로 여러번 호출 될 수 있습니다.
        한번만 호출하기 위해 bool 변수로 상태를 설정하고, Udpate 함수에서 해당 기능을 호출하도록 합니다.
    */
    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<PhotonView>() == null || other.GetComponent<PhotonView>().IsMine == false)
            return;

        if(other.gameObject.tag == "Player"){
            inTrigger = true;
        }
    }

    // Collider 컴포넌트의 is Trigger가 true인 상태로 충돌이 끝났을 때
    private void OnTriggerExit(Collider other)
    {
        inTrigger = false;
    }

    void OnButtonHint(bool active){
        // if(buttonHint != null)
        //     buttonHint.gameObject.SetActive(active);
        buttonHintLayout.SetActive(active);
    }

    public GameObject ButtonHint{
        get=>buttonHint;
    }

    public GameObject ButtonHintLayout{
        get=>buttonHintLayout;
    }
}

