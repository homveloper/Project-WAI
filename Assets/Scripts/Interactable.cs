﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Photon.Pun;

public class Interactable : MonoBehaviourPun
{
    [SerializeField]
    GameObject buttonHint;

    [SerializeField]
    String key = "Showing Key when you approache example 'E'";

    [SerializeField]
    KeyCode keyCode;

    [SerializeField]
    string description;
    bool inTrigger;
    bool hasInteracted = false;

    [SerializeField]
    Color color;
    Text UI_Key;
    Text UI_Description;
    Transform camTransform;
    Quaternion originalRotation;


    public virtual void Interact(){
        //To do Code you want
    }

    void Start(){
        // UI의 초기 회전값
        originalRotation = buttonHint.transform.rotation;

        // 초기화
        UI_Key = buttonHint.transform.Find("Key").GetComponent<Text>();
        buttonHint.transform.Find("Background").GetComponent<Image>().color = color;
        UI_Description = buttonHint.transform.Find("Description").GetComponent<Text>();

        UI_Key.text = key;
        UI_Description.text = description;
    }
    void Update(){

        OnButtonHint(inTrigger);

        if(inTrigger){
            // 본 오브젝트의 위치가 회전하거나 캐릭터가 움직이는 거에 따라 카메라에만 반응합니다.
            buttonHint.transform.rotation = camTransform.rotation * originalRotation;
        }

        if (Input.GetKeyDown(_KeyCode) && inTrigger)
        {
            Interact();
        }
    }

    /*
        중요!!
        OnTriggerStay는 FixedUpdate 방식이므로 여러번 호출 될 수 있습니다.
        한번만 호출하기 위해 bool 변수로 상태를 설정하고, Udpate 함수에서 해당 기능을 호출하도록 합니다.
    */
    private void OnTriggerStay(Collider other)
    {
        // if (other.GetComponent<PhotonView>() == null || other.GetComponent<PhotonView>().IsMine == false)
        //     return;

        if(other.gameObject.tag == "Player"){
            inTrigger = true;
            camTransform = Camera.main.transform;
        }
    }

    // Collider 컴포넌트의 is Trigger가 true인 상태로 충돌이 끝났을 때
    private void OnTriggerExit(Collider other)
    {
        inTrigger = false;
        camTransform = null;
    }

    void OnButtonHint(bool active){
        if(buttonHint != null)
            buttonHint.gameObject.SetActive(active);
    }

    public KeyCode _KeyCode{
        get=> keyCode;
    }

    public string Name{
        get=>name;
    }

    public GameObject ButtonHint{
        get=>buttonHint;
    }

    public string Key{
        get=>key;
    }
}
